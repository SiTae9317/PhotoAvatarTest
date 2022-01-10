using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBlendTest : MonoBehaviour
{
    public GameObject sourceObj;
    public GameObject targetObj;

    public GameObject pointTest;

    // Start is called before the first frame update
    void Start()
    {
        //targetObj.GetComponent<MeshFilter>().mesh = generateCubeMesh();
        //sourceObj.GetComponent<MeshFilter>().mesh = generateCubeMesh();

        Mesh tarMesh = targetObj.GetComponent<MeshFilter>().mesh;

        Vector3[] tarVertices = vertToWorldPos(tarMesh, targetObj.transform.localToWorldMatrix);

        Mesh sourceMesh = sourceObj.GetComponent<MeshFilter>().mesh;

        Vector3[] sourceVertices = vertToWorldPos(sourceMesh, sourceObj.transform.localToWorldMatrix);

        Mesh newMesh = new Mesh();

        newMesh.name = "NewMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector4> tangents = new List<Vector4>();


        vertices.AddRange(sourceMesh.vertices);
        normals.AddRange(sourceMesh.normals);
        triangles.AddRange(sourceMesh.triangles);
        uv.AddRange(sourceMesh.uv);
        tangents.AddRange(sourceMesh.tangents);

        newMesh.vertices = vertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.uv = uv.ToArray();
        //newMesh.tangents = tangents.ToArray();

        //newMesh.RecalculateTangents();

        //for(int i = 0; i < tangents.Count; i++)
        //{
        //    Debug.Log(newMesh.tangents[i]);
        //}

        //for(int j = 0; j < sourceVertices.Length; j++)
        {

            List<Vector3> deltaVert = new List<Vector3>();
            List<Vector3> deltaNor = new List<Vector3>();
            List<Vector3> deltaTan = new List<Vector3>();
            for (int i = 0; i < sourceVertices.Length; i++)
            {
                //if (i == j)
                //if (i == 7 || i == 15 || i == 19)
                {

                    //deltaVert.Add(Vector3.right);
                    deltaVert.Add(tarVertices[i] - sourceVertices[i]);
                }
                //else
                //{
                //    deltaVert.Add(Vector3.zero);
                //}

                GameObject go1 = Instantiate(pointTest);
                go1.transform.position = sourceVertices[i];
                go1.name = i.ToString();

                GameObject go = Instantiate(pointTest);
                go.transform.position = tarVertices[i];
                go.name = i.ToString();

                deltaNor.Add(Vector3.zero);
                deltaTan.Add(Vector3.zero);
            }

            newMesh.AddBlendShapeFrame("test", 100.0f, deltaVert.ToArray(), null, null);
            //newMesh.RecalculateBounds();
            //newMesh.RecalculateNormals();
            //newMesh.RecalculateTangents();
            //newMesh.normals = normals.ToArray();
            //newMesh.tangents = tangents.ToArray();
            newMesh.UploadMeshData(false);

            Vector3[] newDtv = new Vector3[deltaVert.Count];
            Vector3[] newDtn = new Vector3[deltaVert.Count];
            Vector3[] newDtt = new Vector3[deltaVert.Count];

            newMesh.GetBlendShapeFrameVertices(0, 0, newDtv, newDtn, newDtt);

            for(int i = 0; i < newDtv.Length; i++)
            {
                Debug.Log(deltaVert[i] + " " + newDtv[i] + " " + (deltaVert[i] == newDtv[i]).ToString());
                Debug.Log(deltaNor[i] + " " + newDtn[i] + " " + (deltaNor[i] == newDtn[i]).ToString());
                Debug.Log(deltaTan[i] + " " + newDtt[i] + " " + (deltaTan[i] == newDtt[i]).ToString());
            }
        }

        //newMesh.RecalculateTangents();

        for (int i = 0; i < tangents.Count; i++)
        {

            Debug.Log(normals[i] + " " + newMesh.normals[i] + " " + (normals[i] == newMesh.normals[i]).ToString());
            
            //Debug.Log(tangents[i] + " " + newMesh.tangents[i] + " " + (tangents[i] == newMesh.tangents[i]).ToString());
        }

        //Debug.Log(deltaVert[7]);
        //Debug.Log(deltaVert[15]);
        //Debug.Log(deltaVert[19]);

        //Debug.Log(sourceMesh.vertices[7]);
        //Debug.Log(sourceMesh.vertices[15]);
        //Debug.Log(sourceMesh.vertices[19]);

        SkinnedMeshRenderer smr = gameObject.AddComponent<SkinnedMeshRenderer>();
        smr.material = sourceObj.GetComponent<MeshRenderer>().material;
        smr.sharedMesh = newMesh;

        //Debug.Log(newMesh.vertices[7]);
        //Debug.Log(newMesh.vertices[15]);
        //Debug.Log(newMesh.vertices[19]);
    }

    Mesh generateCubeMesh()
    {
        List<Vector3> cubeVertices = new List<Vector3>();

        cubeVertices.Add(new Vector3(-0.5f, -0.5f, -0.5f));
        cubeVertices.Add(new Vector3(0.5f, -0.5f, -0.5f));
        cubeVertices.Add(new Vector3(-0.5f, -0.5f, 0.5f));
        cubeVertices.Add(new Vector3(0.5f, -0.5f, 0.5f));
        cubeVertices.Add(new Vector3(-0.5f, 0.5f, -0.5f));
        cubeVertices.Add(new Vector3(0.5f, 0.5f, -0.5f));
        cubeVertices.Add(new Vector3(-0.5f, 0.5f, 0.5f));
        cubeVertices.Add(new Vector3(0.5f, 0.5f, 0.5f));

        List<Vector3> cubeNormals = new List<Vector3>();

        cubeNormals.AddRange(cubeVertices.ToArray());

        List<int> cubeTriangles = new List<int>();

        Vector2[] cubeUV = new Vector2[cubeVertices.Count];

        cubeTriangles.Add(2);
        cubeTriangles.Add(0);
        cubeTriangles.Add(3);

        cubeTriangles.Add(3);
        cubeTriangles.Add(0);
        cubeTriangles.Add(1);

        cubeTriangles.Add(4);
        cubeTriangles.Add(6);
        cubeTriangles.Add(5);

        cubeTriangles.Add(5);
        cubeTriangles.Add(6);
        cubeTriangles.Add(7);

        cubeTriangles.Add(0);
        cubeTriangles.Add(4);
        cubeTriangles.Add(1);

        cubeTriangles.Add(1);
        cubeTriangles.Add(4);
        cubeTriangles.Add(5);

        cubeTriangles.Add(2);
        cubeTriangles.Add(6);
        cubeTriangles.Add(0);

        cubeTriangles.Add(0);
        cubeTriangles.Add(6);
        cubeTriangles.Add(4);

        cubeTriangles.Add(1);
        cubeTriangles.Add(5);
        cubeTriangles.Add(3);

        cubeTriangles.Add(3);
        cubeTriangles.Add(5);
        cubeTriangles.Add(7);

        cubeTriangles.Add(3);
        cubeTriangles.Add(7);
        cubeTriangles.Add(2);

        cubeTriangles.Add(2);
        cubeTriangles.Add(7);
        cubeTriangles.Add(6);

        Mesh newMesh = new Mesh();

        newMesh.name = "newMesh";

        newMesh.vertices = cubeVertices.ToArray();
        newMesh.normals = cubeNormals.ToArray();
        newMesh.triangles = cubeTriangles.ToArray();
        newMesh.uv = cubeUV;

        return newMesh;
    }

    Vector3[] vertToWorldPos(Mesh sourceMesh, Matrix4x4 ltw, bool isSrc = false)
    {
        Debug.Log(ltw);

        List<Vector3> vertices = new List<Vector3>();

        vertices.AddRange(sourceMesh.vertices);

        Debug.Log(vertices.Count);

        for (int i = 0; i < vertices.Count; i++)
        {
            if(!isSrc)
            {
                vertices[i] = ltw.MultiplyPoint3x4(vertices[i]);
            }
        }

        return vertices.ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
;