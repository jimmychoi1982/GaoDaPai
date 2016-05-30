using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class YourTurnAnimation : MonoBehaviour {

	public Transform txt, line;
	public Image txtObj, lineObj;
	public AudioSource SE_yourTurn;
	//public bool Anime_Start;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
/*	void Update () {

		if(Anime_Start == true){
			StartCoroutine(YourTurnAnimation());
			Anime_Start = false;
		}

	}
*/
	public IEnumerator YourTurn_Animation () {
		//Reset--------------------------------
		lineObj.color = new Color (1, 1, 1, 1);
		txtObj.color = new Color (1, 1, 1, 1);
		line.localScale = new Vector3 (0, 1, 1);
		txt.localScale = new Vector3 (0, 0, 0);

		SE_yourTurn.Play ();

		line.DOScale(new Vector3(1,1,1), 0.8f).SetEase(Ease.OutExpo);
		lineObj.DOFade (0, 0.8f);
		txt.DOScale(1f, 0.8f).SetEase(Ease.OutExpo);

		yield return new WaitForSeconds (0.8f);

		txtObj.DOFade (0, 0.2f);
		yield return new WaitForSeconds (0.2f);
	}

}
