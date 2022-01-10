using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GeometryData
{
    public int vIndex;
    public int tIndex;
}

public class ObjDataLoad : MonoBehaviour
{
    List<string> paths = new List<string>();
    public GameObject tarObj;
    public Material mat;
    public int pathIndex = 0;
    public GameObject genObj = null;
    public bool isNext = false;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo di = new DirectoryInfo("E:\\ScanDatas\\");

        DirectoryInfo[] di1 = di.GetDirectories();

        for (int i = 0; i < di1.Length; i++)
        {
            DirectoryInfo[] di2 = di1[i].GetDirectories();

            for(int j = 0; j < di2.Length; j++)
            {
                FileInfo[] fis = di2[j].GetFiles();

                bool isFront = false;
                bool isBack = false;

                for (int k = 0; k < fis.Length; k++)
                {
                    string check = fis[k].Name;

                    if(check.Contains("_front"))
                    {
                        isFront = true;
                    }

                    if (check.Contains("_back"))
                    {
                        isBack = true;
                    }
                }

                if(isFront && isBack)
                {
                    continue;
                }

                paths.Add(di2[j].FullName + "\\");
            }
        }

        //loadObject("E:\\ScanDatas\\4\\12\\");

        //loadObject("E:\\ScanDatas\\1\\1\\");
        //Texture2D tex = new Texture2D(1, 1);
        //tex.LoadImage(File.ReadAllBytes());

        //string objData = File.ReadAllText("E:\\ScanDatas\\1\\1\\model_mesh.obj");
        //string[] lines = objData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        //Debug.Log(lines.Length);

        //List<Vector3> vertices = new List<Vector3>();
        //List<int> triangles = new List<int>();
        //List<Vector2> uv = new List<Vector2>();

        //Dictionary<int, Vector2> vertIndexAtUV = new Dictionary<int, Vector2>();

        //Dictionary<Vector2, int> vertIndexAtUV2 = new Dictionary<Vector2, int>();

        //int vCount = 0;
        //int vtCount = 0;
        //int fCount = 0;

        //int triMin = int.MaxValue;
        //int triMax = int.MinValue;

        //for(int i = 0; i < lines.Length; i++)
        //{
        //    string curLine = lines[i];

        //    string[] lineData = curLine.Split(' ');

        //    if (lineData[0] == "v")
        //    {
        //        Vector3 curVec = new Vector3(float.Parse(lineData[1]), float.Parse(lineData[2]), float.Parse(lineData[3]));

        //        vertices.Add(curVec);

        //        vCount++;
        //    }
        //    else if(lineData[0] == "vt")
        //    {
        //        Vector2 curUV = new Vector2(float.Parse(lineData[1]), float.Parse(lineData[2]));

        //        uv.Add(curUV);

        //        vtCount++;
        //    }
        //    else if (lineData[0] == "f")
        //    {
        //        for (int j = lineData.Length - 1; j >= 1; j--)
        //        //for (int j = 1; j < lineData.Length; j++)
        //        {
        //            string[] faceData = lineData[j].Split('/');

        //            //if(triangles.Count + 1 != int.Parse(faceData[1]))
        //            //{
        //            //    Debug.Log("error");
        //            //    break;
        //            //}
        //            //else
        //            {
        //                int curIndex = int.Parse(faceData[0]) - 1;
        //                if (triMin > curIndex)
        //                {
        //                    triMin = curIndex;
        //                }

        //                if(triMax < curIndex)
        //                {
        //                    triMax = curIndex;
        //                }

        //                triangles.Add(curIndex);
        //            }
        //        }

        //        fCount++;
        //    }
        //}

        //int startIndex = 0;
        //int endIndex = 60000;
        //bool end = false;

        //while (true)
        //{
        //    if (startIndex + endIndex > triangles.Count)
        //    {
        //        endIndex = triangles.Count - startIndex;
        //        end = true;
        //    }
        //    Debug.Log(startIndex + " " + endIndex);
        //    List<Vector3> newVert = new List<Vector3>();
        //    List<int> newTri = new List<int>();

