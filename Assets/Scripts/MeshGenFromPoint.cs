using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MeshGenFromPoint : MonoBehaviour
{
    public Mesh templateMesh;
    public MeshIndexResorting2 mr2;
    public Material curMat;

    public GameObject point;
    public List<Vector3> dynamicPoints;
    public List<int> guideTriangles;

    public List<GameObject> gos;

    public Mesh newMesh;

    public MeshFilter mf;

    public List<Vector3> oriVertices;
    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<int> triangles;
    public List<Vector2> uv;

    bool isWork = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(!isWork)
            {
                isWork = true;
                test();
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!isWork)
            {
                isWork = true;
                StartCoroutine(pointCor());
            }
        }
    }

    void test()
    {
        mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = curMat;

        Mesh mesh = templateMesh;

        newMesh = new Mesh();
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        oriVertices = new List<Vector3>();

        oriVertices.AddRange(mesh.vertices);
        vertices.AddRange(mesh.vertices);
        normals.AddRange(mesh.normals);
        triangles.AddRange(mesh.triangles);
        uv.AddRange(mesh.uv);

        newMesh.vertices = vertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.uv = uv.ToArray();

        mf.mesh = newMesh;


        dynamicPoints = new List<Vector3>();
        dynamicPoints.AddRange(mr2.dynamicPoints);

        point.SetActive(true);

        gos = new List<GameObject>();

        GameObject newParent = new GameObject();
        newParent.name = "p";

        for (int i = 0; i < dynamicPoints.Count; i++)
        {
            GameObject newGo = Instantiate(point);
            newGo.name = i.ToString();
            newGo.transform.position = dynamicPoints[i];

            newGo.transform.parent = newParent.transform;

            gos.Add(newGo);
        }

        point.SetActive(false);

        readWireData();

        StartCoroutine(pointCor());
    }

    IEnumerator pointCor()
    {
        float tRatio = mr2.faceWidth * mr2.faceHeight;

        while (true)
        {
            for(int i = 0; i < gos.Count; i++)
            {
                dynamicPoints[i] = gos[i].transform.position;
            }

            Vector3 p0 = dynamicPoints[0];
            p0.z = 0.0f;

            Vector3 p16 = dynamicPoints[16];
            p16.z = 0.0f;

            Vector3 p8 = dynamicPoints[8];
            p8.z = 0.0f;

            Vector3 p100 = dynamicPoints[100];
            p100.z = 0.0f;

            float faceWidth = Vector3.Distance(p0, p16);
            float faceHeight = Vector3.Distance(p8, p100);

            float cRatio = faceWidth * faceHeight;

            float ratio = cRatio / tRatio;

            ratio = Mathf.Sqrt(ratio);

            for (int i = 0; i < oriVertices.Count;i++)
            {
                Vector3 curVec = oriVertices[i];
                int[] result = mr2.isInner(curVec);

                if(result != null)
                {
                    Vector3 r = mr2.calcBarycentValue(result, curVec);

                    Vector2 vp0 = new Vector2(dynamicPoints[result[0]].x, dynamicPoints[result[0]].y);
                    Vector2 vp1 = new Vector2(dynamicPoints[result[1]].x, dynamicPoints[result[1]].y);
                    Vector2 vp2 = new Vector2(dynamicPoints[result[2]].x, dynamicPoints[result[2]].y);

                    Vector3 q = BarycentricCoordinates.calcBarycentricPoint(vp0, vp1, vp2, r);

                    q.z = curVec.z * ratio;
                    vertices[i] = q;
                }
            }

            newMesh.vertices = vertices.ToArray();

            mf.mesh = newMesh;

            yield return null;

            break;
        }

        isWork = false;
    }

    public void readWireData()
    {
        guideTriangles = new List<int>();

        string wireData = File.ReadAllText("NewWireData2.txt");

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
}
