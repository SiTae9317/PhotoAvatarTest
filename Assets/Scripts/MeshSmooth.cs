using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinePointData
{
    public OutlinePointData(Vector3 curPos, int curIndex)
    {
        position = curPos;
        index = curIndex;
    }

    public Vector3 position;
    public int index;
}

public class VertexInfomation
{
    public VertexInfomation()
    {
        vertex = Vector3.zero;
        normal = Vector3.zero;
        uv = Vector2.zero;
    }

    public VertexInfomation(Vector3 curVertex, Vector3 curNormal, Vector2 curUV)
    {
        vertex = curVertex;
        normal = curNormal;
        uv = curUV;
    }

    public override int GetHashCode()
    {
        //return vertex.GetHashCode();
        return uv.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        VertexInfomation other = obj as VertexInfomation;
        return vertex == other.vertex && normal == other.normal && uv == other.uv;
        //return vertex == other.vertex;
    }

    public Vector3 vertex;
    public Vector3 normal;
    public Vector2 uv;
}

public class MeshSmooth : MonoBehaviour
{
    public GameObject go;

    public int level = 1;

    public GameObject point;

    public GameObject image;

    private HashSet<int> outlineIndexs;
    //private HashSet<VertexInfomation> vis;
    //private HashSet<VertexInfomation> outlineVis;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<int> triangles;
    private List<Vector2> uv;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    VertexInfomation viDataFromIndex(int index)
    {
        return new VertexInfomation(vertices[index], normals[index], uv[index]);
    }

    void initialize()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();

        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        vertices.AddRange(mesh.vertices);

        for(int i = 0; i < vertices.Count; i++)
        {
            normals.Add(vertices[i].normalized);
        }
        //normals.AddRange(mesh.normals);
        triangles.AddRange(mesh.triangles);
        uv.AddRange(mesh.uv);

        Dictionary<VertexInfomation, int> outlineVis = new Dictionary<VertexInfomation, int>();

        outlineVis.Add(viDataFromIndex(76), 76);
        outlineVis.Add(viDataFromIndex(126), 126);
        outlineVis.Add(viDataFromIndex(106), 106);
        outlineVis.Add(viDataFromIndex(86), 86);
        outlineVis.Add(viDataFromIndex(15), 15);
        outlineVis.Add(viDataFromIndex(35), 35);
        outlineVis.Add(viDataFromIndex(55), 55);
        outlineVis.Add(viDataFromIndex(60), 60);
        outlineVis.Add(viDataFromIndex(45), 45);
        outlineVis.Add(viDataFromIndex(25), 25);
        outlineVis.Add(viDataFromIndex(5), 5);
        outlineVis.Add(viDataFromIndex(96), 96);
        outlineVis.Add(viDataFromIndex(116), 116);
        outlineVis.Add(viDataFromIndex(136), 136);
        outlineVis.Add(viDataFromIndex(66), 66);

