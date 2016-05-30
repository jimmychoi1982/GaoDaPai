using System.Collections;
using UnityEngine;

public class Effect_Play_Limit : MonoBehaviour 
{
	public ParticleSystem Effect;
	public AudioSource SE;
	
	public bool Effect_Start;
	
	void Update() 
	{
		if (this.Effect_Start == true) 
		{
			Play();
			Effect_Start = false;
		}
	}

	public void Play()
	{
		this.Effect.Play();
		this.SE.Play();
	}
}
