using UnityEngine;
using System.Collections;

public class Combination_Demo : MonoBehaviour {

	public Animator animator;
	public ParticleSystem effect;

	public bool anime_Start;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (anime_Start == true) {
			AnimationPlay ();
			anime_Start = false;
		}
	
	}

	public void AnimationPlay(){
		animator.SetTrigger("cutin"); 
		effect.Play ();
	}
}
