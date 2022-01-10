using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DynamicPointGenerator : MonoBehaviour
{
    public Material genMat;
    public List<Vector3> dynamicPoints;
    private List<int> triangles;
    private List<float> zValues;
    private List<Vector2> uvValues;

    private float compWidth;
    private float compHeight;

    public GameObject point;

    public GameObject zFace;

    public GameObject leftObj;
    public GameObject rightObj;

    public CircleGen ccg;

    public bool isWire = false;

    //private Dictionary<int, int> changeNumber;

    // Start is called before the first frame update
    void Start()
    {
        //UVExport();
        //zValueExport();
        //changeNumber = new Dictionary<int, int>();

        //for(int i = 0; i < 92; i++)
        //{
        //    int value = i;
        //    if(value == 70)
        //    {
        //        value = 80;
        //    }
        //    else if(value == 71)
        //    {
        //        value = 72;
        //    }
        //    else if (value == 72)
        //    {
        //        value = 71;
        //    }
        //    else if (value == 80)
        //    {
        //        value = 70;
        //    }
        //    changeNumber.Add(i, value);
        //}
    }

    public void UVImport()
    {
        MemoryStream ms = new MemoryStream(File.ReadAllBytes("UVValues.dat"));
        BinaryReader br = new BinaryReader(ms);

        uvValues = new List<Vector2>();

        while (ms.Position < ms.Length)
        {
            uvValues.Add(new Vector2(br.ReadSingle(), br.ReadSingle()));
        }

        br.Close();
        ms.Close();
    }

    public void UVExport()
    {
        Vector2[] uvs = zFace.GetComponent<MeshFilter>().mesh.uv;

        FileStream fs = new FileStream("UVValues.dat", FileMode.OpenOrCreate);
        BinaryWriter bw = new BinaryWriter(fs);

        for (int i = 0; i < uvs.Length; i++)
        {
            bw.Write(uvs[i].x);
            bw.Write(uvs[i].y);
        }

        bw.Close();
        fs.Close();
    }

    public void zValueImport()
    {
        MemoryStream ms = new MemoryStream(File.ReadAllBytes("ZValues.dat"));
        BinaryReader br = new BinaryReader(ms);

        zValues = new List<float>();

        compWidth = br.ReadSingle();
        compHeight = br.ReadSingle();

        while (ms.Position < ms.Length)
        {
            zValues.Add(br.ReadSingle());
        }

        br.Close();
        ms.Close();
    }

    public void zValueExport()
    {
        Vector3[] verts = zFace.GetComponent<MeshFilter>().mesh.vertices;

        float width = Vector3.Distance(verts[0], verts[16]);
        float height = Vector3.Distance(verts[8], verts[81]);

        FileStream fs = new FileStream("ZValues.dat", FileMode.OpenOrCreate);
        BinaryWriter bw = new BinaryWriter(fs);

        bw.Write(width);
        bw.Write(height);

        for (int i = 0; i < verts.Length; i++)
        {
            //bw.Write(verts[i].z / 100.0f);
            bw.Write(verts[i].z);
        }

        bw.Close();
        fs.Close();
    }

    public void zValueCopy()
    {
        //List<float> ratioZValues = calculateZValue(dynamicPoints);

        //Vector3[] verts = zFace.GetComponent<MeshFilter>().mesh.vertices;

        float width = Vector3.Distance(dynamicPoints[0], dynamicPoints[16]);
        float height = Vector3.Distance(dynamicPoints[8], dynamicPoints[81]);

        float ratio = (width / compWidth) + (height / compHeight);
        ratio /= 2.0f;

        Debug.Log(ratio);

        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            Vector3 keepValue = dynamicPoints[i];
            keepValue.z = zValues[i] * ratio;
            //keepValue /= 100.0f;
            dynamicPoints[i] = keepValue;
        }
    }

    public void readWireData()
    {
        triangles = new List<int>();

        string wireData = File.ReadAllText("WireData.txt");

        string[] triDatas = wireData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for(int i = 0; i < triDatas.Length; i++)
        {
            string[] indices = triDatas[i].Split('\t');

            for(int j = indices.Length - 1; j >= 0; j--)
            {
                //triangles.Add(changeNumber[int.Parse(indices[j])]);

                triangles.Add(int.Parse(indices[j]));
            }
        }

        Debug.Log(triangles.Count);
    }

    public void generateDPSMesh()
    {
        zValueCopy();

        Vector3 lp = dynamicPoints[0] + dynamicPoints[0] - dynamicPoints[1];
        Vector3 rp = dynamicPoints[16] + dynamicPoints[16] - dynamicPoints[15];

        ccg.setPoints(lp, rp);
        dynamicPoints.Add(lp);
        dynamicPoints.Add(rp);
        dynamicPoints.AddRange(ccg.foreHeadGenerate(12, 13).ToArray());

        loadTriData();

        //showPoints();

        Mesh mesh = new Mesh();
        mesh.name = "newMesh";

        //uvValues = new List<Vector2>();

        //for(int i = uvValues.Count; i < dynamicPoints.Count; i++)
        //{
        //    uvValues.Add(Vector2.zero);
        //}

        //for (int i = 0; i < dynamicPoints.Count; i++)
        //{
        //    if (i == 195 || i == 183 || i == 171 || i == 159
        //         || i == 147 || i == 135 || i == 123 || i == 111 || i == 99)
        //    {
        //        //uvValues.Add(Vector2.one);
        //        dynamicPoints[i] += Vector3.right;
        //    }
        //    else
        //    {
        //        //uvValues.Add(Vector2.zero);
        //    }

        //    //if (i == 240 || i == 229 || i == 218 || i == 207)
        //    //{
        //    //    uvValues.Add(Vector2.one);
        //    //}
        //    //else
        //    //{
        //    //    uvValues.Add(Vector2.zero);
        //    //}
        //}

        //Debug.Log(dynamicPoints.Count);
        //Debug.Log(mesh.vertices.Length);
        //Debug.Log(uvValues.Count);

        mesh.vertices = dynamicPoints.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvValues.ToArray();
        mesh.RecalculateNormals();

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = genMat;

        gameObject.AddComponent<SymmetryPoint2>();
        gameObject.AddComponent<TextureSmooth>();
    }

    public void loadTriData()
    {
        string triDatas = File.ReadAllText("TriData.txt");
        string[] triLineDatas = triDatas.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for(int i = 0; i < triLineDatas.Length; i++)
        {
            string[] triData = triLineDatas[i].Split(' ');
            triangles.Add(int.Parse(triData[0]));
            triangles.Add(int.Parse(triData[1]));
            triangles.Add(int.Parse(triData[2]));
        }
    }


    public void showPoints()
    {
        GameObject pGo = new GameObject();
        pGo.name = "GenPoint";
        point.SetActive(true);
        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            GameObject go = Instantiate(point);
            go.name = i.ToString();

            go.transform.position = dynamicPoints[i];

            go.transform.parent = pGo.transform;
        }
        point.SetActive(false);
    }

    public void initialize()
    {
        dynamicPoints = new List<Vector3>();

        zValueImport();
        UVImport();
    }

    public void setValue(List<Vector3> genPoints)
    {
        dynamicPoints.AddRange(genPoints.ToArray());
    }

    public void generateDPS()
    {
        for(int i = dynamicPoints.Count; i < 92; i++)
        {
            dynamicPoints.Add(Vector3.zero);
        }

        Vector3 vertPoint = Vector3.zero;

        // 68
        vertPoint.x = calcPFI(35, 13, 1.0f / 3.0f, "x");
        vertPoint.y = calcPFI(47, 54, 2.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[68] = vertPoint;

        // 69
        vertPoint.x = calcPFI(31, 3, 1.0f / 3.0f, "x");
        vertPoint.y = calcPFI(40, 48, 2.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[69] = vertPoint;

        // 70
        vertPoint = calcPFI(54, 12, 1.0f / 2.0f);

        dynamicPoints[70] = vertPoint;

        // 71
        vertPoint = calcPFI(48, 4, 1.0f / 2.0f);

        dynamicPoints[71] = vertPoint;

        // 72
        vertPoint.x = calcPFI(31, 3, 2.0f / 3.0f, "x");
        vertPoint.y = calcPFI(41, 71, 2.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[72] = vertPoint;

        // 73
        vertPoint = calcPFI(48, 5, 1.0f / 2.0f);

        dynamicPoints[73] = vertPoint;

        // 74
        vertPoint = calcPFI(59, 6, 1.0f / 2.0f);

        dynamicPoints[74] = vertPoint;

        // 75
        vertPoint = calcPFI(58, 7, 1.0f / 2.0f);

        dynamicPoints[75] = vertPoint;

        // 76
        vertPoint = calcPFI(57, 8, 1.0f / 2.0f);

        dynamicPoints[76] = vertPoint;
        
        // 77
        vertPoint = calcPFI(56, 9, 1.0f / 2.0f);

        dynamicPoints[77] = vertPoint;

        // 78
        vertPoint = calcPFI(55, 10, 1.0f / 2.0f);

        dynamicPoints[78] = vertPoint;

        // 79
        vertPoint = calcPFI(54, 11, 1.0f / 2.0f);

        dynamicPoints[79] = vertPoint;

        // 80
        vertPoint.x = calcPFI(35, 13, 2.0f / 3.0f, "x");
        vertPoint.y = calcPFI(46, 70, 2.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[80] = vertPoint;

        // 81
        vertPoint = calcPFI(21, 22, 1.0f / 2.0f);

        dynamicPoints[81] = vertPoint;

        // 82
        vertPoint = calcPFI(36, 1, 1.0f / 2.0f);

        dynamicPoints[82] = vertPoint;

        // 83
        vertPoint = calcPFI(41, 2, 1.0f / 2.0f);

        dynamicPoints[83] = vertPoint;

        // 84
        vertPoint.x = calcPFI(41, 72, 1.0f / 2.0f, "x");
        vertPoint.y = calcPFI(41, 71, 1.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[84] = vertPoint;

        // 85
        vertPoint.x = calcPFI(40, 69, 1.0f / 2.0f, "x");
        vertPoint.y = calcPFI(40, 48, 1.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[85] = vertPoint;

        // 86
        vertPoint = calcPFI(39, 31, 1.0f / 2.0f);

        dynamicPoints[86] = vertPoint;

        // 87
        vertPoint = calcPFI(42, 35, 1.0f / 2.0f);

        dynamicPoints[87] = vertPoint;

        // 88
        vertPoint.x = calcPFI(47, 68, 1.0f / 2.0f, "x");
        vertPoint.y = calcPFI(47, 54, 1.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[88] = vertPoint;

        // 89
        vertPoint.x = calcPFI(46, 80, 1.0f / 2.0f, "x");
        vertPoint.y = calcPFI(46, 70, 1.0f / 3.0f, "y");
        vertPoint.z = 0.0f;

        dynamicPoints[89] = vertPoint;

        // 90
        vertPoint = calcPFI(46, 14, 1.0f / 2.0f);

        dynamicPoints[90] = vertPoint;

        // 91
        vertPoint = calcPFI(45, 15, 1.0f / 2.0f);

        dynamicPoints[91] = vertPoint;
    }

    //public void pointGenerateForeHead()
    //{
    //    // 92
    //    dynamicPoints.Add(calcForeHead(0, 1));

    //    // 93
    //    dynamicPoints.Add(calcForeHead(17, 82));

    //    // 94
    //    dynamicPoints.Add(calcForeHead(18, 36));

    //    // 95
    //    dynamicPoints.Add(calcForeHead(19, 37, 1.1f));

    //    // 96
    //    dynamicPoints.Add(calcForeHead(20, -1, 1.2f));

    //    // 97
    //    dynamicPoints.Add(calcForeHead(21, -1, 1.4f));

    //    // 98
    //    dynamicPoints.Add(calcForeHead(81, 27, 1.4f));

    //    // 99
    //    dynamicPoints.Add(calcForeHead(22, -1, 1.4f));

    //    // 100
    //    dynamicPoints.Add(calcForeHead(23, -1, 1.2f));

    //    // 101
    //    dynamicPoints.Add(calcForeHead(24, 44, 1.1f));

    //    // 102
    //    dynamicPoints.Add(calcForeHead(25, 45));

    //    // 103
    //    dynamicPoints.Add(calcForeHead(26, 91));

    //    // 104
    //    dynamicPoints.Add(calcForeHead(16, 15));

    //    pointGenerateForeHead2();
    //}

    //public void pointGenerateForeHead2()
    //{
    //    // 92
    //    dynamicPoints.Add(calcForeHead(92, -1));

    //    // 93
    //    dynamicPoints.Add(calcForeHead(93, -1));

    //    // 94
    //    dynamicPoints.Add(calcForeHead(94, -1));

    //    // 95
    //    dynamicPoints.Add(calcForeHead(95, -1, 1.1f));

    //    // 96
    //    dynamicPoints.Add(calcForeHead(96, -1, 1.2f));

    //    // 97
    //    dynamicPoints.Add(calcForeHead(97, -1, 1.4f));

    //    // 98
    //    dynamicPoints.Add(calcForeHead(98, -1, 1.4f));

    //    // 99
    //    dynamicPoints.Add(calcForeHead(99, -1, 1.4f));

    //    // 100
    //    dynamicPoints.Add(calcForeHead(100, -1, 1.2f));

    //    // 101
    //    dynamicPoints.Add(calcForeHead(101, -1, 1.1f));

    //    // 102
    //    dynamicPoints.Add(calcForeHead(102, -1));

    //    // 103
    //    dynamicPoints.Add(calcForeHead(103, -1));

    //    // 104
    //    dynamicPoints.Add(calcForeHead(104, -1));
    //}

    Vector3 calcForeHead(int left, int right, float offset = 1.0f)
    {
        Vector3 centerNor = dynamicPoints[81] - dynamicPoints[27];

        float len = Vector3.Distance(dynamicPoints[81], dynamicPoints[27]);

        Vector3 returnVal = Vector3.zero;

        Vector3 nor = centerNor;

        if(right > 0)
        {
            nor = dynamicPoints[left] - dynamicPoints[right];
        }
        nor.Normalize();

        returnVal = dynamicPoints[left] + nor * len * offset;

        return returnVal;
    }

    public Vector3 calcPFI(int pivotIndex, int targetIndex, float divVal)
    {
        return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal);
    }

    public float calcPFI(int pivotIndex, int targetIndex, float divVal, string arg)
    {
        if(arg.ToLower() == "x")
        {
            return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal).x;
        }
        else if (arg.ToLower() == "y")
        {
            return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal).y;
        }
        else
        {
            return 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("mesh gen");
            readWireData();
            generateDPSMesh();
        }
        //Vector3 leftPos = leftObj.transform.position;
        //Vector3 rightPos = rightObj.transform.position;

        //Vector3 clacPoint1 = calcDistancePoint(leftPos, rightPos, 1.0f / 3.0f);
        //Vector3 clacPoint2 = calcDistancePoint(leftPos, rightPos, 2.0f / 3.0f);

        //Debug.DrawLine(leftPos, clacPoint1, Color.red);
        //Debug.DrawLine(clacPoint1, clacPoint2, Color.green);
        //Debug.DrawLine(clacPoint2, rightPos, Color.blue);
    }

    Vector3 calcDistancePoint(Vector3 leftPoint, Vector3 rightPoint, float divCount)
    {
        Vector3 returnVec = Vector3.zero;

        Vector3 pivotVec = rightPoint - leftPoint;

        pivotVec.Normalize();

        float disValue = Vector3.Distance(leftPoint, rightPoint) * divCount;

        returnVec = leftPoint + pivotVec * disValue;

        return returnVec;
    }

    public List<float> calculateZValue(List<Vector3> vertices)
    {
        List<float> normalMags = new List<float>();

        MemoryStream ms = new MemoryStream(File.ReadAllBytes("NormalMag.dat"));
        BinaryReader br = new BinaryReader(ms);

        while (ms.Position < ms.Length)
        {
            normalMags.Add(br.ReadSingle());
        }

        br.Close();
        ms.Close();

        List<Vector3> normals = calculateNormals(vertices, loadAroundPoints().GetEnumerator());

        for (int i = 0; i < normalMags.Count; i++)
        {
            normalMags[i] = normals[i].magnitude / normalMags[i];
        }

        return normalMags;
    }

    public List<Vector3> calculateNormals(List<Vector3> vertices, Dictionary<int, List<List<int>>>.Enumerator vertAroundPointEnum)
    {
        //List<Vector3> vertices = new List<Vector3>();

        List<Vector3> normals = new List<Vector3>();
        normals.AddRange(vertices.ToArray());

        //Dictionary<int, List<List<int>>>.Enumerator vertAroundPointEnum = aroundPoint.GetEnumerator();

        while (vertAroundPointEnum.MoveNext())
        {
            int currentIndex = vertAroundPointEnum.Current.Key;
            Vector3 sourcePoint = vertices[currentIndex];

            List<List<int>> aroundTris = vertAroundPointEnum.Current.Value;

            Vector3 sumNormal = Vector3.zero;

            for (int i = 0; i < aroundTris.Count; i++)
            {
                List<int> twoPoint = aroundTris[i];

                Vector3 vec0 = vertices[twoPoint[0]] - sourcePoint;
                Vector3 vec1 = vertices[twoPoint[1]] - sourcePoint;

                sumNormal += Vector3.Cross(vec0, vec1);
            }

            //sumNormal /= aroundTris.Count;

            //Debug.Log(sumNormal.magnitude);
            normals[currentIndex] = sumNormal;
        }

        return normals;
    }

    Dictionary<int, List<List<int>>> loadAroundPoints()
    {
        Dictionary<int, List<List<int>>> aroundPoint = new Dictionary<int, List<List<int>>>();

        List<int> triangles = new List<int>();

        MemoryStream ms = new MemoryStream(File.ReadAllBytes("TriIndexs.dat"));
        BinaryReader br = new BinaryReader(ms);

        while (ms.Position < ms.Length)
        {
            triangles.Add(br.ReadInt32());
        }

        br.Close();
        ms.Close();

        aroundPoint = new Dictionary<int, List<List<int>>>();

        for (int i = 0; i < triangles.Count; i++)
        {
            List<int> aroundIndexs = new List<int>();

            int triIndex = i / 3;
            triIndex *= 3;

            int targetPoint = -1;

            bool isSame = false;
            int j = 0;

            while (aroundIndexs.Count != 2)
            {
                if (isSame)
                {
                    aroundIndexs.Add(triangles[triIndex + j]);
                }
                else
                {
                    if (i == triIndex + j)
                    {
                        isSame = true;
                        targetPoint = triangles[i];
                    }
                }
                j++;
                if (j == 3)
                {
                    j = 0;
                }
            }

            if (aroundPoint.ContainsKey(targetPoint))
            {
                aroundPoint[targetPoint].Add(aroundIndexs);
            }
            else
            {
                List<List<int>> points = new List<List<int>>();

                points.Add(aroundIndexs);

                aroundPoint.Add(targetPoint, points);
            }
        }

        Debug.Log(aroundPoint.Count);

        return aroundPoint;
    }
}
