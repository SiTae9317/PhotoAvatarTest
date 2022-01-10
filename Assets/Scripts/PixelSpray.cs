using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSpray : MonoBehaviour
{
    public Camera mainCam;
    public Color pointColor = Color.black;
    public GameObject type1;
    public GameObject type2;
    public GameObject type3;
    public GameObject type4;
    private Texture2D newtex1;
    private Texture2D newtex2;
    private Texture2D newtex3;
    private Texture2D newtex4;
    private Texture2D tex = null;
    private int width = 0;
    private int height = 0;
    private Color emptyColor;

    Dictionary<int, List<int>> pointToAround;

    // Start is called before the first frame update
    void Start()
    {
        emptyColor = Color.gray;
        emptyColor.a = 0.0f;

        setPointToAround();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;

            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                Vector2 bc = hit.textureCoord;
                Debug.Log(bc.x + " " + bc.y);

                if(tex == null)
                {
                    tex = hit.collider.gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                    width = tex.width;
                    height = tex.height;
                }

                int pointX = (int)(bc.x * width);
                int pointY = (int)(bc.y * height);

                Debug.Log(pointX + " " + pointY);

                pointColor = tex.GetPixel(pointX, pointY);
                Color[] returnColor1 = getPixelAroundColors(pointX, pointY, 20);
                Color[] returnColor2 = getPixelAroundColors2(pointX, pointY, 20);

                int color1Size = (int)Mathf.Sqrt(returnColor1.Length);
                int color2Size = (int)Mathf.Sqrt(returnColor2.Length);

                newtex1 = new Texture2D(color1Size, color1Size);
                newtex2 = new Texture2D(color2Size, color2Size);

                List<Color> tempColors = new List<Color>();

                //while(tempColors.Count + returnColor2.Length < 1024 * 1024)
                //{
                //    tempColors.AddRange(returnColor2);
                //}

                int adder = 1024 * 1024 - tempColors.Count;

                for(int i = 0; i < 1024 * 1024; i++)
                {
                    int randIndex = (int)Random.RandomRange(0, returnColor1.Length);
                    tempColors.Add(returnColor2[randIndex]);
                }

                Texture2D tempTex = new Texture2D(1024, 1024);

                tempTex.SetPixels(tempColors.ToArray());
                tempTex.Apply();

                System.IO.File.WriteAllBytes("base.jpg", tempTex.EncodeToJPG());
                

                newtex1.SetPixels(returnColor1);
                newtex2.SetPixels(returnColor2);

                newtex1.Apply();
                newtex2.Apply();

                type1.GetComponent<MeshRenderer>().material.mainTexture = newtex1;
                type2.GetComponent<MeshRenderer>().material.mainTexture = newtex2;

                Debug.Log(returnColor1.Length + " " + returnColor2.Length);

                int color3Size = color1Size * 2 - 1;
                int color4Size = color2Size * 2 - 1;

                newtex3 = new Texture2D(color3Size, color3Size);
                newtex4 = new Texture2D(color4Size, color4Size);

                List<Color> newRC3 = new List<Color>();
                List<Color> newRC4 = new List<Color>();

                int pixelIndex = 0;
                for (int i = 0; i < Mathf.Pow(color3Size, 2.0f); i++)
                {
                    if ((i / (color3Size)) % 2 == 0)
                    {
                        if (i % 2 == 0)
                        {
                            newRC3.Add(returnColor1[pixelIndex]);
                            pixelIndex++;
                            continue;
                        }
                    }
                    newRC3.Add(emptyColor);
                }

                pixelIndex = 0;
                for (int i = 0; i < Mathf.Pow(color4Size, 2.0f); i++)
                {
                    if ((i / (color4Size)) % 2 == 0)
                    {
                        if (i % 2 == 0)
                        {
                            newRC4.Add(returnColor2[pixelIndex]);
                            pixelIndex++;
                            continue;
                        }
                    }
                    newRC4.Add(emptyColor);
                }

                newtex3.SetPixels(newRC3.ToArray());
                newtex4.SetPixels(newRC4.ToArray());
            }

            newtex3.Apply();
            newtex4.Apply();

            type3.GetComponent<MeshRenderer>().material.mainTexture = newtex3;
            type4.GetComponent<MeshRenderer>().material.mainTexture = newtex4;
        }
    }

    Color[] getPixelAroundColors2(int pointX, int pointY, int layout = 1)
    {
        List<Color> returnColors = new List<Color>();

        for (int y = -layout; y < layout + 1; y++)
        {
            if (pointY + y >= 0 && pointY + y < height)
            {
                for (int x = -layout; x < layout + 1; x++)
                {
                    if (pointX + x >= 0 && pointX + x < width)
                    {
                        returnColors.Add(tex.GetPixel(pointX + x, pointY + y));
                    }
                }
            }
        }

        return returnColors.ToArray();
    }

    Color[] getPixelAroundColors(int x, int y, int layout = 1)
    {
        List<Color> returnColors = new List<Color>();

        HashSet<int> pixelIndexs = new HashSet<int>();

        pixelIndexs.Add(x + y * width);

        Debug.Log(pointToAround[x + y * width].Count);

        for (int i = 0; i < layout; i++)
        {
            List<int> aroundIndexs = new List<int>();

            using (HashSet<int>.Enumerator piEnum = pixelIndexs.GetEnumerator())
            {
                while (piEnum.MoveNext())
                {
                    aroundIndexs.Add(piEnum.Current);
                }

                for (int j = 0; j < aroundIndexs.Count; j++)
                {
                    List<int> aroundDatas = pointToAround[aroundIndexs[j]];

                    for (int k = 0; k < aroundDatas.Count; k++)
                    {
                        pixelIndexs.Add(aroundDatas[k]);
                    }
                }
            }

            aroundIndexs.Clear();
            aroundIndexs = null;
        }

        Color[] pixelColors = tex.GetPixels();

        using (HashSet<int>.Enumerator piEnum = pixelIndexs.GetEnumerator())
        {
            while(piEnum.MoveNext())
            {
                returnColors.Add(pixelColors[piEnum.Current]);
            }
        }

        return returnColors.ToArray();
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
    }
}
