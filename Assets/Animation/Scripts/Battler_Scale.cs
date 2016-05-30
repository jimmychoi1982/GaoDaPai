using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Battler_Scale : MonoBehaviour {

	public UIElement_BattlerIcon battler;
	private Transform battlerTra;

	public bool DragScale_In, DragScale_Out;

	// Use this for initialization
	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {

		if(DragScale_In == true){
			DragScale_In_Animation();
			DragScale_In = false;
		}

		if(DragScale_Out == true){
			DragScale_Out_Animation();
			DragScale_Out = false;
		}
	}

	void Reset(){
		battlerTra = battler.battlerTra;
	}

	//アイコンタップで少し大きくなるアニメ
	void DragScale_In_Animation () {
		Reset ();
		battlerTra.localScale = new Vector3(1,1,1);
		battlerTra.DOScale(0.1f, 0.2f).SetRelative();
	}


	//指を離すともとの大きさに戻るアニメ
	void DragScale_Out_Animation () {
		Reset ();
		battlerTra.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		battlerTra.DOScale(-0.1f, 0.2f).SetRelative().SetEase(Ease.OutBack);
	}

}
