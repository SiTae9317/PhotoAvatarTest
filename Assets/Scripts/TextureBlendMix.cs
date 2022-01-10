using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureBlendMix : MonoBehaviour
{
    public Texture2D sourceTex;
    public Texture2D baseTex;
    public Texture2D alpha1Tex;
    public Texture2D alpha2Tex;
    public Texture2D baseNAlphaTex;
    public Texture2D combineTex;
    public Texture2D combineTex2;

    // Start is called before the first frame update
    void Start()
    {
        combineTex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                Color keepSourceColor = sourceTex.GetPixel(x, y);
                Color keepAlphaColor = baseNAlphaTex.GetPixel(x, y);

                //Color keepGenSourceColor = sourceTex.GetPixel(x, y);

                float alphaValue = keepAlphaColor.a;

                keepAlphaColor.r *= alphaValue;
                keepAlphaColor.g *= alphaValue;
                keepAlphaColor.b *= alphaValue;

                //float grayScale = keepAlphaColor.grayscale;

                //keepGenSourceColor.r *= grayScale;
                //keepGenSourceColor.g *= grayScale;
                //keepGenSourceColor.b *= grayScale;

                //keepGenSourceColor.r *= alphaValue;
                //keepGenSourceColor.g *= alphaValue;
                //keepGenSourceColor.b *= alphaValue;

                float sourceValue = 1.0f - alphaValue;

                keepSourceColor.r *= sourceValue;
                keepSourceColor.g *= sourceValue;
                keepSourceColor.b *= sourceValue;

                keepSourceColor.r += keepAlphaColor.r;
                keepSourceColor.g += keepAlphaColor.g;
                keepSourceColor.b += keepAlphaColor.b;

                //combineTex.SetPixel(x, y, keepSourceColor);
                combineTex.SetPixel(x, y, keepSourceColor);
            }
        }
        combineTex.Apply();

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = combineTex;

        //combineTex = new Texture2D(1024, 1024);

        //for (int y = 0; y < 1024; y++)
        //{
        //    for (int x = 0; x < 1024; x++)
        //    {
        //        float grayValue = baseNAlphaTex.GetPixel(x,y).grayscale;
        //        grayValue += 0.15f;

        //        Color keepColor = sourceTex.GetPixel(x, y);
        //        keepColor.r *= grayValue;
        //        keepColor.g *= grayValue;
        //        keepColor.b *= grayValue;

        //        combineTex.SetPixel(x, y, keepColor);
        //    }
        //}

        //combineTex.Apply();

        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = combineTex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void keep()
    {
        combineTex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                Color keepSourceColor = sourceTex.GetPixel(x, y);
                Color keepAlphaColor = baseNAlphaTex.GetPixel(x, y);

                //Color keepGenSourceColor = sourceTex.GetPixel(x, y);

                float alphaValue = keepAlphaColor.a;

                keepAlphaColor.r *= alphaValue;
                keepAlphaColor.g *= alphaValue;
                keepAlphaColor.b *= alphaValue;

                //float grayScale = keepAlphaColor.grayscale;

                //keepGenSourceColor.r *= grayScale;
                //keepGenSourceColor.g *= grayScale;
                //keepGenSourceColor.b *= grayScale;

                //keepGenSourceColor.r *= alphaValue;
                //keepGenSourceColor.g *= alphaValue;
                //keepGenSourceColor.b *= alphaValue;

                float sourceValue = 1.0f - alphaValue;

                keepSourceColor.r *= sourceValue;
                keepSourceColor.g *= sourceValue;
                keepSourceColor.b *= sourceValue;

                keepSourceColor.r += keepAlphaColor.r;
                keepSourceColor.g += keepAlphaColor.g;
                keepSourceColor.b += keepAlphaColor.b;

                //combineTex.SetPixel(x, y, keepSourceColor);
                combineTex.SetPixel(x, y, keepSourceColor);
            }
        }
        combineTex.Apply();

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = combineTex;

        //combineTex2 = new Texture2D(1024, 1024);

        //for (int y = 0; y < 1024; y++)
        //{
        //    for (int x = 0; x < 1024; x++)
        //    {
        //        Color keepSourceColor = sourceTex.GetPixel(x, y);

        //        Color keepBaseColor = baseTex.GetPixel(x, y);
        //        float alpha1 = alpha1Tex.GetPixel(x, y).grayscale;
        //        float alpha2 = alpha2Tex.GetPixel(x, y).grayscale;

        //        float combineR = keepSourceColor.r * (1.0f - alpha1) + keepBaseColor.r * alpha1;
        //        combineR = combineR * (1.0f - alpha2) + keepBaseColor.r * alpha2;

        //        float combineG = keepSourceColor.g * (1.0f - alpha1) + keepBaseColor.g * alpha1;
        //        combineG = combineG * (1.0f - alpha2) + keepBaseColor.g * alpha2;

        //        float combineB = keepSourceColor.b * (1.0f - alpha1) + keepBaseColor.b * alpha1;
        //        combineB = combineB * (1.0f - alpha2) + keepBaseColor.b * alpha2;

        //        combineTex2.SetPixel(x, y, new Color(combineR, combineG, combineB));
        //    }
        //}

        //combineTex2.Apply();

        //File.WriteAllBytes("tba.jpg", combineTex.EncodeToJPG());
        //File.WriteAllBytes("tbaba.jpg", combineTex2.EncodeToJPG());
    }
}
