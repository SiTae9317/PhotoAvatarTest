using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TemplateMapHPF : MonoBehaviour
{
    public Texture2D dpgMap;

    int width;
    int height;

    List<float> overlapData = new List<float>();
    Dictionary<int, List<int>> pointToAround = new Dictionary<int, List<int>>();

    // Start is called before the first frame update
    void Start()
    {
        if (dpgMap == null)
        {
            dpgMap = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        }

        width = dpgMap.width;
        height = dpgMap.height;

        outlineGen();
        setPointToAround();

        for(int i = 0; i < 5; i++)
        {
            overlapData = outlineRemove(overlapData);
        }

        StartCoroutine(smoothTexture());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void outlineGen()
    {
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = dpgMap.GetPixel(x, y).grayscale;

                if(alpha > 0.0f)
                {
                    alpha = 1.0f;
                }

                overlapData.Add(alpha);
            }
        }
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

    IEnumerator smoothTexture()
    {
        width = dpgMap.width;
        height = dpgMap.height;

        Texture2D newTex = new Texture2D(width, height);

        newTex.SetPixels(dpgMap.GetPixels());

        newTex.Apply();

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

                alpha = dpgMap.GetPixel(x, y).grayscale;

                for (int y1 = minSY; y1 < maxSY; y1++)
                {
                    for (int x1 = minSX; x1 < maxSX; x1++)
                    {
                        int curIndex = y1 * width + x1;

                        if(overlapData[curIndex] > 0.0f)
                        {
                            keepColor = dpgMap.GetPixel(x1, y1);

                            sumPixelCount++;

                            sumR += keepColor.r;
                            sumG += keepColor.g;
                            sumB += keepColor.b;
                        }
                    }
                }

                if(sumPixelCount != Mathf.Pow(halfValue * 2 + 1, 2))
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

                    keepColor = dpgMap.GetPixel(x, y);

                    sumR = keepColor.r - sumR;
                    sumG = keepColor.g - sumG;
                    sumB = keepColor.b - sumB;

                    sumR *= 10.0f;
                    sumG *= 10.0f;
                    sumB *= 10.0f;
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

        File.WriteAllBytes("DPGMapHPF.jpg", newTex.EncodeToJPG());
    }
}
