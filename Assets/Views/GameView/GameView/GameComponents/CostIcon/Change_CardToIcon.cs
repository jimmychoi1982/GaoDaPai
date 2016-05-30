using System;
using System.Collections;

using UnityEngine;


public class Change_CardToIcon : MonoBehaviour 
{
	public ParticleSystem Change_Effect;
	public AudioSource Change_SE;
	
	public bool Effect_Play_Limit;


	// Update is called once per frame
	void Update () 
	{
		if (this.Effect_Play_Limit == true) 
		{
			StartCoroutine(Effect_Start());
			Effect_Play_Limit = false;
		}
	}

	//
	public IEnumerator Effect_Start() {
		// Wait for object to be active
		while(!gameObject.activeInHierarchy) {
			yield return null;
		}

		// Play effects
		this.Change_Effect.Play();
		yield return new WaitForSeconds (0.2f);
		this.Change_SE.Play();
	}
}
