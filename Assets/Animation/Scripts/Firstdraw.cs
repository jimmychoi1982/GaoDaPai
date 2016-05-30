using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Firstdraw : MonoBehaviour
{
	public UIElement_Firstdraw maincards;
	
	private Transform[] Card;//3 or 5
	private Transform CardDisplay;

	public bool DrawStart_Flg;

	public bool CardSet_Flg;
	
	public Transform aimParent;
	
	const float scaleFit = 1f; //1f; 0.3f;
	void Start () 
	{
		//DOTween初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		Card = maincards.cardTra;
		CardDisplay = maincards.cardDisplay;
		for (int i = 0; i < Card.Length; i++)
		{
			Card[i].localScale = scaleFit*Vector3.one;
		}
	}
	

	void Update () 
	{
		//Card_draw_Flgがオンでドローアニメ再生
		if (this.DrawStart_Flg == true) 
		{
			StartCoroutine(DrawStart_Animation());
			this.DrawStart_Flg = false;
		}

		//Card_Set_Flgがオンでカードセットアニメ再生
		if (this.CardSet_Flg == true) 
		{
			StartCoroutine(CardSet_Animation());
			this.CardSet_Flg = false;
		}
	}


	public IEnumerator DrawStart_Animation()
	{
		//カードドロー時のカード初期位置: リセット用
		CardDisplay.localPosition = new Vector3(0, 0, 0);
		for(int i=0; i<Card.Length; i++)
		{
			Card[i].localPosition = new Vector3(0, 1500, 0);
			Card[i].localScale = new Vector3(2.5f, 2.5f, 1)*scaleFit;
			Card[i].eulerAngles = new Vector3(25, 0, -40);
		}

		//カードの枚数分調べて時間差を設定する
		for (int i=0; i<Card.Length; i++) 
		{
			//IEmurator のコルーチン発動引数 ( 配列番号、　下降を何秒待つか)
			StartCoroutine (CardDraw (i, (float)i / 4));
		}

		//枚数によって待ち時間変化
		if(Card.Length == 5){
			yield return new WaitForSeconds (2f);
		}else if(Card.Length ==3){
			yield return new WaitForSeconds (1.5f);
		}

		//引いたカードが横一列に並ぶアニメ（先攻・後攻）
		if (Card.Length == 5){
			Card[0].DOLocalMoveX(-700, 0.35f).SetEase(Ease.OutSine);
			Card[4].DOLocalMoveX(700, 0.35f).SetEase(Ease.OutSine);
			yield return new WaitForSeconds (0.05f);
			Card[1].DOLocalMoveX(-350, 0.3f).SetEase(Ease.OutSine);
			Card[3].DOLocalMoveX(350, 0.3f).SetEase(Ease.OutSine);
		}
		else if (Card.Length == 3){
			Card[0].DOLocalMoveX(-350, 0.2f).SetEase(Ease.OutSine);
			Card[2].DOLocalMoveX(350, 0.2f).SetEase(Ease.OutSine);
		}
	}


	//カードを場にドローするアニメーション関数 引数( 配列番号、　下降を何秒待つか)
	IEnumerator CardDraw(int Index, float What_Wait_Time)
	{
		yield return new WaitForSeconds (What_Wait_Time);

		this.Card [Index].DOLocalMove(new Vector3(0, 100 ,0), 0.6f).SetEase(Ease.OutExpo);
		this.Card [Index].DORotate(new Vector3(0, 0, 0), 0.6f).SetEase(Ease.OutExpo);
	}
	
	public void Disable()
	{
		CardDisplay.gameObject.SetActive(false);
	}

	public IEnumerator CardSet_Animation()
	{
		//カードボックスを右下に移動
		//CardDisplay.DOLocalMove(new Vector3(600, -400, 0), 0.5f);
		CardDisplay.DOLocalMove(aimParent.localPosition, 0.4f);

		// //カードの大きさを1にする
		for(int i=0; i<5; i++){
			this.Card[i].DOScale(1f*scaleFit, 0.3f);
		}
		//カードとカードの間を縮める
		this.Card[0].DOLocalMoveX(-200, 0.4f);
		this.Card[1].DOLocalMoveX(-100, 0.4f);
		this.Card[2].DOLocalMoveX(0, 0.4f);
		this.Card[3].DOLocalMoveX(100, 0.4f);
		this.Card[4].DOLocalMoveX(200, 0.4f);
		yield return new WaitForSeconds (0.4f);

		//カードを扇形に開く（ロジックがあれば組み替えてください）
		for (int i = 0; i < this.Card.Length; i++)
		{
			//playerMainCards[i].transform.RotateAround( PlayersHandObj.transform.position+aroundPoint, -Vector3.forward, AngleForCardDegrees(i, playerMainCards.Count) );
			var worldPos = CoordinatesForCard(i, Card.Length)+aimParent.position;
			var localPos = CardDisplay.InverseTransformPoint(worldPos);
//			Debug.Log("LOCAL POS1 "+i+" "+localPos.ToString()+" "+CardDisplay.localPosition.ToString());
			var angle = AngleForCardDegrees(i, Card.Length);
			Card[i].DOLocalRotate(new Vector3(0, 0, -angle), 0.4f);
			Card[i].DOLocalMove(localPos + new Vector3(0,100f,0), 0.4f);
			
		}
//		this.Card[0].DOLocalRotate(new Vector3(0, 0, 13), 0.4f);
//		this.Card[0].DOLocalMove(new Vector3(-200, -30, 0), 0.4f);
//		this.Card[1].DOLocalRotate(new Vector3(0, 0, 7), 0.4f);
//		this.Card[1].DOLocalMove(new Vector3(-100, -7, 0), 0.4f);
//		this.Card[2].DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
//		this.Card[2].DOLocalMove(new Vector3(0, 0, 0), 0.4f);
//		this.Card[3].DOLocalRotate(new Vector3(0, 0, -7), 0.4f);
//		this.Card[3].DOLocalMove(new Vector3(100, -7, 0), 0.4f);
//		this.Card[4].DOLocalRotate(new Vector3(0, 0, -13), 0.4f);
//		this.Card[4].DOLocalMove(new Vector3(200, -30, 0), 0.4f);

		yield return new WaitForSeconds (0.8f);

		//手札を少し降ろしてセット完了
		//CardDisplay.DOLocalMoveY(-80, 0.5f).SetRelative();
		//CardDisplay.DOLocalMoveX(-18, 0.5f).SetRelative();
		for (int i = 0; i < this.Card.Length; i++)
		{
			var worldPos = CoordinatesForCard(i, Card.Length)+aimParent.position;
			var localPos = CardDisplay.InverseTransformPoint(worldPos);
//			Debug.Log("LOCAL POS2 "+i+" "+localPos.ToString()+" "+CardDisplay.localPosition.ToString());
//			//playerMainCards[i].transform.RotateAround( PlayersHandObj.transform.position+aroundPoint, -Vector3.forward, AngleForCardDegrees(i, playerMainCards.Count) );
			Card[i].DOLocalMove(localPos, 0.4f);
//			Card[i].DOLocalMove(new Vector3(0,-100f,0), 0.4f).SetRelative();
		}
		yield return new WaitForSeconds (0.5f);
	}
	
	float fanAngle = 12f;
	float aroundRadius = 3f;
	
	float AngleForCardDegrees(int index, int total)
	{
		var rotateAngle = fanAngle / total;
		return ((index - (total - 1)*0.5f)*rotateAngle);
	}
	
	Vector3 CoordinatesForCard(int index, int total)
	{	
		var angle = AngleForCardDegrees(index, total);
		var x = Mathf.Sin(angle/180*Mathf.PI)*aroundRadius;
		var y = (Mathf.Cos(angle/180*Mathf.PI)-1)*aroundRadius;
		var z = (float)index/total;
		return new Vector3(x,y,z);
	}



}
