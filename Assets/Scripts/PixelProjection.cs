using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MeshTexData : IDisposable
{
	public MeshTexData(GameObject go)
	{
		vertices = new List<Vector3> ();
		normals = new List<Vector3> ();
		triangles = new List<int> ();
		uv = new List<Vector2> ();
		
		ltwMatrix = go.transform.localToWorldMatrix;

		Mesh mesh = null;
		Texture2D tex = null;

		SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer> ();

		if (smr == null) 
		{
			MeshFilter mf = go.GetComponent<MeshFilter> ();
			mesh = mf.mesh;

			MeshRenderer mr = go.GetComponent<MeshRenderer> (); 
			tex = mr.material.mainTexture as Texture2D;
		}
		else 
		{
			mesh = smr.sharedMesh;
			tex = smr.material.mainTexture as Texture2D;
		}

		vertices.AddRange (mesh.vertices);
		triangles.AddRange (mesh.triangles);
		uv.AddRange (mesh.uv);

		width = tex.width;
		height = tex.height;

		texColors = tex.GetPixels ();
	}

	public void Dispose()
	{
		vertices.Clear ();
		vertices = null;

		normals.Clear ();
		normals = null;

		triangles.Clear ();
		triangles = null;

		uv.Clear ();
		uv = null;

		texColors = null;
	}
	
	public List<Vector3> vertices = null;
	public List<Vector3> normals = null;
	public List<int> triangles = null;
	public List<Vector2> uv = null;
	public Matrix4x4 ltwMatrix;
	public int width = 0;
	public int height = 0;
//	public Texture2D tex = null;
	public Color[] texColors = null;
}

public class ProjectionData
{
	public ProjectionData(GameObject src, GameObject tar, int size = 0)
	{
		source = new MeshTexData(src);
		target = new MeshTexData(tar);

		if (size > 0) 
		{
			source.width = size;
			source.height = size;
		}

		genTexColor = new Color[source.width * source.height];

        //Color keepColor = Color.black;
        //keepColor.a = 0.0f;

        //for (int i = 0; i < genTexColor.Length; i++)
        //{
        //    genTexColor[i] = keepColor;

        //}

        for (int i = 0; i < genTexColor.Length; i++)
        {
            genTexColor[i] = source.texColors[i];
        }

        overlapTex = new Color[source.width * source.height];

        for(int i = 0; i < overlapTex.Length; i++)
        {
            overlapTex[i] = Color.black;
        }

    }

	public MeshTexData source = null;
	public MeshTexData target = null;
	public Color[] genTexColor = null;
    public Color[] overlapTex = null;
}

public class PixelProjection : MonoBehaviour 
{
	private bool isStart = false;
	public GameObject source;
	public GameObject target;
//	public GameObject cube;
	public GameObject newTexturePlane;
//	public GameObject pointCube;
//	public GameObject pointTri;
//	private GameObject parentObj;
////	public int index = 0;
////	public int index2 = 0;
//
//	List<Vector3> vertices = new List<Vector3>();
//	List<int> triangles = new List<int>();
//	List<Vector2> uv = new List<Vector2>();
//	Matrix4x4 matrix;
//
//	List<Vector3> vertices2 = new List<Vector3>();
//	List<int> triangles2 = new List<int>();
//	List<Vector2> uv2 = new List<Vector2>();
//	Matrix4x4 matrix2;
//
//	Texture2D tex;
//	Texture2D tex2;
//	Texture2D tex3;
//
//	int width = 0;
//	int height = 0;
//
//	int width2 = 0;
//	int height2 = 0;
//
//	int maxIndex = 0;

