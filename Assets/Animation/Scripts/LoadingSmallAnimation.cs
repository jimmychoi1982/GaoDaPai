using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween

public class LoadingSmallAnimation : MonoBehaviour {

	public GameObject loadingEffect;

	// Use this for initialization
	void Start () {
		loadingRound ();
	
	}
	
	void loadingRound(){
		loadingEffect.transform.DORotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);

	}
}
