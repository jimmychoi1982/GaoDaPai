using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class ObjectDestruction : MonoBehaviour {

	public GameObject obj;

	//CanvasGroupを該当オブジェクトに設定しておけば、子を含む透明度を一気に操作する
	public CanvasGroup objectCanvasGroup;

	//SE
	public AudioSource SE_Destruction;

	//音を鳴らすか鳴らさないか
	public bool SE_On_Off;

	public bool destruction;

	// Use this for initialization
	void Start () {

		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {
		if (destruction == true) {
			StartCoroutine(cardDestructionAnimation ());
			destruction = false;
		}
	}

	//破棄アニメーション
	//※後のアップデートで垂直に移動ではなく、墓場まで移動するような動きにする予定。
	IEnumerator cardDestructionAnimation(){
		//SEトリガー
		if (SE_On_Off == true) {
			SE_Destruction.Play ();
		} else if (SE_On_Off == false) {
		}
		//オブジェクトの動き
		obj.transform.DOLocalMoveY (50, 0.3f).SetRelative ();
		objectCanvasGroup.DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);	

		Destroy (obj);
	}
}
