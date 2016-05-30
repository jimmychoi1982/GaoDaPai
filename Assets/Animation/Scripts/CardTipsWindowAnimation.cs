using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardTipsWindowAnimation : MonoBehaviour {

	public UIElement_CardTipsWindow cardTipsWindow;
	private Image windowImage;
	private Text textObj;
	private Transform textTra;
	
	public bool windowIn, windowOut;
	
	void Start () {
		windowImage = cardTipsWindow.windowImage;
		textObj = cardTipsWindow.textObj;
		textTra = cardTipsWindow.textTra;
		windowImage.color = new Color (0, 0, 0, 0);
		textObj.color = new Color (1, 1, 1, 0);
	}

	void Update () {

		if(windowIn == true){
			StartCoroutine(WindowInAnimation());
			windowIn = false;
		}
		if(windowOut == true){
			StartCoroutine(WindowOutAnimation());
			windowOut = false;
		}
	}

	//カード効果詳細ウィンドゥが右からフェード＋スライドイン
	public IEnumerator WindowInAnimation(){
		windowImage.DOFade (0.7f, 0.3f);
		textObj.DOFade (1, 0.3f);
		textTra.DOLocalMove(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutQuart);
		yield return new WaitForSeconds(0.3f);
	}
	
	//カード効果詳細ウィンドゥが右へフェード＋スライドアウト
	public IEnumerator WindowOutAnimation(){
		windowImage.DOFade (0, 0.2f);
		textObj.DOFade (0, 0.2f);
		yield return new WaitForSeconds(0.05f);
		textTra.DOLocalMove(new Vector3(764, 0, 0), 0.3f).SetEase(Ease.OutCubic);
		yield return new WaitForSeconds(0.3f);
	}
	
	public void SetData (TomeObject cardData) {
		textObj.text = (string)cardData["description"][0];
		//textObj.text = cardData["description"][1];
	}
	public void SetData (string description) {
		textObj.text = description;  Debug.Log ("Set Data : Desc -> " + description);
		//textObj.text = cardData["description"][1];
	}
}
