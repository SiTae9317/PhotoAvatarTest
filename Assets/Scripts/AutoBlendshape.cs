using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBlendshape : MonoBehaviour 
{
//	float updownVec = -1;
	Vector3 targetX = Vector3.right;
	Vector3 targetY = Vector3.up;
	
	// Use this for initialization
	void Start () 
	{
		gameObject.AddComponent<CharacterBlendshape> ();	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 rotValue = rotationX ();
		rotValue += rotationY();
		gameObject.transform.localEulerAngles = rotValue;
	}

	Vector3 rotationX()
	{
		float angle = gameObject.transform.localEulerAngles.x;

		if (angle > 180.0f) 
		{
			angle -= 360.0f;
		}
		else if (angle < -180.0f) 
		{
			angle += 360.0f;
		}
		if (angle < -0.9f) 
		{
			targetX = Vector3.right;
		}
		else if (angle > 0.9f) 
		{
			targetX = Vector3.left;
		}

		Vector3 nowRot = new Vector3 (angle, 0.0f, 0.0f);
		return Vector3.Lerp (nowRot, targetX, Time.deltaTime * 2.0f);
	}

	Vector3 rotationY()
	{
		float angle = gameObject.transform.localEulerAngles.y;

		if (angle > 180.0f) 
		{
			angle -= 360.0f;
		}
		else if (angle < -180.0f) 
		{
			angle += 360.0f;
		}
		if (angle < -0.9f) 
		{
			targetY = Vector3.up;
		}
		else if (angle > 0.9f) 
		{
			targetY = Vector3.down;
		}

		Vector3 nowRot = new Vector3 (0.0f, angle, 0.0f);
		return Vector3.Lerp (nowRot, targetY, Time.deltaTime * 5.0f);
	}
}
