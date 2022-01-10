using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairWireGenerator : MonoBehaviour
{
    public bool isDebug = false;
    public GameObject hairObj;

    public GameObject point;

    public GameObject image;

    private List<Vector3> worldVertices = null;
    private List<Vector3> newWorldVertices = null;
    private List<Vector3> guideVertices = null;
    private List<Vector3> newGuideVertices = null;
    private List<Vector3> guideNormals = null;
    private List<int> guideIndexs = null;

    private List<GameObject> newGuidePoint;

    private Mesh newMesh;

    private List<float> alphas;

    private int width;
    private int height;

    // Start is called before the first frame update
    void Start()
    {
        //initialize();
    }

    public void initialize2(List<int> calcGuideIndex)
    {
        alphas = new List<float>();
        Texture2D curTex = image.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        width = curTex.width;
        height = curTex.height;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                alphas.Add(curTex.GetPixel(x, y).a);
            }
        }

        newMesh = new Mesh();

        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        worldVertices = new List<Vector3>();
        newWorldVertices = new List<Vector3>();
        guideVertices = new List<Vector3>();
        newGuideVertices = new List<Vector3>();
        guideIndexs = new List<int>();

        guideNormals = new List<Vector3>();

        Mesh mesh = hairObj.GetComponent<MeshFilter>().mesh;
        Matrix4x4 ltw = hairObj.transform.localToWorldMatrix;

        Vector3[] vertices = mesh.vertices;

        normals.AddRange(mesh.normals);
        triangles.AddRange(mesh.triangles);
        uv.AddRange(mesh.uv);

        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices.Add(ltw.MultiplyPoint3x4(vertices[i]));
        }

        newWorldVertices.AddRange(worldVertices.ToArray());

        for(int i = 0; i < calcGuideIndex.Count; i++)
        {
            guideVertices.Add(worldVertices[calcGuideIndex[i]]);
            guideNormals.Add(normals[calcGuideIndex[i]]);
        }

        for (int i = 0; i < calcGuideIndex.Count - 4; i += 2)
        {
            guideIndexs.Add(i + 1);
            guideIndexs.Add(i + 0);
            guideIndexs.Add(i + 2);

            guideIndexs.Add(i + 1);
            guideIndexs.Add(i + 2);
            guideIndexs.Add(i + 3);
        }

        guideIndexs.Add(calcGuideIndex.Count - 2);
        guideIndexs.Add(calcGuideIndex.Count - 3);
        guideIndexs.Add(calcGuideIndex.Count - 1);

        point.SetActive(true);

        newGuidePoint = new List<GameObject>();

        for (int i = 0; i < guideVertices.Count; i++)
        {
            GameObject newGo = Instantiate(point);
            newGo.name = i.ToString();

            newGo.transform.position = guideVertices[i];
            newGo.transform.parent = gameObject.transform;

            newGuidePoint.Add(newGo);
        }

        point.SetActive(false);

        newGuideVertices.AddRange(guideVertices.ToArray());

        newMesh.vertices = newWorldVertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.uv = uv.ToArray();

        gameObject.AddComponent<MeshFilter>().mesh = newMesh;
        gameObject.AddComponent<MeshRenderer>().material = hairObj.GetComponent<MeshRenderer>().material;

        StartCoroutine(setNewGuideVertices());

        StartCoroutine(hairGuideDebug());

        hairObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator hairGuideDebug()
    {
        while (isDebug)
        {
            for (int i = 0; i < guideIndexs.Count; i += 3)
            {
                int index0 = guideIndexs[i + 0];
                int index1 = guideIndexs[i + 1];
                int index2 = guideIndexs[i + 2];

                //Debug.DrawLine(guideVertices[index0], guideVertices[index1], Color.green);
                //Debug.DrawLine(guideVertices[index1], guideVertices[index2], Color.green);
                //Debug.DrawLine(guideVertices[index2], guideVertices[index0], Color.green);

                Debug.DrawLine(newGuideVertices[index0], newGuideVertices[index1], Color.yellow);
                Debug.DrawLine(newGuideVertices[index1], newGuideVertices[index2], Color.yellow);
                Debug.DrawLine(newGuideVertices[index2], newGuideVertices[index0], Color.yellow);
            }

            yield return null;
        }
    }

    void initialize()
    {
        newMesh = new Mesh();

        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        worldVertices = new List<Vector3>();
        newWorldVertices = new List<Vector3>();
        guideVertices = new List<Vector3>();
        newGuideVertices = new List<Vector3>();
        guideIndexs = new List<int>();

        Mesh mesh = hairObj.GetComponent<MeshFilter>().mesh;
        Matrix4x4 ltw = hairObj.transform.localToWorldMatrix;

        Vector3[] vertices = mesh.vertices;

        normals.AddRange(mesh.normals);
        triangles.AddRange(mesh.triangles);
        uv.AddRange(mesh.uv);
        
        for(int i = 0; i < vertices.Length; i++)
        {
            worldVertices.Add(ltw.MultiplyPoint3x4(vertices[i]));
        }

        newWorldVertices.AddRange(worldVertices.ToArray());

        guideVertices.Add(worldVertices[76]);
        guideVertices.Add(worldVertices[126]);
        guideVertices.Add(worldVertices[106]);
        guideVertices.Add(worldVertices[86]);
        guideVertices.Add(worldVertices[15]);
        guideVertices.Add(worldVertices[35]);
        guideVertices.Add(worldVertices[55]);
        guideVertices.Add(worldVertices[60]);
        guideVertices.Add(worldVertices[45]);
        guideVertices.Add(worldVertices[25]);
        guideVertices.Add(worldVertices[5]);
        guideVertices.Add(worldVertices[96]);
        guideVertices.Add(worldVertices[116]);
        guideVertices.Add(worldVertices[136]);
        guideVertices.Add(worldVertices[66]);

        //int[,] indices = new int[13, 3] { { 0, 1, 14 }
        //                                , { 14, 1, 13 }
        //                                , { 1, 2, 13 }
        //                                , { 13, 2, 12 }
        //                                , { 2, 3, 12 }
        //                                , { 12, 3, 11 }
        //                                , { 3, 4, 11 }
        //                                , { 11, 4, 10 }
        //                                , { 4, 5, 10 }
        //                                , { 10, 5, 9 }
        //                                , { 5, 6, 9 }
        //                                , { 9, 6, 8 }
        //                                , { 6, 7, 8 }};

        int[,] indices = new int[13, 3] { { 14, 0, 1 }
                                        , { 1, 13, 14 }
                                        , { 13, 1, 2 }
                                        , { 2, 12, 13 }
                                        , { 12, 2, 3 }
                                        , { 3, 11, 12 }
                                        , { 11, 3, 4 }
                                        , { 4, 10, 11 }
                                        , { 10, 4, 5 }
                                        , { 5, 9, 10 }
                                        , { 9, 5, 6 }
                                        , { 6, 8, 9 }
                                        , { 8, 6, 7 }};

        for (int i = 0; i < 13; i++)
        {
            guideIndexs.Add(indices[i, 0]);
            guideIndexs.Add(indices[i, 1]);
            guideIndexs.Add(indices[i, 2]);
        }

        point.SetActive(true);

        newGuidePoint = new List<GameObject>();

        for (int i = 0; i < guideVertices.Count; i++)
        {
            GameObject newGo = Instantiate(point);
            newGo.name = i.ToString();

            newGo.transform.position = guideVertices[i];
            newGo.transform.parent = gameObject.transform;

            newGuidePoint.Add(newGo);
        }

        point.SetActive(false);

        newGuideVertices.AddRange(guideVertices.ToArray());

        newMesh.vertices = newWorldVertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.uv = uv.ToArray();

        gameObject.AddComponent<MeshFilter>().mesh = newMesh;
        gameObject.AddComponent<MeshRenderer>().material = hairObj.GetComponent<MeshRenderer>().material;

        StartCoroutine(setNewGuideVertices());

        StartCoroutine(hairGuideDebug());
    }

    IEnumerator setNewGuideVertices()
    {
        while(true)
        {
            for (int i = 0; i < newGuideVertices.Count; i++)
            {
                int curX = Mathf.Max(0, (int)(Mathf.Min((int)newGuideVertices[i].x, width - 1)));
                int curY = Mathf.Max(0, (int)(Mathf.Min((int)newGuideVertices[i].y, height - 1)));

                if (alphas[curY * width + curX] == 0.0f)
                {
                    newGuidePoint[i].transform.position -= guideNormals[i];
                    newGuideVertices[i] = newGuidePoint[i].transform.position;
                }
            }

            verticesReposition();

            newMesh.vertices = newWorldVertices.ToArray();

            yield return null;
        }
    }

    void verticesReposition()
    {
        for (int i = 0; i < worldVertices.Count; i++)
        {
            Vector3 targetPoint = worldVertices[i];

            for (int j = 0; j < guideIndexs.Count; j += 3)
            {
                int index0 = guideIndexs[j + 0];
                int index1 = guideIndexs[j + 1];
                int index2 = guideIndexs[j + 2];

                Vector2 p0 = new Vector2(guideVertices[index0].x, guideVertices[index0].y);
                Vector2 p1 = new Vector2(guideVertices[index1].x, guideVertices[index1].y);
                Vector2 p2 = new Vector2(guideVertices[index2].x, guideVertices[index2].y);
                Vector2 pp = new Vector2(targetPoint.x, targetPoint.y);

                Vector3 hitpos = Vector3.zero;

                if (CustomRayCast.triangleRayCast(p0, p1, p2, pp, Vector3.forward, ref hitpos))
                {
                    Vector3 r = Vector3.zero;

                    BarycentricCoordinates.barycent(p0, p1, p2, pp, out r);

                    Vector2 vp0 = new Vector2(newGuideVertices[index0].x, newGuideVertices[index0].y);
                    Vector2 vp1 = new Vector2(newGuideVertices[index1].x, newGuideVertices[index1].y);
                    Vector2 vp2 = new Vector2(newGuideVertices[index2].x, newGuideVertices[index2].y);

                    Vector3 q = BarycentricCoordinates.calcBarycentricPoint(vp0, vp1, vp2, r);

                    float oriDis = Vector2.Distance(p0, p1);
                    float newDis = Vector2.Distance(vp0, vp1);

                    q.z = targetPoint.z * (newDis / oriDis);

                    newWorldVertices[i] = q;

                    break;
                }

                if (CustomRayCast.triangleRayCast(p0, p1, p2, pp, Vector3.back, ref hitpos))
                {
                    Vector3 r = Vector3.zero;

                    BarycentricCoordinates.barycent(p0, p1, p2, pp, out r);

                    Vector2 vp0 = new Vector2(newGuideVertices[index0].x, newGuideVertices[index0].y);
                    Vector2 vp1 = new Vector2(newGuideVertices[index1].x, newGuideVertices[index1].y);
                    Vector2 vp2 = new Vector2(newGuideVertices[index2].x, newGuideVertices[index2].y);

                    Vector3 q = BarycentricCoordinates.calcBarycentricPoint(vp0, vp1, vp2, r);

                    q.z = targetPoint.z;

                    newWorldVertices[i] = q;

                    break;
                }
            }
        }
    }
}