        //    //			int index = 0;
        //    for (int i = 0; i < endIndex; i++)
        //    {
        //        newVert.Add(vertices[triangles[startIndex + i]]);
        //        newTri.Add(i);
        //        //				index++;
        //    }

        //    GameObject newObj = new GameObject();

        //    Mesh newMesh = new Mesh();
        //    newMesh.vertices = newVert.ToArray();
        //    Vector3[] normals = new Vector3[newVert.Count];
        //    newMesh.normals = normals;
        //    Vector2[] uvs = new Vector2[newVert.Count];
        //    newMesh.uv = uvs;
        //    //		triangles.RemoveRange (0, 300000);
        //    //		triangles.RemoveRange (120000, triangles.Count - 120000);
        //    newMesh.triangles = newTri.ToArray();

        //    MeshFilter mf = newObj.AddComponent<MeshFilter>();
        //    mf.mesh = newMesh;
        //    newObj.AddComponent<MeshRenderer>().material = mat;


        //    startIndex += endIndex;

        //    if (end)
        //    {
        //        break;
        //    }
        //}

        //return;

        //Debug.Log(triMin + " " + triMax);

        //Dictionary<Vector2, GeometryData> uvValToVerIndex = new Dictionary<Vector2, GeometryData>();

        //Debug.Log(uv.Count + " " + triangles.Count);

        ////for(int i = 0; i < uv.Count; i++)
        ////{
        ////    if(!uvValToVerIndex.ContainsKey(uv[i]))
        ////    {
        ////        GeometryData gd = new GeometryData();
        ////        gd.vIndex = triangles[i];
        ////        gd.tIndex = i;

        ////        uvValToVerIndex.Add(uv[i], gd);
        ////    }
        ////}

        ////Debug.Log(vertices.Count + " " + uvValToVerIndex.Count);

        ////Dictionary<Vector2, GeometryData>.Enumerator uvVTVIEnum = uvValToVerIndex.GetEnumerator();

        ////Dictionary<int, Vector2> newUV = new Dictionary<int, Vector2>();

        ////while (uvVTVIEnum.MoveNext())
        ////{
        ////    Vector2 curVec = uvVTVIEnum.Current.Key;
        ////    GeometryData gd = uvVTVIEnum.Current.Value;

        ////    if(newUV.ContainsKey(gd.vIndex))
        ////    {
        ////        if(newUV[gd.vIndex] != curVec)
        ////        {
        ////            vertices.Add(vertices[gd.vIndex]);
        ////            triangles[gd.tIndex] = vertices.Count - 1;
        ////            newUV.Add(vertices.Count - 1, curVec);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        newUV.Add(gd.vIndex, curVec);
        ////    }
        ////}

        //Debug.Log(vertices.Count + " " + uvValToVerIndex.Count);

        //List<Vector2> calcUV = new List<Vector2>(vertices.Count);

        ////for(int i = 0; i < calcUV.Count; i++)
        ////{
        ////    calcUV[i] = newUV[i + 1];
        ////}

        //for (int i = 0; i < triangles.Count; i++)
        //{
        //    //triangles[i] -= 1;
        //    //int keepVal = triangles[i + 0];
        //    //triangles[i + 0] = triangles[i + 2];
        //    ////triangles[i + 1] = triangles[i + 2];
        //    //triangles[i + 2] = keepVal;

        //    //Vector2 keepUV = uv[i + 0];
        //    //uv[i + 0] = uv[i + 2];
        //    ////uv[i + 1] = uv[i + 2];
        //    //uv[i + 2] = keepUV;
        //}

        //Mesh mesh = new Mesh();
        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        //mesh.uv = calcUV.ToArray();

        //gameObject.AddComponent<MeshFilter>().mesh = mesh;
        //gameObject.AddComponent<MeshRenderer>();