        for (int i = 0; i < level; i++)
        {
            Dictionary<VertexInfomation, int> vis = new Dictionary<VertexInfomation, int>();

            for (int j = 0; j < vertices.Count; j++)
            {
                vis.Add(viDataFromIndex(j), j);
            }

            List<int> newTriangles = new List<int>();

            for (int j = 0; j < triangles.Count; j += 3)
            {
                int index0 = triangles[j + 0];
                int index1 = triangles[j + 1];
                int index2 = triangles[j + 2];

                VertexInfomation vi0 = viDataFromIndex(index0);
                VertexInfomation vi1 = viDataFromIndex(index1);
                VertexInfomation vi2 = viDataFromIndex(index2);

                float dis0 = Vector3.Distance(vi0.vertex, vi1.vertex);
                float dis1 = Vector3.Distance(vi1.vertex, vi2.vertex);
                float dis2 = Vector3.Distance(vi2.vertex, vi0.vertex);

                List<float> sortingDis = new List<float>();
                sortingDis.Add(dis0);
                sortingDis.Add(dis1);
                sortingDis.Add(dis2);

                sortingDis.Sort();

                if (sortingDis[2] == dis0)
                {
                    Vector3 newVert = vi0.vertex + vi1.vertex;
                    newVert /= 2.0f;

                    Vector3 newNor = vi0.normal + vi1.normal;
                    newNor /= 2.0f;

                    Vector2 newUV = vi0.uv + vi1.uv;
                    newUV /= 2.0f;

                    VertexInfomation newVI = new VertexInfomation(newVert, newNor, newUV);

                    int newIndex = 0;

                    if(!vis.ContainsKey(newVI))
                    {
                        newIndex = vis.Count;

                        vis.Add(newVI, newIndex);
                    }
                    else
                    {
                        newIndex = vis[newVI];
                    }

                    newTriangles.Add(index0);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index2);

                    newTriangles.Add(index2);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index1);

                    if(!outlineVis.ContainsKey(newVI))
                    {
                        if (outlineVis.ContainsKey(vi0) && outlineVis.ContainsKey(vi1))
                        {
                            outlineVis.Add(newVI, newIndex);
                        }
                    }
                }
                else if (sortingDis[2] == dis1)
                {
                    Vector3 newVert = vi1.vertex + vi2.vertex;
                    newVert /= 2.0f;

                    Vector3 newNor = vi1.normal + vi2.normal;
                    newNor /= 2.0f;

                    Vector2 newUV = vi1.uv + vi2.uv;
                    newUV /= 2.0f;

                    VertexInfomation newVI = new VertexInfomation(newVert, newNor, newUV);

                    int newIndex = 0;

                    if (!vis.ContainsKey(newVI))
                    {
                        newIndex = vis.Count;

                        vis.Add(newVI, newIndex);
                    }
                    else
                    {
                        newIndex = vis[newVI];
                    }

                    newTriangles.Add(index1);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index0);