	// Use this for initialization
	void Start () 
	{
//		matrix = source.transform.localToWorldMatrix;
//
//		MeshFilter mf = source.GetComponent<MeshFilter> ();
//		Mesh mesh = mf.mesh;
//
//		vertices.AddRange (mesh.vertices);
//		triangles.AddRange (mesh.triangles);
//		uv.AddRange (mesh.uv);
//
//		maxIndex = triangles.Count / 3;
//
//		MeshRenderer mr = source.GetComponent<MeshRenderer> ();
//		tex = mr.material.mainTexture as Texture2D;
//
//		width = tex.width;
//		height = tex.height;
//
//		matrix2 = target.transform.localToWorldMatrix;
//
//		MeshFilter mf2 = target.GetComponent<MeshFilter> ();
//		Mesh mesh2 = mf2.mesh;
//
//		vertices2.AddRange (mesh2.vertices);
//		triangles2.AddRange (mesh2.triangles);
//		uv2.AddRange (mesh2.uv);
//
//		MeshRenderer mr2 = target.GetComponent<MeshRenderer> ();
//		tex3 = mr2.material.mainTexture as Texture2D;
//
//		width2 = tex3.width;
//		height2 = tex3.height;
//
//		for (int i = 0; i < vertices2.Count; i++) 
//		{
//			vertices2 [i] = matrix2.MultiplyPoint3x4 (vertices2 [i]);
//		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.S)) 
		{
			if (!isStart) 
			{
				Debug.Log ("start");
				isStart = true;
				StartCoroutine (generateProjection ());
			}
		}
	}

	public void processStart()
	{
		if (!isStart) 
		{
			Debug.Log ("start");
			isStart = true;
			StartCoroutine (generateProjection ());
		}
	}

	IEnumerator generateProjection()
	{
		Debug.Log("a");
		ParameterizedThreadStart pts = new ParameterizedThreadStart(calculatePixel);
		Thread t = new Thread (pts);
		ProjectionData pd = new ProjectionData(source, target, 1024);
		t.Start (pd);
		Debug.Log("b");

		while (true) 
		{
			yield return null;

			if (!t.IsAlive) 
			{
				break;
			}
		}

        //int[] vertIndexs = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 26, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 82, 83, 90, 91 };

        //for(int i = 0; i < vertIndexs.Length; i++)
        //{
        //    Vector2 uvPoint = pd.source.uv[vertIndexs[i]];

        //    int pixelX = (int)(uvPoint.x * 1024.0f);
        //    int pixelY = (int)(uvPoint.y * 1024.0f);

        //    for (int y = -1; y < 2; y++)
        //    {
        //        for (int x = -1; x < 2; x++)
        //        {
        //            int colorIndex = (pixelX + x) + ((pixelY + y) * 1024);
        //            pd.genTexColor[colorIndex] = Color.red;
        //        }
        //    }
        //}

        Texture2D newTex = new Texture2D (pd.source.width, pd.source.height);
		newTex.SetPixels (pd.genTexColor);
		newTex.Apply ();

        //System.IO.FileStream fs = new System.IO.FileStream("ProjectionData.bin", System.IO.FileMode.OpenOrCreate);
        //System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

        //for (int i = 0; i < pd.overlapTex.Length; i++)
        //{
        //    float overlapValue = 0.0f;

        //    if(pd.overlapTex[i] == Color.white)
        //    {
        //        overlapValue = 1.0f;
        //    }
        //    bw.Write(overlapValue);
        //}

        //bw.Close();
        //fs.Close();
        
        Texture2D overlapTex = new Texture2D(pd.source.width, pd.source.height);
        overlapTex.SetPixels(pd.overlapTex);
        overlapTex.Apply();

        newTexturePlane.GetComponent<MeshRenderer> ().material.mainTexture = overlapTex;

        System.IO.File.WriteAllBytes("OverlapTex.jpg", overlapTex.EncodeToJPG());

		SkinnedMeshRenderer smr = source.GetComponent<SkinnedMeshRenderer> ();

		if (smr == null) 
		{
			source.GetComponent<MeshRenderer> ().material.mainTexture = newTex;
		} 
		else 
		{
			smr.material.mainTexture = newTex;
		}

		isStart = false;
		Debug.Log ("end");

        System.IO.File.WriteAllBytes("DPG.jpg", newTex.EncodeToJPG());
        
        //source.AddComponent<AutoBlendshape>();
    }

