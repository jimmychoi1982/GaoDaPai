using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween宣言
using UnityEngine.UI;//uGUI

public class Shelling : MonoBehaviour {

	public UIElement_ShellingBox shellingBox;
	private GameObject btn;
	private Image sightsImage, btnImage;
	private Transform sights;
	private GameObject EF_Shelling_shoot, EF_Shelling_hit;
	private AudioSource SE_lockOn;

	public Transform MS;//発射するMS

	public bool btnActive, lockOn, lockOff, shoots;

	// Use this for initialization
	void Start () {

		btn = shellingBox.btn;
		btnImage = shellingBox.btnImage;
		sightsImage = shellingBox.sightsImage;
		sights = shellingBox.sights;
		EF_Shelling_shoot = shellingBox.EF_Shelling_shoot;
		EF_Shelling_hit = shellingBox.EF_Shelling_hit;
		SE_lockOn = shellingBox.SE_lockOn;
	}
	
	// Update is called once per frame
	void Update () {

		if (btnActive== true) {
			StartCoroutine (btnActiveAnimation());
			btnActive = false;
		}
		if (lockOn == true) {
			MSdrugOn();
			StartCoroutine (sightsInAnimation());
			lockOn = false;
		}
		if (lockOff == true) {
			MSdrugOff();
			StartCoroutine (sightsOutAnimation());
			lockOff  = false;
		}
		if (shoots == true) {
			StartCoroutine (MSshoots());
			btnDisable();
			StartCoroutine (shootsAnimation());
			shoots = false;
		}
	
	}

	//ボタン表示
	IEnumerator btnActiveAnimation() {
		//MSの直下にボタンを出現させる
		Transform btnTra = btn.GetComponent<Transform> ();
		btnTra.transform.localPosition = MS.localPosition + new Vector3(0, -200, 0);
		btn.SetActive (false);
		btnImage.color = new Color (1, 1, 1, 0);
		yield return new WaitForSeconds (0.1f);

		btn.SetActive (true);
		btnImage.DOFade (1, 0.2f);
	}

	//ボタン非表示
	void btnDisable() {
		btn.SetActive (false);
	}

	//MSをボタン位置までドラッグ ※仮の動き、本来はユーザーがボタン上までドラッグさせる
	void MSdrugOn() {
		MS.DOLocalMoveY (-20, 0.4f).SetRelative ();
	}
	//MSから発射させずに指を離す　※仮の動き、アイコンが元の位置に戻ればよい
	void MSdrugOff() {
		MS.DOLocalMoveY (20, 0.4f).SetRelative ();
	}

	//ボタンOn状態で指を離したときのMSの動き
	IEnumerator MSshoots(){
		MS.DOLocalMoveY (100, 0.1f).SetRelative ().SetEase (Ease.InQuart);
		yield return new WaitForSeconds (0.1f);
		MS.DOLocalMoveY (-80, 0.2f).SetRelative ().SetEase (Ease.OutBack);
		yield return new WaitForSeconds (0.2f);
	}

	//照準が母艦の上に乗る
	IEnumerator sightsInAnimation() {
		sightsImage.color = new Color (1, 1, 1, 0);
		sights.localScale = new Vector3 (2, 2, 2);
		yield return new WaitForSeconds (0.1f);

		SE_lockOn.Play ();
		sightsImage.DOFade (1, 0.4f);
		sights.DOScale (new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutExpo);	
	}

	//照準フェードアウト
	IEnumerator sightsOutAnimation() {
		sightsImage.DOFade (0, 0.1f);
		sights.DOScale (new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
		yield return new WaitForSeconds (0.2f);
	}

	//発射エフェクト、着弾エフェクト再生
	IEnumerator shootsAnimation() {
		EF_Shelling_shoot.GetComponent<Effect_Play_Limit>().Effect_Start = true;
		yield return new WaitForSeconds (0.4f);
		EF_Shelling_hit.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		StartCoroutine (sightsOutAnimation ());
	}

}
