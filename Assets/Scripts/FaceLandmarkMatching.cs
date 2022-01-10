using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FaceLandmarkMatching : MonoBehaviour 
{
	public GameObject sphere;
	public int index = 0;

	public GameObject sourceObj;

	private List<int> indexMapping;

	private List<List<int>> pointArea;

	private List<Vector3> vertices;

	private List<Vector3> newVertices;
	private List<Vector3> normals;
	private List<int> triangles;
	private List<Vector2> uv;

	private Material mat;

	private List<Vector3> tps;

	public bool debuging = false;

	private float offset = 2.0f;

	private int otherValue = 0; // 26;

	private bool isReady = false;

	// Use this for initialization
	void Start () 
	{		
		initIndexMapping ();

		initPointArea ();

		Mesh mesh = null;

		SkinnedMeshRenderer smr = sourceObj.GetComponent<SkinnedMeshRenderer> ();

		if (smr == null) 
		{
			mesh = sourceObj.GetComponent<MeshFilter> ().mesh;
			mat = sourceObj.GetComponent<MeshRenderer> ().material;
		}
		else 
		{
			mesh = smr.sharedMesh;
			mat = smr.material;
		}

		vertices = new List<Vector3> ();

		vertices.AddRange (mesh.vertices);

		normals = new List<Vector3> ();
		normals.AddRange (mesh.normals);

		triangles = new List<int> ();
		triangles.AddRange (mesh.triangles);

		uv = new List<Vector2> ();
		uv.AddRange (mesh.uv);

		Matrix4x4 ltw = sourceObj.transform.localToWorldMatrix;

		for (int i = 0; i < vertices.Count; i++) 
		{
			vertices [i] = ltw.MultiplyPoint3x4 (vertices [i]);
		}

//		otherArea ();

//		for (int i = 0; i < vertices.Count - 4; i++) 
//		{
//			dataDrawLine(vertices[i]);
//		}
	}

	public float calcProportionSize(List<Vector3> targetPoints)
	{		
		Vector3 point0 = vertices [indexMapping [0]];
		Vector3 point1 = vertices [indexMapping [16]];

		Vector3 point2 = vertices [indexMapping [8]];
		Vector3 point3 = vertices [indexMapping [21]];
		Vector3 point4 = vertices [indexMapping [22]];

		float oriWidth = Vector3.Distance (point0, point1);
		float oriHeight = Vector3.Distance (point2, ((point3 + point4) / 2.0f));

		float oriArea = oriWidth * oriHeight;

		Debug.Log (oriWidth + " " + oriHeight + " = " + oriArea);

		float targetWidth = Vector3.Distance (targetPoints [0], targetPoints [16]);
		float targetHeight = Vector3.Distance (targetPoints [8], ((targetPoints [21] + targetPoints[22]) / 2.0f));

		float targetArea = targetWidth * targetHeight;

		Debug.Log (targetWidth + " " + targetHeight + " = " + targetArea);

//		float proportionValue = targetArea / oriArea;

		float oriRatio = oriWidth / oriHeight;
		float targetRatio = targetWidth / targetHeight;

		float ratioValue = oriRatio / targetRatio;

		Debug.Log (oriRatio + " " + targetRatio + " " + ratioValue);

		float proportionValue = (targetHeight / oriHeight) * ratioValue;

		Debug.Log (proportionValue);

		return proportionValue;
	}

	public void savePoints(List<Vector3> targetPoints)
	{
		tps = targetPoints;
	}

	public void pointMatching(List<Vector3> targetPoints)
	{
//		Vector3 point0 = targetPoints [0] + (targetPoints [0] - targetPoints [39]) * offset;
//		Vector3 point1 = targetPoints [16] + (targetPoints [16] - targetPoints [42]) * offset;
//		Vector3 point2 = targetPoints [27] + (targetPoints [27] - targetPoints [8]) * offset;
//		Vector3 point3 = targetPoints [8] + (targetPoints [8] - targetPoints [57]) * offset;

		if (debuging) 
		{
			sphere.SetActive (true);

			GameObject parentObj = new GameObject ();
			parentObj.name = "Points";

			for (int i = 0; i < targetPoints.Count; i++) 
			{
				GameObject go = Instantiate (sphere);
				go.transform.position = targetPoints[i];
				go.transform.parent = parentObj.transform;
			}

			sphere.SetActive (false);
		}

		Debug.Log ("point count = " + targetPoints.Count);

		float proportionValue = calcProportionSize (targetPoints);

		newVertices = new List<Vector3> ();

		for (int i = 0; i < vertices.Count - otherValue; i++) 
		{
			newVertices.Add (vertices [i]);
		}

		for (int i = 0; i < targetPoints.Count; i++) 
		{
			newVertices [indexMapping [i]] = targetPoints [i];
		}

//		int vIndex = newVertices.Count;

		if (otherValue > 0) 
		{
			List<Vector3> otherMappingIndex = new List<Vector3> ();

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
			//		otherMappingIndex.Add (new Vector3 (27, 8, 1.3f));

			for (int i = 0; i < otherMappingIndex.Count; i++) 
			{
				int firstIndex = (int)otherMappingIndex [i].x;
				int secondIndex = (int)otherMappingIndex [i].y;
				float thirdIndex = otherMappingIndex [i].z;

				Vector3 point0 = newVertices [indexMapping [firstIndex]] + (newVertices [indexMapping [firstIndex]] - newVertices [indexMapping [secondIndex]]) * thirdIndex;

				int vertIndex = newVertices.Count;
				newVertices.Add (point0);
				//			indexMapping.Add (vertIndex);
			}
			newVertices.Add ((newVertices [newVertices.Count - 2] + newVertices [newVertices.Count - 1]) / 2.0f);
		}

//		List<int> otherIdxs0 = new List<int> ();
//		List<int> otherIdxs1 = new List<int> ();

//		otherIdxs0.Add (vIndex + 0);
//		otherIdxs0.Add (vIndex + 2);
//		otherIdxs0.Add (vIndex + 1);
//
//		otherIdxs1.Add (vIndex + 0);
//		otherIdxs1.Add (vIndex + 1);
//		otherIdxs1.Add (vIndex + 3);

//		debuging = true;

		for (int i = 0; i < vertices.Count - otherValue; i++) 
		{
			Vector3 targetPoint = vertices[i];

			int otherIndex = pointArea.Count;// - 2;
			int selectIndex = -1;

//			Vector3 sPoint0 = Vector3.zero;
//			Vector3 sPoint1 = Vector3.zero;
//			Vector3 sPoint2 = Vector3.zero;

			for (int j = 0; j < pointArea.Count; j++) 
			{
				List<int> paData = pointArea [j];

				int index0 = paData [0];
				int index1 = paData [1];
				int index2 = paData [2];

				if (j < otherIndex) 
				{
					index0 = indexMapping [index0];
					index1 = indexMapping [index1];
					index2 = indexMapping [index2];
				}
//				else 
//				{
//					newVertices [i] = newVertices [i] - Vector3.right * 340.0f;
//					break;
//				}
//				else 
//				{
//					if (selectIndex >= 0) 
//					{
//						continue; 
//					}
//				}

				Vector2 p0 = new Vector2 (vertices [index0].x, vertices [index0].y);
				Vector2 p1 = new Vector2 (vertices [index1].x, vertices [index1].y);
				Vector2 p2 = new Vector2 (vertices [index2].x, vertices [index2].y);
				Vector2 pp = new Vector2 (targetPoint.x, targetPoint.y);

				Vector3 hitpos = Vector3.zero;

				if (CustomRayCast.triangleRayCast (p0, p1, p2, pp, Vector3.forward, ref hitpos)) 
				{
					Vector3 r = Vector3.zero;

					BarycentricCoordinates.barycent (p0, p1, p2, pp, out r);

					Vector2 vp0 = new Vector2(newVertices [index0].x, newVertices [index0].y);
					Vector2 vp1 = new Vector2(newVertices [index1].x, newVertices [index1].y);
					Vector2 vp2 = new Vector2(newVertices [index2].x, newVertices [index2].y);

					Vector3 q = BarycentricCoordinates.calcBarycentricPoint (vp0, vp1, vp2, r);

					newVertices [i] = q;

					break;
				}
			}
		}

		GameObject faceObj = new GameObject ();
		faceObj.name = "newMesh";
		SkinnedMeshRenderer smr = faceObj.AddComponent<SkinnedMeshRenderer> ();
//		MeshRenderer mr = go.AddComponent<MeshRenderer>();
//		mr.material = mat;

//		vertices.RemoveRange (vertices.Count - 4, 4);
//		newVertices.RemoveRange (newVertices.Count - 4, 4);

		List<Vector3> tempVertices = new List<Vector3> ();

		for (int i = 0; i < vertices.Count - otherValue; i++) 
		{
			tempVertices.Add (new Vector3 (newVertices [i].x, newVertices [i].y, vertices [i].z * proportionValue));
		}
//		tempVertices.AddRange (newVertices);
//		tempVertices.RemoveRange(tempVertices.Count - 26, 26);

		Mesh newMesh = new Mesh ();
		newMesh.vertices = tempVertices.ToArray ();
		newMesh.normals = normals.ToArray ();
		newMesh.triangles = triangles.ToArray ();
		newMesh.uv = uv.ToArray ();

		//setBlendShape (newMesh, sourceObj.transform.localToWorldMatrix, proportionValue);

		smr.sharedMesh = newMesh;
		smr.material = mat;

		gameObject.GetComponent<PixelProjection> ().source = faceObj;

		isReady = true;
	}

	Mesh setBlendShape(Mesh targetMesh, Matrix4x4 ltw, float zOffset)
	{
        //Mesh sMesh = sourceObj.GetComponent<SkinnedMeshRenderer> ().sharedMesh;
        Mesh sMesh = sourceObj.GetComponent<MeshFilter>().sharedMesh;
        int count = sMesh.blendShapeCount;
		int vertCount = sMesh.vertexCount;

		Debug.Log ("blendCount = " + count);

		for (int i = 0; i < count; i++) 
		{
			Vector3[] deltaVertices = new Vector3[vertCount];
			Vector3[] deltaNormals = new Vector3[vertCount];
			Vector3[] deltaTangents = new Vector3[vertCount];
//			int frameCount = sMesh.GetBlendShapeFrameCount (i);

			sMesh.GetBlendShapeFrameVertices (i, 0, deltaVertices, deltaNormals, deltaTangents);

			for (int j = 0; j < deltaVertices.Length; j++) 
			{
				Vector3 keepVec = deltaVertices [j];
				keepVec = ltw.MultiplyVector (deltaVertices [j]);
				keepVec *= zOffset;
				deltaVertices [j] = keepVec;
			}

			targetMesh.AddBlendShapeFrame (sMesh.GetBlendShapeName(i), sMesh.GetBlendShapeFrameWeight(i,0), deltaVertices, deltaNormals, deltaTangents);
		}

		return targetMesh;
	}

	void initIndexMapping()
	{
		indexMapping = new List<int> ();

		string indexMappingData = File.ReadAllText ("C:\\Image\\NewIndexMapping.txt");

		string[] imds = indexMappingData.Split (new char[]{ '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < imds.Length; i++) 
		{
			indexMapping.Add (int.Parse (imds [i].Split (' ') [1]));
		}
	}

	void initPointArea()
	{
		pointArea = new List<List<int>> ();

		string pointAreaDatas = File.ReadAllText ("C:\\Image\\PointArea.txt");

		string[] pads = pointAreaDatas.Split(new char[]{ '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < pads.Length; i++) 
		{
			string[] pad = pads [i].Split (' ');

			List<int> idx = new List<int> ();

			for (int j = 0; j < pad.Length; j++) 
			{
				idx.Add (int.Parse (pad [j]));
			}

			pointArea.Add (idx);
		}
	}

	void otherArea()
	{
//		Vector3 point0 = vertices [indexMapping [0]] + (vertices [indexMapping [0]] - vertices [indexMapping [39]]) * offset;
//		Vector3 point1 = vertices [indexMapping [16]] + (vertices [indexMapping [16]] - vertices [indexMapping [42]]) * offset;
//		Vector3 point2 = vertices [indexMapping [27]] + (vertices [indexMapping [27]] - vertices [indexMapping [8]]) * offset;
//		Vector3 point3 = vertices [indexMapping [8]] + (vertices [indexMapping [8]] - vertices [indexMapping [57]]) * offset;
//
//		int vIndex = vertices.Count;
//
//		vertices.Add (point0);
//		vertices.Add (point1);
//		vertices.Add (point2);
//		vertices.Add (point3);
//
//		List<int> otherIdxs0 = new List<int> ();
//		List<int> otherIdxs1 = new List<int> ();
//
//		otherIdxs0.Add (vIndex + 0);
//		otherIdxs0.Add (vIndex + 2);
//		otherIdxs0.Add (vIndex + 1);
//
//		otherIdxs1.Add (vIndex + 0);
//		otherIdxs1.Add (vIndex + 1);
//		otherIdxs1.Add (vIndex + 3);
//
//		pointArea.Add (otherIdxs0);
//		pointArea.Add (otherIdxs1);


		List<Vector3> otherMappingIndex = new List<Vector3> ();

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
//		otherMappingIndex.Add (new Vector3 (27, 8, 1.3f));

		for (int i = 0; i < otherMappingIndex.Count; i++) 
		{
			int firstIndex = (int)otherMappingIndex [i].x;
			int secondIndex = (int)otherMappingIndex [i].y;
			float thirdIndex = otherMappingIndex [i].z;

			Vector3 point0 = vertices [indexMapping [firstIndex]] + (vertices [indexMapping [firstIndex]] - vertices [indexMapping [secondIndex]]) * thirdIndex;

			int vertIndex = vertices.Count;
			vertices.Add (point0);
			indexMapping.Add (vertIndex);
		}

		indexMapping.Add (vertices.Count);
		vertices.Add ((vertices [vertices.Count - 2] + vertices [vertices.Count - 1]) / 2.0f);
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			pointMatching (tps);
			gameObject.GetComponent<PixelProjection> ().processStart ();
		}
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			pointMatching (tps);
		}
		
		if (debuging && isReady) 
		{
			dataDrawLine(vertices[index]);

			int otherIndex = pointArea.Count;// - 2;

			for (int i = 0; i < pointArea.Count; i++) 
			{
				List<int> paData = pointArea [i];

				int index0 = paData [0];
				int index1 = paData [1];
				int index2 = paData [2];

				if (i < otherIndex) 
				{
					index0 = indexMapping [index0];
					index1 = indexMapping [index1];
					index2 = indexMapping [index2];
				}

				Debug.DrawLine (newVertices[index0], newVertices[index1], Color.green);
				Debug.DrawLine (newVertices[index1], newVertices[index2], Color.green);
				Debug.DrawLine (newVertices[index0], newVertices[index2], Color.green);
			}
		}
	}

	void dataDrawLine(Vector3 targetPoint)
	{
		int otherIndex = pointArea.Count;// - 2;
		int selectIndex = -1;

		Vector3 sPoint0 = Vector3.zero;
		Vector3 sPoint1 = Vector3.zero;
		Vector3 sPoint2 = Vector3.zero;

		for (int i = 0; i < pointArea.Count; i++) 
		{
			List<int> paData = pointArea [i];

			int index0 = paData [0];
			int index1 = paData [1];
			int index2 = paData [2];

			if (i < otherIndex) 
			{
				index0 = indexMapping [index0];
				index1 = indexMapping [index1];
				index2 = indexMapping [index2];
			}
//			else 
//			{
//				if (selectIndex >= 0) 
//				{
//					continue; 
//				}
//			}

			Debug.DrawLine (vertices[index0], vertices[index1], Color.green);
			Debug.DrawLine (vertices[index1], vertices[index2], Color.green);
			Debug.DrawLine (vertices[index0], vertices[index2], Color.green);

			if (i >= otherIndex && selectIndex >= 0) 
			{
				continue;
			}

			Vector2 p0 = new Vector2 (vertices [index0].x, vertices [index0].y);
			Vector2 p1 = new Vector2 (vertices [index1].x, vertices [index1].y);
			Vector2 p2 = new Vector2 (vertices [index2].x, vertices [index2].y);
			Vector2 pp = new Vector2 (targetPoint.x, targetPoint.y);

			Vector3 hitpos = Vector3.zero;

			if (CustomRayCast.triangleRayCast (p0, p1, p2, pp, Vector3.forward, ref hitpos)) 
			{
				selectIndex = i;

				sPoint0 = vertices [index0];
				sPoint1 = vertices [index1];
				sPoint2 = vertices [index2];
			}
		}

		if (selectIndex < 0) 
		{
			Debug.Log ("error");
		}
		else 
		{
			Debug.DrawLine (sPoint0 - Vector3.forward * 10.0f, sPoint1 - Vector3.forward * 10.0f, Color.red);
			Debug.DrawLine (sPoint1 - Vector3.forward * 10.0f, sPoint2 - Vector3.forward * 10.0f, Color.red);
			Debug.DrawLine (sPoint0 - Vector3.forward * 10.0f, sPoint2 - Vector3.forward * 10.0f, Color.red);
		}
	}
}
