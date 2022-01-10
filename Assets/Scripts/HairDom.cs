using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HairDom : MonoBehaviour
{
    public GameObject cam;
    public GameObject hairImage;
    public GameObject point;
    public GameObject hair;

    public GameObject meshSmooth;

    private Vector3[] vertices;
    private Vector3[] normals;
    private List<bool> sdCheck;
    private List<Vector3> tan;

    private Vector3 centerPos;
    // Start is called before the first frame update
    void Start()
    {
        readImages();

        //genAlphaMap();

        //pointMapping();

        //rayDebug();
    }

    void readImages()
    {
        Texture2D hairTex = new Texture2D(1, 1);
        hairTex.LoadImage(File.ReadAllBytes("hairTex.png"));
        hairTex.Apply();

        Debug.Log(hairTex.width + " " + hairTex.height);

        int width = hairTex.width;
        int height = hairTex.height;

        hairImage.transform.localScale = new Vector3(hairTex.width, hairTex.height, 1.0f);
        centerPos = new Vector3(hairTex.width / 2.0f, hairTex.height / 2.0f, 0.0f);
        hairImage.transform.position = centerPos;

        hair.transform.position = centerPos;
        cam.transform.position = new Vector3(centerPos.x, centerPos.y, -1000.0f);
        hair.transform.localScale = new Vector3(width / 20.0f, height / 20.0f, width / 20.0f);

        Texture2D faceTex = new Texture2D(1, 1);
        faceTex.LoadImage(File.ReadAllBytes("faceTex.png"));
        faceTex.Apply();

        Texture2D newTex = new Texture2D(width, height);

        Texture2D bgTex = new Texture2D(1, 1);
        bgTex.LoadImage(File.ReadAllBytes("bgTex.png"));
        bgTex.Apply();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color hairKeepColor = hairTex.GetPixel(x, y);
                Color faceKeepColor = faceTex.GetPixel(x, y);

                float hairAlpha = hairKeepColor.a;
                float faceAlpha = faceKeepColor.a;

                float r = Mathf.Min(1.0f, hairKeepColor.r * hairAlpha + faceKeepColor.r * faceAlpha);
                float g = Mathf.Min(1.0f, hairKeepColor.g * hairAlpha + faceKeepColor.g * faceAlpha);
                float b = Mathf.Min(1.0f, hairKeepColor.b * hairAlpha + faceKeepColor.b * faceAlpha);
                float a = Mathf.Min(1.0f, hairKeepColor.a + faceKeepColor.a);

                if(a == 0.0f)
                {
                    Color bgKeepColor = bgTex.GetPixel(x, y);
                    r = bgKeepColor.r;
                    g = bgKeepColor.g;
                    b = bgKeepColor.b;
                }

                newTex.SetPixel(x, y, new Color(r, g, b, a));
            }
        }

        newTex.Apply();

        hairImage.GetComponent<MeshRenderer>().material.mainTexture = newTex;

        meshSmooth.SetActive(true);
    }

    void pointMapping()
    {
        Mesh mesh = hair.GetComponent<MeshFilter>().mesh;

        Texture2D curTex = hair.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        Texture2D newTex = new Texture2D(curTex.width, curTex.height);

        newTex.SetPixels32(curTex.GetPixels32());
        newTex.Apply();

        Vector2[] uv = mesh.uv;

        for(int i = 0; i < uv.Length; i++)
        {
            Vector2 curUV = uv[i];

            int curX = (int)(curUV.x * curTex.width);
            int curY = (int)(curUV.y * curTex.height);

            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    int cx = Mathf.Min(curTex.width - 1, Mathf.Max(0, curX + x));
                    int cy = Mathf.Min(curTex.height - 1, Mathf.Max(0, curY + y));

                    //if(cy != curY + y)
                    //{
                    //    Debug.Log(cy + " " + curUV.x + " " + curUV.y + " " + curY + " " + y);
                    //}

                    newTex.SetPixel(cx, cy, Color.white);
                }
            }
        }

        newTex.Apply();
        File.WriteAllBytes("testAlpha.png", newTex.EncodeToPNG());
    }

    void genAlphaMap()
    {
        Color keepColor = Color.black;
        keepColor.a = 0.3f;
        Texture2D newTex = new Texture2D(512, 512);
        for(int y = 0; y < 512; y++)
        {
            for(int x = 0; x < 512; x++)
            {
                newTex.SetPixel(x, y, keepColor);
            }
        }

        newTex.Apply();

        File.WriteAllBytes("testAlpha.png", newTex.EncodeToPNG());
    }

    void rayDebug()
    {
        Mesh mesh = hair.GetComponent<MeshFilter>().mesh;

        tan = new List<Vector3>();

        vertices = mesh.vertices;
        normals = mesh.normals;
        Vector2[] uv = mesh.uv;
        Vector4[] tangents = mesh.tangents;
        sdCheck = new List<bool>();

        Debug.Log(tangents.Length);

        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject newGo = Instantiate(point);
            newGo.name = i.ToString();// + " " + normals[i].x + " " + normals[i].y + " " + normals[i].z;
            newGo.transform.position = vertices[i];

            newGo.transform.parent = gameObject.transform;

            //tan.Add(new Vector3(tangents[i].x, tangents[i].y, tangents[i].z));

            //if (normals[i].x != tangents[i].x
            //    && normals[i].y != tangents[i].y
            //    && normals[i].z != tangents[i].z)
            //{
            //    Debug.Log("diff " + i + " " + tangents[i].w);
            //    sdCheck.Add(false);
            //}
            //else
            //{
            //    Debug.Log("same " + i + " " + tangents[i].w);
            //    sdCheck.Add(true);
            //}
        }

        point.SetActive(false);

        //StartCoroutine(checkDebug());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator checkDebug()
    {
        while(true)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Debug.DrawRay(vertices[i], (new Vector3(normals[i].x, normals[i].y, 0.0f)).normalized, sdCheck[i] ? Color.green : Color.red);
                Debug.DrawRay(vertices[i], tan[i], Color.yellow);
            }
            yield return null;
        }
    }
}
