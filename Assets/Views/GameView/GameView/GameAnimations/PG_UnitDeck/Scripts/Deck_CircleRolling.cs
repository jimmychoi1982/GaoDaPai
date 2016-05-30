using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Deck_CircleRolling : MonoBehaviour {

	public Transform RollObject;

	// Use this for initialization
	void Start () {
	
		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);
		CircleRolling_Animation ();
	}

	//deck_circleが回るアニメーション
	void CircleRolling_Animation () {
		RollObject.DORotate(new Vector3(0, 0, 360), 7f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
	}
}
