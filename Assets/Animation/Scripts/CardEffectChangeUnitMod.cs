using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectChangeUnitMod : MonoBehaviour {

	//ドローされるカード
	public GameObject newCard;

	//交換する場合、前のユニットカード、アイコン
	public GameObject oldIcon, oldCard;
	//交換する場合生成するエフェクト
	public GameObject EF_UnitToCard;
	public AudioSource SE_ChangeUnit;

	//ポジション
	private Vector3 oldCardPos, newCardPos;

	public bool change;

	public void EffectStart ()
	{
		StartCoroutine(changeUnitAnimation());
	}
	
	//カードが画面上からドローされる動き
	public IEnumerator changeUnitAnimation()
	{

		CardPositionGet ();

		//カードが画面上からドローされる
		newCard.transform.DOLocalMove(newCardPos, 1.5f).SetEase(Ease.OutQuart);
		yield return new WaitForSeconds (1f);

		//交換するカードの場合
		if (change == true) 
		{
			StartCoroutine (cardChange());
		} 
		//破棄するカードの場合
		else if (change == false) 
		{
			yield return new WaitForSeconds (0.5f);
			StartCoroutine(cardDestruction());
		}
	}

	//カードを対象ユニットの中に吸い込ませるアニメ
	IEnumerator cardChange(){

		//交換対象のユニットがカード化
		StartCoroutine (EF_UnitToCard_EffectBirth ());
		StartCoroutine (UnitToCard ());

		yield return new WaitForSeconds (0.5f);

		//カード位置チェンジ
		newCard.transform.DOLocalMove (oldCardPos, 0.6f).SetEase (Ease.OutCubic);
		oldCard.transform.DOLocalMove (newCardPos, 0.6f).SetEase (Ease.OutCubic);

		SE_ChangeUnit.Play ();

	}


	//カード位置取得
	void CardPositionGet(){
		oldCardPos = oldIcon.transform.localPosition;
		newCardPos = new Vector3 (-780, 325, 0);
	}

	//アイコンがカード化するエフェクト生成
	IEnumerator EF_UnitToCard_EffectBirth(){
		GameObject efObj = (GameObject)Instantiate(EF_UnitToCard, transform.position, Quaternion.identity);
		Transform efTra = efObj.GetComponent<Transform>();
		efObj.transform.SetParent(oldIcon.transform, true);
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit_noSE>().Effect_Start = true;
			
		yield return new WaitForSeconds (1f);
		Destroy (efObj);
	}

	//交換対象のユニットアイコンを非表示、カードを表示
	IEnumerator UnitToCard(){
		CanvasGroup oldIconCanvas = oldIcon.GetComponent<CanvasGroup> ();
		CanvasGroup oldCardCanvas = oldCard.GetComponent<CanvasGroup> ();

		oldCard.transform.DOScale (0.5f, 0.2f);
		oldIconCanvas.DOFade(0, 0.2f);
		oldCardCanvas.DOFade (1, 0.2f);
		yield return new WaitForSeconds (0.5f);

		oldIcon.SetActive (false);
	}


	//↓共通エフェクトを呼び出すようにしたら以下削除-------------------------------------------------------------------------------

	//カード破棄アニメーション
	IEnumerator cardDestruction(){
		CanvasGroup newCardCanvasGroup = newCard.GetComponent<CanvasGroup> ();
		newCard.transform.DOLocalMoveY (50, 0.3f).SetRelative ();
		newCardCanvasGroup.DOFade (0, 0.3f);
		
		yield return new WaitForSeconds (0.3f);
		Destroy (newCard);
	}

}
