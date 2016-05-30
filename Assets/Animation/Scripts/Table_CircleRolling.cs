using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Table_CircleRolling : MonoBehaviour {

	public Transform circle1, circle2;

	void Start () {

		circle1.DORotate(new Vector3(0, 0, -360), 10f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		circle2.DORotate(new Vector3(0, 0, 360), 7f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);

	}

	void Update () {
		
	}

	//DOTweenのLoopを解除してStop
	void RollingStop_Animation (Transform circle1, Transform circle2){
		circle1.DOPause();
		circle2.DOPause();
	}
}
