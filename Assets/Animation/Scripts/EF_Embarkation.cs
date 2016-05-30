using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class EF_Embarkation : MonoBehaviour {

	public ParticleSystem Effect;
	public AudioSource SE;
	public Transform logo;
	public Image logoImage;
	public bool Anime_Start;

	// Use this for initialization
	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	}


	void Update () {
		if (Anime_Start == true){
			StartCoroutine(Embarkation_Start());
			Anime_Start = false;
		}
	}


	IEnumerator Embarkation_Start()
	{
		//Reset
		logo.localScale = new Vector3(0.01f, 1.5f, 1.5f);
		logoImage.DOFade (0, 0);
		yield return new WaitForSeconds (0.2f);

		Effect.Play(); 
		SE.Play();

		yield return new WaitForSeconds (0.2f);

		logoImage.DOFade (1, 0.2f);
		logo.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).SetEase(Ease.OutExpo);

		yield return new WaitForSeconds (1f);
		logo.DOScale(new Vector3(3.5f, 3.5f, 3.5f), 0.4f);
		logoImage.DOFade (0, 0.1f);
		
	}

	
}
