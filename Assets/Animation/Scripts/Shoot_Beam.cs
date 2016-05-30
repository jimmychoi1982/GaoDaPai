using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Shoot_Beam : MonoBehaviour {

	private ParticleSystem shoot, hit;
	public Transform Player;
	public bool Anime_Start;

	// Use this for initialization
	void Start () {

		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);

		shoot = this.transform.Find("Beam_shoot/LongSpakles").GetComponent<ParticleSystem>();
		hit = this.transform.Find("Beam_hit/Glow").GetComponent<ParticleSystem>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Anime_Start == true){
			StartCoroutine(Shelling_Animation());
			Anime_Start = false;
		}
	
	}

	IEnumerator Shelling_Animation() {

		Sequence PlayerBack = DOTween.Sequence();
			PlayerBack.Prepend(Player.DOLocalMoveY(-10, 0.3f).SetRelative())
			.Append(Player.DOLocalMoveY(30, 0.1f).SetRelative())
			.Append(Player.DOLocalMoveY(-30, 0.2f).SetRelative())
			.Append(Player.DOLocalMoveY(10, 1f).SetRelative());

		yield return new WaitForSeconds (0.4f);

		shoot.Play();

		yield return new WaitForSeconds (0.5f);

		hit.Play();

	}

}
