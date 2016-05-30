using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CounterCard_Animation : MonoBehaviour {
	
	public UIElement_launcher launcher;
	private GameObject launcherObj, cardObj;
	private Transform cardTra;

	public UIElement_Table tableOpponent, tablePlayer;
	private GameObject counterNumber;
	private Transform coverTop, coverBot;
	private Image counterIconImage;
	private GameObject EF_Light_Bomb, EF_CounterCardInvoke;
	private AudioSource SE_CoverOpen;

	public UIElement_InvokeCard invokeCard;
	private GameObject invCardObj;
	private Transform invCardTra;
	private Image invCardImage;

	private Vector3[] pathSet, pathInvoke;

	public bool MyTurn;

	public int cardCountPlayer, cardCountOpp;//0,1,2,3,4,5（一応、上限5枚にしてある）

	public bool CounterCard_Set, CounterCard_Invoke;

	// Use this for initialization
	void Start () {


		launcherObj = launcher.launcher;
		cardObj = launcher.cardSetObj;
		cardTra = launcher.cardSet;
		invCardObj = invokeCard.invCardObj;
		invCardTra = invokeCard.invCardTra;
		invCardImage = invokeCard.invCardImage;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(CounterCard_Set == true){
			StartCoroutine(Set_Animation());
			CounterCard_Set = false;
		}

		if(CounterCard_Invoke == true){
			StartCoroutine(Counter_Invoke_Animation());
			CounterCard_Invoke = false;
		}

	
	}


//Animation===================================================================

	void TurnReset(){
		if(MyTurn == true){
			counterIconImage = tablePlayer.counterIconImage;
			EF_Light_Bomb = tablePlayer.EF_Light_Bomb;
			EF_CounterCardInvoke = tablePlayer.EF_CounterCardInvoke;
			SE_CoverOpen = tablePlayer.SE_CounterCover;
			coverTop = tablePlayer.coverTop;
			coverBot = tablePlayer.coverBot;
			counterNumber = tablePlayer.counterNumber;
		}else if(MyTurn == false){
			counterIconImage = tableOpponent.counterIconImage;
			EF_Light_Bomb = tableOpponent.EF_Light_Bomb;
			EF_CounterCardInvoke = tableOpponent.EF_CounterCardInvoke;
			SE_CoverOpen = tableOpponent.SE_CounterCover;
			coverTop = tableOpponent.coverTop;
			coverBot = tableOpponent.coverBot;
			counterNumber = tableOpponent.counterNumber;
		}
	}

	//カウンターカードをエリアにセット
	IEnumerator Set_Animation () {

		//Reset
		TurnReset ();
		launcherObj.SetActive(true);
		cardObj.SetActive(true);
		cardTra.localPosition = new Vector3(0,258,0);
		cardTra.localScale = new Vector3(1f, 1f, 1f);
		yield return new WaitForSeconds(0.01f);

		//Launcherの位置からカウンターエリアまでカードが縮小しながら移動
		if(MyTurn == true){
			pathSet = new[] {
				new Vector3(300, 52, 0),
				new Vector3(182, -376, 0)
			};
			cardTra.DOLocalPath(pathSet, 1f, PathType.CatmullRom).SetEase(Ease.OutQuart);
			if(cardCountPlayer == 0){//エリアに0枚なら、カバーを開く
				Counter_Cover_Open();
			}
		}else if(MyTurn == false){
			pathSet = new[] {
				new Vector3(-50, 0, 0),
				new Vector3(-200, 361, 0)
			};
			cardTra.DOLocalPath(pathSet, 1f, PathType.CatmullRom).SetEase(Ease.OutQuart);
			if(cardCountOpp == 0){//エリアに0枚なら、カバーを開く
				Counter_Cover_Open();
			}
		}
		cardTra.DOScale(0.2f, 1.2f).SetEase(Ease.OutQuart);

		//エフェクト再生
		yield return new WaitForSeconds(0.3f);
		EF_Light_Bomb.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//吸い込まれたカードは消す
		yield return new WaitForSeconds(0.2f);
		cardObj.SetActive(false);

		//エリアに0枚のときはフェードインでカウンターアイコン画像フェードイン
		//残り0枚でないときはカウンターアイコン画像を変更する
		if (MyTurn == true) {
			if (cardCountPlayer == 0) {
				counterIconImage.DOFade (1, 0.5f);
			}else if (cardCountPlayer != 0){//※暫定
				counterIconImage.sprite = Resources.Load("Reletion_Card_Atlas/Counter_Icon/ui_c_BT01-104", typeof(Sprite)) as Sprite;
			}
		}else if(MyTurn == false){
			if(cardCountOpp == 0){
				counterIconImage.DOFade (1, 0.5f);
			}else if(cardCountOpp != 0){//※暫定
				counterIconImage.sprite = Resources.Load("Reletion_Card_Atlas/Counter_Icon/ui_c_BT01-104", typeof(Sprite)) as Sprite;
			}
		}

		//カウンターエリアの数字を増やしていく
		if(MyTurn == true){
			cardCountPlayer++;
		}else if(MyTurn == false){
			cardCountOpp++;
		}

		yield return new WaitForSeconds(0.5f);

		//カウンターエリアの数字変化アニメーション
		if(MyTurn == true){
			CardNumber_Change_Animation(cardCountPlayer);
		}else if(MyTurn == false){
			CardNumber_Change_Animation(cardCountOpp);
		}

	}

	//カウンターカード発動
	IEnumerator Counter_Invoke_Animation () {

		//Reset
		TurnReset ();
		if(MyTurn == true){
			invCardTra.localPosition = new Vector3(182, -360, 0);
			cardCountPlayer--;
		}else if(MyTurn == false){
			invCardTra.localPosition = new Vector3(-182, 360, 0);
			cardCountOpp--;
		}
		invCardTra.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		invCardImage.color = new Color (1, 1, 1, 0);

		//発動エフェクト
		EF_CounterCardInvoke.GetComponent<Effect_Play_Limit> ().Effect_Start = true;
		yield return new WaitForSeconds(1f);

		//カウンターエリアの数字変化アニメーション
		if(MyTurn == true){
			CardNumber_Change_Animation(cardCountPlayer);
		}else if(MyTurn == false){
			CardNumber_Change_Animation(cardCountOpp);
		}

		//エリアから出ていくカードのアニメーション
		invCardObj.SetActive(true);
		invCardImage.DOFade (1, 0.3f);
		if(MyTurn == true){
			pathInvoke = new[] {
				new Vector3(-144, -116, 0),
				new Vector3(-790, 0, 0)
			};
		}else if(MyTurn == false){
			pathInvoke = new[] {
				new Vector3(-144, 116, 0),
				new Vector3(-790, 0, 0)
			};
		}
		invCardTra.DOLocalPath(pathInvoke, 0.7f, PathType.CatmullRom).SetEase(Ease.OutCubic);
		invCardTra.DOScale(0.6f, 0.7f).SetEase(Ease.OutQuad);

		//残り0枚になったときはカードの画像のアルファを透明にして、カバーを閉じる
		//残り0枚でないときは裏のカード画像を変更する
		if (MyTurn == true) {
			if (cardCountPlayer == 0) {
				counterIconImage.DOFade (0, 0.3f);
				yield return new WaitForSeconds (0.5f);
				Counter_Cover_Close ();
			}else if (cardCountPlayer != 0){//※暫定
				counterIconImage.sprite = Resources.Load("Reletion_Card_Atlas/Counter_Icon/ui_c_BT01-104", typeof(Sprite)) as Sprite;
			}
		}else if(MyTurn == false){
			if(cardCountOpp == 0){
				counterIconImage.DOFade (0, 0.3f);
				yield return new WaitForSeconds(0.5f);
				Counter_Cover_Close();
			}else if(cardCountOpp != 0){//※暫定
				counterIconImage.sprite = Resources.Load("Reletion_Card_Atlas/Counter_Icon/ui_c_BT01-104", typeof(Sprite)) as Sprite;
			}
		}
	}

//Player,Enemy共通------------------------------------------------------------------------------

	//カウンターカバーが開く
	void Counter_Cover_Open () {
		TurnReset ();
		SE_CoverOpen.Play ();
		coverTop.DOLocalMoveX(-150, 0.5f).SetRelative();
		coverBot.DOLocalMoveX(150, 0.5f).SetRelative();
	}

	//カウンターカバーが閉じる
	void Counter_Cover_Close () {
		TurnReset ();
		SE_CoverOpen.Play ();
		coverTop.DOLocalMoveX(150, 0.5f).SetRelative();
		coverBot.DOLocalMoveX(-150, 0.5f).SetRelative();
	}

	//カウンターエリアのカード枚数が変わったときの拡縮アニメーション
	void CardNumber_Change_Animation (int num) {
		TurnReset ();

		//数字画像を変更する
		Text numText = counterNumber.GetComponent<Text> ();
		numText.text = "x"+num;

		//数字の拡大縮小アニメーション
		Transform numTra = counterNumber.GetComponent<Transform>();
		numTra.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f, 1, 1);
	
	}

}
