using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IndexChecking : MonoBehaviour 
{
	public GameObject targetObj;
	public int index = 0;
	public int wireIndex = 0;
	public GameObject point;
	public List<int> indexMapping;
	private List<Vector3> vertices;
	private List<List<int>> triIndexs;
	private int maxIndex = 0;
	private int maxWireIndex = 0;
	private bool isLBtnDown = false;
	private bool isRBtnDown = false;
	private int indexVec = 0;
	private float saveTime = 0.0f;
	private int vertIndex = 0;
	private List<int> otherPoint;

	// Use this for initialization
	void Start () 
	{		
		indexMapping = new List<int> ();

		string indexMappingData = File.ReadAllText ("C:\\Image\\NewIndexMapping.txt");

		string[] imds = indexMappingData.Split (new char[]{ '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < imds.Length; i++) 
		{
			indexMapping.Add (int.Parse (imds [i].Split (' ') [1]));
		}

		triIndexs = new List<List<int>> ();

		string triIndexData = File.ReadAllText ("C:\\Image\\PointArea.txt");

		string[] tids = triIndexData.Split(new char[]{ '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < tids.Length; i++) 
		{
			string[] tid = tids [i].Split (' ');

			List<int> idx = new List<int> ();

			for (int j = 0; j < tid.Length; j++) 
			{
				idx.Add (int.Parse (tid [j]));
			}

			triIndexs.Add (idx);
		}

		maxWireIndex = triIndexs.Count;

		vertIndex = indexMapping.Count;
		
		Mesh mesh = targetObj.GetComponent<MeshFilter>().mesh;

		vertices = new List<Vector3> ();

		vertices.AddRange (mesh.vertices);

		maxIndex = vertices.Count;

		Matrix4x4 ltw = targetObj.transform.localToWorldMatrix;

		for (int i = 0; i < maxIndex; i++) 
		{
			vertices [i] = ltw.MultiplyPoint3x4 (vertices [i]);
		}

        for (; index < maxIndex; index++)
        {
            dataDrawLine(vertices[index]);
        }
        //		otherArea ();
    }

	void otherArea()
	{
		otherPoint = new List<int> ();

		float xMin = float.MaxValue;
		float xMax = float.MinValue;
		float yMin = float.MaxValue;
		float yMax = float.MinValue;

//		for (int i = 0; i < indexMapping.Count; i++) 
//		{
//			Vector3 mapPoint = vertices [indexMapping [i]];
//
//			if (mapPoint.x < xMin) 
//			{
//				xMin = mapPoint.x;
//			}
//			if (mapPoint.x > xMax) 
//			{
//				xMax = mapPoint.x;
//			}
//			if (mapPoint.y < yMin) 
//			{
//				yMin = mapPoint.y;
//			}
//			if (mapPoint.y > yMax) 
//			{
//				yMax = mapPoint.y;
//			}
//		}

//		float width = xMax - xMin;
//		float height = yMax - yMin;
//
//		xMin -= width / 4.0f;
//		xMax += width / 4.0f;
//
//		yMin -= height / 4.0f;
//		yMax += height;

//		otherPoint.Add (new Vector3 (xMin, yMin, 0.0f));
//		otherPoint.Add (new Vector3 (xMin, yMax, 0.0f));
//		otherPoint.Add (new Vector3 (xMax, yMax, 0.0f));
//		otherPoint.Add (new Vector3 (xMax, yMin, 0.0f));

//		Vector3 point0 = vertices [indexMapping [0]] + (vertices [indexMapping [0]] - vertices [indexMapping [39]]) * 1.5f;
//		Vector3 point1 = vertices [indexMapping [16]] + (vertices [indexMapping [16]] - vertices [indexMapping [42]]) * 1.5f;
//		Vector3 point2 = vertices [indexMapping [27]] + (vertices [indexMapping [27]] - vertices [indexMapping [8]]) * 1.5f;
//		Vector3 point3 = vertices [indexMapping [8]] + (vertices [indexMapping [8]] - vertices [indexMapping [57]]) * 1.5f;

		List<Vector3> otherMappingIndex = new List<Vector3> ();

//		Dictionary<int, int> otherMappingIndex = new Dictionary<int, int> ();

		otherMappingIndex.Add (new Vector3 (8, 57, 1));
		otherMappingIndex.Add (new Vector3 (7, 58, 1));
		otherMappingIndex.Add (new Vector3 (9, 56, 1));
		otherMappingIndex.Add (new Vector3 (6, 48, 1));
		otherMappingIndex.Add (new Vector3 (10, 54, 1));
		otherMappingIndex.Add (new Vector3 (5, 48, 1));
		otherMappingIndex.Add (new Vector3 (11, 54, 1));
		otherMappingIndex.Add (new Vector3 (4, 48, 1));
		otherMappingIndex.Add (new Vector3 (12, 54, 1));
		otherMappingIndex.Add (new Vector3 (1, 36, 1));
		otherMappingIndex.Add (new Vector3 (15, 45, 1));
		otherMappingIndex.Add (new Vector3 (0, 36, 1));
		otherMappingIndex.Add (new Vector3 (16, 45, 1));
		otherMappingIndex.Add (new Vector3 (17, 37, 1.5f));
		otherMappingIndex.Add (new Vector3 (26, 44, 1.5f));
		otherMappingIndex.Add (new Vector3 (17, 36, 2.5f));
		otherMappingIndex.Add (new Vector3 (26, 45, 2.5f));
		otherMappingIndex.Add (new Vector3 (18, 37, 3));
		otherMappingIndex.Add (new Vector3 (25, 44, 3));
		otherMappingIndex.Add (new Vector3 (18, 36, 3.5f));
		otherMappingIndex.Add (new Vector3 (25, 45, 3.5f));
		otherMappingIndex.Add (new Vector3 (19, 37, 5));
		otherMappingIndex.Add (new Vector3 (24, 44, 5));
		otherMappingIndex.Add (new Vector3 (20, 38, 5.5f));
		otherMappingIndex.Add (new Vector3 (23, 43, 5.5f));
		otherMappingIndex.Add (new Vector3 (27, 8, 1.3f));

		for (int i = 0; i < otherMappingIndex.Count; i++) 
		{
			int firstIndex = (int)otherMappingIndex [i].x;
			int secondIndex = (int)otherMappingIndex [i].y;
			float thirdIndex = otherMappingIndex [i].z;

			Vector3 point0 = vertices [indexMapping [firstIndex]] + (vertices [indexMapping [firstIndex]] - vertices [indexMapping [secondIndex]]) * thirdIndex;

			GameObject go0 = Instantiate (point);
			go0.transform.position = point0;

			int vertIndex = vertices.Count;
			vertices.Add (point0);
			indexMapping.Add (vertIndex);
		}

//		otherPoint.Add(

//		Dictionary<int, int>.Enumerator omiEnum = otherMappingIndex.GetEnumerator ();
//
//		while (omiEnum.MoveNext ()) 
//		{
//			int firstIndex = omiEnum.Current.Key;
//			int secondIndex = omiEnum.Current.Value;
//			
//			Vector3 point0 = vertices [indexMapping [firstIndex]] + (vertices [indexMapping [firstIndex]] - vertices [indexMapping [secondIndex]]);
//
//			GameObject go0 = Instantiate (point);
//			go0.transform.position = point0;
//		}

//		otherPoint.Add (point0);
//		otherPoint.Add (point2);
//		otherPoint.Add (point1);
//		otherPoint.Add (point0);
//		otherPoint.Add (point1);
//		otherPoint.Add (point3);
	}

	void genPoints()
	{
		for (int i = 0; i < indexMapping.Count; i++) 
		{
			GameObject go = Instantiate (point);
			go.transform.localScale = Vector3.one * 0.001f;

			go.transform.position = vertices [indexMapping [i]];
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
			isLBtnDown = true;

			indexVec -= 1;

			index--;
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) 
		{
			isLBtnDown = false;

			indexVec += 1;

			saveTime = 0.0f;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			isRBtnDown = true;

			indexVec += 1;

			index++;
		}
		if (Input.GetKeyUp (KeyCode.RightArrow)) 
		{
			isRBtnDown = false;

			indexVec -= 1;

			saveTime = 0.0f;
		}

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			Debug.Log (vertIndex + " -> " + index);
			indexMapping.Add (index);
			vertIndex++;
		}

		if (Input.GetKeyDown (KeyCode.S)) 
		{
			Debug.Log ("gen points");
			genPoints ();
			point.SetActive (false);
		}

		if (Input.GetKeyDown (KeyCode.E)) 
		{
			string exportData = "";

			for (int i = 0; i < indexMapping.Count; i++) 
			{
				exportData += i;
				exportData += " ";
				exportData += indexMapping [i];
				exportData += "\r\n";
			}

			File.WriteAllText ("C:\\Image\\NewIndexMapping.txt", exportData);

			Debug.Log ("export");
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			wireIndex++;
		}

		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			wireIndex--;
		}

		if (isLBtnDown || isRBtnDown) 
		{
			saveTime += Time.deltaTime;

			if (saveTime > 0.5f) 
			{
				index += indexVec;
			}
		}

		if (index >= maxIndex) 
		{
			index = 0;
		}
		if (index < 0) 
		{
			index = maxIndex - 1;
		}

		point.transform.position = vertices [index];


        dataDrawLine(vertices[index]);
    }

	void dataDrawLine(Vector3 targetPoint)
	{
		wireIndex = -1;

		for (int i = 0; i < triIndexs.Count; i++) 
		{
			List<int> triData = triIndexs [i];

			int index0 = indexMapping[triData [0]];
			int index1 = indexMapping[triData [1]];
			int index2 = indexMapping[triData [2]];

			Vector2 p0 = new Vector2 (vertices [index0].x, vertices [index0].y);
			Vector2 p1 = new Vector2 (vertices [index1].x, vertices [index1].y);
			Vector2 p2 = new Vector2 (vertices [index2].x, vertices [index2].y);
			Vector2 pp = new Vector2 (targetPoint.x, targetPoint.y);

			Vector3 hitpos = Vector3.zero;

			if (CustomRayCast.triangleRayCast (p0, p1, p2, pp, Vector3.forward, ref hitpos)) 
			{
				wireIndex = i;
			}
//			if (isPointInTriangle (p0, p1, p2, pp)) 
//			{
//				wireIndex = i;
//			}

			Debug.DrawLine (vertices[index0], vertices[index1], Color.green);
			Debug.DrawLine (vertices[index1], vertices[index2], Color.green);
			Debug.DrawLine (vertices[index0], vertices[index2], Color.green);
		}

//		for (int i = 0; i < otherPoint.Count; i += 3) 
//		{
//			int point0 = indexMapping [otherPoint [i + 0]];
//			int point1 = indexMapping [otherPoint [i + 1]];
//			int point2 = indexMapping [otherPoint [i + 2]];
//
//			Debug.DrawLine (vertices[point0], vertices [point1], Color.yellow);
//			Debug.DrawLine (vertices[point1], vertices [point2], Color.yellow);
//			Debug.DrawLine (vertices[point0], vertices [point2], Color.yellow);
//		}

//		Debug.DrawLine (otherPoint [0], otherPoint [1], Color.yellow);
//		Debug.DrawLine (otherPoint [1], otherPoint [2], Color.yellow);
//		Debug.DrawLine (otherPoint [2], otherPoint [0], Color.yellow);
//		Debug.DrawLine (otherPoint [3], otherPoint [4], Color.yellow);
//		Debug.DrawLine (otherPoint [4], otherPoint [5], Color.yellow);
//		Debug.DrawLine (otherPoint [5], otherPoint [3], Color.yellow);

		if (wireIndex > 0) 
		{

			//		if (wireIndex >= maxWireIndex) 
			//		{
			//			wireIndex = 0;
			//		}
			//		if (wireIndex < 0) 
			//		{
			//			wireIndex = maxWireIndex - 1;
			//		}

			int index00 = indexMapping [triIndexs [wireIndex] [0]];
			int index11 = indexMapping [triIndexs [wireIndex] [1]];
			int index22 = indexMapping [triIndexs [wireIndex] [2]];

            float size = 1.0f;

			Debug.DrawLine (vertices [index00] - Vector3.forward * size, vertices [index11] - Vector3.forward * size, Color.red);
			Debug.DrawLine (vertices [index11] - Vector3.forward * size, vertices [index22] - Vector3.forward * size, Color.red);
			Debug.DrawLine (vertices [index00] - Vector3.forward * size, vertices [index22] - Vector3.forward * size, Color.red);
		}
		else 
		{
//			for (int i = 0; i < 6; i += 3) 
//			{
//				Vector2 p0 = new Vector2 (otherPoint [i + 0].x, otherPoint [i + 0].y);
//				Vector2 p1 = new Vector2 (otherPoint [i + 1].x, otherPoint [i + 1].y);
//				Vector2 p2 = new Vector2 (otherPoint [i + 2].x, otherPoint [i + 2].y);
//				Vector2 pp = new Vector2 (targetPoint.x, targetPoint.y);
//
//				Vector3 hitpos = Vector3.zero;
//
//				if (CustomRayCast.triangleRayCast (p0, p1, p2, pp, Vector3.forward, ref hitpos)) 
//				{
//					Debug.DrawLine (otherPoint [i + 0] - Vector3.forward * 10.0f, otherPoint [i + 1] - Vector3.forward * 10.0f, Color.red);
//					Debug.DrawLine (otherPoint [i + 1] - Vector3.forward * 10.0f, otherPoint [i + 2] - Vector3.forward * 10.0f, Color.red);
//					Debug.DrawLine (otherPoint [i + 2] - Vector3.forward * 10.0f, otherPoint [i + 0] - Vector3.forward * 10.0f, Color.red);
//					wireIndex = i;
//				}
//			}
		}

		if (wireIndex < 0) 
		{
			Debug.Log ("error " + index);
		}
	}

	void OnGUI()
	{
		GUIStyle guiStyle = new GUIStyle ();

		guiStyle.fontSize = 100;
		guiStyle.normal.textColor = Color.white;

		GUI.TextArea (new Rect (0, 0, 200, 200), indexMapping.Count.ToString() + " -> " + index.ToString (), guiStyle);
	}

//	bool isPointInTriangle(Vector2 uvp0, Vector2 uvp1, Vector2 uvp2, Vector2 uvtp)
//	{
//		Vector3 vec0 = uvp2 - uvp0;
//		Vector3 vec1 = uvp1 - uvp0;
//		Vector3 vec2 = uvtp - uvp0;
//
//		float dot00 = Vector3.Dot (vec0, vec0);
//		float dot01 = Vector3.Dot (vec0, vec1);
//		float dot02 = Vector3.Dot (vec0, vec2);
//		float dot11 = Vector3.Dot (vec1, vec1);
//		float dot12 = Vector3.Dot (vec1, vec2);
//
//		float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
//		float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
//		float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
//
//		return (u >= 0.0f) && (v >= 0.0f) && (u + v < 1.0f);
//	}
}
