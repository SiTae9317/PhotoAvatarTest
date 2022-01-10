using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ImageDistortion : MonoBehaviour
{
    public GameObject point;
    public GameObject[] points;

    private bool isDetect = true;

    Texture2D testTeX;

    public int width = 10;
    public int height = 10;

    public bool isDebug = false;
    public bool InOut = false;

    Color[] srcColors;
    Color[] dstColors;

    [Range(-1.0f, 1.0f)]
    public float fA = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float fB = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float fC = 0.0f;

    [Range(0.0f, 1.0f)]
    public float xValue = 0.5f;
    [Range(0.0f, 1.0f)]
    public float yValue = 0.5f;

    Vector3 pivot;

	// Use this for initialization
	void Start ()
    {
        if (isDebug)
        {
            pivot = new Vector3(0.0f, 0.0f, 0.0f);

            GameObject newObj = new GameObject();
            newObj.name = "points";

            points = new GameObject[width * height];

            float yPos = pivot.y - (float)height / 2.0f + 0.5f;

            for (int y = 0; y < height; y++)
            {
                float xPos = pivot.x - (float)width / 2.0f + 0.5f;

                for (int x = 0; x < width; x++)
                {
                    GameObject obj = Instantiate(point);
                    obj.transform.position = new Vector3(xPos, yPos, 0.0f);
                    xPos++;

                    obj.transform.parent = newObj.transform;

                    points[y * width + x] = obj;
                }
                yPos++;
            }
        }

        point.SetActive(false);

        testTeX = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        srcColors = testTeX.GetPixels();

        //List<Color> colorData = new List<Color>();

        //colorData.AddRange(srcColors);

        dstColors = new Color[srcColors.Length];

        width = testTeX.width;
        height = testTeX.height;

        float wx = width / 1000.0f;
        float hy = height / 1000.0f;

        gameObject.transform.localScale = wx > hy ? new Vector3(2.0f, 1.0f, (2.0f * hy) / wx) : new Vector3((2.0f * wx) / hy, 1.0f, 2.0f);

        testTeX = new Texture2D(width, height);
        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = testTeX;

        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = distortionTex(testTeX);

    }
	
	// Update is called once per frame
	void Update ()
    {
        drawGraph();

        gameObject.GetComponent<MeshRenderer>().material.mainTexture = testTeX;

        //if(Input.GetKeyDown(KeyCode.D))
        //{
        //    Vector2 cp = gameObject.GetComponent<DetectFace>().getCenterPosition();
        //    xValue = cp.x;
        //    yValue = cp.y;
        //}

        return;
        if (Input.GetKeyDown(KeyCode.D))
        {
            testTeX = gameObject.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = distortionTex3(testTeX);
        }
        if (!isDebug)
        {
            return;
        }
        if(points.Length > 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 normal = pivot - points[i].transform.position;

                //normal /= (float)width;

                float dis = Vector3.Distance(pivot, points[i].transform.position);

                dis = Mathf.Sqrt(dis);

                //normal = pivot + (points[i].transform.position * dis);

                normal = pivot + (normal.normalized * dis);

                //normal.Normalize();

                if (normal.magnitude < 0.4f)
                {
                    Debug.DrawRay(points[i].transform.position, normal, Color.green);
                }
                else if (normal.magnitude < 0.5f)
                {
                    Debug.DrawRay(points[i].transform.position, normal, Color.yellow);
                }
                else if (normal.magnitude < 0.6f)
                {
                    Debug.DrawRay(points[i].transform.position, normal, Color.red);
                }
                else
                {
                    Debug.DrawRay(points[i].transform.position, normal, Color.blue);
                }
            }
        }
	}

    public Texture2D distortionTex(Texture2D curTex)
    {
        int texWidth = curTex.width;
        int texHeight = curTex.height;

        Vector2 centerPivot = new Vector2((float)texWidth / 2.0f - 0.5f, (float)texHeight / 2.0f - 0.5f);

        float len = (float)texWidth;

        Texture2D newTex = new Texture2D(texWidth, texHeight);

        float value = 0.0f;

        if(InOut)
        {
            value = 1.0f;
        }
        else
        {
            value = -1.0f;
        }

        for(int y = 0; y < texHeight; y++)
        {
            for(int x = 0; x < texWidth; x++)
            {
                Vector2 curPos = new Vector2(x, y);

                Vector3 normal = centerPivot - curPos;

                //normal /= 2.5f;// len;

                int newX = x;
                int newY = y;

                float dis = Vector3.Distance(centerPivot, curPos);

                dis = 2.0f * Mathf.Sqrt(dis);

                //normal = pivot + (points[i].transform.position * dis);

                normal = pivot + (normal.normalized * dis * value);

                //if (normal.magnitude > 500.0f)// || normal.magnitude < -100.0f)
                {
                    //normal /= 5f;// len;
                    newX = Mathf.RoundToInt(curPos.x + normal.x);
                    newY = Mathf.RoundToInt(curPos.y + normal.y);
                }

                newTex.SetPixel(x, y, curTex.GetPixel(newX, newY));
            }
        }

        newTex.Apply();

        Debug.Log("newTex");


        return newTex;
    }

    public Texture2D distortionTex2(Texture2D curTex)
    {
        int texWidth = curTex.width;
        int texHeight = curTex.height;

        Vector2 centerPivot = new Vector2((float)texWidth / 2.0f - 0.5f, (float)texHeight / 2.0f - 0.5f);
        
        Texture2D newTex = new Texture2D(texWidth, texHeight);

        float paramA = 0.0f;
        float paramB = -0.02f;
        float paramC = 0.0f;
        float paramD = 1.0f - paramA - paramB - paramC;
        
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Vector2 curPos = new Vector2(x, y);

                Vector3 normal = centerPivot - curPos;

                float dstR = Mathf.Sqrt(Mathf.Pow(normal.x, 2.0f) + Mathf.Pow(normal.y, 2.0f));

                float srcR = ((paramA * Mathf.Pow(dstR, 3.0f)) + (paramB * Mathf.Pow(dstR, 2.0f)) + (paramC * dstR) + paramD) * dstR;

                float factor = Mathf.Abs(dstR / srcR);

                int newX = Mathf.RoundToInt(centerPivot.x - (normal.x * factor));
                int newY = Mathf.RoundToInt(centerPivot.y - (normal.y * factor));
                
                if(newX >= 0 && newY >= 0 && newX < texWidth && newY < texHeight)
                {
                    newTex.SetPixel(x, y, curTex.GetPixel(newX, newY));
                }
            }
        }

        newTex.Apply();

        Debug.Log("newTex");


        return newTex;
    }

    public Texture2D distortionTex3(Texture2D curTex)
    {
        int texWidth = curTex.width;
        int texHeight = curTex.height;

        Dictionary<int, List<int>> pointToAround = generatePixelMap(texWidth, texHeight);

        Texture2D newTex = new Texture2D(texWidth, texHeight);

        double cpX = ((double)texWidth - 1.0) / 2.0;
        double cpY = ((double)texHeight - 1.0) / 2.0;

        double paramA = 0.01;// 0.007715;
        double paramB = 0.0;// -0.026731;
        //double paramA = 0.08;// 0.007715;
        //double paramB = -0.27;// -0.026731;
        double paramC = 0.0;
        double paramD = 1.0 - paramA - paramB - paramC;

        double d1 = texWidth / 2.0;
        double d2 = texHeight / 2.0;
        
        for(int y = 0; y < texHeight; y++)
        {
            for(int x = 0; x < texWidth; x++)
            {
                newTex.SetPixel(x, y, Color.green);
            }
        }

        newTex.Apply();

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                double curX = x;
                double curY = y;

                double diffX = (cpX - curX) / d1;
                double diffY = (cpY - curY) / d2;

                double dstR = Math.Sqrt(diffX * diffX + diffY * diffY);

                double srcR = ((paramA * dstR * dstR * dstR) + (paramB * dstR * dstR) + (paramC * dstR) + paramD) * dstR;

                double factor = Math.Abs(dstR / srcR);

                int newX = (int)(cpX - (diffX * factor * d1));
                int newY = (int)(cpY - (diffY * factor * d2));

                if (newX >= 0 && newY >= 0 && newX < texWidth && newY < texHeight)
                {
                    newTex.SetPixel(x, y, curTex.GetPixel(newX, newY));
                    //newTex.SetPixel(newX, newY, curTex.GetPixel(x, y));
                }
            }
        }

        Color[] datas = newTex.GetPixels();

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int curPoint = y * texWidth + x;
                Color keepColor = datas[curPoint];

                if (keepColor.Equals(Color.green))
                {
                    float r = 0.0f;
                    float g = 0.0f;
                    float b = 0.0f;

                    List<int> arountData = pointToAround[curPoint];

                    int sumCount = 0;

                    for (int i = 0; i < arountData.Count; i++)
                    {
                        keepColor = datas[arountData[i]];

                        if(!keepColor.Equals(Color.green))
                        {
                            r += keepColor.r;
                            g += keepColor.g;
                            b += keepColor.b;

                            sumCount++;
                        }

                    }

                    r /= (float)sumCount;
                    g /= (float)sumCount;
                    b /= (float)sumCount;

                    //newTex.SetPixel(x, y, new Color(r, g, b));
                }
            }
        }

        newTex.Apply();

        Debug.Log("newTex");

        return newTex;
    }

    Dictionary<int, List<int>> generatePixelMap(int texWidth, int texHeight)
    {
        Dictionary<int, List<int>> pointToAround = new Dictionary<int, List<int>>();

        for (int y = 0; y < texHeight; y++)
        {
            for(int x = 0; x < texWidth; x++)
            {
                int curPoint = y * texWidth + x;

                int minX = Math.Max(0, x - 1);
                int maxX = Math.Min(texWidth, x + 1);

                int minY = Math.Max(0, y - 1);
                int maxY = Math.Min(texHeight, y + 1);

                List<int> otherPoints = new List<int>();

                for(int j = minY; j < maxY; j++)
                {
                    for(int i = minX; i < maxX; i++)
                    {
                        int otherPoint = j * width + i;

                        if(otherPoint != curPoint)
                        {
                            otherPoints.Add(otherPoint);
                        }
                    }
                }

                pointToAround.Add(curPoint, otherPoints);

            }
        }

        return pointToAround;
    }

    void drawGraph()
    {
        if(isDetect)
        {
            isDetect = false;
            //Vector2 cp = Vector2.one * 0.5f;//gameObject.GetComponent<DetectFace>().getCenterPosition();
            //xValue = cp.x;
            //yValue = cp.y;
        }

        for(int i = 0; i < dstColors.Length; i++)
        {
            dstColors[i] = Color.gray;
        }

        float a = 0.16f;
        float b = 0.54f;
        float c = 0.0f;
        float d = 1.0f - a - b - c;

        //double paramA = 0.0;// 0.16;
        //double paramB = 0.5;// -0.54;
        //double paramC = 0.0;
        //double paramD = 1.0 - paramA - paramB - paramC;

        double paramA = (double)fA;
        double paramB = (double)fB;
        double paramC = (double)fC;
        double paramD = 1.0 - paramA - paramB - paramC;

        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.zero;

        Vector3 beforeP = Vector3.zero;
        Vector3 nowP = Vector3.zero;

        //int width = 10;
        //int height = 10;

        double wd = width / 2.0;
        double wh = height / 2.0;

        float time = 0.0f;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                //double cpX = (width - 1.0) / 2.0;
                //double cpY = (height - 1.0) / 2.0;

                double cpX = (width - 1.0) * (double)xValue;
                double cpY = (height - 1.0) * (double)yValue;

                double diffX = (cpX - x) / wd;
                double diffY = (cpY - y) / wh;

                double dstR = Math.Sqrt(diffX * diffX + diffY * diffY);

                double srcR = ((paramA * dstR * dstR * dstR) + (paramB * dstR * dstR) + (paramC * dstR) + paramD) * dstR;

                double factor = Math.Abs(dstR / srcR);

                int newX = (int)(cpX - (diffX * factor * wd));
                int newY = (int)(cpY - (diffY * factor * wh));

                nowP.x = ++time;
                nowP.y = (float)factor;

                //if (beforeP != Vector3.zero)
                //{
                //    Debug.DrawLine(beforeP, nowP, Color.green);
                //}

                beforeP.x = nowP.x;
                beforeP.y = nowP.y;

                if (newX >= 0 && newY >= 0 && newX < width && newY < height)
                {
                    //dstColors[y * width + x] = Color.gray;
                    dstColors[newX + newY * width] = srcColors[y * width + x];
                }
            }

            //Debug.DrawLine(new Vector3(0.0f, 0.5f, 0.0f), new Vector3(100.0f, 0.5f, 0.0f), Color.red);
            //Debug.DrawLine(new Vector3(0.0f, 0.75f, 0.0f), new Vector3(100.0f, 0.75f, 0.0f), Color.yellow);
            //Debug.DrawLine(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(100.0f, 1.0f, 0.0f), Color.blue);
        }

        testTeX.SetPixels(dstColors);
        testTeX.Apply();

        //for (int i = -100; i < 100; i++)
        //{
        //    float x = i;
        //    float y = x / ((a * x * x * x) + (b * x * x) + (c * x) + d) * x;

        //    p0.x = x;
        //    p0.y = y;

        //    x += 1.0f;
        //    y = x / ((a * x * x * x) + (b * x * x) + (c * x) + d) * x;

        //    p1.x = x;
        //    p1.y = y;

        //    Debug.DrawLine(p0, p1, Color.green);
        //}
    }
}
