using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexNumber : MonoBehaviour
{
    public int vertIndex = 0;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public GameObject point;
    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    void setData()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

        vertices.AddRange(mesh.vertices);

        Matrix4x4 ltw = gameObject.transform.localToWorldMatrix;

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = ltw.MultiplyPoint3x4(vertices[i]);
        }

        triangles.AddRange(mesh.triangles);

        point.transform.localScale *= 1000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            vertIndex++;
            vertIndex = vertIndex < vertices.Count ? vertIndex : 0;

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            vertIndex--;
            vertIndex = vertIndex < 0 ? vertices.Count - 1 : vertIndex;
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            setData();
        }
        if(vertices.Count > 0)
        {
            point.transform.position = vertices[vertIndex];
        }
    }
}