	void calculatePixel(object arg)
	{
		ProjectionData pd = (ProjectionData)arg;

		MeshTexData src = pd.source;
		MeshTexData tar = pd.target;

		Color[] genTex = pd.genTexColor;
        Color[] overlapTex = pd.overlapTex;

		int genTexMaxValue = genTex.Length;

		for (int i = 0; i < tar.vertices.Count; i++) 
		{
			tar.vertices[i] = tar.ltwMatrix.MultiplyPoint3x4(tar.vertices[i]);
		}

		int maxIndex = src.triangles.Count;

		List<Vector3> triVertices = new List<Vector3> ();
		List<Vector2> triUV = new List<Vector2> ();

		int tarTriCount = tar.triangles.Count;

//		float size = tar.width;

		for (int i = 0; i < tarTriCount; i++) 
		{
			int tarTriIndex = tar.triangles [i];
			triVertices.Add(tar.vertices [tarTriIndex]);
//			triUV.Add (tar.uv [tarTriIndex] * size);
			triUV.Add (new Vector2(tar.uv [tarTriIndex].x * tar.width, tar.uv [tarTriIndex].y * tar.height));
		}

        //for (int index = 0; index < maxIndex; index += 3)
        for (int index = 0; index < 154 * 3; index += 3) 
		{		
			int v0 = src.triangles [index + 0];
			int v1 = src.triangles [index + 1];
			int v2 = src.triangles [index + 2];

			Vector3 vp0 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v0]);
			Vector3 vp1 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v1]);
			Vector3 vp2 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v2]);

			List<float> xSort = new List<float> ();
			List<float> ySort = new List<float> ();

			xSort.Add (src.uv [v0].x);
			xSort.Add (src.uv [v1].x);
			xSort.Add (src.uv [v2].x);

			ySort.Add (src.uv [v0].y);
			ySort.Add (src.uv [v1].y);
			ySort.Add (src.uv [v2].y);

			xSort.Sort ();
			ySort.Sort ();

			Dictionary<float, float> overXValue = new Dictionary<float, float> ();
			Dictionary<float, float> overYValue = new Dictionary<float, float> ();

			float beforeMinX = xSort [0] * src.width;
			float beforeMaxX = xSort [2] * src.width;

			float beforeMinY = ySort [0] * src.height;
			float beforeMaxY = ySort [2] * src.height;

			int minX = floatToDownInt(beforeMinX);
			int maxX = floatToUpInt(beforeMaxX);

			int minY = floatToDownInt(beforeMinY);
			int maxY = floatToUpInt(beforeMaxY);

			overXValue.Add (beforeMinX, minX - 1.0f);
			if (!overXValue.ContainsKey (xSort [1] * src.width)) 
			{
				overXValue.Add (xSort [1] * src.width, xSort [1] * src.width);
			}
			if (!overXValue.ContainsKey (beforeMaxX)) 
			{
				overXValue.Add (beforeMaxX, maxX);
			}

			overYValue.Add (beforeMinY, minY - 1.0f);
			if (!overYValue.ContainsKey (ySort [1] * src.height)) 
			{
				overYValue.Add (ySort [1] * src.height, ySort [1] * src.height);
			}
			if (!overYValue.ContainsKey (beforeMaxY)) 
			{
				overYValue.Add (beforeMaxY, maxY);
			}

			Vector2 a = new Vector2 (overXValue [src.uv [v0].x * (float)src.width], overYValue [src.uv [v0].y * (float)src.height]);
			Vector2 b = new Vector2 (overXValue [src.uv [v1].x * (float)src.width], overYValue [src.uv [v1].y * (float)src.height]);
			Vector2 c = new Vector2 (overXValue [src.uv [v2].x * (float)src.width], overYValue [src.uv [v2].y * (float)src.height]);

			List<int> backupIndex = new List<int> ();

			for (int y = minY; y < maxY; y++) 
			{
				for (int x = minX; x < maxX; x++) 
				{
					Vector2 p = new Vector2 ((float)(x), (float)y);

					if (isPointInTriangle (a, b, c, p)) 
					{
						float r1 = 0.0f;
						float r2 = 0.0f;
						float r3 = 0.0f;

						Vector3 r = Vector3.zero;

						BarycentricCoordinates.barycent (a, b, c, p, out r);

						Vector3 q = BarycentricCoordinates.calcBarycentricPoint (vp0, vp1, vp2, r);

						bool isHit = false;

						Vector3 hitPos = Vector3.zero;

						if (backupIndex.Count > 0) 
						{
							for (int i = 0; i < backupIndex.Count; i++) 
							{
								int bi = backupIndex [i];

								Vector3 tv0 = triVertices [bi + 0];
								Vector3 tv1 = triVertices [bi + 1];
								Vector3 tv2 = triVertices [bi + 2];

								if(CustomRayCast.triangleRayCast(tv0, tv1, tv2, q, Vector3.forward, ref hitPos))
								{
									Vector3 newR = Vector3.zero;

									BarycentricCoordinates.barycent (tv0, tv1, tv2, hitPos, out newR);

									if (float.IsNaN (newR.x))
									{
										newR.x = 0.0f;
									}
									if (float.IsNaN (newR.y))
									{
										newR.y = 0.0f;
									}
									if (float.IsNaN (newR.z))
									{
										newR.z = 0.0f;
									}

									q = BarycentricCoordinates.calcBarycentricPoint (triUV [bi + 0], triUV [bi + 1], triUV [bi + 2], newR);

									genTex[y * src.width + x] = tar.texColors[(int)q.y * tar.width + (int)q.x];
                                    overlapTex[y * src.width + x] = Color.white;

//									Color getColorData = tar.texColors[(int)q.y * tar.width + (int)q.x];
//
//									for (int oy = -5; oy < 6; oy++) 
//									{
//										for (int ox = -5; ox < 6; ox++) 
//										{
//											int areaIndexs = (y + oy) * src.width + (x + ox);
//
//											areaIndexs = Mathf.Max (0, areaIndexs);
//											areaIndexs = Mathf.Min (areaIndexs, genTexMaxValue);
//
//											if (genTex [areaIndexs].a == 0.0f) 
//											{
//												genTex [areaIndexs] = getColorData;
//											}
//										}
//									}
//
//									genTex[y * src.width + x] = getColorData;

									isHit = true;

									break;
								}
							}
						}

						if (!isHit) 
						{
							for (int i = 0; i < tarTriCount; i += 3) 
							{
								Vector3 tv0 = triVertices [i + 0];
								Vector3 tv1 = triVertices [i + 1];
								Vector3 tv2 = triVertices [i + 2];

								if(CustomRayCast.triangleRayCast(tv0, tv1, tv2, q, Vector3.forward, ref hitPos))
								{
									Vector3 newR = Vector3.zero;

									BarycentricCoordinates.barycent (tv0, tv1, tv2, hitPos, out newR);

									if (float.IsNaN (newR.x))
									{
										newR.x = 0.0f;
									}
									if (float.IsNaN (newR.y))
									{
										newR.y = 0.0f;
									}
									if (float.IsNaN (newR.z))
									{
										newR.z = 0.0f;
									}

									q = BarycentricCoordinates.calcBarycentricPoint (triUV [i + 0], triUV [i + 1], triUV [i + 2], newR);

									genTex[y * src.width + x] = tar.texColors[(int)q.y * tar.width + (int)q.x];
                                    overlapTex[y * src.width + x] = Color.white;

//									Color getColorData = tar.texColors[(int)q.y * tar.width + (int)q.x];
//
//									for (int oy = -5; oy < 6; oy++) 
//									{
//										for (int ox = -5; ox < 6; ox++) 
//										{
//											int areaIndexs = (y + oy) * src.width + (x + ox);
//
//											areaIndexs = Mathf.Max (0, areaIndexs);
//											areaIndexs = Mathf.Min (areaIndexs, genTexMaxValue);
//
//											if (genTex [areaIndexs].a == 0.0f) 
//											{
//												genTex [areaIndexs] = getColorData;
//											}
//										}
//									}
//
//									genTex[y * src.width + x] = getColorData;

                                    backupIndex.Add(i);

									break;
								}
							}
						}
					}
				} 
			}

			backupIndex.Clear ();
			backupIndex = null;

			xSort.Clear ();
			xSort = null;

			ySort.Clear ();
			ySort = null;

			overXValue.Clear ();
			overXValue = null;

			overYValue.Clear ();
			overYValue = null;
		}


		triVertices.Clear ();
		triVertices = null;

		triUV.Clear ();
		triUV = null;

		Debug.Log("c");

