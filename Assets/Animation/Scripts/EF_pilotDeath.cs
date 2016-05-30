using UnityEngine;
using System.Collections;

public class EF_pilotDeath : MonoBehaviour 
{
	public ParticleSystem Effect;
	public AudioSource SE_Bomb, SE_pilotDeath;
	
	public bool Effect_Start;
	
	void Update () 
	{
		if (this.Effect_Start == true) 
		{
			StartCoroutine(pilotDeath());
			Effect_Start = false;
		}
	}

	IEnumerator pilotDeath()
	{
		SE_pilotDeath.Play ();
		Effect.Play ();
		yield return new WaitForSeconds(0.5f);
		SE_Bomb.Play ();
	}
}
