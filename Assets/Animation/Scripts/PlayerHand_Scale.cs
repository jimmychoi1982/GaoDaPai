using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerHand_Scale : MonoBehaviour
{
	public UIElement_PlayerHand playerHand;
	private Transform playerHandTra;
	private AudioSource SE_Scale_Up;
	private Vector3 zoomOutPos;

	//MainCardsを拡大縮小させるフラグ
	public bool MainCards_Scale_In;
	public bool MainCards_Scale_Out;

	//拡大されたMainCardsを退場させるフラグ
	public bool MainCards_Move_Out;

	void Start () 
	{
		//DOTween初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		playerHandTra = playerHand.playerHandTra;
		SE_Scale_Up = playerHand.SE_Scale_Up;
		zoomOutPos = new Vector3(600, -500, 0);//PlayerHandの縮小時の位置を記述
	}

	void Update () 
	{
		if (this.MainCards_Scale_In == true) 
		{
			StartCoroutine(MainCards_Scale_In_Animation());
			this.MainCards_Scale_In = false;
		}
		if (this.MainCards_Scale_Out == true) 
		{
			StartCoroutine(MainCards_Scale_Out_Animation());
			this.MainCards_Scale_Out = false;
		}
		if (this.MainCards_Move_Out == true) 
		{
			StartCoroutine(MainCards_Move_Out_Animation());
			this.MainCards_Move_Out = false;
		}

	}
	

	//手札を拡大させるアニメ
	IEnumerator MainCards_Scale_In_Animation()
	{
		// Reset
		playerHandTra.localPosition = zoomOutPos;
		playerHandTra.localScale = new Vector3(1, 1, 1);
		yield return new WaitForSeconds (0.01f);

		SE_Scale_Up.Play ();

		//拡大
		playerHandTra.DOLocalMove(new Vector3(0, -400, 0), 0.5f);
		playerHandTra.DOScale(2, 0.5f);
		yield return new WaitForSeconds (0.5f);
	}

	//手札を縮小させるアニメ
	IEnumerator MainCards_Scale_Out_Animation()
	{
		// Reset
		playerHandTra.localPosition = new Vector3(0, -400, 0);
		playerHandTra.localScale = new Vector3(2, 2, 2);
		yield return new WaitForSeconds (0.01f);

		//縮小
		playerHandTra.DOLocalMove(zoomOutPos, 0.5f);
		playerHandTra.DOScale(1, 0.5f);
		yield return new WaitForSeconds (0.5f);
	}

	//手札を退場させるアニメ
	IEnumerator MainCards_Move_Out_Animation()
	{
		// Reset
		playerHandTra.localPosition = new Vector3(0, -400, 0);
		playerHandTra.localScale = new Vector3(2, 2, 2);
		yield return new WaitForSeconds (0.01f);

		//真下に引く
		playerHandTra.DOLocalMove(new Vector3(0, -1000, 0), 0.4f).SetEase(Ease.InQuad);
		yield return new WaitForSeconds (0.4f);
		//大きさ縮小して、基準位置の下から出てくる
		playerHandTra.localScale = new Vector3(1, 1, 1);
		playerHandTra.localPosition = new Vector3(600, -1000, 0);
		yield return new WaitForSeconds (0.2f);
		playerHandTra.DOLocalMove(zoomOutPos, 0.5f).SetEase(Ease.OutExpo);
		yield return new WaitForSeconds (0.5f);
	}


}
