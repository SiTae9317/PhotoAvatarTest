using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureSmooth : MonoBehaviour
{
    public Texture2D tex;
    private Texture2D newTex;
    private Texture2D hfTex;
    private Texture2D newTex2;
    private Texture2D newTex3;
    private Texture2D dpgTex;

    private int width;
    private int height;

    private List<float> overlapData;

    private float[][] highPassFilter =
    {
        new float[] { -1, -1, -1 },
        new float[] { -1, 8, -1 },
        new float[] { -1, -1, -1 }
    };

    //private float[][] highPassFilter =
    //{
    //    new float[] { -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, 24, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1}
    //};

    //private float[][] highPassFilter =
    //{
    //    new float[] { -1, -1, -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, 48, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1, -1, -1},
    //    new float[] { -1, -1, -1, -1, -1, -1, -1}
    //};

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = calculateHighpassFilter(tex);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("smooth start");

            tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes("DPGMap.jpg"));
            tex.Apply();
            //tex = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            initialize();
            detectMainColor();
            StartCoroutine(smoothTexture());
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("export");
            File.WriteAllBytes("smoothDPG.jpg", newTex3.EncodeToJPG());
        }
    }

    IEnumerator smoothTexture()
    {
        newTex = new Texture2D(width, height);

        newTex.SetPixels(tex.GetPixels());

        newTex.Apply();

        hfTex = calculateHighpassFilter(tex);

        int halfValue = 2;

        Color keepColor = Color.black;

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

                int index = y * width + x;

                alpha = overlapData[index];

                if (overlapData[index] > 0.0f)
                {
                    for (int y1 = minSY; y1 < maxSY; y1++)
                    {
                        for (int x1 = minSX; x1 < maxSX; x1++)
                        {
                            int curIndex = y1 * width + x1;

                            if (overlapData[curIndex] > 0.0f)
                            {
                                keepColor = tex.GetPixel(x1, y1);

                                sumPixelCount++;

                                sumR += keepColor.r;
                                sumG += keepColor.g;
                                sumB += keepColor.b;
                            }
                        }
                    }

                    //if(sumPixelCount != Mathf.Pow(halfValue * 2 + 1, 2))
                    //{
                    //    keepColor = tex.GetPixel(x, y);
                    //    sumR = keepColor.r;
                    //    sumG = keepColor.g;
                    //    sumB = keepColor.b;
                    //}
                    //else
                    //{
                        sumR /= sumPixelCount;
                        sumG /= sumPixelCount;
                        sumB /= sumPixelCount;
                    //}
                }

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;
                keepColor.a = alpha;

                newTex.SetPixel(x, y, keepColor);
            }

            newTex.Apply();

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex;
        }

        StartCoroutine(sharpenTexture());
        //StartCoroutine(combineTexture());
    }

    IEnumerator combineTexture()
    {
        newTex2 = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            yield return null;

            for (int x = 0; x < width; x++)
            {
                Color keepColor = newTex.GetPixel(x, y);
                Color sourceColor = tex.GetPixel(x, y);

                float sourceAlpha = hfTex.GetPixel(x,y).grayscale;

                if(sourceAlpha > 0.75f)
                {
                    sourceAlpha = 0.0f;
                }
                else
                {
                    sourceAlpha *= 2.0f;
                }

                float smoothAlpha = 1.0f - sourceAlpha;

                //if (adderValue > 0.1f && adderValue < 0.25f)
                //{
                //    //adderValue *= 2.0f;

                //    adderValue += 1.0f;

                //    keepColor.r *= adderValue;
                //    keepColor.g *= adderValue;
                //    keepColor.b *= adderValue;
                //}

                keepColor.r *= smoothAlpha;
                keepColor.g *= smoothAlpha;
                keepColor.b *= smoothAlpha;

                sourceColor.r *= sourceAlpha;
                sourceColor.g *= sourceAlpha;
                sourceColor.b *= sourceAlpha;

                keepColor.r += sourceColor.r;
                keepColor.g += sourceColor.g;
                keepColor.b += sourceColor.b;

                newTex2.SetPixel(x, y, keepColor);
            }

            newTex2.Apply();

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
        }
    }

    IEnumerator sharpenTexture()
    {
        newTex2 = new Texture2D(width, height);

        newTex2.SetPixels(newTex.GetPixels());

        newTex2.Apply();

        int halfValue = 2;

        Color keepColor = Color.black;
        Color inputColor = Color.black;

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

                int index = y * width + x;

                alpha = overlapData[index];

                if (overlapData[index] > 0.0f)
                {
                    for (int y1 = minSY; y1 < maxSY; y1++)
                    {
                        for (int x1 = minSX; x1 < maxSX; x1++)
                        {
                            int curIndex = y1 * width + x1;

                            if (overlapData[curIndex] > 0.0f)
                            {
                                keepColor = newTex.GetPixel(x1, y1);

                                sumPixelCount++;

                                sumR += keepColor.r;
                                sumG += keepColor.g;
                                sumB += keepColor.b;
                            }
                        }
                    }

                    if (sumPixelCount != Mathf.Pow(halfValue * 2 + 1, 2))
                    {
                        sumR = 0.0f;
                        sumG = 0.0f;
                        sumB = 0.0f;
                    }
                    else
                    {
                        sumR /= sumPixelCount;
                        sumG /= sumPixelCount;
                        sumB /= sumPixelCount;
                    }
                }

                inputColor = newTex.GetPixel(x, y);

                float inputR = inputColor.r;
                float inputG = inputColor.g;
                float inputB = inputColor.b;

                if(sumR != 0.0f && sumG != 0.0f && sumB != 0.0f)
                {
                    sumR = inputR - sumR;
                    sumG = inputG - sumG;
                    sumB = inputB - sumB;
                }

                sumR *= 5.0f;
                sumG *= 5.0f;
                sumB *= 5.0f;

                //float delta = 0.3f;

                //if (sumR < delta || sumG < delta || sumB < delta)
                //{
                //    sumR = 0.0f;
                //    sumG = 0.0f;
                //    sumB = 0.0f;
                //}

                //sumR = inputR + (sumR * 0.5f);
                //sumG = inputG + (sumG * 0.5f);
                //sumB = inputB + (sumB * 0.5f);

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;
                keepColor.a = alpha;

                newTex2.SetPixel(x, y, keepColor);
            }

            newTex2.Apply();

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex2;
        }

        StartCoroutine(sharpenTexture2());
    }

    IEnumerator sharpenTexture2()
    {
        newTex3 = new Texture2D(width, height);

        newTex3.SetPixels(newTex2.GetPixels());

        newTex3.Apply();

        int halfValue = 2;

        Color keepColor = Color.black;
        Color inputColor = Color.black;

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

                int index = y * width + x;

                alpha = overlapData[index];

                if (overlapData[index] > 0.0f)
                {
                    for (int y1 = minSY; y1 < maxSY; y1++)
                    {
                        for (int x1 = minSX; x1 < maxSX; x1++)
                        {
                            int curIndex = y1 * width + x1;

                            if (overlapData[curIndex] > 0.0f)
                            {
                                keepColor = newTex2.GetPixel(x1, y1);

                                sumPixelCount++;

                                sumR += keepColor.r;
                                sumG += keepColor.g;
                                sumB += keepColor.b;
                            }
                        }
                    }

                    if (sumPixelCount != Mathf.Pow(halfValue * 2 + 1, 2))
                    {
                        sumR = 0.0f;
                        sumG = 0.0f;
                        sumB = 0.0f;
                    }
                    else
                    {
                        sumR /= sumPixelCount;
                        sumG /= sumPixelCount;
                        sumB /= sumPixelCount;
                    }
                }

                inputColor = newTex.GetPixel(x, y);

                float inputR = inputColor.r;
                float inputG = inputColor.g;
                float inputB = inputColor.b;

                //if (sumR != 0.0f && sumG != 0.0f && sumB != 0.0f)
                //{
                //    sumR = inputR - sumR;
                //    sumG = inputG - sumG;
                //    sumB = inputB - sumB;
                //}

                //sumR *= 10.0f;
                //sumG *= 10.0f;
                //sumB *= 10.0f;

                //float delta = 0.3f;

                //if (sumR < delta || sumG < delta || sumB < delta)
                //{
                //    sumR = 0.0f;
                //    sumG = 0.0f;
                //    sumB = 0.0f;
                //}

                sumR = inputR + (sumR * 0.5f);
                sumG = inputG + (sumG * 0.5f);
                sumB = inputB + (sumB * 0.5f);

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;
                keepColor.a = alpha;

                newTex3.SetPixel(x, y, keepColor);
            }

            newTex3.Apply();

            gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex3;
        }

        for(int y = 0; y < height; y++)
        {
            yield return null;

            for (int x = 0; x < width; x++)
            {
                keepColor = newTex3.GetPixel(x, y);

                if(keepColor.a == 0.0f)
                {
                    newTex3.SetPixel(x, y, tex.GetPixel(x, y));
                }
            }

            newTex3.Apply();
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = newTex3;
        }

        //File.WriteAllBytes("smoothDPG.jpg", newTex3.EncodeToJPG());

        gameObject.GetComponent<SymmetryPoint2>().startProcess();

        Debug.Log("end");
    }

    void initialize()
    {
        overlapData = new List<float>();

        Texture2D overlapTex = new Texture2D(1, 1);
        overlapTex.LoadImage(File.ReadAllBytes("OverlapTex.jpg"));

        overlapTex.Apply();

        width = tex.width;
        height = tex.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
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
            }
        }
    }

    Texture2D calculateHighpassFilter(Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height;

        Texture2D tex2 = new Texture2D(width, height);

        int filterCount = highPassFilter.Length;
        int offset = filterCount / 2;

        int count = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sumValue = 0.0f;

                if (x >= offset && x <= (width - offset) && y >= offset && y <= (height - offset))
                {
                    for (int i = 0; i < filterCount; i++)
                    {
                        for (int j = 0; j < filterCount; j++)
                        {
                            Color keepColor = tex.GetPixel(x + (i - 1), y + (j - 1));
                            sumValue += (keepColor.r * 0.299f + keepColor.g * 0.587f + keepColor.b * 0.114f) * highPassFilter[i][j];
                        }
                    }
                }

                sumValue = Mathf.Abs(sumValue);
                
                Color newColor = new Color(sumValue, sumValue, sumValue);
                tex2.SetPixel(x, y, newColor);
            }
        }

        Debug.Log(count);
        tex2.Apply();

        return tex2;
    }

    void detectMainColor()
    {
        Texture2D dpgTex = new Texture2D(1, 1);

        dpgTex.LoadImage(File.ReadAllBytes("DPG.jpg"));

        dpgTex.Apply();

        Dictionary<Color, int>  colorCount = new Dictionary<Color, int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int curIndex = y * width + x;

                if (overlapData[curIndex] > 0.0f)
                {
                    Color keepColor = dpgTex.GetPixel(x, y);

                    if (colorCount.ContainsKey(keepColor))
                    {
                        colorCount[keepColor] += 1;
                    }
                    else
                    {
                        colorCount.Add(keepColor, 1);
                    }
                }
            }
        }

        Dictionary<Color, int>.Enumerator colorEnum = colorCount.GetEnumerator();

        Debug.Log(colorCount.Count);

        Color mainColor = Color.black;

        int maxColor = 0;

        while (colorEnum.MoveNext())
        {
            if (maxColor < colorEnum.Current.Value)
            {
                mainColor = colorEnum.Current.Key;
                maxColor = colorEnum.Current.Value;
            }
        }

        float[] sourceColor = { 0.7333333f, 0.6235294f, 0.5686275f };
        float[] dpgMainColor = { 0.0f, 0.0f, 0.0f };
        float[] diff = { 0.0f, 0.0f, 0.0f };

        dpgMainColor[0] = mainColor.r;
        dpgMainColor[1] = mainColor.g;
        dpgMainColor[2] = mainColor.b;

        Color tempColor1 = tex.GetPixel(350, 1024 - 640);
        Color tempColor2 = tex.GetPixel(650, 1024 - 640);
        Color dpgColor1 = dpgTex.GetPixel(350, 1024 - 640);
        Color dpgColor2 = dpgTex.GetPixel(650, 1024 - 640);

        sourceColor[0] = tempColor1.r + tempColor2.r;
        sourceColor[1] = tempColor1.g + tempColor2.g;
        sourceColor[2] = tempColor1.b + tempColor2.b;

        dpgMainColor[0] = dpgColor1.r + dpgColor2.r;
        dpgMainColor[1] = dpgColor1.g + dpgColor2.g;
        dpgMainColor[2] = dpgColor1.b + dpgColor2.b;

        //Color tempColor = tex.GetPixel(420, 1024 - 600);
        //Color dpgColor = dpgTex.GetPixel(420, 1024 - 600);

        ////Color tempColor = tex.GetPixel(512, 592);
        ////Color dpgColor = dpgTex.GetPixel(512, 592);

        //sourceColor[0] = tempColor.r;
        //sourceColor[1] = tempColor.g;
        //sourceColor[2] = tempColor.b;

        //dpgMainColor[0] = dpgColor.r;
        //dpgMainColor[1] = dpgColor.g;
        //dpgMainColor[2] = dpgColor.b;

        for (int i = 0; i < 3; i++)
        {
            diff[i] = dpgMainColor[i] / 2.0f - sourceColor[i] / 2.0f;
            //diff[i] -= 0.1f;
        }

        Texture2D dpgTempTex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                int curIndex = y * width + x;

                Color keepColor = tex.GetPixel(x, y);

                float sumR = keepColor.r;
                float sumG = keepColor.g;
                float sumB = keepColor.b;

                sumR += diff[0];
                sumG += diff[1];
                sumB += diff[2];

                sumR = Mathf.Min(Mathf.Max(0.0f, sumR), 1.0f);
                sumG = Mathf.Min(Mathf.Max(0.0f, sumG), 1.0f);
                sumB = Mathf.Min(Mathf.Max(0.0f, sumB), 1.0f);

                keepColor.r = sumR;
                keepColor.g = sumG;
                keepColor.b = sumB;

                //keepColor.r += diff[0];
                //keepColor.g += diff[1];
                //keepColor.b += diff[2];

                if (overlapData[curIndex] > 0.0f)
                {
                    keepColor = dpgTex.GetPixel(x, y);
                }

                dpgTempTex.SetPixel(x, y, keepColor);
            }
        }

        dpgTempTex.Apply();

        tex = dpgTempTex;
    }
}
