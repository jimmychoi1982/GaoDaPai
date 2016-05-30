using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardTipsWindow : MonoBehaviour {

	public GameObject tipsWindow;
	
	public bool windowIn, windowOut;

	void Update () {

		//Demo In Start
		if(windowIn == true){
			WindowInAnimation();
			windowIn = false;
		}
		//Demo Out Start
		if(windowOut == true){
			WindowOutAnimation();
			windowOut = false;
		}
	}

	//カード効果詳細ウィンドゥが右からフェード＋スライドイン
	public void WindowInAnimation(){
		tipsWindow.transform.DOLocalMove(new Vector3(575, 415, 0), 0.3f).SetEase (Ease.OutCubic);
		tipsWindow.GetComponent<CanvasGroup>().DOFade (1, 0.3f);
	}
	
	//カード効果詳細ウィンドゥが右へフェード＋スライドアウト
	public void WindowOutAnimation(){
		tipsWindow.transform.DOLocalMove(new Vector3(1200, 415, 0), 0.3f).SetEase (Ease.InQuad);
		tipsWindow.GetComponent<CanvasGroup>().DOFade (0, 0.3f);
	}
}