                    newTriangles.Add(index0);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index2);

                    if (!outlineVis.ContainsKey(newVI))
                    {
                        if (outlineVis.ContainsKey(vi1) && outlineVis.ContainsKey(vi2))
                        {
                            outlineVis.Add(newVI, newIndex);
                        }
                    }
                }
                else if (sortingDis[2] == dis2)
                {
                    Vector3 newVert = vi2.vertex + vi0.vertex;
                    newVert /= 2.0f;

                    Vector3 newNor = vi2.normal + vi0.normal;
                    newNor /= 2.0f;

                    Vector2 newUV = vi2.uv + vi0.uv;
                    newUV /= 2.0f;

                    VertexInfomation newVI = new VertexInfomation(newVert, newNor, newUV);

                    int newIndex = 0;

                    if (!vis.ContainsKey(newVI))
                    {
                        newIndex = vis.Count;

                        vis.Add(newVI, newIndex);
                    }
                    else
                    {
                        newIndex = vis[newVI];
                    }

                    newTriangles.Add(index2);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index1);

                    newTriangles.Add(index1);
                    newTriangles.Add(newIndex);
                    newTriangles.Add(index0);

                    if (!outlineVis.ContainsKey(newVI))
                    {
                        if (outlineVis.ContainsKey(vi2) && outlineVis.ContainsKey(vi0))
                        {
                            outlineVis.Add(newVI, newIndex);
                        }
                    }
                }
            }

            int totalCount = vis.Count;

            vertices.Clear();
            normals.Clear();
            uv.Clear();

            for(int j = 0; j < totalCount; j++)
            {
                vertices.Add(Vector3.zero);
                normals.Add(Vector3.zero);
                uv.Add(Vector2.zero);
            }

            Dictionary<VertexInfomation, int>.Enumerator visEnum = vis.GetEnumerator();

            while(visEnum.MoveNext())
            {
                int curValue = visEnum.Current.Value;
                VertexInfomation curVI = visEnum.Current.Key;

                vertices[curValue] = curVI.vertex;
                normals[curValue] = curVI.normal;
                uv[curValue] = curVI.uv;
            }

            triangles.Clear();
            triangles.AddRange(newTriangles.ToArray());
            newTriangles.Clear();

            Mesh newMesh = new Mesh();
            newMesh.vertices = vertices.ToArray();
            newMesh.normals = normals.ToArray();
            newMesh.triangles = triangles.ToArray();
            newMesh.uv = uv.ToArray();

            mf.mesh = newMesh;
        }

        Dictionary<VertexInfomation, int>.Enumerator ovisEnum = outlineVis.GetEnumerator();

        point.SetActive(true);

        List<OutlinePointData> opds = new List<OutlinePointData>();

        while (ovisEnum.MoveNext())
        {
            int curIndex = ovisEnum.Current.Value;
            string indexName = curIndex.ToString();
            Vector3 position = ovisEnum.Current.Key.vertex;

            OutlinePointData opd = new OutlinePointData(position, curIndex);
            opds.Add(opd);

            //GameObject newGo = Instantiate(point);
            //newGo.transform.parent = gameObject.transform;

            //newGo.name = indexName;
            //newGo.transform.position = position;
        }

        point.SetActive(false);
        Debug.Log(outlineVis.Count);

        opds.Sort(delegate(OutlinePointData left, OutlinePointData right)
        {
            if(left.position.y == right.position.y)
            {
                if(left.position.x > right.position.x)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (left.position.y > right.position.y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        });

        HairWireGenerator hwg = gameObject.AddComponent<HairWireGenerator>();
        hwg.hairObj = go;
        hwg.point = point;
        hwg.image = image;
        hwg.isDebug = true;

        List<int> calcSortData = new List<int>();

        for(int i = 0; i < opds.Count;i++)
        {
            calcSortData.Add(opds[i].index);
        }
        hwg.initialize2(calcSortData);
    }

    //void initialize()
    //{
    //    outlineIndexs = new HashSet<int>();

    //    outlineVis.Add(viDataFromIndex(76);
    //    outlineVis.Add(viDataFromIndex(126);
    //    outlineVis.Add(viDataFromIndex(106);
    //    outlineVis.Add(viDataFromIndex(86);
    //    outlineVis.Add(viDataFromIndex(15);
    //    outlineVis.Add(viDataFromIndex(35);
    //    outlineVis.Add(viDataFromIndex(55);
    //    outlineVis.Add(viDataFromIndex(60);
    //    outlineVis.Add(viDataFromIndex(45);
    //    outlineVis.Add(viDataFromIndex(25);
    //    outlineVis.Add(viDataFromIndex(5);
    //    outlineVis.Add(viDataFromIndex(96);
    //    outlineVis.Add(viDataFromIndex(116);
    //    outlineVis.Add(viDataFromIndex(136);
    //    outlineVis.Add(viDataFromIndex(66);

    //    MeshFilter mf = go.GetComponent<MeshFilter>();
    //    Mesh mesh = mf.mesh;

    //    List<Vector3> vertices = new List<Vector3>();
    //    List<Vector3> normals = new List<Vector3>();
    //    List<int> triangles = new List<int>();
    //    List<Vector2> uv = new List<Vector2>();

    //    vertices.AddRange(mesh.vertices);
    //    normals.AddRange(mesh.normals);
    //    triangles.AddRange(mesh.triangles);
    //    uv.AddRange(mesh.uv);

    //    for (int i = 0; i < level; i++)
    //    {
    //        for (int j = 0; j < triangles.Count; j += 3)
    //        {
    //            int index0 = triangles[j + 0];
    //            int index1 = triangles[j + 1];
    //            int index2 = triangles[j + 2];

    //            float dis0 = Vector3.Distance(vertices[index0], vertices[index1]);
    //            float dis1 = Vector3.Distance(vertices[index0], vertices[index2]);
    //            float dis2 = Vector3.Distance(vertices[index2], vertices[index1]);

    //            List<float> sortingDis = new List<float>();
    //            sortingDis.Add(dis0);
    //            sortingDis.Add(dis1);
    //            sortingDis.Add(dis2);

    //            sortingDis.Sort();

    //            if (sortingDis[0] == dis0)
    //            {

    //            }
    //            else if (sortingDis[0] == dis1)
    //            {

    //            }
    //            else if (sortingDis[0] == dis2)
    //            {

    //            }
    //        }
    //    }
    //}
}
