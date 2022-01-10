using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SymmetryPoint2 : MonoBehaviour
{
    public GameObject testPlane;
    public GameObject cube;

    private Texture2D newTex;
    private Texture2D newTex2;
    private Texture2D newTex3;

    List<int> defaultOutline;

    Dictionary<int, List<int>> pointToAround = new Dictionary<int, List<int>>();
    Dictionary<int, List<int>> pointToGradient = new Dictionary<int, List<int>>();

    List<float> overlapData;
    float alphaGradient = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize();
        //setPointToAround();
        //setPointToGradient();

        //List<int> outlineDatas = detectOutlineIndexs(overlapData);

        //for(int i = 0; i < outlineDatas.Count; i++)
        //{
        //    int currentIndex = outlineDatas[i];
        //    int cpx = currentIndex % 1024;
        //    int cpy = currentIndex / 1024;

        //    //if(cpx > 370 && cpx < 490)
        //    //{
        //    //    if(cpy >517 && cpy < 540)
        //    //    {
        //    //        continue;
        //    //    }
        //    //}

        //    //if(cpx > 512 + 22 && cpx < 512 + 22 + 220)
        //    //{
        //    //    if (cpy > 517 && cpy < 540)
        //    //    {
        //    //        continue;
        //    //    }
        //    //}

        //    GameObject go = Instantiate(cube);
        //    go.name = cpx.ToString() + "," + cpy.ToString();
        //    go.transform.position = new Vector3(cpx, cpy, 0.0f);
        //}

        //cube.SetActive(false);

        //StartCoroutine(testDraw());
    }

    int minimumDistancePoint(int index, List<int> outlineDatas, bool eyeCheck = true)
    {
        int x = index % 1024;
        int y = index / 1024;

        float minDis = float.MaxValue;
        int minIndex = 0;

        for (int i = 0; i < outlineDatas.Count; i++)
        {
            int currentIndex = outlineDatas[i];

            int cx = currentIndex % 1024;
            int cy = currentIndex / 1024;

            if(!isEyes(cx, cy) && eyeCheck)
            {
                continue;
            }

            float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

            if (minDis > curDis)
            {
                minDis = curDis;
                minIndex = currentIndex;
            }
        }

        return minIndex;
    }

    bool isEyes(int x, int y)
    {
        //// left eye
        //if (x >= 350 && x <= 498)
        //{
        //    if (y >= 457 && y <= 545)
        //    {
        //        return false;
        //    }
        //}

        //// right eye
        //if (x >= 528 && x <= 672)
        //{
        //    if (y >= 457 && y <= 545)
        //    {
        //        return false;
        //    }
        //}

        // left eye
        if (x >= 380 && x <= 498)
        {
            if (y >= 457 && y <= 545)
            {
                return false;
            }
        }

        // right eye
        if (x >= 528 && x <= 642)
        {
            if (y >= 457 && y <= 545)
            {
                return false;
            }
        }

        //// left eye
        //if(x >= 390 && x <= 488)
        //{
        //    if (y >= 467 && y <= 535)
        //    {
        //        return false;
        //    }
        //}

        //// right eye
        //if (x >= 538 && x <= 632)
        //{
        //    if (y >= 467 && y <= 535)
        //    {
        //        return false;
        //    }
        //}

        return true;
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

        Debug.Log(indexs.Count);

        return indexs;
    }

    IEnumerator sumTexture()
    {
        //Texture2D dpgHPFMap = new Texture2D(1, 1);

        //dpgHPFMap.LoadImage(File.ReadAllBytes("DPGMapHPF.jpg"));

        //dpgHPFMap.Apply();

        int width = newTex.width;
        int height = newTex.height;

        for (int y = 0; y < height; y++)
        {
            yield return null;

            for (int x = 0; x < width; x++)
            {
                Color sourceTex = newTex.GetPixel(x, y);
                Color smoothTex = newTex3.GetPixel(x, y);

                float smoothAlpha = smoothTex.a;
                float sourceAlpha = 1.0f - smoothAlpha;

                sourceTex.r *= sourceAlpha;
                sourceTex.g *= sourceAlpha;
                sourceTex.b *= sourceAlpha;

                smoothTex.r *= smoothAlpha;
                smoothTex.g *= smoothAlpha;
                smoothTex.b *= smoothAlpha;

                sourceTex.r += smoothTex.r;
                sourceTex.g += smoothTex.g;
                sourceTex.b += smoothTex.b;

                //int curIndex = y * width + x;

                ////if (overlapData[curIndex] == 0.0f)
                //{
                //    Color dpgHPFColor = dpgHPFMap.GetPixel(x, y);

                //    sourceTex.r += dpgHPFColor.r * 0.2f;
                //    sourceTex.g += dpgHPFColor.g * 0.2f;
                //    sourceTex.b += dpgHPFColor.b * 0.2f;
                //}

                newTex2.SetPixel(x, y, sourceTex);
            }

            newTex2.Apply();
            testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
        }

        //StartCoroutine(endDraw1());
        StartCoroutine(endDraw2());
    }

    IEnumerator addHPF()
    {
        Texture2D dpgHPFMap = new Texture2D(1, 1);

        dpgHPFMap.LoadImage(File.ReadAllBytes("DPGMapHPF.jpg"));

        dpgHPFMap.Apply();

        Texture2D dpgEmsMap = new Texture2D(1, 1);

        dpgEmsMap.LoadImage(File.ReadAllBytes("emissionMap.jpg"));

        dpgEmsMap.Apply();

        int width = newTex2.width;
        int height = newTex2.height;

        for (int y = 0; y < height; y++)
        {
            yield return null;

            for (int x = 0; x < width; x++)
            {
                Color sourceTex = newTex2.GetPixel(x, y);

                int curIndex = y * width + x;

                //if (overlapData[curIndex] == 0.0f)
                {
                    Color dpgHPFColor = dpgHPFMap.GetPixel(x, y);

                    sourceTex.r += dpgHPFColor.r * 0.1f;
                    sourceTex.g += dpgHPFColor.g * 0.1f;
                    sourceTex.b += dpgHPFColor.b * 0.1f;

                    Color dpgEmsColor = dpgEmsMap.GetPixel(x, y);

                    sourceTex.r += dpgEmsColor.r * 0.5f;
                    sourceTex.g += dpgEmsColor.g * 0.5f;
                    sourceTex.b += dpgEmsColor.b * 0.5f;

                    //sourceTex.r += dpgEmsColor.r * 0.25f;
                    //sourceTex.g += dpgEmsColor.g * 0.25f;
                    //sourceTex.b += dpgEmsColor.b * 0.25f;
                }

                newTex2.SetPixel(x, y, sourceTex);
            }

            newTex2.Apply();
            testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
        }

        Debug.Log("end");
    }

    //IEnumerator sumTexture()
    //{
    //    int width = newTex.width;
    //    int height = newTex.height;

    //    for(int y = 0; y < height; y++)
    //    {
    //        yield return null;

    //        for(int x = 0; x < width; x++)
    //        {
    //            Color sourceTex = newTex.GetPixel(x, y);
    //            Color smoothTex = newTex3.GetPixel(x, y);

    //            float smoothAlpha = smoothTex.a;
    //            float sourceAlpha = 1.0f - smoothAlpha;

    //            sourceTex.r *= sourceAlpha;
    //            sourceTex.g *= sourceAlpha;
    //            sourceTex.b *= sourceAlpha;

    //            smoothTex.r *= smoothAlpha;
    //            smoothTex.g *= smoothAlpha;
    //            smoothTex.b *= smoothAlpha;

    //            sourceTex.r += smoothTex.r;
    //            sourceTex.g += smoothTex.g;
    //            sourceTex.b += smoothTex.b;

    //            newTex2.SetPixel(x, y, sourceTex);
    //        }

    //        newTex2.Apply();
    //        testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
    //    }

    //    //StartCoroutine(endDraw1());
    //    StartCoroutine(endDraw2());
    //}

    IEnumerator smoothTexture()
    {
        int width = newTex2.width;
        int height = newTex2.height;

        newTex3 = new Texture2D(width, height);

        newTex3.SetPixels(newTex2.GetPixels());
        newTex3.Apply();

        Color keepColor3 = Color.black;

        //int pixelRange = 2;
        //int pixelRange = 50;
        int pixelRange = 25;

        int halfValue = pixelRange;// pixelRange / 2;

        //while(halfValue > 2)
        {
            halfValue = pixelRange / 2;

            for (int y = 0; y < height; y++)
            {
                yield return null;

                for (int x = 0; x < width; x++)
                {
                    int minSY = Mathf.Max(0, y - halfValue);
                    int maxSY = Mathf.Min(height, y + halfValue + 1);

                    int minSX = Mathf.Max(0, x - halfValue);
                    int maxSX = Mathf.Min(width, x + halfValue + 1);

                    float sumR = 0.0f;
                    float sumG = 0.0f;
                    float sumB = 0.0f;
                    float alpha = 0.0f;
                    float sumPixelCount = 0;

                    keepColor3 = newTex2.GetPixel(x, y);
                    alpha = keepColor3.a;

                    if (alpha > 0.0f)
                    {
                        for (int y1 = minSY; y1 < maxSY; y1++)
                        {
                            for (int x1 = minSX; x1 < maxSX; x1++)
                            {
                                keepColor3 = newTex2.GetPixel(x1, y1);
                                if (keepColor3.a > 0.0f)
                                {
                                    sumPixelCount++;
                                    sumR += keepColor3.r;
                                    sumG += keepColor3.g;
                                    sumB += keepColor3.b;
                                }
                            }
                        }

                        sumR /= sumPixelCount;
                        sumG /= sumPixelCount;
                        sumB /= sumPixelCount;

                        //keepColor3 = newTex.GetPixel(x, y);

                        //sumR *= 0.99f;
                        //sumG *= 0.99f;
                        //sumB *= 0.99f;

                        //sumR += keepColor3.r * 0.01f;
                        //sumG += keepColor3.g * 0.01f;
                        //sumB += keepColor3.b * 0.01f;

                        //sumR *= 0.75f;
                        //sumG *= 0.75f;
                        //sumB *= 0.75f;

                        //sumR += keepColor3.r * 0.25f;
                        //sumG += keepColor3.g * 0.25f;
                        //sumB += keepColor3.b * 0.25f;
                    }

                    keepColor3.r = sumR;
                    keepColor3.g = sumG;
                    keepColor3.b = sumB;
                    keepColor3.a = alpha;

                    newTex3.SetPixel(x, y, keepColor3);
                }

                newTex3.Apply();

                testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex3;
            }

            pixelRange = halfValue;
        }

        StartCoroutine(sumTexture());
    }

    
    // type 1
    IEnumerator endDraw1()
    {
        for (int i = 0; i < 5; i++)
        {
            overlapData = outlineRemove(overlapData);
        }

        List<float> innerLayer = new List<float>();
        List<float> outerLayer = new List<float>();

        innerLayer.AddRange(overlapData.ToArray());
        outerLayer.AddRange(overlapData.ToArray());

        int innerCount = 0;
        int outerCount = 1;

        alphaGradient = 1.0f;

        while (alphaGradient >= 0.0f)
        //while(innerCount < 1)
        {
            innerLayer = outlineRemove(innerLayer);

            List<int> innerOutline = detectOutlineIndexs(innerLayer);

            for (int k = 0; k < outerCount; k++)
            {
                yield return null;

                List<int> outlineIndex = detectOutlineIndexs(outerLayer);

                for (int i = 0; i < outlineIndex.Count; i++)
                {
                    int index = outlineIndex[i];

                    List<int> aroundIndex = pointToAround[index];

                    for (int j = 0; j < aroundIndex.Count; j++)
                    {
                        int currentIndex = aroundIndex[j];

                        if (outerLayer[currentIndex] == 0.0f)
                        {
                            int cpx = currentIndex % 1024;
                            int cpy = currentIndex / 1024;

                            int targetIndex = minimumDistancePoint(currentIndex, innerOutline, false);

                            int tpx = targetIndex % 1024;
                            int tpy = targetIndex / 1024;

                            Color targetColor = newTex2.GetPixel(cpx, cpy);
                            Color sourceColor = newTex2.GetPixel(tpx, tpy);

                            float sourceAlpha = 1.0f - alphaGradient;

                            //if (Random.Range(0.0f, 1.0f) > sourceAlpha)
                            //if(targetColor.grayscale > 0.01f)
                            {
                                targetColor.r *= sourceAlpha;
                                targetColor.g *= sourceAlpha;
                                targetColor.b *= sourceAlpha;

                                sourceColor.r *= alphaGradient;
                                sourceColor.g *= alphaGradient;
                                sourceColor.b *= alphaGradient;

                                targetColor.r += sourceColor.r;
                                targetColor.g += sourceColor.g;
                                targetColor.b += sourceColor.b;

                                //targetColor.r = sourceColor.r;
                                //targetColor.g = sourceColor.g;
                                //targetColor.b = sourceColor.b;

                                //targetColor.a = alphaGradient;

                                newTex2.SetPixel(cpx, cpy, targetColor);
                            }

                            outerLayer[currentIndex] = 1.0f;
                        }
                    }
                }

                alphaGradient -= 0.1f;

                newTex2.Apply();

                testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
            }

            //outerCount++;
        }

        Debug.Log("end");
    }


    // type 2
    IEnumerator endDraw2()
    {
        List<float> innerLayer = new List<float>();
        List<float> outerLayer = new List<float>();

        outerLayer.AddRange(overlapData.ToArray());

        List<int> outerIndexs = detectOutlineIndexs(outerLayer);

        for (int k = 0; k < 1; k++)
        {
            for (int i = 0; i < outerIndexs.Count; i++)
            {
                int curIndex = outerIndexs[i];

                List<int> aroundIndex = pointToAround[curIndex];

                for (int j = 0; j < aroundIndex.Count; j++)
                {
                    int aroundCurIndex = aroundIndex[j];
                    outerLayer[aroundCurIndex] = 1.0f;
                }
            }

            outerIndexs = detectOutlineIndexs(outerLayer);
        }

        innerLayer.AddRange(overlapData.ToArray());

        for (int i = 0; i < 3; i++)
        {
            innerLayer = outlineRemove(innerLayer);
        }

        alphaGradient = 1.0f;

        while (alphaGradient >= 0.0f)
        //while(innerCount < 1)
        {
            List<int> innerOutline = detectOutlineIndexs(innerLayer);

            yield return null;

            for (int k = 0; k < innerOutline.Count; k++)
            {
                int innerCurIndex = innerOutline[k];

                List<int> aroundIndex = pointToAround[innerCurIndex];

                for (int i = 0; i < aroundIndex.Count; i++)
                {
                    int currentIndex = aroundIndex[i];

                    if (innerLayer[currentIndex] == 0.0f)
                    {
                        int cpx = currentIndex % 1024;
                        int cpy = currentIndex / 1024;

                        int targetIndex = minimumDistancePoint(currentIndex, outerIndexs, false);

                        int tpx = targetIndex % 1024;
                        int tpy = targetIndex / 1024;

                        Color sourceColor = newTex2.GetPixel(cpx, cpy);
                        Color targetColor = newTex2.GetPixel(tpx, tpy);

                        float targetAlpha = 1.0f - alphaGradient;

                        {
                            sourceColor.r *= alphaGradient;
                            sourceColor.g *= alphaGradient;
                            sourceColor.b *= alphaGradient;

                            targetColor.r *= targetAlpha;
                            targetColor.g *= targetAlpha;
                            targetColor.b *= targetAlpha;

                            sourceColor.r += targetColor.r;
                            sourceColor.g += targetColor.g;
                            sourceColor.b += targetColor.b;

                            //targetColor.r = sourceColor.r;
                            //targetColor.g = sourceColor.g;
                            //targetColor.b = sourceColor.b;

                            //targetColor.a = alphaGradient;

                            newTex2.SetPixel(cpx, cpy, sourceColor);
                        }

                        innerLayer[currentIndex] = 1.0f;
                    }
                }
            }

            alphaGradient -= 0.34f;

            newTex2.Apply();

            testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
        }

        StartCoroutine(addHPF());
    }

    IEnumerator newDraw()
    {
        List<float> innerLayer = new List<float>();
        List<float> outerLayer = new List<float>();

        innerLayer.AddRange(overlapData.ToArray());
        outerLayer.AddRange(overlapData.ToArray());

        int innerCount = 0;
        int outerCount = 2;

        while(alphaGradient > 0.0f)
        //while(innerCount < 1)
        {
            innerLayer = outlineRemove(innerLayer);

            List<int> innerOutline = detectOutlineIndexs(innerLayer);

            for(int k = 0; k < outerCount; k++)
            {
                yield return null;

                List<int> outlineIndex = detectOutlineIndexs(outerLayer);

                for (int i = 0; i < outlineIndex.Count; i++)
                {
                    int index = outlineIndex[i];

                    List<int> aroundIndex = pointToAround[index];

                    for (int j = 0; j < aroundIndex.Count; j++)
                    {
                        int currentIndex = aroundIndex[j];

                        if (outerLayer[currentIndex] == 0.0f)
                        {
                            int cpx = currentIndex % 1024;
                            int cpy = currentIndex / 1024;

                            int targetIndex = minimumDistancePoint(currentIndex, innerOutline);

                            int tpx = targetIndex % 1024;
                            int tpy = targetIndex / 1024;

                            Color targetColor = newTex.GetPixel(cpx, cpy);
                            Color sourceColor = newTex.GetPixel(tpx, tpy);

                            float sourceAlpha = 1.0f - alphaGradient;

                            //if (Random.Range(0.0f, 1.0f) > sourceAlpha)
                            //if(targetColor.grayscale > 0.01f)
                            {
                                //targetColor.r *= sourceAlpha;
                                //targetColor.g *= sourceAlpha;
                                //targetColor.b *= sourceAlpha;

                                //sourceColor.r *= alphaGradient;
                                //sourceColor.g *= alphaGradient;
                                //sourceColor.b *= alphaGradient;

                                //targetColor.r += sourceColor.r;
                                //targetColor.g += sourceColor.g;
                                //targetColor.b += sourceColor.b;

                                targetColor.r = sourceColor.r;
                                targetColor.g = sourceColor.g;
                                targetColor.b = sourceColor.b;

                                targetColor.a = alphaGradient;

                                newTex2.SetPixel(cpx, cpy, targetColor);
                            }

                            outerLayer[currentIndex] = 1.0f;
                        }
                    }
                }

                //alphaGradient -= 0.01f;
                //alphaGradient -= 0.0075f;
                alphaGradient -= 0.005f;
                //alphaGradient -= 0.0025f;

                newTex2.Apply();

                testPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
            }

            outerCount += 2;
        }

        StartCoroutine(smoothTexture());
    }

    int minimumDistancePoint(int index)
    {
        int x = index % 1024;
        int y = index / 1024;

        float minDis = float.MaxValue;
        int minIndex = 0;

        for (int i = 0; i < defaultOutline.Count; i++)
        {
            int currentIndex = defaultOutline[i];

            int cx = currentIndex % 1024;
            int cy = currentIndex / 1024;

            float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

            if (minDis > curDis)
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

                    newTex.SetPixel(cpx, cpy, targetColor);

                    overlapData[currentIndex] = 1.0f;
                }
            }
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
        newTex2 = new Texture2D(width, height);

        Color keepColor2 = Color.black;
        keepColor2.a = 0.0f;

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

                if (grayValue > 0.5f)
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

                newTex2.SetPixel(x, y, keepColor2);
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

            for (int j = 0; j < matchIndexs.Count; j++)
            {
                int currentIndex = matchIndexs[j];

                int cx = currentIndex % 1024;
                int cy = currentIndex / 1024;

                float curDis = Vector2.Distance(new Vector2((float)x, (float)y), new Vector2((float)cx, (float)cy));

                if (curDis > maxValue)
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

        //for (int i = 0; i < 5; i++)
        //{
        //    overlapData = outlineRemove(overlapData);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("export");
            File.WriteAllBytes("NewDPG.jpg", newTex2.EncodeToJPG());
        }

        //if(Input.GetKeyDown(KeyCode.D))
        //{
        //    Debug.Log("draw");
        //    StartCoroutine(newDraw());
        //}

        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    Debug.Log("export");
        //    File.WriteAllBytes("GenerateCombineTexture.jpg", newTex2.EncodeToJPG());
        //}
    }

    public void startProcess()
    {
        if(testPlane == null)
        {
            testPlane = gameObject;
        }

        initialize();
        setPointToAround();
        setPointToGradient();

        Debug.Log("draw");
        StartCoroutine(newDraw());
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
            y1 = p0y + (p0y - y);
        }
        else if (float.IsInfinity(a))
        {
            x1 = p0x + (p0x - x);
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
