using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ComparePixel : MonoBehaviour
{
    public Material tarMat;
    // Start is called before the first frame update
    void Start()
    {
        Texture2D comp1 = new Texture2D(1, 1);
        comp1.LoadImage(File.ReadAllBytes("comp1.png"));
        comp1.Apply();

        Texture2D comp2 = new Texture2D(1, 1);
        comp2.LoadImage(File.ReadAllBytes("comp2.png"));
        comp2.Apply();

        Texture2D newTex = new Texture2D(2048, 2048);

        int compCount = 0;
        for(int y = 0; y < comp1.height; y++)
        {
            for(int x = 0; x < comp1.width; x++)
            {
                if(comp1.GetPixel(x,y) != comp2.GetPixel(x,y))
                {
                    compCount++;

                    newTex.SetPixel(x, y, comp1.GetPixel(x, y));
                }
            }
        }

        newTex.Apply();

        tarMat.mainTexture = newTex;

        Debug.Log(compCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
