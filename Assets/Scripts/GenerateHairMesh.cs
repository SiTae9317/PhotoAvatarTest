using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateHairMesh : MonoBehaviour
{
    public GameObject point;
    public Color manyUseColor = Color.black;
    //public Texture2D testTex;
    int width;
    int height;

    private Texture2D hairTex;
    private Texture2D keepHairTex;
    Dictionary<int, List<int>> pointToAround = new Dictionary<int, List<int>>();
    List<float> overlapData;
    List<List<Vector3>> verticesVectors;

    // Start is called before the first frame update
    void Start()
    {
        //overlapData = new List<float>();

        //hairTex = new Texture2D(1, 1);
        //hairTex.LoadImage(File.ReadAllBytes("hairTex.png"));
        //hairTex.Apply();

        //width = hairTex.width;
        //height = hairTex.height;

        //setPointToAround(width, height);

        //testHSVChange();

        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = hairTex;
    }

    void testHSVChange()
    {
        int divCount = 60;
        int divVal = 360 / divCount;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color keepColor = hairTex.GetPixel(x, y);

                float sumR = keepColor.r;
                float sumG = keepColor.g;
                float sumB = keepColor.b;

                float keepAlpha = keepColor.a;

                for(int i = 0; i < 3; i++)
                {
                    HSV hsv = ColorUtils.RGBToHSV(keepColor);
                    float adderVal = 1.0f + hsv.s;
                    hsv.v = Mathf.Min(1.0f, hsv.v * adderVal);
                    keepColor = ColorUtils.HSVToRGB(hsv);
                    keepColor.a = keepAlpha;
                }

                sumR += keepColor.r;
                sumG += keepColor.g;
                sumB += keepColor.b;

                sumR /= 2.0f;
                sumG /= 2.0f;
                sumB /= 2.0f;

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;

                //HSV hsv = ColorUtils.RGBToHSV(keepColor);
                //hsv.v = 1.0f;

                //if(hsv.s > 0.5f)
                //{
                //    keepColor = Color.black;
                //}
                //else
                //{
                //    hsv.s = 1.0f;

                //    if(hsv.h >= 0.0f && hsv.h < 60.0f)
                //    {
                //        hsv.h = 0.0f;
                //    }
                //    else if(hsv.h >= 60.0f && hsv.h < 180.0f)
                //    {
                //        hsv.h = 120.0f;
                //    }
                //    else if(hsv.h > 180.0f && hsv.h < 300.0f)
                //    {
                //        hsv.h = 240.0f;
                //    }
                //    else
                //    {
                //        hsv.h = 0.0f;
                //    }

                //    keepColor = ColorUtils.HSVToRGB(hsv);
                //}

                //int curIndex = y * width + x;

                //List<int> aroundIndexs = pointToAround[curIndex];

                //float sumDis = 0.0f;

                //for (int i = 0; i < aroundIndexs.Count; i++)
                //{
                //    int aroundIndex = aroundIndexs[i];

                //    if (aroundIndex == curIndex)
                //    {
                //        continue;
                //    }

                //    int cx = aroundIndex % width;
                //    int cy = aroundIndex / width;

                //    HSV leftHSV = ColorUtils.RGBToHSV(keepColor);
                //    HSV rightHSV = ColorUtils.RGBToHSV(hairTex.GetPixel(cx, cy));

                //    leftHSV.v = 1.0f;
                //    rightHSV.v = 1.0f;

                //    sumDis += ColorUtils.euclidean(ColorUtils.HSVToRGB(leftHSV), ColorUtils.HSVToRGB(rightHSV), EculideanModel.HSV_HUE);
                //}

                //sumDis /= (aroundIndexs.Count - 1);

                //keepColor.r = sumDis;
                //keepColor.g = sumDis;
                //keepColor.b = sumDis;

                //keepColor.a = 1.0f;

                //HSV hsv = HSVTest.RGBToHSV(keepColor);

                //float h;
                //float s;
                //float v;

                //Color.RGBToHSV(keepColor, out h, out s, out v);

                //keepColor.r = hsv.h / 360.0f;
                //keepColor.g = hsv.s;
                //keepColor.b = hsv.v;

                //if(keepColor.a > 0.0f)
                //{
                //    HSV hsv = HSVTest.RGBToHSV(keepColor);
                //    hsv.v = 1.0f;
                //    keepColor = HSVTest.HSVToRGB(hsv);
                //}


                ///// normalize rg
                ///

                //float r = keepColor.r;
                //float g = keepColor.g;
                //float b = keepColor.b;

                //r = r / (r + g + b);
                //g = g / (r + g + b);
                //b = 0.0f;

                //keepColor.r = r;
                //keepColor.g = g;
                //keepColor.b = b;

                hairTex.SetPixel(x, y, keepColor);
            }
        }

        hairTex.Apply();
    }

    void process()
    {
        //genGammaLinear();
        //return;
        //emissionMapGen();

        overlapData = new List<float>();
        setPointToAround(224);

        hairTex = new Texture2D(1, 1);
        hairTex.LoadImage(File.ReadAllBytes("hairTex.png"));
        hairTex.Apply();

        width = hairTex.width;
        height = hairTex.height;

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = hairTex;

        //testHSVChange();

        detectMainColor();

        hairTexResize();
        hairTexSmooth();
        hairMeshGen();

        //for (int i = 0; i < 3; i++)
        //{
        //    overlapData = outlineRemove(overlapData);
        //}

        overlapToTexture();

        hairPixelDetect();

        //StartCoroutine(hairTexResize());
        //StartCoroutine(hairMeshGen());

        //List<List<Vector3>> testData = new List<List<Vector3>>();

        //for (int i = 0; i < 5; i++)
        //{
        //    List<Vector3> yDatas = new List<Vector3>();

        //    for (int j = 0; j < 5; j++)
        //    //for(int j = -5; j <= 0; j++)
        //    {
        //        yDatas.Add(new Vector3(i, j, 0.0f));
        //    }

        //    testData.Add(yDatas);
        //}

        //genHairMesh(testData);

        smoothCombine();

        genHairMesh(verticesVectors);
        //grayscaleTexture();

        // keep
        //manyColorCalc();
    }

    void smoothCombine()
    {
        int keepWidth = keepHairTex.width;
        int keepHeight = keepHairTex.height;

        int hairWidth = hairTex.width;
        int hairHeight = hairTex.height;

        float ratioX = (float)keepWidth / (float)hairWidth;
        float ratioY = (float)keepHeight / (float)hairHeight;

        for (int y = 0; y < keepHeight; y++)
        {
            for(int x = 0; x < keepWidth; x++)
            {
                Color keepColor = keepHairTex.GetPixel(x, y);

                int curX = (int)((float)x / ratioX);
                int curY = (int)((float)y / ratioY);

                //if(keepColor.a != 1.0f)
                {
                    float sumAlpha = hairTex.GetPixel(curX, curY).a;
                    sumAlpha *= 2.0f;
                    sumAlpha = Mathf.Min(1.0f, sumAlpha);
                    //sumAlpha = Mathf.Pow(sumAlpha, 2.0f);
                    //sumAlpha += keepColor.a;
                    //sumAlpha /= 2.0f;
                    keepColor.a = sumAlpha;
                    keepHairTex.SetPixel(x, y, keepColor);
                }
            }
        }

        keepHairTex.Apply();
    }

    void manyColorCalc()
    {
        Texture2D faceLoad = new Texture2D(1, 1);
        faceLoad.LoadImage(File.ReadAllBytes("faceTex.png"));
        faceLoad.Apply();

        Texture2D bgLoad = new Texture2D(1, 1);
        bgLoad.LoadImage(File.ReadAllBytes("bgTex.png"));
        bgLoad.Apply();

        Texture2D curTex = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        int curWidth = curTex.width;
        int curHeight = curTex.height;

        Color keepColor = Color.black;

        for (int y = 0; y < curHeight; y++)
        {
            for (int x = 0; x < curWidth; x++)
            {
                keepColor = curTex.GetPixel(x, y);
                //keepColor = keepColor.gamma;

                float sourceR = keepColor.r;
                float sourceG = keepColor.g;
                float sourceB = keepColor.b;

                float mainR = manyUseColor.r;
                float mainG = manyUseColor.g;
                float mainB = manyUseColor.b;

                sourceR = Mathf.Abs(sourceR - mainR);
                sourceG = Mathf.Abs(sourceG - mainG);
                sourceB = Mathf.Abs(sourceB - mainB);

                float grayValue = 0.0f;


                //if(ColorUtils.euclidean(keepColor, manyUseColor, EculideanModel.HSV_CONIC) > 0.4f)
                //if(ColorUtils.euclidean(keepColor, manyUseColor, EculideanModel.RGB) > 0.4f)
                if (sourceR > 0.3f || sourceG > 0.3f || sourceB > 0.3f)
                {
                    //float avgRGB = sourceR + sourceG + sourceB;
                    //avgRGB /= 3.0f;

                    //sourceR = Mathf.Abs(sourceR - avgRGB);
                    //sourceG = Mathf.Abs(sourceG - avgRGB);
                    //sourceB = Mathf.Abs(sourceB - avgRGB);

                    //if (sourceR < 0.05f && sourceG < 0.05f && sourceB < 0.05f)
                    //{
                    //    grayValue = 0.0f;
                    //}
                    //else
                    //{
                        grayValue = 1.0f;
                    //}
                }

                if(faceLoad.GetPixel(x,y).a > 0.0f || bgLoad.GetPixel(x,y).a > 0.0f)
                {
                    grayValue = 1.0f;
                }

                //Color calcColor = keepColor - manyUseColor;

                ////if(grayValue > 0.25f)
                ////{
                ////    keepColor.r = 0.0f;
                ////    keepColor.g = 0.0f;
                ////    keepColor.b = 0.0f;
                ////}

                ////keepColor = keepColor.linear * 0.5f + keepColor.gamma * 0.5f;

                ////float r = keepColor.r;
                ////float g = keepColor.g;
                ////float b = keepColor.b;

                ////float gr = keepColor.gamma.r;
                ////float gg = keepColor.gamma.g;
                ////float gb = keepColor.gamma.b;

                ////keepColor.r = keepColor.r + (gr - r) * 0.5f;
                ////keepColor.g = keepColor.g + (gg - g) * 0.5f;
                ////keepColor.b = keepColor.b + (gb - b) * 0.5f;

                //float grayValue = calcColor.grayscale;

                //if(grayValue > 0.3f)
                //{
                //    grayValue = 1.0f;
                //}

                keepColor.a = 1.0f - grayValue;

                curTex.SetPixel(x, y, keepColor);
            }
        }

        curTex.Apply();

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = curTex;
    }

    void detectMainColor()
    {
        Color keepColor = Color.black;

        Dictionary<Color, int> colorCounting = new Dictionary<Color, int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                keepColor = hairTex.GetPixel(x, y);

                if(keepColor.a > 0.0f)
                {
                    if(colorCounting.ContainsKey(keepColor))
                    {
                        colorCounting[keepColor]++;
                    }
                    else
                    {
                        colorCounting.Add(keepColor, 1);
                    }
                }
            }
        }

        Dictionary<Color, int>.Enumerator colorEnum = colorCounting.GetEnumerator();

        int manyCount = int.MinValue;

        while(colorEnum.MoveNext())
        {
            if(manyCount < colorEnum.Current.Value)
            {
                manyCount = colorEnum.Current.Value;
                manyUseColor = colorEnum.Current.Key;
            }
        }

        //StartCoroutine(hairMeshGen());
    }

    //void genGammaLinear()
    //{
    //    //hairTex = new Texture2D(1, 1);
    //    //hairTex.LoadImage(File.ReadAllBytes("hairTex.png"));
    //    //hairTex.Apply();

    //    int newWidth = testTex.width;
    //    int newHeight = testTex.height;

    //    Texture2D ori = new Texture2D(newWidth, newHeight);
    //    Texture2D lin = new Texture2D(newWidth, newHeight);
    //    Texture2D gam = new Texture2D(newWidth, newHeight);

    //    for (int y = 0; y < newHeight; y++)
    //    {
    //        for(int x = 0; x < newWidth; x++)
    //        {
    //            Color keepColor = testTex.GetPixel(x, y);
    //            keepColor.a = 1.0f;

    //            ori.SetPixel(x, y, keepColor);
    //            lin.SetPixel(x, y, keepColor.linear);
    //            gam.SetPixel(x, y, keepColor.gamma);
    //        }
    //    }

    //    ori.Apply();
    //    lin.Apply();
    //    gam.Apply();

    //    File.WriteAllBytes("originalTex.jpg", ori.EncodeToJPG());
    //    File.WriteAllBytes("linearTex.jpg", lin.EncodeToJPG());
    //    File.WriteAllBytes("gammaTex.jpg", gam.EncodeToJPG());
    //}

    void grayscaleTexture()
    {
        Texture2D curTex = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        int curWidth = curTex.width;
        int curHeight = curTex.height;

        Color keepColor = Color.black;

        for(int y = 0; y < curHeight; y++)
        {
            for(int x = 0; x <  curWidth; x++)
            {
                keepColor = curTex.GetPixel(x, y);
                //keepColor = keepColor.gamma;
                float grayValue = keepColor.grayscale;

                //if(grayValue > 0.25f)
                //{
                //    keepColor.r = 0.0f;
                //    keepColor.g = 0.0f;
                //    keepColor.b = 0.0f;
                //}

                //keepColor = keepColor.linear * 0.5f + keepColor.gamma * 0.5f;

                //float r = keepColor.r;
                //float g = keepColor.g;
                //float b = keepColor.b;

                //float gr = keepColor.gamma.r;
                //float gg = keepColor.gamma.g;
                //float gb = keepColor.gamma.b;

                //keepColor.r = keepColor.r + (gr - r) * 0.5f;
                //keepColor.g = keepColor.g + (gg - g) * 0.5f;
                //keepColor.b = keepColor.b + (gb - b) * 0.5f;

                keepColor.a = 1.0f - grayValue;

                curTex.SetPixel(x, y, keepColor);
            }
        }

        curTex.Apply();

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = curTex;
    }

    void emissionMapGen()
    {
        Texture2D dpgTex = new Texture2D(1, 1);
        dpgTex.LoadImage(File.ReadAllBytes("DPGMap.jpg"));
        dpgTex.Apply();

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                Color keepColor = dpgTex.GetPixel(x, y);
                float grayValue = keepColor.grayscale;

                grayValue = Mathf.Max(0.0f, grayValue - 0.65f);

                keepColor.r = grayValue;
                keepColor.g = grayValue;
                keepColor.b = grayValue;

                dpgTex.SetPixel(x, y, keepColor);
            }
        }

        dpgTex.Apply();
        File.WriteAllBytes("emissionMap.jpg", dpgTex.EncodeToJPG());

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = dpgTex;
    }

    void genHairMesh(List<List<Vector3>> data)
    {
        Mesh mesh = new Mesh();

        Dictionary<Vector3, int> hashVertices = new Dictionary<Vector3, int>();

        int vertIndex = 0;

        for(int i = 0; i < data.Count; i++)
        {
            for(int j = 0; j < data[i].Count; j++)
            {
                Vector3 curVec = data[i][j];

                if (hashVertices.ContainsKey(curVec))
                {
                    Debug.Log("error");
                }
                else
                {
                    hashVertices.Add(curVec, vertIndex);
                    vertIndex++;
                }
            }
        }

        Debug.Log(hashVertices.Count + " " + vertIndex);

        List<int> triangles = new List<int>();

        //for (int i = 98; i < 99; i++)
        for (int i = 0; i < data.Count - 1; i++)
        {
            //if(i == 98 || i == 101 || i == 103 || i == 104 || i == 105)
            //{
            //    continue;
            //}
            List<Vector3> leftData = data[i];
            List<Vector3> rightData = data[i+1];

            int leftIndex = 0;
            int rightIndex = 0;

            int maxLeftIndex = Mathf.Max(0, leftData.Count - 1);
            int maxRighttIndex = Mathf.Max(0, rightData.Count - 1);

            //int loop = 0;

            //Debug.Log(leftData[maxLeftIndex].y);
            //Debug.Log(rightData[maxRighttIndex].y);

            while (true)
            {
                //if(loop == 62)
                //{
                //    break;
                //}

                if (leftIndex == maxLeftIndex && rightIndex == maxRighttIndex)
                {
                    break;
                }

                Vector3 leftVec = leftData[leftIndex];
                Vector3 rightVec = rightData[rightIndex];

                if (leftVec.y <= rightVec.y)
                {
                    if(leftIndex == maxLeftIndex)
                    {
                        triangles.Add(hashVertices[leftData[leftIndex]]);
                        triangles.Add(hashVertices[rightData[rightIndex + 1]]);
                        triangles.Add(hashVertices[rightData[rightIndex]]);
                        rightIndex = Mathf.Min(rightIndex + 1, maxRighttIndex);
                    }
                    else
                    {
                        triangles.Add(hashVertices[leftData[leftIndex]]);
                        triangles.Add(hashVertices[leftData[leftIndex + 1]]);
                        triangles.Add(hashVertices[rightData[rightIndex]]);
                        leftIndex = Mathf.Min(leftIndex + 1, maxLeftIndex);
                    }
                }
                else if (leftVec.y > rightVec.y)
                {
                    if(rightIndex == maxRighttIndex)
                    {
                        triangles.Add(hashVertices[rightData[rightIndex]]);
                        triangles.Add(hashVertices[leftData[leftIndex]]);
                        triangles.Add(hashVertices[leftData[leftIndex + 1]]);
                        leftIndex = Mathf.Min(leftIndex + 1, maxLeftIndex);
                    }
                    else
                    {
                        triangles.Add(hashVertices[rightData[rightIndex]]);
                        triangles.Add(hashVertices[leftData[leftIndex]]);
                        triangles.Add(hashVertices[rightData[rightIndex + 1]]);
                        rightIndex = Mathf.Min(rightIndex + 1, maxRighttIndex);
                    }
                }
                //loop++;
            }
        }

        List<Vector3> vertices = new List<Vector3>();

        List<Vector2> uv = new List<Vector2>();

        Dictionary<Vector3, int>.Enumerator vertEnum = hashVertices.GetEnumerator();

        while(vertEnum.MoveNext())
        {
            Vector3 curVec = vertEnum.Current.Key;
            vertices.Add(curVec);

            uv.Add(new Vector2(curVec.x / 224.0f, curVec.y / 224.0f));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = new Vector3[vertices.Count];
        mesh.uv = uv.ToArray();

        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = keepHairTex;
    }

    void overlapToTexture()
    {
        Texture2D newTex = new Texture2D(224, 224);

        Color keepColor = Color.black;
        keepColor.a = 0.0f;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                int curIndex = y * width + x;

                if(overlapData[curIndex] > 0.0f)
                {
                    newTex.SetPixel(x, y, hairTex.GetPixel(x, y));
                }
                else
                {
                    newTex.SetPixel(x, y, keepColor);
                }
            }
        }

        newTex.Apply();

        hairTex = newTex;

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = hairTex;
    }

    void hairTexSmooth()
    {
        Texture2D smoothTex = new Texture2D(224, 224);

        int newWidth = 224;
        int newHeight = 224;

        int halfValue = 15;
        //int halfValue = 15;
        //int halfValue = 3;

        int zeroCount = 0;
        int oneCount = 0;
        int otherCount = 0;
        Color keepColor = Color.black;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int minX = Mathf.Max(0, x - halfValue);
                int maxX = Mathf.Min(newWidth, x + halfValue + 1);

                int minY = Mathf.Max(0, y - halfValue);
                int maxY = Mathf.Min(newHeight, y + halfValue + 1);

                int aroundPixelCount = 0;

                float sumR = 0.0f;
                float sumG = 0.0f;
                float sumB = 0.0f;
                float sumA = 0.0f;

                for (int y1 = minY; y1 < maxY; y1++)
                {
                    for(int x1 = minX; x1 < maxX; x1++)
                    {
                        keepColor = hairTex.GetPixel(x1, y1);

                        sumR += keepColor.r;
                        sumG += keepColor.g;
                        sumB += keepColor.b;
                        sumA += keepColor.a;

                        aroundPixelCount++;
                    }
                }

                sumR /= aroundPixelCount;
                sumG /= aroundPixelCount;
                sumB /= aroundPixelCount;
                sumA /= aroundPixelCount;

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;
                keepColor.a = sumA;

                if(sumA == 0.0f)
                {
                    zeroCount++;
                }
                else if(sumA == 1.0f)
                {
                    oneCount++;
                }
                else
                {
                    otherCount++;
                }

                smoothTex.SetPixel(x, y, keepColor);
            }
        }

        Debug.Log(zeroCount + " " + oneCount + " " + otherCount);

        smoothTex.Apply();

        hairTex = smoothTex;

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = hairTex;

        //StartCoroutine(hairMeshGen());
    }

    void hairTexResize()
    {
        Texture2D resizeTex = new Texture2D(224, 224);

        int newWidth = 224;
        int newHeight = 224;

        float deltaX = (float)width / (float)newWidth;
        float deltaY = (float)height / (float)newHeight;

        //yield return null;

        int zeroCount = 0;
        int oneCount = 0;
        int otherCount = 0;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int curX = (int)(x * deltaX);
                int curY = (int)(y * deltaY);

                Color keepColor = hairTex.GetPixel(curX, curY);
                if(keepColor.a == 0.0f)
                {
                    zeroCount++;
                }
                else if(keepColor.a == 1.0f)
                {
                    oneCount++;
                }
                else
                {
                    otherCount++;
                }

                resizeTex.SetPixel(x, y, keepColor);
            }
        }

        Debug.Log(zeroCount + " " + oneCount + " " + otherCount);

        resizeTex.Apply();

        keepHairTex = hairTex;

        hairTex = resizeTex;

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = hairTex;

        //StartCoroutine(hairMeshGen());
    }

    void hairMeshGen()
    {
        width = hairTex.width;
        height = hairTex.height;

        Debug.Log(width + " " + height);

        int zeroCount = 0;
        int oneCount = 0;
        int otherCount = 0;

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color keepColor = hairTex.GetPixel(x, y);
                if(keepColor.a == 0.0f)
                {
                    zeroCount++;
                }
                else if(keepColor.a == 1.0f)
                {
                    oneCount++;
                }
                else
                {
                    otherCount++;
                }
                overlapData.Add(keepColor.a);
            }
        }

        Debug.Log(zeroCount + " " + oneCount + " " + otherCount);

        //for (int i = 0; i < 10; i++)
        //{
        //    overlapData = outlineRemove(overlapData);
        //}
    }

    void hairPixelDetect()
    {
        width = hairTex.width;
        height = hairTex.height;

        Dictionary<float, List<float>> verticesData = new Dictionary<float, List<float>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(hairTex.GetPixel(x, y).a > 0.0f)
                {
                    if(verticesData.ContainsKey((float)x))
                    {
                        verticesData[x].Add((float)y);
                    }
                    else
                    {
                        List<float> yList = new List<float>();
                        yList.Add((float)y);

                        verticesData.Add((float)x, yList);
                    }
                }
            }
        }

        Debug.Log(verticesData.Count);

        verticesVectors = new List<List<Vector3>>();

        Dictionary<float, List<float>>.Enumerator verEnum = verticesData.GetEnumerator();

        float avgX = 0.0f;
        float avgY = 0.0f;
        float topAvgY = 0.0f;


        while (verEnum.MoveNext())
        {
            float x = verEnum.Current.Key;
            List<float> ys = verEnum.Current.Value;

            avgX += x;
            topAvgY += ys[ys.Count - 1];

            List<Vector3> combineVectors = new List<Vector3>();

            float curAvgY = 0.0f;

            for(int i = 0; i < ys.Count; i++)
            {
                curAvgY += ys[i];
                combineVectors.Add(new Vector3(x, ys[i], 0.0f));
            }

            curAvgY /= (float)(ys.Count);

            avgY += curAvgY;

            verticesVectors.Add(combineVectors);
        }

        topAvgY /= (float)(verticesData.Count);

        avgX /= (float)(verticesData.Count);
        avgY /= (float)(verticesData.Count);

        gameObject.transform.position = new Vector3(0.0f, 0.0f, -10.0f);

        if (point != null)
        {
            GameObject avgP = Instantiate(point);
            avgP.name = "avg Point";
            avgP.transform.position = new Vector3(avgX, avgY, 0.0f);
            GameObject avgTopP = Instantiate(point);
            avgTopP.name = "avg Top Point";
            avgTopP.transform.position = new Vector3(avgX, topAvgY, 0.0f);

            Debug.Log(avgX + " " + avgY + " " + topAvgY);

            //gameObject.transform.position = Vector3.zero;
            //gameObject.transform.localScale = Vector3.one;
            point.SetActive(false);
            //Debug.Log(pixelCount);
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

                if(currentAroundValues.Count != 9)
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
        if(Input.GetKeyDown(KeyCode.R))
        {
            process();
        }

        //if(Input.GetKeyDown(KeyCode.R))
        //{
        //    overlapData = outlineRemove(overlapData);
        //    overlapToTexture();
        //}
    }
}
