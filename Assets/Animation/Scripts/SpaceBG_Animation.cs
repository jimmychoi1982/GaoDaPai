using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class SpaceBG_Animation : MonoBehaviour {

	public Image bg_blue;
	
	// Use this for initialization
	void Start () {

		SpaceBackGroundAnimation ();
	
	}

	//宇宙背景アニメ
	void SpaceBackGroundAnimation(){
		bg_blue.DOFade (0.3f, 30).SetLoops (-1, LoopType.Yoyo);
	}

	//隕石浮遊アニメ
//	void MeteoAnimation(){
//		meteo1.DORotate (new Vector3(0,0,360), 120f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Yoyo).SetEase (Ease.Linear);
//		meteo2.DORotate (new Vector3(0,0,-360), 120f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Yoyo).SetEase (Ease.Linear);
//		meteo3.DORotate (new Vector3(0,0,360), 100f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Yoyo).SetEase (Ease.Linear);
//		meteo4.DORotate (new Vector3(0,0,-360), 200f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Yoyo).SetEase (Ease.Linear);
//		meteo5.DORotate (new Vector3(0,0,360), 180f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Yoyo).SetEase (Ease.Linear);
//	}


}
