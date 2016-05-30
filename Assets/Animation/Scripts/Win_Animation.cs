using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class WinAnimation : MonoBehaviour {

	public Transform winBox;
	public Image win_under, win_over, bk;
	public ParticleSystem particle;
	public AudioSource SE_win;

	//public bool Win_Start;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
/*	void Update () {

		if (Win_Start == true) {
			StartCoroutine (Win_Animation_Start ());
			Win_Start = false;
		}
	
	}
*/
	public IEnumerator Win_Animation_Start(){

		winBox.localScale = new Vector3 (6, 6, 6);
		yield return new WaitForSeconds (0.1f);

		bk.DOFade (0.5f, 0.5f);
		SE_win.Play ();
		winBox.DOScale(1, 0.5f).SetEase (Ease.OutExpo);
		win_over.DOFade (1,0.2f);
		yield return new WaitForSeconds (0.2f);

		//Particleスタート
		particle.Play ();

		//文字の裏の光画像が明滅する
		win_under.DOFade (1, 0.4f);
		yield return new WaitForSeconds (0.4f);
		win_under.DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);
		win_under.DOFade (1, 0.4f);
		yield return new WaitForSeconds (0.4f);
		win_under.DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);

		//Particleストップ
		particle.Stop ();

		//fade out
		win_over.DOFade (0, 0.5f);
	}




}
