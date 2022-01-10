using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JpgToPng : MonoBehaviour
{
    public Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {
        int width = tex.width;
        int height = tex.height;

        Texture2D newTex = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Color keepColor = tex.GetPixel(x, y);

                float a = keepColor.grayscale;

                keepColor = Color.black;

                keepColor.a = 1.0f - a;// - 0.5f;

                newTex.SetPixel(x, y, keepColor);
            }
        }

        newTex.Apply();

        File.WriteAllBytes("shadow.png", newTex.EncodeToPNG());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
