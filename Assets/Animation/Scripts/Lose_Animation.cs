using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class LoseAnimation : MonoBehaviour {

	public Transform loseBox;
	public Image loseText, bk;
	public AudioSource SE_lose;

//	public bool Lose_Start;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
/*	void Update () {

		if (Lose_Start == true) {
			StartCoroutine (Lose_Animation_Start ());
			Lose_Start = false;
		}
	}
*/
	public IEnumerator Lose_Animation_Start(){
		//初期化
		loseText.color = new Color (1, 1, 1, 1);
		loseBox.localPosition = new Vector3 (0, 700, 0);
		yield return new WaitForSeconds (0.1f);

		bk.DOFade (0.5f, 0.5f);
		SE_lose.Play ();
		loseBox.DOLocalMove (new Vector3 (0, 0, 0), 0.8f).SetEase (Ease.OutBounce);
		yield return new WaitForSeconds (2.5f);

		loseText.DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.5f);

	}

}
