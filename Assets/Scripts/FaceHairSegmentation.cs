using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System;
using System.IO;

public class FaceHairSegmentation : MonoBehaviour
{
    private Texture2D currentTex;
    private Texture2D faceTex;
    private Texture2D hairTex;
    private Texture2D bgTex;

    private Action cb;

    Dictionary<int, List<int>> pointToAround;
    // Start is called before the first frame update
    void Start()
    {
        //// pixelTest

        //Texture2D newTex = new Texture2D(1, 1);
        //newTex.LoadImage(File.ReadAllBytes("OverlapTex.jpg"));
        //newTex.Apply();

        //HashSet<Color> testa = new HashSet<Color>();

        //int width = newTex.width;
        //int height = newTex.height;

        //for(int y = 0; y < height; y++)
        //{
        //    for(int x = 0; x < width; x++)
        //    {
        //        testa.Add(newTex.GetPixel(x, y));
        //    }
        //}

        //Debug.Log(testa.Count);

        //// pixelTest

        //initialize();
        //runFHS(currentTex.EncodeToJPG());
    }

    public void runFHS(byte[] data, Action callback)
    {
        cb = callback;

        initialize();

        currentTex = new Texture2D(1, 1);

        currentTex.LoadImage(data);
        currentTex.Apply();


        ////test
        ////

        //for(int y = 0; y < currentTex.height; y++)
        //{
        //    for(int x = 0; x< currentTex.width; x++)
        //    {
        //        Color keepColor = currentTex.GetPixel(x, y);

        //        for (int i = 0; i < 3; i++)
        //        {
        //            HSV hsv = ColorUtils.RGBToHSV(keepColor);
        //            float adderVal = 1.0f + hsv.s;
        //            hsv.v = Mathf.Min(1.0f, hsv.v * adderVal);
        //            keepColor = ColorUtils.HSVToRGB(hsv);
        //        }

        //        currentTex.SetPixel(x, y, keepColor);
        //    }
        //}

        //currentTex.Apply();

        ////
        ////test end

        faceTex = new Texture2D(currentTex.width, currentTex.height);
        hairTex = new Texture2D(currentTex.width, currentTex.height);
        bgTex = new Texture2D(currentTex.width, currentTex.height);

        StartCoroutine(request(currentTex.EncodeToJPG()));
    }

    public Texture2D getCurrentTex()
    {
        return currentTex;
    }

