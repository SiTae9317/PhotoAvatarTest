using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWeight : MonoBehaviour
{
    public GameObject obj;
    public List<GameObject> joints;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uv;
    private List<int> triangles;

    private List<BoneWeight> boneWeights;
    private List<Transform> bones;
    private List<Matrix4x4> bindposes;

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        setMeshData();

        setRigging();

        generateMesh();
    }

    void setRigging()
    {
        boneWeights = new List<BoneWeight>();
        bones = new List<Transform>();
        bindposes = new List<Matrix4x4>();

        for(int i = 0; i < joints.Count; i++)
        {
            joints[i].transform.parent = this.transform;
        }

        bones.Add(this.transform);
        bindposes.Add(this.transform.worldToLocalMatrix);

        foreach (GameObject curObj in joints.ToArray())
        {
            bones.Add(curObj.transform);
            bindposes.Add(curObj.transform.worldToLocalMatrix);
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            BoneWeight bw = new BoneWeight();

            Vector3 pos = vertices[i];

            SortedList<float, int> sortingIndex = new SortedList<float, int>();

            float minDis = float.MaxValue;
            Stack<int> stackData = new Stack<int>();
            int minIndex = 0;

            stackData.Push(0);

            for (int j = 1; j < bones.Count; j++)
            {
                Vector3 TargetPos = bones[j].position;
                float dis = Vector3.Distance(pos, TargetPos);

                if(minDis > dis)
                {
                    minDis = dis;
                    stackData.Push(j);
                }
                else
                {
                    Stack<int> keepStack = new Stack<int>();

                    while(stackData.Count > 0)
                    {
                        keepStack.Push(stackData.Pop());
                    }

                    keepStack.Push(j);

                    while(keepStack.Count > 0)
                    {
                        stackData.Push(keepStack.Pop());
                    }
                }
            }

            bw.boneIndex0 = stackData.Pop();
            bw.boneIndex1 = stackData.Pop();
            bw.boneIndex2 = stackData.Pop();
            bw.boneIndex3 = stackData.Pop();

            bw.weight0 = 1.0f;
            bw.weight1 = 0.0f;
            bw.weight2 = 0.0f;
            bw.weight3 = 0.0f;

            boneWeights.Add(bw);
        }
    }

    void setMeshData()
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        mat = obj.GetComponent<MeshRenderer>().material;

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uv = new List<Vector2>();
        triangles = new List<int>();

        vertices.AddRange(mesh.vertices);
        normals.AddRange(mesh.normals);
        uv.AddRange(mesh.uv);
        triangles.AddRange(mesh.triangles);

        Destroy(obj.GetComponent<MeshFilter>());
        Destroy(obj.GetComponent<MeshRenderer>());
    }

    void generateMesh()
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "new mesh";
        newMesh.vertices = vertices.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.uv = uv.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.bindposes = bindposes.ToArray();
        newMesh.boneWeights = boneWeights.ToArray();

        SkinnedMeshRenderer smr = obj.AddComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = newMesh;
        smr.bones = bones.ToArray();
        smr.rootBone = this.transform;
        smr.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
