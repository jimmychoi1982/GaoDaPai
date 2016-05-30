using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class StrageCardsView : MonoBehaviour {

	public GameObject  strageView;
	public CanvasGroup cursorCanvas, cardBoxCanvas;
	public GameObject cardBox;
	private Vector3 outPos, viewPos;

	public Image bk;

	public bool strageCardsIn, strageCardsOut;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (strageCardsIn == true) {
			StartCoroutine(strageCardsInAnimation());
			strageCardsIn = false;
		}

		if (strageCardsOut == true) {
			StartCoroutine(strageCardsOutAnimation());
			strageCardsOut = false;
		}

		outPos = new Vector3 (0, -400, 0);//画面外
		viewPos = new Vector3 (0, -206, 0);//表示
	
	}


	IEnumerator strageCardsInAnimation(){
		//Reset
		cardBoxCanvas.alpha = 0;
		strageView.SetActive (false);
		cursorCanvas.alpha = 0;
		cardBox.transform.localPosition = outPos;
		yield return new WaitForSeconds (0.1f);

		bkInAnimation ();

		strageView.SetActive (true);
		cardBoxCanvas.DOFade (1, 0.3f);
		cardBox.transform.DOLocalMove (viewPos, 0.4f).SetEase (Ease.OutQuart);

		yield return new WaitForSeconds (0.1f);
		cursorCanvas.DOFade (1, 0.3f);
	}


	IEnumerator strageCardsOutAnimation(){
		//Reset
		cardBoxCanvas.alpha = 1;
		strageView.SetActive (true);
		cursorCanvas.alpha = 1;
		cardBox.transform.localPosition = viewPos;
		yield return new WaitForSeconds (0.1f);

		cursorCanvas.DOFade (0, 0.3f);
		cardBoxCanvas.DOFade (0, 0.3f);
		cardBox.transform.DOLocalMove (outPos, 0.3f).SetEase (Ease.OutSine);

		yield return new WaitForSeconds (0.3f);
		strageView.SetActive (false);

		bkOutAnimation ();

	}

	//↓共通アニメーションを呼び出すようにしたら以下削除------------------------------------------------------------

	void bkInAnimation(){
		bk.DOFade (0.5f, 0.3f);
	}
	
	void bkOutAnimation(){
		bk.DOFade (0f, 0.4f);
	}

}
