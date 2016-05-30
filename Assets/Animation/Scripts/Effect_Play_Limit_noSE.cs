using UnityEngine;
using System.Collections;

public class Effect_Play_Limit_noSE : MonoBehaviour 
{
	public ParticleSystem Effect;
	
	public bool Effect_Start;
	
	void Update () 
	{
		if (this.Effect_Start == true) 
		{
			this.Effect.Play();
			Effect_Start = false;
		}
	}
}