        ////for(int i = 0; i < triangles.Count; i++)
        ////{
        ////    int curIndex = triangles[i];

        ////    if(vertIndexAtUV.ContainsKey(curIndex))
        ////    {
        ////        Vector2 curVec = uv[i];

        ////        if (vertIndexAtUV[curIndex] != curVec)
        ////        {
        ////            vertices.Add(vertices[curIndex]);
        ////            triangles[i] = vertices.Count - 1;
        ////            vertIndexAtUV.Add(vertices.Count - 1, uv[i]);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        vertIndexAtUV.Add(curIndex, uv[i]);
        ////    }
        ////}

        ////Debug.Log(vertices.Count + " " + vertIndexAtUV.Count);

        ////List<Vector2> newUV = new List<Vector2>();

        ////for(int i = 0; i < vertices.Count; i++)
        ////{
        ////    newUV.Add(vertIndexAtUV[i]);
        ////}

        ////Debug.Log(vCount + " " + vertices.Count);
        ////Debug.Log(vtCount + " " + uv.Count);
        ////Debug.Log(fCount + " " + triangles.Count);
        ////Debug.Log(newUV.Count);
    }

    public void testLoad()
    {
        Mesh tMesh = tarObj.GetComponent<MeshFilter>().mesh;

        Vector3[] tVert = tMesh.vertices;

        HashSet<Vector3> compVertices = new HashSet<Vector3>();

        int sameCount = 0;

        for(int i = 0; i < tVert.Length; i++)
        {
            if(!compVertices.Add(tVert[i]))
            {
                sameCount++;
            }
        }

        Debug.Log(sameCount);

        Debug.Log(sameCount + 99701);

        sameCount = 0;

        Vector2[] tUV = tMesh.uv;

        HashSet<Vector2> compUV = new HashSet<Vector2>();

        for(int i = 0; i < tUV.Length; i++)
        {
            if (!compUV.Add(tUV[i]))
            {
                sameCount++;
            }
        }

        Debug.Log(sameCount);
    }

    string vector3ToString(Vector3 value)
    {
        return value.x + " " + value.y + " " + value.z;
    }

    public void loadObject(string path)
    {

        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(File.ReadAllBytes(path + "model_texture_02.jpg"));
        tex.Apply();

        Material newMat = Instantiate(mat);

        newMat.mainTexture = tex;

        string objData = File.ReadAllText(path + "model_mesh.obj");
        string[] lines = objData.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        Debug.Log(lines.Length);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        Dictionary<int, Vector2> vertIndexAtUV = new Dictionary<int, Vector2>();

        Dictionary<Vector2, int> vertIndexAtUV2 = new Dictionary<Vector2, int>();

        int vCount = 0;
        int vtCount = 0;
        int fCount = 0;

        int triMin = int.MaxValue;
        int triMax = int.MinValue;

        float maxYvalue = float.MinValue;

        for (int i = 0; i < lines.Length; i++)
        {
            string curLine = lines[i];

            string[] lineData = curLine.Split(' ');

            if (lineData[0] == "v")
            {
                float x = 0.0f;

                if(!float.TryParse(lineData[1], out x))
                {
                    Debug.Log("v x parse error");
                    return;
                }

                float y = 0.0f;

                if (!float.TryParse(lineData[2], out y))
                {
                    Debug.Log("v y parse error");
                    return;
                }

                float z = 0.0f;

                if (!float.TryParse(lineData[3], out z))
                {
                    Debug.Log("v z parse error");
                    return;
                }

                Vector3 curVec = new Vector3(-x, y, z);

                if(curVec.y > maxYvalue)
                {
                    maxYvalue = curVec.y;
                }

                vertices.Add(curVec);

                vCount++;
            }
            else if (lineData[0] == "vt")
            {
                float x = 0.0f;

                if (!float.TryParse(lineData[1], out x))
                {
                    Debug.Log("vt x parse error");
                    return;
                }

                float y = 0.0f;

                if (!float.TryParse(lineData[2], out y))
                {
                    Debug.Log("vt y parse error");
                    return;
                }

                Vector2 curUV = new Vector2(x, y);

                uv.Add(curUV);

                vtCount++;
            }
            else if (lineData[0] == "f")
            {
                for (int j = lineData.Length - 1; j >= 1; j--)
                //for (int j = 1; j < lineData.Length; j++)
                {
                    string[] faceData = lineData[j].Split('/');

                    //if(triangles.Count + 1 != int.Parse(faceData[1]))
                    //{
                    //    Debug.Log("error");
                    //    break;
                    //}
                    //else
                    {
                        int curIndex = 0;

                        if (!int.TryParse(faceData[0], out curIndex))
                        {
                            Debug.Log("f parse error");
                            return;
                        }

                        curIndex -= 1;

                        if (triMin > curIndex)
                        {
                            triMin = curIndex;
                        }

                        if (triMax < curIndex)
                        {
                            triMax = curIndex;
                        }

                        triangles.Add(curIndex);
                    }
                }

                fCount++;
            }
        }

        for (int i = 0; i < uv.Count; i += 3)
        {
            Vector2 keepValue = uv[i + 0];
            uv[i + 0] = uv[i + 2];
            uv[i + 2] = keepValue;

        }

        Debug.Log(vCount + " " + fCount + " " + vtCount);

        //Dictionary<Vector2, HashSet<int>> uvVertCount = new Dictionary<Vector2, HashSet<int>>();

        //for(int i = 0; i < triangles.Count; i++)
        //{
        //    Vector2 curUV = uv[i];
        //    int vertIndex = triangles[i];

        //    if(uvVertCount.ContainsKey(curUV))
        //    {
        //        uvVertCount[curUV].Add(vertIndex);
        //    }
        //    else
        //    {
        //        HashSet<int> newHash = new HashSet<int>();
        //        newHash.Add(vertIndex);

        //        uvVertCount.Add(curUV, newHash);
        //    }
        //}

        //Dictionary<int, HashSet<Vector2>> indexToUVCount = new Dictionary<int, HashSet<Vector2>>();

        //for (int i = 0; i < triangles.Count; i++)
        //{
        //    int vertIndex = triangles[i];
        //    Vector2 curUV = uv[i];

        //    if(indexToUVCount.ContainsKey(vertIndex))
        //    {
        //        indexToUVCount[vertIndex].Add(curUV);
        //    }
        //    else
        //    {
        //        HashSet<Vector2> newHash = new HashSet<Vector2>();
        //        newHash.Add(curUV);
        //        indexToUVCount.Add(vertIndex, newHash);
        //    }
        //}

        //int sameVert = 0;

        //HashSet<Vector3> sameVertDetect = new HashSet<Vector3>();

        //for(int i = 0; i < vertices.Count; i++)
        //{
        //    if(!sameVertDetect.Add(vertices[i]))
        //    {
        //        sameVert++;
        //    }
        //}

        //Debug.Log(sameVert);

        //Debug.Log(indexToUVCount.Count);

        //int sameCount = 0;

        //Dictionary<int, HashSet<Vector2>>.Enumerator ituc = indexToUVCount.GetEnumerator();

        //while(ituc.MoveNext())
        //{
        //    HashSet<Vector2> curHash = ituc.Current.Value;

        //    sameCount += curHash.Count;

        //    //if(curHash.Count > 1)
        //    //{
        //    //    sameCount++;
        //    //}
        //}

        //Debug.Log(sameCount);

        //testLoad();

        //return;

        Dictionary<int, Dictionary<Vector2, int>> vertToUV = new Dictionary<int, Dictionary<Vector2, int>>();

        for(int i = 0; i < triangles.Count; i++)
        {
            int vertIndex = triangles[i];
            Vector2 curUV = uv[i];

            if(vertToUV.ContainsKey(vertIndex))
            {
                if(vertToUV[vertIndex].ContainsKey(curUV))
                {
                    triangles[i] = vertToUV[vertIndex][curUV];
                    //if(vertToUV[vertIndex][curUV] != vertIndex)
                    //{
                    //    Debug.Log("error");
                    //    break;
                    //}
                    //else
                    //{
                    //    triangles[i] = vertToUV[vertIndex][curUV];
                    //}
                }
                else
                {
                    vertices.Add(vertices[vertIndex]);
                    int curIndex = vertices.Count - 1;
                    vertToUV[vertIndex].Add(curUV, curIndex);
                    triangles[i] = curIndex;
                }
            }
            else
            {
                Dictionary<Vector2, int> uvToIndex = new Dictionary<Vector2, int>();

                uvToIndex.Add(curUV, vertIndex);

                vertToUV.Add(vertIndex, uvToIndex);
            }
        }

        Debug.Log(vertices.Count);

        Vector2[] calcUV = new Vector2[vertices.Count];

        if(uv.Count != triangles.Count)
        {
            Debug.Log("error");
        }

        //int min = int.MaxValue;
        //int max = int.MinValue;

        for(int i = 0; i < triangles.Count; i++)
        {
            //if(triangles[i] < min)
            //{
            //    min = triangles[i];
            //}
            //if (triangles[i] > max)
            //{
            //    max = triangles[i];
            //}

            if (vertices.Count - 1 < triangles[i])
            {
                Debug.Log(triangles[i]);
                break;
            }
            calcUV[triangles[i]] = uv[i];
        }

        //Debug.Log(calcUV.Length);
        //Debug.Log(min + " " + max);

        //return;

        ////Dictionary<int, Dictionary<Vector2, int>>.Enumerator testEnum = vertToUV.GetEnumerator();

        ////int sumCount = 0;

        ////while(testEnum.MoveNext())
        ////{
        ////    Dictionary<Vector2, int> value = testEnum.Current.Value;
        ////    sumCount += value.Count;
        ////}

        ////Debug.Log(sumCount);

        ////return;

        //Dictionary<Vector2, HashSet<int>> newTargetIndex = new Dictionary<Vector2, HashSet<int>>();

        ////Dictionary<Vector2, int> newTargetIndex = new Dictionary<Vector2, int>();

        //Dictionary<int, Vector2> indexToUV = new Dictionary<int, Vector2>();

        //int diffCount = 0;

        //for(int i = 0; i < triangles.Count; i++)
        //{
        //    int curIndex = triangles[i];
        //    Vector2 curVec = uv[i];

        //    if(newTargetIndex.ContainsKey(curVec))
        //    {
        //        newTargetIndex[curVec].Add(curIndex);
        //        //if (!newTargetIndex[curVec].Contains(curIndex))
        //        //{
        //        //    newTargetIndex[curVec].Add(curIndex);
        //        //}
        //    }
        //    else
        //    {
        //        HashSet<int> newIndexs = new HashSet<int>();
        //        newIndexs.Add(curIndex);
        //        newTargetIndex.Add(curVec, newIndexs);
        //    }
        //}

        //Dictionary<Vector2, HashSet<int>>.Enumerator ntiEnum = newTargetIndex.GetEnumerator();

        //while(ntiEnum.MoveNext())
        //{
        //    HashSet<int> curList = ntiEnum.Current.Value;

        //    diffCount += curList.Count;
        //}

        //Debug.Log(diffCount);

        //return;

        //for(int i = 0; i < triangles.Count; i++)
        //{
        //    int vertIndex = triangles[i];
        //    Vector2 curUV = uv[i];

        //    if(indexToUV.ContainsKey(vertIndex))
        //    {
        //        if(indexToUV[vertIndex] != curUV)
        //        {
        //            if(newTargetIndex.ContainsKey(curUV))
        //            {
        //                triangles[i] = newTargetIndex[curUV];
        //            }
        //            else
        //            {
        //                vertices.Add(vertices[vertIndex]);
        //                int newIndex = vertices.Count - 1;
        //                triangles[i] = newIndex;
        //                newTargetIndex.Add(curUV, newIndex);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        indexToUV.Add(vertIndex, curUV);
        //        //newTargetIndex.Add(curUV, vertIndex);
        //        if(newTargetIndex.ContainsKey(curUV))
        //        {
        //            if (newTargetIndex[curUV] == vertIndex)
        //            {
        //                ;
        //            }
        //            else
        //            {
        //                Debug.Log("error");
        //            }
        //        }
        //        else
        //        {
        //            newTargetIndex.Add(curUV, vertIndex);
        //        }
        //    }
        //}

        //Debug.Log(newTargetIndex.Count + " " + vertices.Count);

        //Dictionary<int, HashSet<Vector2>> vertToUV = new Dictionary<int, HashSet<Vector2>>();

        ////Dictionary<Vector2, int> uvToVertIndex = new Dictionary<Vector2, int>();

        ////Dictionary<int, Vector2> vertToUV = new Dictionary<int, Vector2>();

        //for (int i = 0; i < uv.Count; i++)
        //{
        //    Vector2 curUV = uv[i];

        //    int vertIndex = triangles[i];

        //    if (vertToUV.ContainsKey(vertIndex))
        //    {
        //        vertToUV[vertIndex].Add(curUV);
        //    }
        //    else
        //    {
        //        HashSet<Vector2> uvs = new HashSet<Vector2>();
        //        uvs.Add(curUV);
        //        vertToUV.Add(vertIndex, uvs);
        //    }
        //}

        //Debug.Log(vertToUV.Count + " " + vertices.Count);

        //Dictionary<int, HashSet<Vector2>>.Enumerator vtuvEnum = vertToUV.GetEnumerator();

        //Dictionary<Vector2, int> uvToVertIndex = new Dictionary<Vector2, int>();

        //while (vtuvEnum.MoveNext())
        //{
        //    int vertIndex = vtuvEnum.Current.Key;
        //    HashSet<Vector2> uvs = vtuvEnum.Current.Value;

        //    HashSet<Vector2>.Enumerator uvsEnum = uvs.GetEnumerator();

        //    int count = 0;

        //    while (uvsEnum.MoveNext())
        //    {
        //        if(count > 0)
        //        {
        //            vertices.Add(vertices[vertIndex]);
        //            vertIndex = vertices.Count - 1;
        //        }

        //        if(uvToVertIndex.ContainsKey(uvsEnum.Current))
        //        {
        //            Debug.Log(uvsEnum.Current);
        //            Debug.Log(uvToVertIndex[uvsEnum.Current] + " " + vertIndex);
        //        }
        //        uvToVertIndex.Add(uvsEnum.Current, vertIndex);

        //        count++;
        //    }
        //}

        //Debug.Log(uvToVertIndex.Count + " " + vertices.Count);


        int startIndex = 0;
        int endIndex = 60000;
        bool end = false;

        if(genObj != null)
        {
            foreach(MeshRenderer mr in genObj.GetComponentsInChildren<MeshRenderer>())
            {
                Texture2D desTex = mr.material.mainTexture as Texture2D;
                mr.material.mainTexture = null;
                Destroy(desTex);
                desTex = null;

                Destroy(mr);
            }

            foreach (MeshFilter mf in genObj.GetComponentsInChildren<MeshFilter>())
            {
                Mesh desMesh = mf.mesh;
                mf.mesh = null;
                Destroy(desMesh);
                desMesh = null;

                Destroy(mf);
                Destroy(mf.gameObject);
            }

            Destroy(genObj);
            genObj = null;
        }

        genObj = new GameObject();

        while (true)
        {
            if (startIndex + endIndex > triangles.Count)
            {
                endIndex = triangles.Count - startIndex;
                end = true;
            }
            Debug.Log(startIndex + " " + endIndex);
            List<Vector3> newVert = new List<Vector3>();
            List<int> newTri = new List<int>();
            List<Vector2> partUV = new List<Vector2>();

            //			int index = 0;
            for (int i = 0; i < endIndex; i++)
            {
                newVert.Add(vertices[triangles[startIndex + i]]);
                partUV.Add(calcUV[triangles[startIndex + i]]);
                newTri.Add(i);
                //				index++;
            }

            GameObject newObj = new GameObject();

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVert.ToArray();
            Vector3[] normals = new Vector3[newVert.Count];
            newMesh.normals = normals;
            //Vector2[] uvs = new Vector2[partUV.Count];
            newMesh.uv = partUV.ToArray();// uvs;
            //		triangles.RemoveRange (0, 300000);
            //		triangles.RemoveRange (120000, triangles.Count - 120000);
            newMesh.triangles = newTri.ToArray();

            MeshFilter mf = newObj.AddComponent<MeshFilter>();
            mf.mesh = newMesh;
            newObj.AddComponent<MeshRenderer>().material = newMat;

            newObj.transform.parent = genObj.transform;

            startIndex += endIndex;

            if (end)
            {
                break;
            }
        }

        genObj.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        genObj.transform.localPosition -= Vector3.up * (maxYvalue - 0.2f);

        vertices.Clear();
        vertices = null;

        triangles.Clear();
        triangles = null;

        uv.Clear();
        uv = null;

        calcUV = null;

        vertToUV.Clear();
        vertToUV = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("start");
            StartCoroutine(generateImages());
        }
        //if(Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    pathIndex = pathIndex > paths.Count - 1 ? 0 : pathIndex + 1;

        //    loadObject(paths[pathIndex]);

        //    StartCoroutine(screenShot());
        //}
    }

    IEnumerator generateImages()
    {
        for (; pathIndex < paths.Count; )
        {
            isNext = true;

            //pathIndex = pathIndex > paths.Count - 1 ? 0 : pathIndex + 1;

            Debug.Log(paths[pathIndex]);

            loadObject(paths[pathIndex]);

            StartCoroutine(screenShot());

            while (isNext)
            {
                yield return null;
            }
        }
    }

    IEnumerator screenShot()
    {
        float setTime = 2.0f;

        while (setTime > 0.0f)
        {
            setTime -= Time.deltaTime;
            yield return null;
        }

        string[] fullNames = paths[pathIndex].Split('\\');

        string name = fullNames[fullNames.Length - 3] + "_" + fullNames[fullNames.Length - 2];

        ScreenCapture.CaptureScreenshot(paths[pathIndex] + name + "_front.png");

        setTime = 2.0f;

        while (setTime > 0.0f)
        {
            setTime -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("save : " + paths[pathIndex] + name + "_front.png");

        genObj.transform.localEulerAngles = Vector3.zero;

        setTime = 2.0f;

        ScreenCapture.CaptureScreenshot(paths[pathIndex] + name + "_back.png");

        while (setTime > 0.0f)
        {
            setTime -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("save : " + paths[pathIndex] + name + "_back.png");

        Texture2D keepTex = new Texture2D(1, 1);
        
        keepTex.LoadImage(File.ReadAllBytes(paths[pathIndex] + name + "_back.png"));

        keepTex.Apply();

        int width = keepTex.width;
        int height = keepTex.height;

        Texture2D newTex = new Texture2D(width, height);

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                newTex.SetPixel(width - 1 - x, y, keepTex.GetPixel(x, y));
            }
        }

        newTex.Apply();

        File.WriteAllBytes(paths[pathIndex] + name + "_back.png", newTex.EncodeToPNG());

        setTime = 2.0f;

        while (setTime > 0.0f)
        {
            setTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(keepTex);
        Destroy(newTex);

        Debug.Log("end");

        pathIndex++;

        isNext = false;
    }
}
