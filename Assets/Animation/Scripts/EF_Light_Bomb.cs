using UnityEngine;
using System.Collections;

public class EF_Light_Bomb : MonoBehaviour
{
	public ParticleSystem Light_Bomb_Effect;
	public AudioSource Light_Bomb_SE;
	
	public bool Effect_Play_Limit;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.Effect_Play_Limit == true) 
		{
			this.Light_Bomb_Effect.Play();
			this.Light_Bomb_SE.Play();
			Effect_Play_Limit = false;
		}
	}
}
