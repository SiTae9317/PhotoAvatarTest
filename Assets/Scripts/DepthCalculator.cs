using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DepthCalculator : MonoBehaviour
{
    //public GameObject go;

    //private Dictionary<int, List<List<int>>> aroundPoint;

    // Start is called before the first frame update
    void Start()
    {
        //initialize();
        //Matrix4x4 ltw = go.transform.localToWorldMatrix;

        //List<Vector3> vertices = new List<Vector3>();
        //List<int> triangles = new List<int>();

        //Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        //vertices.AddRange(mesh.vertices);
        //triangles.AddRange(mesh.triangles);

        //for (int i = 0; i < vertices.Count; i++)
        //{
        //    vertices[i]= ltw.MultiplyPoint3x4(vertices[i]);
        //}

        //initialize(triangles);
        //List<Vector3> normals = calculateNormals(vertices);

        //FileStream fs = new FileStream("TriIndexs.dat", FileMode.OpenOrCreate);
        //BinaryWriter bw = new BinaryWriter(fs);

        //for(int i = 0; i < triangles.Count; i++)
        //{
        //    bw.Write(triangles[i]);
        //}

        //bw.Close();
        //fs.Close();
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

    //void initialize()
    //{
    //    List<int> triangles = new List<int>();

    //    MemoryStream ms = new MemoryStream(File.ReadAllBytes("TriIndexs.dat"));
    //    BinaryReader br = new BinaryReader(ms);

    //    while(ms.Position < ms.Length)
    //    {
    //        triangles.Add(br.ReadInt32());
    //    }

    //    br.Close();
    //    ms.Close();

    //    aroundPoint = new Dictionary<int, List<List<int>>>();

    //    for (int i = 0; i < triangles.Count; i++)
    //    {
    //        List<int> aroundIndexs = new List<int>();

    //        int triIndex = i / 3;
    //        triIndex *= 3;

    //        int targetPoint = -1;

    //        bool isSame = false;
    //        int j = 0;

    //        while (aroundIndexs.Count != 2)
    //        {
    //            if (isSame)
    //            {
    //                aroundIndexs.Add(triangles[triIndex + j]);
    //            }
    //            else
    //            {
    //                if (i == triIndex + j)
    //                {
    //                    isSame = true;
    //                    targetPoint = triangles[i];
    //                }
    //            }
    //            j++;
    //            if (j == 3)
    //            {
    //                j = 0;
    //            }
    //        }

    //        if (aroundPoint.ContainsKey(targetPoint))
    //        {
    //            aroundPoint[targetPoint].Add(aroundIndexs);
    //        }
    //        else
    //        {
    //            List<List<int>> points = new List<List<int>>();

    //            points.Add(aroundIndexs);

    //            aroundPoint.Add(targetPoint, points);
    //        }
    //    }

    //    Debug.Log(aroundPoint.Count);
    //}

    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < vertices.Count; i++)
        //{
        //    Debug.DrawRay(vertices[i], normals[i]);
        //}
    }
}
