using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class TimeOutAnimation : MonoBehaviour {

	public Transform timeOutBox;
	public Image timeOutText;
	public AudioSource SE_timeOut;
	
//	public bool TimeOut_Start;
	
	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
/*	void Update () {
		
		if (TimeOut_Start == true) {
			StartCoroutine (TimeOut_Animation_Start ());
			TimeOut_Start = false;
		}
	}
*/	
	public IEnumerator TimeOut_Animation_Start(){
		//初期化
		timeOutText.color = new Color (1, 1, 1, 0);
		timeOutBox.localPosition = new Vector3 (-1700, 0, 0);
		timeOutBox.localScale = new Vector3 (1, 0, 1);
		yield return new WaitForSeconds (0.1f);

		SE_timeOut.Play ();
		
		timeOutBox.DOLocalMove (new Vector3 (0, 0, 0), 0.3f).SetEase (Ease.OutExpo);
		timeOutBox.DOScale (new Vector3 (1, 1, 1), 0.3f);
		timeOutText.DOFade (1, 0.3f);
		yield return new WaitForSeconds (1f);
		
		timeOutBox.DOLocalMove (new Vector3 (1000, 0, 0), 0.3f).SetEase (Ease.InQuart);
		timeOutText.DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);
		
	}
	
}