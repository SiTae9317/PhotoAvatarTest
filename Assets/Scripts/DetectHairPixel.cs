using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DetectHairPixel : MonoBehaviour
{
    public GameObject tarObj;
    public GameObject go;
    public Color hairMainColor;
    public Color faceMainColor;

    public EculideanModel em;
    public float threshHold;

    Texture2D hairTex;
    Texture2D onlyHairTex;
    Texture2D onlyFaceTex;

    List<float> faceOverlapData;
    List<float> hairOverlapData;

    List<int> beforeIndexs;

    public float faceHairDis = 0.0f;

    Dictionary<int, List<int>> pointToAround;

    int width;
    int height;

    // Start is called before the first frame update
    void Start()
    {
        //LAB lab = ColorUtils.RGBToLAB(Color.white);
        //Debug.Log(lab.l + " " + lab.a + " " + lab.b);

        beforeIndexs = new List<int>();
        hairTex = new Texture2D(1, 1);
        hairTex.LoadImage(File.ReadAllBytes("hairTex.png"));
        hairTex.Apply();

        onlyFaceTex = new Texture2D(1, 1);
        onlyFaceTex.LoadImage(File.ReadAllBytes("faceTex.png"));
        onlyFaceTex.Apply();

        width = hairTex.width;
        height = hairTex.height;

        setPointToAround(width, height);

        detectTextureMainColor(hairTex, out hairMainColor, out hairOverlapData);
        detectTextureMainColor(onlyFaceTex, out faceMainColor, out faceOverlapData);

        faceHairDis = ColorUtils.euclidean(hairMainColor, faceMainColor, em);

        Debug.Log(ColorUtils.euclidean(hairMainColor, faceMainColor, em));

        copyOnlyHairTex();
        
        tarObj.GetComponent<MeshRenderer>().material.mainTexture = hairTex;
        go = Instantiate(tarObj);
        go.transform.position += Vector3.right * 10.0f;
        go.GetComponent<MeshRenderer>().material.mainTexture = onlyHairTex;
    }

    void copyOnlyHairTex()
    {
        //Dictionary<Color, int> pixelColorCount = new Dictionary<Color, int>();

        onlyHairTex = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color keepColor = hairTex.GetPixel(x, y);

                if(keepColor.a > 0.0f)
                {
                    //if(pixelColorCount.ContainsKey(keepColor))
                    //{
                    //    pixelColorCount[keepColor] += 1;
                    //}
                    //else
                    //{
                    //    pixelColorCount.Add(keepColor, 1);
                    //}

                    onlyHairTex.SetPixel(x, y, keepColor);
                }
            }
        }

        //SortedList<int, Color> sortingMainColors = new SortedList<int, Color>();

        //int mainCount = int.MinValue;

        //Dictionary<Color, int>.Enumerator pccEnum = pixelColorCount.GetEnumerator();

        //while(pccEnum.MoveNext())
        //{
        //    int curCount = pccEnum.Current.Value;

        //    if(mainCount < curCount)
        //    {
        //        mainCount = curCount;
        //        hairMainColor = pccEnum.Current.Key;
        //    }
        //}

        onlyHairTex.Apply();
    }

    IEnumerator hairDetecting()
    {
        int beforeCount = 0;

        //for(int i = 0; i < 3; i++)
        //{
        //    faceOverlapData = outlineRemove(faceOverlapData);
        //}

        while (true)
        {
            yield return null;

            beforeCount = detectOutlineIndexs(hairOverlapData).Count;

            hairFaceColorRemove();
            detectOuntlineHairPixel();

            int nowCount = detectOutlineIndexs(hairOverlapData).Count;

            if(beforeCount == nowCount)
            {
                break;
            }
            Debug.Log(beforeCount + " -> " + nowCount);
        }

        overlapDrawTex();
        go.GetComponent<MeshRenderer>().material.mainTexture = onlyHairTex;

        Debug.Log("end");
    }

    void hairFaceColorRemove()
    {
        for(int i = 0; i < hairOverlapData.Count; i++)
        {
            if(hairOverlapData[i] > 0.0f)
            {
                int x = i % width;
                int y = i / width;

                Color keepColor = hairTex.GetPixel(x, y);

                float d = ColorUtils.euclidean(keepColor, faceMainColor, em);

                if (faceHairDis < 0.3f)
                {
                    if (faceOverlapData[i] > 0.0f)
                    {
                        hairOverlapData[i] = 0.0f;
                    }
                }
                else
                {
                    //if (d < faceHairDis / 1.25f)
                    if (d < faceHairDis / 2.0f)
                    {
                        hairOverlapData[i] = 0.0f;
                    }
                }

                //if (faceOverlapData[i] > 0.0f)
                //{
                //    hairOverlapData[i] = 0.0f;
                //}
                //else if (d < faceHairDis / 2.0f)
                //{
                //    hairOverlapData[i] = 0.0f;
                //}
            }
        }

        //onlyHairTex = new Texture2D(width, height);

        //for(int y = 0; y < height; y++)
        //{
        //    for(int x = 0; x < width; x++)
        //    {
        //        int curIndex = y * width + x;

        //        if(hairOverlapData[curIndex] > 0.0f)
        //        {
        //            onlyHairTex.SetPixel(x, y, hairTex.GetPixel(x, y));
        //        }
        //    }
        //}

        //onlyHairTex.Apply();
    }

    void detectOuntlineHairPixel()
    {
        List<int> outlineIndexs = detectOutlineIndexs(hairOverlapData);

        //HashSet<int> sameIndexRemover = new HashSet<int>();

        //for(int i = 0; i < beforeIndexs.Count; i++)
        //{
        //    sameIndexRemover.Add(beforeIndexs[i]);
        //}

        //List<int> newOutlines = new List<int>();

        //for(int i = 0; i < outlineIndexs.Count; i++)
        //{
        //    if(!sameIndexRemover.Contains(outlineIndexs[i]))
        //    {
        //        newOutlines.Add(outlineIndexs[i]);
        //    }
        //}

        //outlineIndexs.Clear();
        //outlineIndexs.AddRange(newOutlines.ToArray());
        //beforeIndexs.Clear();
        //beforeIndexs.AddRange(newOutlines.ToArray());

        for (int i = 0; i < outlineIndexs.Count; i++)
        {
            int curIndex = outlineIndexs[i];

            if(hairOverlapData[curIndex] > 0.0f)
            {
                List<int> aroundIndexs = pointToAround[curIndex];

                for(int j = 0; j < aroundIndexs.Count; j++)
                {
                    int aroundIndex = aroundIndexs[j];

                    if(hairOverlapData[aroundIndex] == 0.0f)
                    {
                        int cx = aroundIndex % width;
                        int cy = aroundIndex / width;

                        Color curColor = hairTex.GetPixel(cx, cy);

                        float d = ColorUtils.euclidean(hairMainColor, curColor, em);
                        //float hue = ColorUtils.euclidean(hairMainColor, curColor, EculideanModel.HSV_HUE) / 360.0f;
                        //d += hue;

                        //d /= ColorUtils.maxRGBDistance;

                        //if (d < 0.15f)
                        //{
                        //    hairOverlapData[aroundIndex] = 1.0f;
                        //}

                        if (d < threshHold)
                        {
                            //hairOverlapData[aroundIndex] = 1.0f - (d / ColorUtils.maxRGBDistance);
                            hairOverlapData[aroundIndex] = 1.0f;
                        }
                    }
                }
                
            }
        }
    }

    void overlapDrawTex()
    {
        onlyHairTex = new Texture2D(width, height);

        Color removeColor = Color.black;
        removeColor.a = 0.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int curIndex = y * width + x;

                if (hairOverlapData[curIndex] > 0.0f)
                {
                    Color keepColor = hairTex.GetPixel(x, y);
                    keepColor.a = hairOverlapData[curIndex];
                    onlyHairTex.SetPixel(x, y, keepColor);
                }
                else
                {
                    onlyHairTex.SetPixel(x, y, removeColor);
                }
            }
        }

        onlyHairTex.Apply();
    }


    void checkAroundColorDis()
    {
        onlyHairTex = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color keepColor = hairTex.GetPixel(x, y);

                if(keepColor.a > 0.0f)
                {
                    int curIndex = y * width + x;

                    List<int> aroundIndexs = pointToAround[curIndex];

                    for(int i = 0; i < aroundIndexs.Count; i++)
                    {
                        int aroundIndex = aroundIndexs[i];

                        int cx = aroundIndex % width;
                        int cy = aroundIndex / width;

                        Color curColor = hairTex.GetPixel(cx, cy);

                        if(curColor.a == 0.0f)
                        {
                            //if(onlyFaceTex.GetPixel(cx,cy).a == 0.0f)
                            {
                                float d = ColorUtils.euclidean(hairMainColor, curColor, em);

                                if (d < threshHold)
                                {
                                    curColor.a = 1.0f;
                                    hairTex.SetPixel(cx, cy, curColor);
                                    onlyHairTex.SetPixel(cx, cy, curColor);
                                }
                            }
                        }
                    }
                    onlyHairTex.SetPixel(x, y, keepColor);
                }
            }
        }

        hairTex.Apply();
        onlyHairTex.Apply();

        Debug.Log("end");
    }

    void checkColorDis()
    {
        //onlyHairTex = new Texture2D(width, height);

        float minD = float.MaxValue;
        float maxD = float.MinValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color keepColor = hairTex.GetPixel(x, y);

                float d = ColorUtils.euclidean(hairMainColor, keepColor, em);

                if(d < minD)
                {
                    minD = d;
                }
                if(d > maxD)
                {
                    maxD = d;
                }

                if(d < threshHold)
                {
                    onlyHairTex.SetPixel(x, y, keepColor);
                }
                else
                {
                    onlyHairTex.SetPixel(x, y, Color.gray);
                }

                //int curIndex = y * width + x;

                //List<int> pta = pointToAround[curIndex];

                //for(int j = 0; j < pta.Count; j++)
                //{
                //    int aroundIndex = pta[j];
                //    if (aroundIndex != )
                //}
            }
        }

        Debug.Log(minD + " " + maxD);

        onlyHairTex.Apply();
    }

    void setPointToAround(int countX, int countY)
    {
        pointToAround = new Dictionary<int, List<int>>();

        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                List<int> aroundValues = new List<int>();

                int pointIndex = x + y * countX;

                for (int y1 = -1; y1 < 2; y1++)
                {
                    for (int x1 = -1; x1 < 2; x1++)
                    {
                        int nX = x + x1;
                        int nY = y + y1;

                        if (nX > -1 && nX < countX)
                        {
                            if (nY > -1 && nY < countY)
                            {
                                int aroundIndex = nX + nY * countX;
                                aroundValues.Add(aroundIndex);
                            }
                        }
                    }
                }

                pointToAround.Add(pointIndex, aroundValues);
            }
        }

        Debug.Log(pointToAround.Count);

        //for (int i = 0; i < 5; i++)
        //{
        //    overlapData = outlineRemove(overlapData);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            checkColorDis();
            go.GetComponent<MeshRenderer>().material.mainTexture = onlyHairTex;
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            checkAroundColorDis();
            go.GetComponent<MeshRenderer>().material.mainTexture = onlyHairTex;
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("start detecting");
            StartCoroutine(hairDetecting());
            //hairFaceColorRemove();
            //detectOuntlineHairPixel();
            //go.GetComponent<MeshRenderer>().material.mainTexture = onlyHairTex;
        }
    }

    void detectTextureMainColor(Texture2D curTex, out Color mainColor, out List<float> overlapData)
    {
        Dictionary<Color, int> pixelColorCount = new Dictionary<Color, int>();

        List<float> alphaMap = new List<float>();

        int curWidth = curTex.width;
        int curHeight = curTex.height;

        for (int y = 0; y < curHeight; y++)
        {
            for (int x = 0; x < curWidth; x++)
            {
                Color keepColor = curTex.GetPixel(x, y);

                if (keepColor.a > 0.0f)
                {
                    if (pixelColorCount.ContainsKey(keepColor))
                    {
                        pixelColorCount[keepColor] += 1;
                    }
                    else
                    {
                        pixelColorCount.Add(keepColor, 1);
                    }
                }

                alphaMap.Add(keepColor.a);
            }
        }

        overlapData = alphaMap;

        int mainCount = int.MinValue;

        Dictionary<Color, int>.Enumerator pccEnum = pixelColorCount.GetEnumerator();

        Color manyColor = Color.black;

        while (pccEnum.MoveNext())
        {
            int curCount = pccEnum.Current.Value;

            if (mainCount < curCount)
            {
                mainCount = curCount;
                manyColor = pccEnum.Current.Key;
            }
        }

        mainColor = manyColor;
    }

    List<int> detectOutlineIndexs(List<float> layer)
    {
        List<int> indexs = new List<int>();

        for (int i = 0; i < pointToAround.Count; i++)
        {
            float currentValue = layer[i];

            bool isEdge = false;

            if (currentValue > 0.0f)
            {
                //currentValue = 0.0f;

                List<int> aroundData = pointToAround[i];

                for (int j = 0; j < aroundData.Count; j++)
                {
                    int aroundIndex = aroundData[j];

                    if (layer[aroundIndex] == 0.0f)
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

        //Debug.Log(indexs.Count);

        return indexs;
    }

    List<float> outlineRemove(List<float> alphas)
    {
        List<float> newAlphas = new List<float>();

        for (int i = 0; i < alphas.Count; i++)
        {
            float currentValue = alphas[i];

            if (currentValue > 0.0f)
            {
                List<int> currentAroundValues = pointToAround[i];

                if (currentAroundValues.Count != 9)
                {
                    currentValue = 0.0f;
                }
                else
                {
                    for (int j = 0; j < currentAroundValues.Count; j++)
                    {
                        int aroundValue = currentAroundValues[j];

                        if (alphas[aroundValue] == 0.0f)
                        {
                            currentValue = 0.0f;
                            break;
                        }
                    }
                }
            }

            newAlphas.Add(currentValue);
        }

        return newAlphas;
    }
}
