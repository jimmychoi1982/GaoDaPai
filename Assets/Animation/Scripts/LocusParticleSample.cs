using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LocusParticleSample : MonoBehaviour {

	public Transform Card;
	private Vector3 pos;
	public bool MoveY;
	public bool MoveX;
	public bool MoveRotate;

	// Use this for initialization
	void Start () {

		DOTween.Init(true, false, LogBehaviour.Default);
		pos = Card.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (MoveY == true){
			MoveY_Animation();
			MoveY = false;
		}

		if (MoveX == true){
			MoveX_Animation();
			MoveX = false;
		}
	
		if (MoveRotate == true){
			MoveRotate_Animation();
			MoveRotate = false;
		}
	}


	//カード縦移動
	void MoveY_Animation () {
		Card.transform.localPosition = pos;
		Card.DORotate(new Vector3(0,0,0),0);

		Card.DOLocalMoveY(500f, 2f).SetEase(Ease.OutExpo);
	}

	//カード横移動
	void MoveX_Animation () {
		Card.transform.localPosition = pos;
		Card.DORotate(new Vector3(0,0,0),0);
		
		Card.DOLocalMoveX(300f, 2f).SetEase(Ease.OutExpo);
	}

	//カード回転移動
	void MoveRotate_Animation () {
		Card.transform.localPosition = pos;
		Card.DORotate(new Vector3(0,0,0),0);

		Card.DOLocalMoveX(300f, 1f).SetEase(Ease.OutExpo);
		Card.DORotate(new Vector3(0,5,25),2f);
	}

}
