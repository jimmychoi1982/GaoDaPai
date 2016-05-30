using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class CreditView : MonoBehaviour {

	public GameObject creditViewObj;
	public Transform contents;
	public GameObject tapScreen,tapImage;

	void Awake () {
		new Task (CreditAnimation());
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {
			contents.DOKill();
			contents.localPosition = new Vector3(0, -277, 0);
			tapScreen.GetComponent<CanvasGroup>().alpha = 0;
			creditViewObj.SetActive (false);
		}
	}

	IEnumerator CreditAnimation () {
		yield return new WaitForSeconds (2f);
		contents.DOLocalMoveY (256, 20f).SetEase (Ease.Linear);

		yield return new WaitForSeconds (20f);
		tapScreen.GetComponent<CanvasGroup>().alpha = 1;
		tapImage.GetComponent<TapScreenBlink> ().animeStart = true;
	}
}
