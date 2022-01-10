using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestTemplateMap : MonoBehaviour
{
    public Texture2D tex;
    public Color[] rankColor = new Color[10];
    private SortedList sortColor;


    float[] source = { 0.7333333f, 0.6235294f, 0.5686275f };
    //float[] source = { 0.7568628f, 0.627451f, 0.5529412f };
    float[] target = { 0.9098039f, 0.8078431f, 0.7490196f };
    //float[] target = { 0.7137255f, 0.6039216f, 0.6588235f };
    
    //float[] target = { 0.172549f, 0.1098039f, 0.05882353f };
    //float[] diff = { 0.1764706f, 0.1843137f, 0.1803921f };
    float[] diff = new float[3];

    Dictionary<Color, int> colorCount;
    // Start is called before the first frame update
    void Start()
    {
        //Texture2D newTex = new Texture2D(1, 1);
        //newTex.LoadImage(File.ReadAllBytes("newBase.jpg"));

        //for(int y = 0; y < 1024; y++)
        //{
        //    for(int x = 0; x < 1024; x++)
        //    {
        //        Color keepColor = tex.GetPixel(x, y);
        //        Color keepColor2 = newTex.GetPixel(x, y);

        //        keepColor.r += keepColor2.r;
        //        keepColor.g += keepColor2.g;
        //        keepColor.b += keepColor2.b;

        //        newTex.SetPixel(x, y, keepColor);
        //    }
        //}

        //newTex.Apply();

        //File.WriteAllBytes("newCombineTex.jpg", newTex.EncodeToJPG());

        //genBase();
        genTex();
        //test();
        //genBase();
    }

    void genTex()
    {
        for(int i = 0; i < 3; i++)
        {
            diff[i] = target[i] - source[i];
        }

        Texture2D newTex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                Color keepColor = tex.GetPixel(x, y);
                keepColor.r += diff[0];
                keepColor.g += diff[1];
                keepColor.b += diff[2];
                newTex.SetPixel(x, y, keepColor);
            }
        }

        newTex.Apply();

        File.WriteAllBytes("diffValue.png", newTex.EncodeToPNG());
    }

    void genBase()
    {
        colorCount = new Dictionary<Color, int>();

        int width = tex.width;
        int height = tex.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color keepColor = tex.GetPixel(x, y);

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

        sortColor = new SortedList();

        Dictionary<Color, int>.Enumerator colorEnum = colorCount.GetEnumerator();

        Debug.Log(colorCount.Count);

        int maxColor = 0;

        while (colorEnum.MoveNext())
        {
            if (!sortColor.Contains(colorEnum.Current.Value))
            {
                sortColor.Add(colorEnum.Current.Value, colorEnum.Current.Key);
            }
            //if (maxColor < colorEnum.Current.Value)
            //{
            //    mainColor = colorEnum.Current.Key;
            //    maxColor = colorEnum.Current.Value;
            //}
        }

        for (int i = 0; i < rankColor.Length; i++)
        {
            int index = sortColor.Count - 1 - i;
            rankColor[i] = (Color)sortColor.GetByIndex(index);
        }

        //Color baseColor = rankColor[2];

        //Texture2D newTex = new Texture2D(1024, 1024);

        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        Color keepColor = tex.GetPixel(x, y);

        //        keepColor.r -= baseColor.r;
        //        keepColor.g -= baseColor.g;
        //        keepColor.b -= baseColor.b;

        //        newTex.SetPixel(x, y, keepColor);
        //    }
        //}

        //newTex.Apply();

        //File.WriteAllBytes("newBase.jpg", newTex.EncodeToJPG());

        //Debug.Log(sortColor.Count);

        Color keep1Color = rankColor[35];

        Debug.Log(keep1Color.r + " " + keep1Color.g + " " + keep1Color.b);
    }

    void test()
    {
        Texture2D baseTex = new Texture2D(1, 1);
        Texture2D grayTex = new Texture2D(1, 1);

        baseTex.LoadImage(File.ReadAllBytes("newBase.jpg"));
        //baseTex.LoadImage(File.ReadAllBytes("base.jpg"));
        grayTex.LoadImage(File.ReadAllBytes("grayscale.jpg"));

        baseTex.Apply();
        grayTex.Apply();

        Texture2D newTex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                Color baseColor = baseTex.GetPixel(x, y);
                Color grayColor = grayTex.GetPixel(x, y);

                baseColor.r += grayColor.r;
                baseColor.g += grayColor.g;
                baseColor.b += grayColor.b;

                //if(grayColor.r < 0.5f)
                //{
                //    baseColor.r *= grayColor.r;
                //    baseColor.g *= grayColor.g;
                //    baseColor.b *= grayColor.b;
                //}

                //float grayValue = grayColor.grayscale;

                //grayValue += grayValue;

                //grayValue = grayValue > 1.0f ? 1.0f : grayValue;

                //baseColor.r *= grayValue;
                //baseColor.g *= grayValue;
                //baseColor.b *= grayValue;

                //float grayValue = grayColor.grayscale;

                //baseColor.r += grayValue;
                //baseColor.g += grayValue;
                //baseColor.b += grayValue;

                //baseColor.r /= 2.0f;
                //baseColor.g /= 2.0f;
                //baseColor.b /= 2.0f;

                newTex.SetPixel(x, y, baseColor);
            }
        }

        newTex.Apply();

        File.WriteAllBytes("combine.jpg", newTex.EncodeToJPG());

        //int width = tex.width;
        //int height = tex.height;

        //for(int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        float grayColor = tex.GetPixel(x, y).grayscale;
        //        tex.SetPixel(x, y, new Color(grayColor, grayColor, grayColor));
        //    }
        //}

        //tex.Apply();

        //File.WriteAllBytes("grayscale.jpg", tex.EncodeToJPG());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