    public void initialize()
    {
        pointToAround = new Dictionary<int, List<int>>();

        for (int y = 0; y < 224; y++)
        {
            for (int x = 0; x < 224; x++)
            {
                List<int> aroundValues = new List<int>();

                int pointIndex = x + y * 224;

                for (int y1 = -1; y1 < 2; y1++)
                {
                    for (int x1 = -1; x1 < 2; x1++)
                    {
                        int nX = x + x1;
                        int nY = y + y1;

                        if (nX > -1 && nX < 224)
                        {
                            if (nY > -1 && nY < 224)
                            {
                                int aroundIndex = nX + nY * 224;
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

    IEnumerator request(byte[] testTex)
    {
        //string baseUrl = "http://ravatar.iptime.org/";
        string baseUrl = "http://192.168.0.117:19317/";
        string command = "faceseg";
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(testTex);

        UnityWebRequest request = UnityWebRequest.Get(baseUrl + command);
        request.method = "POST";
        request.uploadHandler = uploadHandler;

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadedBytes);
            binaryToTexture2D(binarayToMask(request.downloadHandler.data));
        }

        request.Dispose();
        request = null;
        uploadHandler.Dispose();
        uploadHandler = null;

        cb();
    }

    IEnumerator faceSegRequest()
    {
        WWW www = new WWW("http://192.168.0.117:9009/faceseg");

        while (!www.isDone)
        {
            yield return null;
        }

        Debug.Log(www.text);
    }

    List<float> binarayToMask(byte[] imageBin)
    {
        MemoryStream ms = new MemoryStream(imageBin);
        BinaryReader br = new BinaryReader(ms);

        List<float> alphas = new List<float>();

        while (ms.Position < ms.Length)
        {
            alphas.Add((float)br.ReadInt64());
        }

        br.Close();
        ms.Close();

        // bg Test
        //

        for (int i = 0; i < 3; i++)
        {
            alphas = outlineRemove(alphas);
        }

        //
        // bg Test End

        //Texture2D newTex = generateTexFromValues(alphas);

        //sourceImgPlane.GetComponent<MeshRenderer>().material.mainTexture = newTex;

        return alphas;
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

    Texture2D generateTexFromValues(List<float> alpha)
    {
        Texture2D newTex = new Texture2D(224, 224);

        Color keepColor = Color.black;

        for (int y = 0; y < 224; y++)
        {
            for (int x = 0; x < 224; x++)
            {
                keepColor = Color.black;

                int currentPoint = x + ((223 - y) * 224);

                float currentValue = alpha[currentPoint];

                if (currentValue > 0.0f)
                {
                    keepColor = Color.white;
                }

                newTex.SetPixel(x, y, keepColor);
            }
        }

        newTex.Apply();

        return newTex;
    }

    void binaryToTexture2D(List<float> alphas)
    {
        int width = currentTex.width;
        int height = currentTex.height;

        Color faceColor = Color.black;
        Color hairColor = Color.black;
        Color bgColor = Color.black;

        float alphaX = (float)width / 224.0f;
        float alphaY = (float)height / 224.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bgColor = faceColor = hairColor = currentTex.GetPixel(x, y);
                float alphaValue = alphas[(int)(x / alphaX) + ((int)((height - 1 - y) / alphaY) * 224)];

                bgColor.a = hairColor.a = faceColor.a = 0.0f;
                if (alphaValue == 1.0f)
                {
                    faceColor.a = 1.0f;
                }
                if (alphaValue == 2.0f)
                {
                    hairColor.a = 1.0f;
                }
                if (alphaValue == 0.0f)
                {
                    bgColor.a = 1.0f;
                    currentTex.SetPixel(x, y, Color.gray);
                }

                faceTex.SetPixel(x, y, faceColor);
                hairTex.SetPixel(x, y, hairColor);
                bgTex.SetPixel(x, y, bgColor);
            }
        }

        currentTex.Apply();
        faceTex.Apply();
        hairTex.Apply();
        bgTex.Apply();

        File.WriteAllBytes("faceTex.png", faceTex.EncodeToPNG());
        File.WriteAllBytes("hairTex.png", hairTex.EncodeToPNG());
        File.WriteAllBytes("bgTex.png", bgTex.EncodeToPNG());
    }

    //void binaryToTexture2D(byte[] imageBin)
    //{
    //    MemoryStream ms = new MemoryStream(imageBin);
    //    BinaryReader br = new BinaryReader(ms);

    //    List<float> alphas = new List<float>();

    //    while (ms.Position < ms.Length)
    //    {
    //        alphas.Add((float)br.ReadInt64());
    //    }

    //    br.Close();
    //    ms.Close();

    //    //Texture2D testTex = sourceImgPlane.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

    //    int width = currentTex.width;
    //    int height = currentTex.height;

    //    Texture2D faceTex = new Texture2D(width, height);
    //    Texture2D hairTex = new Texture2D(width, height);

    //    Color faceColor = Color.black;
    //    Color hairColor = Color.black;

    //    float alphaX = (float)width / 224.0f;
    //    float alphaY = (float)height / 224.0f;

    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            faceColor = hairColor = currentTex.GetPixel(x, y);
    //            float alphaValue = alphas[(int)(x / alphaX) + ((int)((height - 1 - y) / alphaY) * 224)];

    //            hairColor.a = faceColor.a = 0.0f;
    //            if (alphaValue == 1.0f)
    //            {
    //                faceColor.a = 1.0f;
    //            }
    //            if (alphaValue == 2.0f)
    //            {
    //                hairColor.a = 1.0f;
    //            }

    //            faceTex.SetPixel(x, y, faceColor);
    //            hairTex.SetPixel(x, y, hairColor);
    //        }
    //    }

    //    faceTex.Apply();
    //    hairTex.Apply();

    //    File.WriteAllBytes("FaceAlpha.png", faceTex.EncodeToPNG());
    //    File.WriteAllBytes("hairAlpha.png", hairTex.EncodeToPNG());

    //    GameObject faceObj = Instantiate(sourceImgPlane);
    //    GameObject hairObj = Instantiate(sourceImgPlane);

    //    faceObj.GetComponent<MeshRenderer>().material.mainTexture = faceTex;
    //    hairObj.GetComponent<MeshRenderer>().material.mainTexture = hairTex;


    //    Vector3 scale = new Vector3((float)width / (float)height, 1.0f, 1.0f);
    //    sourceImgPlane.transform.localScale = faceObj.transform.localScale = hairObj.transform.localScale = scale;
    //    faceObj.transform.localPosition = hairObj.transform.localPosition = Vector3.down * 5.5f;
    //}
}
