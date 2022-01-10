using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Test : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();

        string allData = File.ReadAllText("E:\\ScanDatas\\1\\1\\model_mesh.obj");
        //		string allData = File.ReadAllText ("C:\\Models\\Laurent_sport02.obj");
        //		string allData = File.ReadAllText ("C:\\Models\\model_mesh_2.obj");

        string[] lineData = allData.Split (new char[]{ '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

//		int lineCount = 0;

//		int minIndex = int.MaxValue;
//		int maxIndex = int.MinValue;


		for (int i = 0; i < lineData.Length; i++) 
		{
			string[] arg = lineData [i].Split (' ');

			if (arg [0].Equals ("v")) 
			{
				float x = float.Parse (arg [1]);
				float y = float.Parse (arg [2]);
				float z = float.Parse (arg [3]);
				vertices.Add (new Vector3 (-x, y, z));
			}

			if (arg [0].Equals ("f")) 
			{
//				if (lineData [i].Contains ("99651")) 
//				{
//					Debug.Log (lineData [i]);
//				}
//				lineCount++;

//				if (arg.Length != 4) 
//				{
//					lineCount++;
//				}

				for (int j = arg.Length - 1; j >= 1; j--) 
				{
					string triIndex = arg [j].Split ('/') [0];

					int value = int.Parse (triIndex);

					triangles.Add (value - 1);
//
//					if (value < minIndex) 
//					{
//						minIndex = value;
//					}
//
//					if (value > maxIndex) 
//					{
//						maxIndex = value;
//					}
				}
//				break;
			}
		}

		int startIndex = 0;
		int endIndex = 60000;
		bool end = false;

		while (true) 
		{
			if (startIndex + endIndex > triangles.Count) 
			{
				endIndex = triangles.Count - startIndex;
				end = true;
			}
			Debug.Log (startIndex + " " + endIndex);
			List<Vector3> newVert = new List<Vector3> ();
			List<int> newTri = new List<int> ();

//			int index = 0;
			for (int i = 0; i < endIndex; i++) 
			{
				newVert.Add(vertices [triangles [startIndex + i]]);
				newTri.Add (i);
//				index++;
			}

			GameObject newObj = new GameObject ();

			Mesh newMesh = new Mesh ();
			newMesh.vertices = newVert.ToArray ();
			Vector3[] normals = new Vector3[newVert.Count];
			newMesh.normals = normals;
			Vector2[] uv = new Vector2[newVert.Count];
			newMesh.uv = uv;
			//		triangles.RemoveRange (0, 300000);
			//		triangles.RemoveRange (120000, triangles.Count - 120000);
			newMesh.triangles = newTri.ToArray ();

			MeshFilter mf = newObj.AddComponent<MeshFilter> ();
			mf.mesh = newMesh;
			newObj.AddComponent<MeshRenderer> ();

			startIndex += endIndex;

			if (end) 
			{
				break;
			}
		}

////		Debug.Log (lineCount);
//		//		Debug.Log (minIndex + " " + maxIndex);
//		Debug.Log(vertices.Count);
//		Debug.Log(triangles.Count);
//
//		Mesh newMesh = new Mesh ();
//		newMesh.vertices = vertices.ToArray ();
//		Vector3[] normals = new Vector3[vertices.Count];
//		newMesh.normals = normals;
//		Vector2[] uv = new Vector2[vertices.Count];
//		newMesh.uv = uv;
////		triangles.RemoveRange (0, 300000);
////		triangles.RemoveRange (120000, triangles.Count - 120000);
//		newMesh.triangles = triangles.ToArray ();
//
//		MeshFilter mf = gameObject.AddComponent<MeshFilter> ();
//		mf.mesh = newMesh;
//		gameObject.AddComponent<MeshRenderer> ();

//		Debug.Log (lineCount);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
