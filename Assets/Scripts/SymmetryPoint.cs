using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SymmetryPoint : MonoBehaviour
{
    public bool pointTest = false;

    public GameObject testPlane;

    public GameObject point0;
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;

    private Texture2D newTex;

    List<int> defaultOutline;

    Dictionary<int, List<int>> pointToAround = new Dictionary<int, List<int>>();
    Dictionary<int, List<int>> pointToGradient = new Dictionary<int, List<int>>();

    List<float> overlapData;
    float alphaGradient = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(!pointTest)
        {
            point0.SetActive(false);
            point1.SetActive(false);
            point2.SetActive(false);
            point3.SetActive(false);

            initialize();
            setPointToAround();
            setPointToGradient();

            StartCoroutine(testDraw());
        }
        else
        {
            testPlane.SetActive(false);
        }
    }

    int minimumDistancePoint(int index)
    {
        int x = index % 1024;
        int y = index / 1024;

        float minDis = float.MaxValue;
        int minIndex = 0;

        for(int i = 0; i < defaultOutline.Count; i++)
        {
            int currentIndex = defaultOutline[i];

            int cx = currentIndex % 1024;
            int cy = currentIndex / 1024;

            float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

            if(minDis > curDis)
            {
                minDis = curDis;
                minIndex = currentIndex;
            }
        }

        return minIndex;
    }

    void drawOuntline()
    {
        List<int> outlineIndex = detectOutlineIndexs();

        for (int i = 0; i < outlineIndex.Count; i++)
        {
            int index = outlineIndex[i];

            List<int> gradient = pointToGradient[minimumDistancePoint(index)];
            List<int> aroundIndex = pointToAround[index];

            int p0 = gradient[0];
            int p1 = gradient[1];

            int p0x = p0 % 1024;
            int p0y = p0 / 1024;

            int p1x = p1 % 1024;
            int p1y = p1 / 1024;

            for (int j = 0; j < aroundIndex.Count; j++)
            {
                int currentIndex = aroundIndex[j];

                if (overlapData[currentIndex] == 0.0f)
                {
                    int cpx = currentIndex % 1024;
                    int cpy = currentIndex / 1024;

                    int[] resultPoint = calcSymmetryPoint(p0x, p0y, p1x, p1y, cpx, cpy);
                    //Debug.Log(resultPoint[0] + " " + resultPoint[1]);

                    Color targetColor = newTex.GetPixel(cpx, cpy);
                    Color sourceColor = newTex.GetPixel(resultPoint[0], resultPoint[1]);

                    targetColor.r *= (1.0f - alphaGradient);
                    targetColor.g *= (1.0f - alphaGradient);
                    targetColor.b *= (1.0f - alphaGradient);

                    sourceColor.r *= alphaGradient;
                    sourceColor.g *= alphaGradient;
                    sourceColor.b *= alphaGradient;

                    targetColor.r += sourceColor.r;
                    targetColor.g += sourceColor.g;
                    targetColor.b += sourceColor.b;

                    targetColor.a = alphaGradient;

                    //newTex.SetPixel(cpx, cpy, sourceColor);
                    newTex.SetPixel(cpx, cpy, targetColor);

                    overlapData[currentIndex] = 1.0f;
                }
            }

            //float p0 = (float)gradient[0];
            //float p1 = (float)gradient[1];

            //Vector2 vp0 = new Vector2(p0 % 1024.0f, (int)(p0 / 1024.0f));
            //Vector2 vp1 = new Vector2(p1 % 1024.0f, (int)(p1 / 1024.0f));

            //for (int j = 0; j < aroundIndex.Count; j++)
            //{
            //    float currentIndex = (float)aroundIndex[j];

            //    if (overlapData[(int)currentIndex] == 0.0f)
            //    {
            //        Vector2 curP = new Vector2(currentIndex % 1024.0f, (int)(currentIndex / 1024.0f));

            //        Debug.Log(vp0.x + " " + vp0.y);
            //        Debug.Log(vp1.x + " " + vp1.y);
            //        Debug.Log(calcEquation(vp0, vp1));
            //        Debug.Log(curP.x + " " + curP.y);

            //        Debug.Log(calcSymmetryPoint(vp0.x, vp0.y, vp1.x, vp1.y, curP.x, curP.y));
            //    }
            //}
            //break;
        }

        newTex.Apply();

        testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex;
    }

    void initialize()
    {
        overlapData = new List<float>();

        Texture2D tex = testPlane.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        Texture2D overlapTex = new Texture2D(1, 1);
        overlapTex.LoadImage(File.ReadAllBytes("OverlapTex.jpg"));

        overlapTex.Apply();

        int width = tex.width;
        int height = tex.height;

        newTex = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color keepColor = tex.GetPixel(x, y);

                float grayValue = overlapTex.GetPixel(x, y).grayscale;

                if (x == 305 && y == 544)
                {
                    grayValue = 0.0f;
                }

                if (x == 556 && y == 359)
                {
                    grayValue = 1.0f;
                }

                if(grayValue > 0.5f)
                {
                    grayValue = 1.0f;
                }
                else
                {
                    grayValue = 0.0f;
                }

                overlapData.Add(grayValue);

                keepColor.a = grayValue;

                newTex.SetPixel(x, y, keepColor);
            }
        }

        newTex.Apply();

        testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex;
    }

    void setPointToGradient()
    {
        defaultOutline = new List<int>();

        List<int> outlineIndex = detectOutlineIndexs();

        defaultOutline.AddRange(outlineIndex.ToArray());

        for (int i = 0; i < outlineIndex.Count; i++)
        {
            int index = outlineIndex[i];

            List<int> aroundIndex = pointToAround[index];

            int checkCount = 0;

            List<int> matchIndexs = new List<int>();

            for (int j = 0; j < aroundIndex.Count; j++)
            {
                int currentIndex = aroundIndex[j];

                for (int k = 0; k < defaultOutline.Count; k++)
                {
                    if (currentIndex == defaultOutline[k])
                    {
                        checkCount++;
                        matchIndexs.Add(currentIndex);
                        //Debug.Log(currentIndex);
                        break;
                    }
                }
            }

            int x = index % 1024;
            int y = index / 1024;

            float maxValue = float.MinValue;
            int maxIndex = 0;

            for(int j = 0; j < matchIndexs.Count; j++)
            {
                int currentIndex = matchIndexs[j];

                int cx = currentIndex % 1024;
                int cy = currentIndex / 1024;

                float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

                if(curDis > maxValue)
                {
                    maxValue = curDis;
                    maxIndex = currentIndex;
                }
            }

            x = maxIndex % 1024;
            y = maxIndex / 1024;

            maxValue = float.MinValue;
            int maxIndex2 = 0;

            for (int j = 0; j < matchIndexs.Count; j++)
            {
                int currentIndex = matchIndexs[j];

                int cx = currentIndex % 1024;
                int cy = currentIndex / 1024;

                float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

                if (curDis > maxValue)
                {
                    maxValue = curDis;
                    maxIndex2 = currentIndex;
                }
            }

            List<int> gradientIndexs = new List<int>();

            gradientIndexs.Add(maxIndex);
            gradientIndexs.Add(maxIndex2);

            pointToGradient.Add(index, gradientIndexs);
        }
    }

    //void setPointToGradient()
    //{
    //    defaultOutline = new List<int>();

    //    List<int> outlineIndex = detectOutlineIndexs();

    //    defaultOutline.AddRange(outlineIndex.ToArray());

    //    for (int i = 0; i < outlineIndex.Count; i++)
    //    {
    //        int index = outlineIndex[i];

    //        List<int> aroundIndex = pointToAround[index];

    //        int checkCount = 0;

    //        List<int> matchIndexs = new List<int>();

    //        for (int j = 0; j < aroundIndex.Count; j++)
    //        {
    //            int currentIndex = aroundIndex[j];

    //            for (int k = 0; k < defaultOutline.Count; k++)
    //            {
    //                if (currentIndex == defaultOutline[k])
    //                {
    //                    checkCount++;
    //                    matchIndexs.Add(currentIndex);
    //                    //Debug.Log(currentIndex);
    //                    break;
    //                }
    //            }
    //        }

    //        matchIndexs.Sort();

    //        while (matchIndexs.Count > 2)
    //        {
    //            matchIndexs.RemoveAt(1);
    //        }
    //        pointToGradient.Add(index, matchIndexs);
    //    }
    //}

    List<float> detectOutline()
    {
        int ountlineCounts = 0;
        List<float> outlineData = new List<float>();

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
                        ountlineCounts++;
                        break;
                    }
                }
            }

            if (isEdge)
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

        for (int i = 0; i < 5; i++)
        {
            overlapData = outlineRemove(overlapData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.X))
        //{
        //    if(alphaGradient > 0.0f)
        //    {
        //        alphaGradient -= 0.01f;
        //        drawOuntline();
        //    }
        //}

        if(pointTest)
        {
            Vector3 pos0 = point0.transform.position;
            Vector3 pos1 = point1.transform.position;

            Debug.DrawLine(pos0, pos1, Color.green);

            int[] result = calcSymmetryPoint(pos0.x, pos0.y, pos1.x, pos1.y, point2.transform.position.x, point2.transform.position.y);
            point3.transform.position = new Vector3(result[0], result[1], 0.0f);
            //point3.transform.position = calcSymmetryPoint(calcEquation(pos0, pos1), point2.transform.position);
        }
    }

    public Vector2 calcEquation(Vector2 p0, Vector2 p1)
    {
        float a = (p1.y - p0.y) / (p1.x - p0.x);
        float b = p0.y - (p0.x * a);

        Debug.Log("y = " + a + "x + " + b);

        return new Vector2(a, b);
    }

    public Vector2 calcSymmetryPoint(Vector2 ab, Vector2 xy)
    {
        return calcSymmetryPoint(ab.x, ab.y, xy.x, xy.y);
    }

    public Vector2 calcSymmetryPoint(float a, float b, float x, float y)
    {
        float x1 = 0.0f;
        float y1 = 0.0f;

        //Debug.Log(x + " " + y);

        if (a == 0.0f)
        {
            x1 = x;
            y1 = -y;
        }
        else if (float.IsInfinity(a))
        {
            x1 = -x;
            y1 = y;
        }
        else
        {
            x1 = ((-1.0f * Mathf.Pow(a, 2.0f) * x + x) - (2.0f * a * b) + (2.0f * a * y)) / (Mathf.Pow(a, 2.0f) + 1.0f);
            y1 = (-1.0f / a) * (x1 - x) + y;
        }

        return new Vector2(x1, y1);
    }

    public int[] calcSymmetryPoint(float p0x, float p0y, float p1x, float p1y, float x, float y)
    {
        int[] resultPoint = new int[2];

        float x1 = 0.0f;
        float y1 = 0.0f;

        float a = (p1y - p0y) / (p1x - p0x);
        float b = p0y - (p0x * a);

        if (a == 0.0f)
        {
            x1 = x;
            y1 = p0y + (p0y-y);
        }
        else if (float.IsInfinity(a))
        {
            x1 = p0x + (p0x -x);
            y1 = y;
        }
        else
        {
            x1 = ((-1.0f * Mathf.Pow(a, 2.0f) * x + x) - (2.0f * a * b) + (2.0f * a * y)) / (Mathf.Pow(a, 2.0f) + 1.0f);
            y1 = (-1.0f / a) * (x1 - x) + y;
        }

        resultPoint[0] = (int)x1;
        resultPoint[1] = (int)y1;

        return resultPoint;
    }

    IEnumerator testDraw()
    {
        while (alphaGradient > 0.0f)
        {
            alphaGradient -= 0.01f;
            drawOuntline();
            yield return null;
        }
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

            newAlphas.Add(currentValue);
        }

        return newAlphas;
    }
}
