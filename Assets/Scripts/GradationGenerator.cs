using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GradationGenerator : MonoBehaviour
{
    public Material tarMat;
    List<float> overlapData;
    Dictionary<int, List<int>> pointToAround;
    //List<float> calcVal = new List<float>();
    float gradationValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        setPointToAround();

        initialize();

        List<float> outlineDatas = detectOutline();
        List<int> outlineIndexs = detectOutlineIndexs();

        //gradationOutline(outlineIndexs, 0.0f);

        //setTextureFromOverlapdata(outlineDatas);
        setTextureFromOverlapdata();

        StartCoroutine(gradationGenerate());
    }

    void initialize()
    {
        overlapData = new List<float>();

        MemoryStream ms = new MemoryStream(File.ReadAllBytes("ProjectionData.bin"));
        BinaryReader br = new BinaryReader(ms);

        while (ms.Position < ms.Length)
        {
            overlapData.Add(br.ReadSingle());
        }

        br.Close();
        ms.Close();

        ///////////////////////////////////////////////
        // amend

        int targetIndex = (1023 - 479) * 1024;
        targetIndex += 305;
 
        overlapData[targetIndex] = 0.0f;

        List<int> tarIndexs = pointToAround[targetIndex];
        for(int i = 0; i < tarIndexs.Count; i++)
        {
            overlapData[tarIndexs[i]] = 0.0f;
        }

        targetIndex = (1023 - 664) * 1024;
        targetIndex += 556;

        overlapData[targetIndex] = 1.0f;

        tarIndexs = pointToAround[targetIndex];
        for (int i = 0; i < tarIndexs.Count; i++)
        {
            overlapData[tarIndexs[i]] = 1.0f;
        }

        ///////////////////////////////////////////////

        Debug.Log(overlapData.Count);
    }

    void gradationOutline(List<int> outline, float value)
    {
        Dictionary<int, List<int>> gradationIndexs = new Dictionary<int, List<int>>();

        for(int i = 0; i < outline.Count; i++)
        {
            int currentIndex = outline[i];

            List<int> aroundIndexs = pointToAround[currentIndex];

            for(int j = 0; j < aroundIndexs.Count; j++)
            {
                int aroundIndex = aroundIndexs[j];

                if(overlapData[aroundIndex] == 0.0f)
                {
                    if (gradationIndexs.ContainsKey(aroundIndex))
                    {
                        gradationIndexs[aroundIndex].Add(currentIndex);
                    }
                    else
                    {
                        List<int> newSumData = new List<int>();
                        newSumData.Add(currentIndex);

                        gradationIndexs.Add(aroundIndex, newSumData);
                    }
                    
                }
            }
        }

        Dictionary<int, List<int>>.Enumerator graEnum = gradationIndexs.GetEnumerator();

        while (graEnum.MoveNext())
        {
            overlapData[graEnum.Current.Key] = value;
        }
    }

    List<float> detectOutline()
    {
        int ountlineCounts = 0;
        List<float> outlineData = new List<float>();

        for(int i = 0; i < pointToAround.Count; i++)
        {
            float currentValue = overlapData[i];
            
            bool isEdge = false;

            if (currentValue > 0.0f)
            {
                //currentValue = 0.0f;

                List<int> aroundData = pointToAround[i];

                for(int j = 0; j <aroundData.Count; j++)
                {
                    int aroundIndex = aroundData[j];

                    if (overlapData[aroundIndex] == 0.0f)
                    {
                        isEdge = true;
                        //currentValue = 1.0f;
                        ountlineCounts++;
                        break;
                    }
                }
            }

            if(isEdge)
            {
                outlineData.Add(currentValue);
            }
            else
            {
                outlineData.Add(0.0f);
            }
        }

        Debug.Log(ountlineCounts);

        return outlineData;
    }

    List<int> detectOutlineIndexs()
    {
        List<int> indexs = new List<int>();

        for (int i = 0; i < pointToAround.Count; i++)
        {
            float currentValue = overlapData[i];

            bool isEdge = false;

            if (currentValue > 0.0f)
            {
                //currentValue = 0.0f;

                List<int> aroundData = pointToAround[i];

                for (int j = 0; j < aroundData.Count; j++)
                {
                    int aroundIndex = aroundData[j];

                    if (overlapData[aroundIndex] == 0.0f)
                    {
                        isEdge = true;
                        //currentValue = 1.0f;
                        break;
                    }
                }
            }

            if (isEdge)
            {
                indexs.Add(i);
            }
        }

        Debug.Log(indexs.Count);

        return indexs;
    }

    void setPointToAround(int count = 1024)
    {
        pointToAround = new Dictionary<int, List<int>>();

        for (int y = 0; y < count; y++)
        {
            for (int x = 0; x < count; x++)
            {
                List<int> aroundValues = new List<int>();

                int pointIndex = x + y * count;

                for (int y1 = -1; y1 < 2; y1++)
                {
                    for (int x1 = -1; x1 < 2; x1++)
                    {
                        int nX = x + x1;
                        int nY = y + y1;

                        if (nX > -1 && nX < count)
                        {
                            if (nY > -1 && nY < count)
                            {
                                int aroundIndex = nX + nY * count;
                                aroundValues.Add(aroundIndex);
                            }
                        }
                    }
                }

                pointToAround.Add(pointIndex, aroundValues);
            }
        }

        Debug.Log(pointToAround.Count);
    }

    void setTextureFromOverlapdata()
    {
        Color keepColor = Color.black;

        Texture2D tex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                keepColor = Color.black;

                float overlapValue = overlapData[x + y * 1024];
                if (overlapValue > 0.0f)
                {
                    keepColor = Color.white;

                    keepColor.r *= overlapValue;
                    keepColor.g *= overlapValue;
                    keepColor.b *= overlapValue;
                }

                tex.SetPixel(x, y, keepColor);
            }
        }
        tex.Apply();

        tarMat.mainTexture = tex;
    }

    void setTextureFromOverlapdata(List<float> overData)
    {
        Color keepColor = Color.black;

        Texture2D tex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                keepColor = Color.black;

                float overlapValue = overlapData[x + y * 1024];
                if (overlapValue > 0.0f)
                {
                    keepColor = Color.white;

                    keepColor.r *= overlapValue;
                    keepColor.g *= overlapValue;
                    keepColor.b *= overlapValue;
                }

                tex.SetPixel(x, y, keepColor);
            }
        }
        tex.Apply();

        tarMat.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.O))

    }

    IEnumerator gradationGenerate()
    {
        while (gradationValue > 0.0f)
        {
            yield return null;

            List<int> outlineIndexs = detectOutlineIndexs();

            gradationOutline(outlineIndexs, gradationValue);

            setTextureFromOverlapdata();

            gradationValue -= 0.01f;

            outlineIndexs.Clear();
            outlineIndexs = null;
        }

        List<int> keyValues = new List<int>();

        using (Dictionary<int, List<int>>.Enumerator ptaEnum = pointToAround.GetEnumerator())
        {
            while (ptaEnum.MoveNext())
            {
                keyValues.Add(ptaEnum.Current.Key);
            }
        }

        for (int i = 0; i < keyValues.Count; i++)
        {
            pointToAround[keyValues[i]].Clear();
            pointToAround[keyValues[i]] = null;
        }

        pointToAround.Clear();
        pointToAround = null;

        keyValues.Clear();
        keyValues = null;
    }
}
