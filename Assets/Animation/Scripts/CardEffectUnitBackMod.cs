using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectUnitBackMod : MonoBehaviour {
	
	//救出される対象ユニットが乗っているBattler
	public GameObject battler;
	//手札になるカード
	public GameObject card;

	//生成されるエフェクト
	public GameObject EF_UnitBack;
	//エフェクトが行き着く場所
	public Vector3 lastPosition;
	
	public void EffectStart () {

			StartCoroutine(unitBack_Animation());
	}
	
	//ユニットが破壊されたとき、手札まで戻るアニメーション
	public IEnumerator unitBack_Animation () {

		StartCoroutine (EffectBirth ());
		UnitIconFadeOut ();

		yield return new WaitForSeconds (0.2f);
		BackCardAnimation ();

	}

	//エフェクト生成
	IEnumerator EffectBirth(){
		GameObject efObj = (GameObject)Instantiate(EF_UnitBack, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battler.transform, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3 (0, 0, 0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//エフェクトを表示したいコストアイコンの場所まで移動する　※ワールド座標（ローカル座標の方が使い勝手が良い場合はDOLocalMoveに変更してください）
		efTra.DOMove (lastPosition, 0.4f).SetEase (Ease.InOutQuart);

		//エフェクトDestroy
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy(efObj);
	}


	//手札に戻されたカード、拡大しながら表示される動き
	void BackCardAnimation(){
		CanvasGroup cardCanvasGroup = card.GetComponent<CanvasGroup> ();
		cardCanvasGroup.alpha = 0;
		card.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		
		cardCanvasGroup.DOFade (1, 0.3f);
		card.transform.DOScale (0.4f, 0.3f);//手札のカードの最終的な大きさ
	}

	//ユニットアイコンを消す
	void UnitIconFadeOut(){
		CanvasGroup battlerCanvasGroup = battler.GetComponent<CanvasGroup> ();
		battlerCanvasGroup.DOFade (0, 0.3f);
	}
	
	
}
