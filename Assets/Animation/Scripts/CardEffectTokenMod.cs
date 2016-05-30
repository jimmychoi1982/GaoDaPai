using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectTokenMod : MonoBehaviour {

	public Transform tokenParent;//トークンを射出するユニット
	public GameObject[] token;//射出されるトークン 1, 2, 3（現状３機まで対応。以降増える可能性あり)
	public GameObject EF_tokenLight;//トークンの上に出るエフェクト
	public AudioSource SE_Token;
	public bool MyTurn;//自分のターンかどうか
	public float tokenInterval = 150f;
	public float moveY = 220;
	public float[] tokenPosition;

	//アニメーション内で使用するプライベート変数
	private float addY;
	private Ease easing;
	private float time;
	private int tokenCount;

	void Awake () {
		tokenPosition = new float[7];
	}
	
	//※射出されるトークンGameObjectを生成してから再生して下さい
	//※生成するトークンGameObjectの初期値は、Scale(0.1f, 0.1f, 0.1f)、位置はスキル発動ユニットと同Positionにして下さい
	public void EffectStart ( int count ) {
		tokenCount = count;
		StartCoroutine(TokenAnimation());
	}


	//ビット・トークンを射出するアニメーションシーケンス
	IEnumerator TokenAnimation(){
		yield return new WaitForSeconds (0.1f);

		StartCoroutine(TokenInjectionAnimation ());
		yield return new WaitForSeconds (1f);

		StartCoroutine(TokenSetAnimation ());
		yield return new WaitForSeconds (0.4f);

	}

	//トークンを自機の上に拡大しながら射出
	IEnumerator TokenInjectionAnimation(){
		//Reset
		for (int i=0; i<tokenCount; i++) {
			token [i].SetActive (true);
			token [i].transform.localPosition = tokenParent.localPosition;
			token [i].transform.localScale = new Vector3(0f, 0f, 0f);
		}
		StartCoroutine (TokenEffectBirth ());

		yield return new WaitForSeconds (0.4f);
		SE_Token.Play ();
		if (MyTurn == true) {
			addY = moveY;
		} else if (MyTurn == false) {
			addY = -moveY;
		}

		//徐々に拡大する
		for (int i = 0; i<token.Length; i++) {
			token [i].transform.DOScale (new Vector3 (1.2f, 1.2f, 1.2f), 0.3f);
		}
		//射出するトークン数によってトークンを出す場所を設定
		time = 0.6f;
		easing = Ease.OutQuad;
		for (int i = 0; i < tokenCount; i++) {
			token [i].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (tokenPosition[i], addY, 0), time).SetEase (easing);
		} 
		/*
		if (token.Length == 1) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (0, addY, 0), time).SetEase (easing);
		} else if (token.Length == 2) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (-150, addY, 0), time).SetEase (easing);
			token [1].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (150, addY, 0), time).SetEase (easing);
		} else if (token.Length == 3) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (-300, addY, 0), time).SetEase (easing);
			token [1].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (0, addY, 0), time).SetEase (easing);
			token [2].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (300, addY, 0), time).SetEase (easing);
		}*/
		yield return new WaitForSeconds (time);
	}
	

	//トークンの中にエフェクトを生成、再生
	IEnumerator TokenEffectBirth(){
		for (int i = 0; i<token.Length; i++) {
			GameObject efObj = (GameObject)Instantiate (EF_tokenLight, transform.position, Quaternion.identity);
			efObj.transform.SetParent (token [i].transform, true);//各トークンの子要素としてエフェクトを生成
			Transform efTra = efObj.GetComponent<Transform> ();
			efTra.localScale = new Vector3 (1, 1, 1);
			efTra.localPosition = new Vector3 (0, 0, 0);
			efObj.GetComponent<Effect_Play_Limit_noSE> ().Effect_Start = true;
		}
		yield return new WaitForSeconds (3f);
	}


	//自機の上に射出したトークンが親ユニットの左右に並ぶ
	//最終的にアイコンが並ぶ位置なので、X軸の数値はロジックがあれば別に書き換えて下さい
	IEnumerator TokenSetAnimation(){
		for (int i = 0; i<token.Length; i++) {
			token [i].transform.DOScale (new Vector3 (1, 1, 1f), time);
		}
		time = 0.3f;
		easing = Ease.InQuad;
		if (token.Length== 1) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (-244, 0, 0), time).SetEase (easing);
		} else if (token.Length == 2) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (-244, 0, 0), time).SetEase (easing);
			token [1].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (244, 0, 0), time).SetEase (easing);
		} else if (token.Length == 3) {
			token [0].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (-244, 0, 0), time).SetEase (easing);
			token [1].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (244, 0, 0), time).SetEase (easing);
			token [2].transform.DOLocalMove (tokenParent.localPosition + new Vector3 (488, 0, 0), time).SetEase (easing);
		}
		yield return new WaitForSeconds (time);
	}



}
