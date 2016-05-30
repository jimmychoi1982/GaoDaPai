using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Card_DrawInUnit : MonoBehaviour {

	public Transform[] Card;
	public Transform screenCenteredObj;
	private Vector3[] path;
	private Vector3 nowPos;

	//自分のターン
	public bool MyTurn;

	public int cardCount;

	//暫定的なパラメータ(扇形ロジックに合わせて変更してください)
	public Vector3 LastPos;//手札内での最終的なカードの位置
	public float Angle;//手札内での最終的なカードの角度
	public float lastScale; 

	public bool Anime_Start;
	public Transform dummy;

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	
	// Update is called once per frame
	void Update ()
	{
		if(Anime_Start == true)
		{
			for(int i=0; i<cardCount; i++)
			{
				StartCoroutine(DrawInSet(i, LastPos, Angle));
			}
			Anime_Start = false;
		}

	}

	//======================ユニットカードを一枚ドローし、手札に収納======================

	public IEnumerator DrawInSet(int Index, Vector3 _lastPos, float _Angle){ 
		Debug.Log (" - - - - Draw Start");
		var cardParent = Card[Index].transform.parent;
		if (screenCenteredObj != null) Card[Index].transform.SetParent(screenCenteredObj, true);
		
		//カードの最初の位置を指定
		Card[Index].gameObject.SetActive(true);
		Card [Index].position = new Vector3 (0f, 800f, 0f);  Debug.Log (" - - - Local Pos : "+ Card [Index].localPosition);
		Card[Index].eulerAngles = new Vector3(0, 0, 0);
		Card[Index].localScale = new Vector3(0.6f, 0.6f, 0.6f);
		Card_xPosition();

		//カードを画面にドロー
		StartCoroutine (DrawInUnit(Card[Index], (float)Index / 5));
		yield return new WaitForSeconds(1.5f);


		//縮まりながら手札付近に移動
		//※手札の扇形のポジションに_lastpos,_Angleを繋ぎこんでください。
		if(MyTurn == true){
			path = new[] {
				new Vector3(434, 236, 0),
				new Vector3(700, -243, 0),
				(_lastPos+new Vector3(0,100,0))
			};
		}else if(MyTurn == false){
			path = new[] {
				//new Vector3(-200, 0, 0),
				new Vector3(-227, 100, 0),
				(_lastPos+new Vector3(0,100,0))
			};
		}
		Card[Index].DOLocalPath(path, 1f, PathType.CatmullRom).SetEase(Ease.OutSine);
		Card[Index].DOScale(lastScale, 0.4f);
		Card[Index].DORotate(new Vector3(0, 0, _Angle), 0.3f).SetRelative().SetEase(Ease.InSine);
	
		yield return new WaitForSeconds(1f);

		//スッと奥に入る
		if(MyTurn == true){
			Card[Index].DOLocalMove(new Vector3(0, -100f, 0), 0.3f).SetRelative();
		}else if(MyTurn == false){
			Card[Index].DOLocalMove(new Vector3(0, 100f, 0), 0.3f).SetRelative();
		}

		//Card[Index].transform.SetParent( cardParent, true );
	}

	public Vector3 GetAnimLocalPosition ( Vector3 worldPos) {
		dummy.position = worldPos;
		return dummy.localPosition;
	}

	//カードが画面上からドローされる動き
	public IEnumerator DrawInUnit(Transform Card, float What_Wait_Time)
	{
		yield return new WaitForSeconds (What_Wait_Time);

		Card.DOLocalMoveY(150, 1.5f).SetEase(Ease.OutExpo);
	}


	//画面に表示されるドローされたカードの位置、ドローされた枚数によって設定する
	public void Card_xPosition()
	{
		if(cardCount == 5){
			Card[0].localPosition = new Vector3(-620, 1900, 0);
			Card[1].localPosition = new Vector3(-310, 1900, 0);
			Card[2].localPosition = new Vector3(0, 900, 0);
			Card[3].localPosition = new Vector3(310, 1900, 0);
			Card[4].localPosition = new Vector3(620, 1900, 0);
		}else if(cardCount == 4){
			Card[0].localPosition = new Vector3(-465, 1900, 0);
			Card[1].localPosition = new Vector3(-155, 1900, 0);
			Card[2].localPosition = new Vector3(155, 1900, 0);
			Card[3].localPosition = new Vector3(465, 1900, 0);
		}else if(cardCount == 3){
			Card[0].localPosition = new Vector3(-310, 1900, 0);
			Card[1].localPosition = new Vector3(0, 1900, 0);
			Card[2].localPosition = new Vector3(310, 1900, 0);
		}else if(cardCount == 2){
			Card[0].localPosition = new Vector3(-155, 1900, 0);
			Card[1].localPosition = new Vector3(155, 1900, 0);
		}else if(cardCount == 1){
			Card[0].localPosition = new Vector3(0, 900, 0);
		}
	}
	

}
