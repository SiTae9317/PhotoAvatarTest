using UnityEngine;
using System.Collections;

public class CharacterBlendshape : MonoBehaviour 
{
	public SkinnedMeshRenderer[] smr;

	private float eyeCloseTime = 0.0f;

	private bool isWink = false;

	private bool initSuccess = false;

	private bool isAction = false;

	// Use this for initialization
	void Start () 
	{
		smr = new SkinnedMeshRenderer[1];
		smr [0] = gameObject.GetComponent<SkinnedMeshRenderer> ();

		initSuccess = true;
		//smr = GetComponent<VoiceRecorder>().;
	}

	/*
	public void setBlendCharacter(SkinnedMeshRenderer activeAvatar)
	{
		smr = activeAvatar;
		initSuccess = true;
	}
	*/

	IEnumerator eyeBlend()
	{
		float eyeBlendValue = 0.0f;
		float eyeBlendTime = 0.0f;
		float stateChangeValue = 1.0f;

		while (eyeBlendValue >= 0.0f) 
		{
			yield return null;

			if (eyeBlendValue >= 100.0f) 
			{
				stateChangeValue = -1.0f;
			}

			eyeBlendValue += (Time.deltaTime * stateChangeValue * Random.Range(600.0f, 800.0f));

			for (int j = 0; j < smr.Length; j++) 
			{
				for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
				{
					if (smr[j].sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("closed_eye")) 
					{
						smr[j].SetBlendShapeWeight (i, eyeBlendValue);
//						Material[] mats = smr [j].materials;
//						for(int k = 0; k < mats.Length; k++)
//						{
//							mats[k].SetFloat ("_Blend1", eyeBlendValue / 100.0f);
//						}
//						smr [j].material.SetFloat ("_Blend1", eyeBlendValue / 100.0f);
					}
				}
			}
		}

		for (int j = 0; j < smr.Length; j++) 
		{
			for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
			{
				if (smr[j].sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("closed_eye")) 
				{
					smr[j].SetBlendShapeWeight (i, 0.0f);
//					smr [j].material.SetFloat ("_Blend1", eyeBlendValue / 100.0f);
//					Material[] mats = smr [j].materials;
//					for(int k = 0; k < mats.Length; k++)
//					{
//						mats[k].SetFloat ("_Blend1", eyeBlendValue / 100.0f);
//					}
				}
			}
		}
	}

	/*
	IEnumerator eyeBlend()
	{
		float eyeBlendTime = 0.0f;
		float stateChangeValue = 1.0f;

		for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++) 
		{
			if (smr.sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("eye_closed")) 
			{
				smr.SetBlendShapeWeight (i, 100.0f);
			}
		}

		while (eyeBlendTime < 0.1f) 
		{
			eyeBlendTime += Time.deltaTime;
			yield return null;
		}

		for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++) 
		{
			if (smr.sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("eye_closed")) 
			{
				smr.SetBlendShapeWeight (i, 0.0f);
			}
		}
	}
	*/

	IEnumerator aaBlend()
	{
		float winkValue = 0.0f;
		float eyeStatusValue = 1.0f;

		while (winkValue >= 0.0f) {
			yield return null;

			if (winkValue >= 200.0f) {
				eyeStatusValue = -1.0f;
			}

			winkValue += (Time.deltaTime * 450.0f * eyeStatusValue);

			for (int j = 0; j < smr.Length; j++) 
			{
				for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
				{
					if (smr[j].sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("aa")) 
					{
						smr[j].SetBlendShapeWeight (i, winkValue);
					}
				}
			}
		}
		isAction = false;
	}

	IEnumerator smileBlend()
	{
		float smileValue = 0.0f;

		while (smileValue < 100.0f) 
		{
			yield return null;

			smileValue += (Time.deltaTime * 300.0f);


			for (int j = 0; j < smr.Length; j++) 
			{
				for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
				{
					if (smr[j].sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("aa")) 
					{
						smr[j].SetBlendShapeWeight (i, smileValue);
					}
				}
			}
		}
	}

	IEnumerator winkBlend()
	{
		isWink = true;

		float winkValue = 0.0f;
		float eyeStatusValue = 1.0f;

		while (winkValue >= 0.0f) {
			yield return null;

			if (winkValue >= 150.0f) {
				eyeStatusValue = -1.0f;
			}

			winkValue += (Time.deltaTime * 450.0f * eyeStatusValue);

			for (int j = 0; j < smr.Length; j++) 
			{
				for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
				{
					if (smr[j].sharedMesh.GetBlendShapeName (i).ToLower ().Contains ("r_eye")) 
					{
						smr[j].SetBlendShapeWeight (i, winkValue);
					}
				}
			}
		}

		isWink = false;
	}

	public void resetBlend()
	{
		for (int j = 0; j < smr.Length; j++) 
		{
			for (int i = 0; i < smr[j].sharedMesh.blendShapeCount; i++) 
			{
				smr[j].SetBlendShapeWeight (i, 0);
			}
		}
	}

	public void smileAction()
	{
		StartCoroutine (smileBlend ());
	}

	public void winkAction()
	{
		StartCoroutine (winkBlend ());
	}
	
	// Update is called once per frame
	void Update () 
	{			
		if (initSuccess) 
		{
			if (!isWink) 
			{
				eyeCloseTime -= Time.deltaTime;
			}

			if (eyeCloseTime <= 0.0f) 
			{
				eyeCloseTime = Random.Range (2.0f, 4.0f);

				StartCoroutine (eyeBlend ());
			}
		}

		if (Input.GetMouseButtonDown (0) && !isAction) 
		{
			isAction = true;
			StartCoroutine (aaBlend ());
		}
	}
}
