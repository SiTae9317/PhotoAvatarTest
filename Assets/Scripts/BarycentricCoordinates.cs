using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarycentricCoordinates : MonoBehaviour 
{
//	public GameObject p0;
//	public GameObject p1;
//	
//	Vector3 a = new Vector3 (0.0f, 0.0f, 0.0f);
//	Vector3 b = new Vector3 (2.0f, 4.0f, 0.0f);
//	Vector3 c = new Vector3 (4.0f, 2.0f, 0.0f);
//	Vector3 p = new Vector3 (2.0f, 3.0f, 0.0f);
//
//	Vector3 d = new Vector3 (0.0f, 0.0f, 1.0f);
//	Vector3 e = new Vector3 (2.0f, 4.0f, -1.0f);
//	Vector3 f = new Vector3 (4.0f, 2.0f, 4.0f);
//
//	// Use this for initialization
//	void Start () 
//	{		
////		float r1 = 0.0f;
////		float r2 = 0.0f;
////		float r3 = 0.0f;
////
//////		barycent (a.x, a.y, a.z, b.x, b.y, b.z, c.x, c.y, c.z, p.x, p.y, p.z, out r1, out r2, out r3);
////
////		Vector3 r = Vector3.zero;
////
////		barycent (a, b, c, p, out r);
////
////		Vector3 q = calcBarycentricPoint (d, e, f, r);
////
//////		Vector3 q = new Vector3 (d.x * r1 + e.x * r2 + f.x * r3, 
//////			            		 d.y * r1 + e.y * r2 + f.y * r3,
//////			            		 d.z * r1 + e.z * r2 + f.z * r3);
////
//////		Debug.Log (r1 + " " + r2 + " " + r3);
////
////		Debug.Log (r.x + " " + r.y + " " + r.z);
////		Debug.Log (q.x + " " + q.y + " " + q.z);
////
////		p0.transform.position = p;
////		p1.transform.position = q;
//	}
//	
//	// Update is called once per frame
//	void Update () 
//	{
//		Vector3 p = p0.transform.position;//new Vector3 (2.0f, 3.0f, 0.0f);
//		float r1 = 0.0f;
//		float r2 = 0.0f;
//		float r3 = 0.0f;
//
//		//		barycent (a.x, a.y, a.z, b.x, b.y, b.z, c.x, c.y, c.z, p.x, p.y, p.z, out r1, out r2, out r3);
//
//		Vector3 r = Vector3.zero;
//
//		barycent (a, b, c, p, out r);
//
//		Vector3 q = calcBarycentricPoint (d, e, f, r);
//
//		//		Vector3 q = new Vector3 (d.x * r1 + e.x * r2 + f.x * r3, 
//		//			            		 d.y * r1 + e.y * r2 + f.y * r3,
//		//			            		 d.z * r1 + e.z * r2 + f.z * r3);
//
//		//		Debug.Log (r1 + " " + r2 + " " + r3);
//
//		Debug.Log (r.x + " " + r.y + " " + r.z);
//		Debug.Log (q.x + " " + q.y + " " + q.z);
//
//		p0.transform.position = p;
//		p1.transform.position = q;
//		
//		Debug.DrawLine (a, b, Color.green);
//		Debug.DrawLine (b, c, Color.green);
//		Debug.DrawLine (c, a, Color.green);
//
//		Debug.DrawLine (d, e, Color.red);
//		Debug.DrawLine (e, f, Color.red);
//		Debug.DrawLine (f, d, Color.red);
//
//		Debug.DrawLine (a, d, Color.blue);
//		Debug.DrawLine (b, e, Color.blue);
//		Debug.DrawLine (c, f, Color.blue);
//	}

	public static float triArea(float a, float b, float c)
	{
		float s = (a + b + c) / 2.0f;

		return Mathf.Sqrt (s * (s - a) * (s - b) * (s - c));
	}

	public static float dist(float x0, float y0, float z0, float x1, float y1, float z1)
	{
		float a = x1 - x0;
		float b = y1 - y0;
		float c = z1 - z0;

		return Mathf.Sqrt (a * a + b * b + c * c);
	}

	public static float dist(Vector3 v0, Vector3 v1)
	{
		float a = v1.x - v0.x;
		float b = v1.y - v0.y;
		float c = v1.z - v0.z;

		return Mathf.Sqrt (a * a + b * b + c * c);
	}

	public static Vector3 calcBarycentricPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 r)
	{
		return new Vector3 (v0.x * r.x + v1.x * r.y + v2.x * r.z,
			v0.y * r.x + v1.y * r.y + v2.y * r.z,
			v0.z * r.x + v1.z * r.y + v2.z * r.z);
	}

	public static void barycent(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 vp, out Vector3 vr)
	{
		float a = dist (v0.x, v0.y, v0.z, v1.x, v1.y, v1.z);
		float b = dist (v1.x, v1.y, v1.z, v2.x, v2.y, v2.z);
		float c = dist (v2.x, v2.y, v2.z, v0.x, v0.y, v0.z);

		float totalArea = triArea (a, b, c);

		float length0 = dist (v0.x, v0.y, v0.z, vp.x, vp.y, vp.z);
		float length1 = dist (v1.x, v1.y, v1.z, vp.x, vp.y, vp.z);
		float length2 = dist (v2.x, v2.y, v2.z, vp.x, vp.y, vp.z);

		vr.x = triArea (b, length1, length2) / totalArea;
		vr.y = triArea (c, length0, length2) / totalArea;
		vr.z = triArea (a, length0, length1) / totalArea;
	}

	public static void barycent(float x0, float y0, float z0, float x1, float y1, float z1, float x2, float y2, float z2,
								  float vx, float vy, float vz, out float u, out float v, out float w)
	{
		float a = dist (x0, y0, z0, x1, y1, z1);
		float b = dist (x1, y1, z1, x2, y2, z2);
		float c = dist (x2, y2, z2, x0, y0, z0);

		float totalArea = triArea (a, b, c);

		float length0 = dist (x0, y0, z0, vx, vy, vz);
		float length1 = dist (x1, y1, z1, vx, vy, vz);
//		float length1 = dist (x1, y1, z2, vx, vy, vz);
		float length2 = dist (x2, y2, z2, vx, vy, vz);

		u = triArea (b, length1, length2) / totalArea;
		v = triArea (c, length0, length2) / totalArea;
		w = triArea (a, length0, length1) / totalArea;
	}
}
