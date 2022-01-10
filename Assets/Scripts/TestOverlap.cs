using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestOverlap : MonoBehaviour
{
    public Material tarMat;
    public Texture2D sourceTex;
    GradationGen gg;

    // Start is called before the first frame update
    void Start()
    {
        List<float> overlapData = new List<float>();

        MemoryStream ms = new MemoryStream(File.ReadAllBytes("ProjectionData.bin"));
        BinaryReader br = new BinaryReader(ms);

        while (ms.Position < ms.Length)
        {
            overlapData.Add(br.ReadSingle());
        }

        br.Close();
        ms.Close();

        Color[] colorData = sourceTex.GetPixels();

        gg = new GradationGen(colorData, overlapData.ToArray());

        StartCoroutine(testGradationClass());
    }

    IEnumerator testGradationClass()
    {
        while (gg.gradationValue > 0.0f)
        {
            yield return null;
            gg.gradationGenerate();
            setTextureFromOverlapdata(gg.getColorData(), gg.getOverlapDATA());
            //setTextureFromOverlapdata(gg.getOverlapDATA());

        }

        gg.Dispose();
        gg = null;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void setTextureFromOverlapdata(Color[] colorData, float[] overlapData)
    {
        Color keepColor = Color.black;

        Texture2D tex = new Texture2D(1024, 1024);

        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                keepColor = colorData[x + y * 1024];
                float overlapValue = overlapData[x + y * 1024];

                //if (overlapValue > 0.0f)
                //{
                //    keepColor = Color.white;

                //    keepColor.r *= overlapValue;
                //    keepColor.g *= overlapValue;
                //    keepColor.b *= overlapValue;
                //}

                keepColor.a = overlapValue;

                tex.SetPixel(x, y, keepColor);
            }
        }
        //tex.SetPixels(overlapData);
        tex.Apply();

        tarMat.mainTexture = tex;
    }

    void setTextureFromOverlapdata(float[] overlapData)
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
}
