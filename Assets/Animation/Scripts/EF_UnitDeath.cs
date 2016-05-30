using UnityEngine;
using System.Collections;

public class EF_UnitDeath : MonoBehaviour 
{
	public ParticleSystem UnitDeath_Effect;
	public AudioSource UnitDeath_SE;
	
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
			this.UnitDeath_Effect.Play();
			Effect_Play_Limit = false;

			StartCoroutine("Card_Death_Bomb");
		}
	}

	IEnumerator Card_Death_Bomb()
	{
		yield return new WaitForSeconds (0.1f);
		this.UnitDeath_SE.Play ();
	}
}
