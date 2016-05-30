using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class EF_Rare4Over : MonoBehaviour {

	public ParticleSystem Effect;
	public AudioSource SE;
	public GameObject growSpace;

	public bool anime_Start;

	void Update () {
		
		if (anime_Start == true) {
			AnimationPlay ();
			anime_Start = false;
		}
		
	}

	public void AnimationPlay(){
		this.Effect.Play();
		this.SE.Play();
		growSpace.GetComponent<Image> ().DOFade (0.8f, 0.3f).SetLoops (2, LoopType.Yoyo);
	}
}