//		for (int y = 2; y < src.height - 2; y++) 
//		{
//			for (int x = 2; x < src.width - 2; x++) 
//			{
//				List<int> arrayIndexs = new List<int> ();
//
//				bool isAllEmpty = true;
//				float r = 0.0f;
//				float g = 0.0f;
//				float b = 0.0f;
//				int notEmptyCount = 0;
//
//				for (int oy = -2; oy < 3; oy++) 
//				{
//					for (int ox = -2; ox < 3; ox++) 
//					{
//						int tIndex = (y + oy) * src.width + (x + ox);
//
//						Color keepColor = genTex [tIndex];
//
//						if (keepColor.a == 0.0f) 
//						{
//							arrayIndexs.Add (tIndex);
//						}
//						else 
//						{
//							isAllEmpty = false;
//							notEmptyCount++;
//							r += keepColor.r;
//							g += keepColor.g;
//							b += keepColor.b;
//						}
//					}
//				}
//
//				if (!isAllEmpty) 
//				{
//					Color keepColor = new Color (r / notEmptyCount, g / notEmptyCount, b / notEmptyCount, 1.0f);
//
//					for (int i = 0; i < arrayIndexs.Count; i++) 
//					{
//						genTex [arrayIndexs [i]] = keepColor;
//					}
//				}
//			}
//		}
	}

