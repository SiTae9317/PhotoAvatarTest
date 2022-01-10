using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class MeshIndexResorting : MonoBehaviour
{
    public GameObject point;
    public GameObject go;
    public Camera cam;

    public CircleGen ccg;

    public List<Vector3> dynamicPoints;

    //public bool isWireFrame = false;

    public int index = 0;
    private int beforeIndex = 0;

    public float senstive = 1;

    int maxCount = 0;

    List<Vector3> verties;
    List<Vector3> normals;
    List<int> triangles;
    List<int> newTriangles;
    List<Vector2> uv;

    List<int> guideTriangles;

    Dictionary<int, HashSet<Vector3>> vertexTriInfo;

    bool isPress = false;
    int adderVec = 0;

    bool isUDMovePress = false;
    bool isLRMovePress = false;
    bool isQRMovePress = false;

    float adderUDVec = 0;
    float adderLRVec = 0;
    float adderQRVec = 0;

    // Start is called before the first frame update

    void Start()
    {    
        string readText = Encoding.UTF8.GetString(File.ReadAllBytes("FaceIndexs.txt"));

        string[] splitLine = readText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        List<int> faceLineIndexs = new List<int>();

        for(int i = 0; i < splitLine.Length; i++)
        {
            string[] numbers = splitLine[i].Split(' ');

            for(int j = 0; j < numbers.Length; j++)
            {
                faceLineIndexs.Add(int.Parse(numbers[j]));
            }
        }

        Debug.Log(faceLineIndexs.Count);

        //StartCoroutine(randomValueCheck());

        //return;

        newTriangles = new List<int>();

        Mesh mesh = go.GetComponent<MeshFilter>().mesh;

        verties = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();

        Matrix4x4 ltw = go.transform.localToWorldMatrix;

        verties.AddRange(mesh.vertices);

        for (int i = 0; i < verties.Count; i++)
        {
            verties[i] = ltw.MultiplyPoint3x4(verties[i]);
        }

        normals.AddRange(mesh.normals);
        triangles.AddRange(mesh.triangles);
        uv.AddRange(mesh.uv);

        vertexTriInfo = new Dictionary<int, HashSet<Vector3>>();

        for (int i = 0; i < triangles.Count; i++)
        {
            int vertIndex = triangles[i];

            int baseIndex = vertIndex / 3;

            Vector3 indices = new Vector3(triangles[baseIndex + 0], triangles[baseIndex + 1], triangles[baseIndex + 2]);

            if (!vertexTriInfo.ContainsKey(vertIndex))
            {
                vertexTriInfo.Add(vertIndex, new HashSet<Vector3>());
            }

            vertexTriInfo[vertIndex].Add(indices);
        }

        Debug.Log(verties.Count);
        Debug.Log(normals.Count);
        Debug.Log(triangles.Count + " " + triangles.Count / 3);
        Debug.Log(uv.Count);

        //GameObject points = new GameObject();
        //points.name = "verts";

        //for(int i = 0; i < verties.Count; i++)
        //{
        //    GameObject verP = Instantiate(point);
        //    verP.transform.position = verties[i];

        //    verP.name = i.ToString();

        //    verP.transform.parent = points.transform;
        //}

        //point.SetActive(false);

        maxCount = triangles.Count / 3;

        maxCount = verties.Count;



        dynamicPoints = new List<Vector3>();

        for (int i = 0; i < faceLineIndexs.Count; i++)
        {
            float adderVal = 1.0f;
            if(i < 17)
            {
                adderVal = 1.01f;
            }
            dynamicPoints.Add(verties[faceLineIndexs[i]] * adderVal);
        }

        generateDPS();

        point.SetActive(true);

        GameObject guideLine = new GameObject();
        guideLine.name = "guide";

        point.transform.localScale = Vector3.one * 0.1f;

        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            GameObject verP = Instantiate(point);

            Vector3 curP = dynamicPoints[i];

            curP.z = -5.0f;

            verP.transform.position = curP;

            verP.name = i.ToString();

            verP.transform.parent = guideLine.transform;
        }

        //point.SetActive(false);

        readWireData();
    }

    void vertexGroupFromGuideLine()
    {
        Dictionary<Vector3, List<int>> verticesInGuide = new Dictionary<Vector3, List<int>>();

        for(int i = 0; i < guideTriangles.Count; i += 3)
        {
            Vector3 guideVec = new Vector3(guideTriangles[i + 0], guideTriangles[i + 1], guideTriangles[i + 2]);

            if (verticesInGuide.ContainsKey(guideVec))
            {
                Debug.Log(guideVec);
            }
            else
            {
                List<int> newIndexs = new List<int>();
                verticesInGuide.Add(guideVec, newIndexs);
            }
        }

        for(int i = 0; i < verties.Count; i++)
        {
            Vector3 curPoint = verties[i];
            int[] innerIndexs = isInner(curPoint);

            Vector3 guideVec = new Vector3(innerIndexs[0], innerIndexs[1], innerIndexs[2]);

            if(verticesInGuide.ContainsKey(guideVec))
            {
                verticesInGuide[guideVec].Add(i);
            }
            else
            {
                List<int> newIndexs = new List<int>();

                newIndexs.Add(i);

                verticesInGuide.Add(guideVec, newIndexs);
            }
        }

        Dictionary<Vector3, List<int>>.Enumerator vigEnum = verticesInGuide.GetEnumerator();

        int totalVertices = 0;

        while(vigEnum.MoveNext())
        {
            totalVertices += vigEnum.Current.Value.Count;
        }

        Debug.Log(verties.Count + " " + totalVertices);

        Debug.Log(verticesInGuide.Count);
    }

    public void readWireData()
    {
        guideTriangles = new List<int>();

        string wireData = File.ReadAllText("NewWireData.txt");

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

        vertexGroupFromGuideLine();
    }

    IEnumerator randomValueCheck()
    {
        point.SetActive(true);

        List<float> randomVal = new List<float>();

        List<GameObject> gos = new List<GameObject>();

        for(int i = 0; i < 100; i++)
        {
            gos.Add(Instantiate(point));
        }

        point.SetActive(false);

        while (true)
        {
            yield return null;

            if(randomVal.Count == 100)
            {
                //Debug.Log(randomVal.Count);
                randomVal.RemoveAt(0);
                //Debug.Log(randomVal.Count);
            }

            float yVal = UnityEngine.Random.Range(0.0f, 1000000.0f);

            randomVal.Add(yVal);

            int overCount = 0;
            int underCount = 0;

            for (int i = 0; i < randomVal.Count-1; i++)
            {
                Vector3 p0 = new Vector3(i, randomVal[i] / 10000.0f, 0.0f);
                Vector3 p1 = new Vector3(i + 1, randomVal[i + 1] / 10000.0f, 0.0f);

                if (p0.y < 20.0f && p1.y < 20.0f)
                {
                    Debug.DrawLine(p0, p1, Color.green);
                    underCount++;
                }
                else
                {
                    Debug.DrawLine(p0, p1, Color.red);
                    overCount++;
                }
            }
            if(overCount + underCount == 99)
            {
                Debug.Log((float)underCount / 99.0f);
            }
        }
    }

    int[] isInner(Vector3 targetPoint)
    {
        for(int i = 0; i < guideTriangles.Count; i += 3)
        {
            int index0 = guideTriangles[i + 0];
            int index1 = guideTriangles[i + 1];
            int index2 = guideTriangles[i + 2];

            Vector2 p0 = new Vector2(dynamicPoints[index0].x, dynamicPoints[index0].y);
            Vector2 p1 = new Vector2(dynamicPoints[index1].x, dynamicPoints[index1].y);
            Vector2 p2 = new Vector2(dynamicPoints[index2].x, dynamicPoints[index2].y);
            Vector2 pp = new Vector2(targetPoint.x, targetPoint.y);

            Vector3 hitpos = Vector3.zero;

            if (CustomRayCast.triangleRayCast(p0, p1, p2, pp, Vector3.back, ref hitpos))
            {
                int[] returnVal = new int[3];
                returnVal[0] = index0;
                returnVal[1] = index1;
                returnVal[2] = index2;

                //Vector3 r = Vector3.zero;

                //BarycentricCoordinates.barycent(p0, p1, p2, pp, out r);

                //Vector2 vp0 = new Vector2(newVertices[index0].x, newVertices[index0].y);
                //Vector2 vp1 = new Vector2(newVertices[index1].x, newVertices[index1].y);
                //Vector2 vp2 = new Vector2(newVertices[index2].x, newVertices[index2].y);

                //Vector3 q = BarycentricCoordinates.calcBarycentricPoint(vp0, vp1, vp2, r);

                //newVertices[i] = q;

                return returnVal;
            }
        }

        return null;

    }

    //void OnPreRender()
    //{
    //    if(isWireFrame)
    //    {
    //        GL.wireframe = true;
    //    }
    //}
    //void OnPostRender()
    //{
    //    if(isWireFrame)
    //    {
    //        GL.wireframe = false;
    //    }
    //}


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Debug.Log(Physics.Raycast(ray, out hit));

            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.triangleIndex);
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            isPress = true;
            adderVec = 1;
        }
        if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            isPress = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isPress = true;
            adderVec = -1;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isPress = false;
        }

        if(isPress)
        {
            index += adderVec;
        }

        index = Mathf.Min(index, maxCount - 1);
        index = Mathf.Max(index, 0);

        //if (beforeIndex != index)
        //{
        //    beforeIndex = index;

        //    newTriCalc();
        //    setMesh();
        //}

        if(Input.GetKeyDown(KeyCode.W))
        {
            isUDMovePress = true;
            adderUDVec = senstive;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            isUDMovePress = false;
            adderUDVec = 0;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isUDMovePress = true;
            adderUDVec = -senstive;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            isUDMovePress = false;
            adderUDVec = 0;
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            isLRMovePress = true;
            adderLRVec = -senstive;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            isLRMovePress = false;
            adderLRVec = 0;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            isLRMovePress = true;
            adderLRVec = senstive;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            isLRMovePress = false;
            adderLRVec = 0;
        }

        if(isUDMovePress)
        {
            cam.gameObject.transform.position += Vector3.up * adderUDVec;
        }

        if (isLRMovePress)
        {
            cam.gameObject.transform.position += Vector3.right * adderLRVec;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isQRMovePress = true;
            adderQRVec = senstive;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            isQRMovePress = false;
            adderQRVec = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isQRMovePress = true;
            adderQRVec = -senstive;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            isQRMovePress = false;
            adderQRVec = 0;
        }

        if(isQRMovePress)
        {
            float orthVal = cam.orthographicSize + adderQRVec;

            orthVal = Mathf.Max(orthVal, 0.0001f);

            senstive = orthVal/100.0f;

            cam.orthographicSize = orthVal;
        }

        //int remindCount = guideTriangles.Count - 462;

        //guideTriangles.RemoveRange(462, remindCount);

        for (int i = 0; i < guideTriangles.Count; i += 3)
        {
            int v0 = guideTriangles[i + 0];
            int v1 = guideTriangles[i + 1];
            int v2 = guideTriangles[i + 2];

            Debug.DrawLine(dynamicPoints[v0], dynamicPoints[v1], Color.green);
            Debug.DrawLine(dynamicPoints[v1], dynamicPoints[v2], Color.green);
            Debug.DrawLine(dynamicPoints[v2], dynamicPoints[v0], Color.green);
        }

        int[] returnVal = isInner(verties[index]);
        point.transform.position = verties[index];

        if (returnVal != null)
        {
            Vector3 vp0 = dynamicPoints[returnVal[0]];
            Vector3 vp1 = dynamicPoints[returnVal[1]];
            Vector3 vp2 = dynamicPoints[returnVal[2]];

            vp0.z -= 5.0f;
            vp1.z -= 5.0f;
            vp2.z -= 5.0f;

            Debug.DrawLine(vp0, vp1, Color.red);
            Debug.DrawLine(vp1, vp2, Color.red);
            Debug.DrawLine(vp2, vp0, Color.red);
        }
        else
        {
            Debug.Log("error");
        }

        //if(Input.GetKeyDown(KeyCode.O))
        //{
        //    string exportData = "";
        //    for(int i = 0; i < guideTriangles.Count; i += 3)
        //    {
        //        exportData += guideTriangles[i + 0].ToString();
        //        exportData += "\t";
        //        exportData += guideTriangles[i + 1].ToString();
        //        exportData += "\t";
        //        exportData += guideTriangles[i + 2].ToString();
        //        exportData += "\r\n";
        //    }

        //    File.WriteAllText("NewWireData.txt", exportData);
        //}
    }

    void newTriCalc()
    {
        newTriangles.Clear();

        for (int i = 0; i < index; i++)
        {
            newTriangles.Add(triangles[i * 3 + 0]);
            newTriangles.Add(triangles[i * 3 + 1]);
            newTriangles.Add(triangles[i * 3 + 2]);
        }
    }

    void setMesh()
    {
        Mesh newMesh = new Mesh();

        newMesh.vertices = verties.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.uv = uv.ToArray();

        go.GetComponent<MeshFilter>().mesh = newMesh;
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

        Vector3 lp = dynamicPoints[0] + dynamicPoints[0] - dynamicPoints[1];
        Vector3 rp = dynamicPoints[16] + dynamicPoints[16] - dynamicPoints[15];

        ccg.setPoints(lp, rp);
        dynamicPoints.Add(lp);
        dynamicPoints.Add(rp);
        dynamicPoints.AddRange(ccg.foreHeadGenerate(12, 7).ToArray());

        float leftXValue = Vector3.Distance(dynamicPoints[0], dynamicPoints[2]) / 2.0f;
        float rightValue = Vector3.Distance(dynamicPoints[14], dynamicPoints[16]) / 2.0f;


        Vector3 checkP0 = Vector3.zero;
        Vector3 checkP1 = Vector3.zero;
        Vector3 normalVal = Vector3.zero;

        checkP0 = dynamicPoints[0];
        checkP0.z = 0.0f;

        checkP1 = dynamicPoints[36];
        checkP1.z = 0.0f;

        normalVal = checkP0 - checkP1;
        normalVal.Normalize();

        Vector3 earP0 = dynamicPoints[0] + normalVal * leftXValue;
        Vector3 earP1 = dynamicPoints[1] + normalVal * leftXValue;
        Vector3 earP2 = dynamicPoints[2] + normalVal * leftXValue;
        Vector3 earP92 = dynamicPoints[92] + normalVal * leftXValue;

        checkP0 = dynamicPoints[16];
        checkP0.z = 0.0f;

        checkP1 = dynamicPoints[45];
        checkP1.z = 0.0f;

        normalVal = checkP0 - checkP1;
        normalVal.Normalize();

        Vector3 earP14 = dynamicPoints[14] + normalVal * rightValue;
        Vector3 earP15 = dynamicPoints[15] + normalVal * rightValue;
        Vector3 earP16 = dynamicPoints[16] + normalVal * rightValue;
        Vector3 earP93 = dynamicPoints[93] + normalVal * rightValue;

        dynamicPoints.Add(earP0);
        dynamicPoints.Add(earP1);
        dynamicPoints.Add(earP2);
        dynamicPoints.Add(earP92);
        dynamicPoints.Add(earP14);
        dynamicPoints.Add(earP15);
        dynamicPoints.Add(earP16);
        dynamicPoints.Add(earP93);

        for(int i = 0; i < dynamicPoints.Count; i++)
        {
            Vector3 curVec = dynamicPoints[i];
            curVec.z = 0.0f;
            dynamicPoints[i] = curVec;
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
