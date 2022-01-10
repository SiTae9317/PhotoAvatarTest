using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SubMeshTest : MonoBehaviour
{
    public GameObject go;
    public GameObject point;
    private List<int> detectIndexs;
    private List<Vector3> dynamicPoints;
    private List<int> guideTriangles;
    // Start is called before the first frame update
    void Start()
    {
        readWireData();
        infoInit();
        setMeshData();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < guideTriangles.Count; i += 3)
        {
            int index0 = guideTriangles[i + 0];
            int index1 = guideTriangles[i + 1];
            int index2 = guideTriangles[i + 2];

            Debug.DrawLine(dynamicPoints[index0], dynamicPoints[index1], Color.green);
            Debug.DrawLine(dynamicPoints[index0], dynamicPoints[index2], Color.green);
            Debug.DrawLine(dynamicPoints[index2], dynamicPoints[index1], Color.green);
        }

    }

    public void readWireData()
    {
        guideTriangles = new List<int>();

        string wireData = File.ReadAllText("WireData.txt");

        string[] triDatas = wireData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < triDatas.Length; i++)
        {
            string[] indices = triDatas[i].Split('\t');

            for (int j = indices.Length - 1; j >= 0; j--)
            {
                //triangles.Add(changeNumber[int.Parse(indices[j])]);

                guideTriangles.Add(int.Parse(indices[j]));
            }
        }

        Debug.Log(guideTriangles.Count);
    }

    void infoInit()
    {
        detectIndexs = new List<int>();

        TextAsset ta = Resources.Load("Meshes\\Template2\\Fat_face\\point number_02") as TextAsset;
        string faceIndexData = ta.text;

        string[] lineDatas = faceIndexData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lineDatas.Length; i++)
        {
            string[] curLine = lineDatas[i].Split('=');

            if (curLine.Length > 1)
            {
                Debug.Log(curLine[0] + " " + curLine[1]);

                detectIndexs.Add(int.Parse(curLine[curLine.Length - 1]));
            }
        }
    }

    void setMeshData()
    {
        Matrix4x4 ltw = go.transform.localToWorldMatrix;

        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = smr.sharedMesh;
        Debug.Log(mesh.subMeshCount);

        int subMeshCount = mesh.subMeshCount;

        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        List<Vector3> normals = new List<Vector3>();
        mesh.GetNormals(normals);

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = ltw.MultiplyPoint3x4(vertices[i]);
            normals[i] = ltw.MultiplyVector(normals[i]);
        }

        List<List<Vector2>> uv = new List<List<Vector2>>();
        List<List<int>> triangles = new List<List<int>>();

        for (int i = 0; i < subMeshCount; i++)
        {
            List<Vector2> cUV = new List<Vector2>();
            mesh.GetUVs(i, cUV);

            Debug.Log(cUV.Count);

            uv.Add(cUV);

            List<int> cTriangles = new List<int>();
            cTriangles.AddRange(mesh.GetTriangles(i));

            Debug.Log(cTriangles.Count);

            triangles.Add(cTriangles);
        }

        Mesh newMesh = new Mesh();

        newMesh.SetVertices(vertices);
        newMesh.SetNormals(normals);
        newMesh.subMeshCount = subMeshCount;

        for (int i = 0; i < subMeshCount; i++)
        {
            newMesh.SetUVs(i, uv[i]);

            int maxVal = int.MinValue;
            int minVal = int.MaxValue;

            for (int j = 0; j < triangles[i].Count; j++)
            {
                if (maxVal < triangles[i][j])
                {
                    maxVal = triangles[i][j];
                }
                if (minVal > triangles[i][j])
                {
                    minVal = triangles[i][j];
                }
            }

            Debug.Log(triangles[i].Count);

            Debug.Log(minVal + " " + maxVal);
            newMesh.SetTriangles(triangles[i].ToArray(), i);
        }

        GameObject newObj = new GameObject();
        newObj.name = "copy";
        newObj.AddComponent<MeshFilter>().mesh = newMesh;
        newObj.AddComponent<MeshRenderer>().materials = smr.materials;


        dynamicPoints = new List<Vector3>();

        for(int i = 0; i < detectIndexs.Count; i++)
        {
            //GameObject newGo = Instantiate(point);
            //newGo.name = i.ToString();
            //newGo.transform.position = vertices[detectIndexs[i]];
            //newGo.transform.parent = newObj.transform;

            dynamicPoints.Add(vertices[detectIndexs[i]]);
        }

        generateDPS();

        point.SetActive(true);
        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            GameObject newGo = Instantiate(point);
            newGo.name = i.ToString();
            newGo.transform.position = dynamicPoints[i];
            newGo.transform.parent = newObj.transform;
        }
        point.SetActive(false);
    }

    public void generateDPS()
    {
        for (int i = dynamicPoints.Count; i < 92; i++)
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

        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            Vector3 keepVec = dynamicPoints[i];
            keepVec.z = 0.0f;
            dynamicPoints[i] = keepVec;
        }
    }

    public Vector3 calcPFI(int pivotIndex, int targetIndex, float divVal)
    {
        return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal);
    }

    public float calcPFI(int pivotIndex, int targetIndex, float divVal, string arg)
    {
        if (arg.ToLower() == "x")
        {
            return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal).x;
        }
        else if (arg.ToLower() == "y")
        {
            return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal).y;
        }
        else if (arg.ToLower() == "z")
        {
            return calcDistancePoint(dynamicPoints[pivotIndex], dynamicPoints[targetIndex], divVal).z;
        }
        else
        {
            return 0.0f;
        }
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
}