//	void calculatePixel(object arg)
//	{
//		ProjectionData pd = (ProjectionData)arg;
//
//		MeshTexData src = pd.source;
//		MeshTexData tar = pd.target;
//
//		Color[] genTex = pd.genTexColor;
//
//		for (int i = 0; i < tar.vertices.Count; i++) 
//		{
//			tar.vertices[i] = tar.ltwMatrix.MultiplyPoint3x4(tar.vertices[i]);
//		}
//
//		int maxIndex = src.triangles.Count;
//
//		for (int index = 0; index < maxIndex; index += 3) 
//		{		
//			int v0 = src.triangles [index + 0];
//			int v1 = src.triangles [index + 1];
//			int v2 = src.triangles [index + 2];
//
//			Vector3 vp0 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v0]);
//			Vector3 vp1 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v1]);
//			Vector3 vp2 = src.ltwMatrix.MultiplyPoint3x4 (src.vertices [v2]);
//
//			List<float> xSort = new List<float> ();
//			List<float> ySort = new List<float> ();
//
//			xSort.Add (src.uv [v0].x);
//			xSort.Add (src.uv [v1].x);
//			xSort.Add (src.uv [v2].x);
//
//			ySort.Add (src.uv [v0].y);
//			ySort.Add (src.uv [v1].y);
//			ySort.Add (src.uv [v2].y);
//
//			xSort.Sort ();
//			ySort.Sort ();
//
//			Dictionary<float, float> overXValue = new Dictionary<float, float> ();
//			Dictionary<float, float> overYValue = new Dictionary<float, float> ();
//
//			float beforeMinX = xSort [0] * src.width;
//			float beforeMaxX = xSort [2] * src.width;
//
//			float beforeMinY = ySort [0] * src.height;
//			float beforeMaxY = ySort [2] * src.height;
//
//			int minX = floatToDownInt(beforeMinX);
//			int maxX = floatToUpInt(beforeMaxX);
//
//			int minY = floatToDownInt(beforeMinY);
//			int maxY = floatToUpInt(beforeMaxY);
//
//			overXValue.Add (beforeMinX, minX - 1.0f);
//			if (!overXValue.ContainsKey (xSort [1] * src.width)) 
//			{
//				overXValue.Add (xSort [1] * src.width, xSort [1] * src.width);
//			}
//			if (!overXValue.ContainsKey (beforeMaxX)) 
//			{
//				overXValue.Add (beforeMaxX, maxX);
//			}
//
//			overYValue.Add (beforeMinY, minY - 1.0f);
//			if (!overYValue.ContainsKey (ySort [1] * src.height)) 
//			{
//				overYValue.Add (ySort [1] * src.height, ySort [1] * src.height);
//			}
//			if (!overYValue.ContainsKey (beforeMaxY)) 
//			{
//				overYValue.Add (beforeMaxY, maxY);
//			}
//
//			for (int y = minY; y < maxY; y++) 
//			{
//				for (int x = minX; x < maxX; x++) 
//				{
//					if (isPointInTriangle (new Vector2(overXValue[src.uv [v0].x * (float)src.width], overYValue[src.uv[v0].y * (float)src.height])
//						, new Vector2(overXValue[src.uv [v1].x * (float)src.width], overYValue[src.uv[v1].y * (float)src.height])
//						, new Vector2(overXValue[src.uv [v2].x * (float)src.width], overYValue[src.uv[v2].y * (float)src.height])
//						, new Vector2 ((float)(x), (float)y))) 
////					if (isPointInTriangle (new Vector2 (uv [v0].x * (float)width, uv [v0].y * (float)height)
////						, new Vector2 (uv [v1].x * (float)width, uv [v1].y * (float)height)
////						, new Vector2 (uv [v2].x * (float)width, uv [v2].y * (float)height)
////						, new Vector2 ((float)(x), (float)y))) 
//					{
//
//						float r1 = 0.0f;
//						float r2 = 0.0f;
//						float r3 = 0.0f;
//
//						Vector3 r = Vector3.zero;
//
//						Vector3 a = new Vector3 (overXValue [src.uv [v0].x * (float)src.width], overYValue [src.uv [v0].y * (float)src.height], 0.0f);
//						Vector3 b = new Vector3 (overXValue [src.uv [v1].x * (float)src.width], overYValue [src.uv [v1].y * (float)src.height], 0.0f);
//						Vector3 c = new Vector3 (overXValue [src.uv [v2].x * (float)src.width], overYValue [src.uv [v2].y * (float)src.height], 0.0f);
//						Vector3 p = new Vector3 ((float)x, (float)y, 0.0f);
//
//						BarycentricCoordinates.barycent (a, b, c, p, out r);
//
//						Vector3 q = BarycentricCoordinates.calcBarycentricPoint (vp0, vp1, vp2, r);
//
//						for (int i = 0; i < tar.triangles.Count; i += 3) 
//						{
//							Vector3 hitPos = Vector3.zero;
//							if(CustomRayCast.triangleRayCast(tar.vertices[tar.triangles[i + 0]], tar.vertices[tar.triangles[i + 1]], tar.vertices[tar.triangles[i + 2]]
//								,q, Vector3.forward, ref hitPos))
//							{
//								Vector3 newR = Vector3.zero;
//
//								BarycentricCoordinates.barycent (tar.vertices [tar.triangles [i + 0]], tar.vertices [tar.triangles [i + 1]], tar.vertices [tar.triangles [i + 2]], hitPos, out newR);
//
//								if (float.IsNaN (newR.x))
//								{
//									newR.x = 0.0f;
//								}
//								if (float.IsNaN (newR.y))
//								{
//									newR.y = 0.0f;
//								}
//								if (float.IsNaN (newR.z))
//								{
//									newR.z = 0.0f;
//								}
//
//								q = BarycentricCoordinates.calcBarycentricPoint (tar.uv [tar.triangles [i + 0]] * tar.width, tar.uv [tar.triangles [i + 1]] * tar.width, tar.uv [tar.triangles [i + 2]] * tar.width, newR);
//								genTex[y * src.width + x] = tar.texColors[(int)q.y * tar.width + (int)q.x];
//
//								break;
//							}
//						}
//					}
//				} 
//			}
//
//			xSort.Clear ();
//			xSort = null;
//
//			ySort.Clear ();
//			ySort = null;
//
//			overXValue.Clear ();
//			overXValue = null;
//
//			overYValue.Clear ();
//			overYValue = null;
//		}
//
//		Debug.Log("c");
//	}

	int floatToDownInt(float value)
	{
		value *= 10.0f;

		value = value - (value % 10.0f);
		value /= 10.0f;

		return (int)value;
	}

	int floatToUpInt(float value)
	{
		value *= 10.0f;

		value = value - (value % 10.0f);
		value /= 10.0f;

		value += 1.0f;

		return (int)value;
	}

	bool isPointInTriangle(Vector2 uvp0, Vector2 uvp1, Vector2 uvp2, Vector2 uvtp)
	{
		Vector3 vec0 = uvp2 - uvp0;
		Vector3 vec1 = uvp1 - uvp0;
		Vector3 vec2 = uvtp - uvp0;

		float dot00 = Vector3.Dot (vec0, vec0);
		float dot01 = Vector3.Dot (vec0, vec1);
		float dot02 = Vector3.Dot (vec0, vec2);
		float dot11 = Vector3.Dot (vec1, vec1);
		float dot12 = Vector3.Dot (vec1, vec2);

		float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
		float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
		float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

		return (u >= 0.0f) && (v >= 0.0f) && (u + v < 1.0f);
	}
}
