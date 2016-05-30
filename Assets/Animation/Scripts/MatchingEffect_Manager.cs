using UnityEngine;
using System.Collections;

public class MatchingEffect_Manager : MonoBehaviour 
{
	public ParticleSystem VS_Effect;

	public bool Effect_Play_Limit, Effect_Stop_Limit;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.Effect_Play_Limit == true) 
		{
			this.VS_Effect.Play();
			Effect_Play_Limit = false;
		}

		if (this.Effect_Stop_Limit == true) 
		{
			this.VS_Effect.Stop();
			Effect_Stop_Limit = false;
		}
	}
}
