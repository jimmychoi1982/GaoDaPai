using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CostCards_Scale : MonoBehaviour
{
	public UIElement_CostCards costCards;
	private Transform CostCardsTra;
	private AudioSource SE_Scale_Up;
	private Vector3 zoomInPos,zoomOutPos;

	//MainCardsを拡大縮小させるフラグ
	public bool CostCards_Scale_In;
	public bool CostCards_Scale_Out;

	void Start () 
	{
		//DOTween初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		CostCardsTra = costCards.costCardsTra;
		SE_Scale_Up = costCards.SE_Scale_Up;
		zoomOutPos = new Vector3(-490, -416, 0);//CostCards_Playerの縮小時の位置を記述
		zoomInPos = new Vector3(-378, -180, 0);//CostCards_Playerの拡大時の位置を記述
	}

	void Update () 
	{
		if (this.CostCards_Scale_In == true) 
		{
			StartCoroutine(CostCards_Scale_In_Animation());
			this.CostCards_Scale_In = false;
		}
		if (this.CostCards_Scale_Out == true) 
		{
			StartCoroutine(CostCards_Scale_Out_Animation());
			this.CostCards_Scale_Out = false;
		}

	}

	//CostCardsを拡大させるアニメ
	IEnumerator CostCards_Scale_In_Animation()
	{
		// Reset
		CostCardsTra.localPosition = zoomOutPos;
		CostCardsTra.localScale = new Vector3(1, 1, 1);
		yield return new WaitForSeconds (0.01f);

		SE_Scale_Up.Play ();

		//縮小
		CostCardsTra.DOLocalMove(zoomInPos, 0.5f);
		CostCardsTra.DOScale(1.5f, 0.5f);
	}

	//CostCardsを縮小させるアニメ
	IEnumerator CostCards_Scale_Out_Animation()
	{
		// Reset
		CostCardsTra.localPosition = zoomInPos;
		CostCardsTra.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		yield return new WaitForSeconds (0.01f);

		//縮小
		CostCardsTra.DOLocalMove(zoomOutPos, 0.5f);
		CostCardsTra.DOScale(1, 0.5f);
	}

}
