using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

using GameView;

public class Card_DrawInCost : MonoBehaviour {

	GameView.GameView gameView { get { return GameView.GameView.Instance; }}
	public UIElement_motherShip motherShip_Player, motherShip_Opp;
	private Image motherShipLight;
	private AudioSource SE_DrawCost;

	public UIElement_DrawMainCard drawmain;
	private GameObject cardBoxObj;
	private Image cardLightImage, cardImage;
	private Transform cardBoxTra, cardLightTra;
	private GameObject change_ef;

	//射出されたコストカードが生成するコストアイコン
	public GameObject costIcon;

	//カードが移動する軌跡
	private Vector3[] path;

	//カードが最終的に行きつく場所（※暫定的、ロジックはやりやすいよう組み替えてください）
	public Vector3 lastPosition;

	//自分のターンか相手のターンか（カードが移動する軌跡が若干異なるので分ける）
	public bool MyTurn;

	//アニメーション再生トリガー
	public bool DrawInCost;

	void Start ()
	{
		change_ef = drawmain.Change_Cost_CardToIcon;
		cardBoxObj = drawmain.cardBoxObj;
		cardBoxTra = drawmain.cardBoxTra;
		cardImage = drawmain.cardImage;
		cardLightImage = drawmain.cardLightImage;
		cardLightTra = drawmain.cardLightTra;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(DrawInCost == true)
		{
			StartCoroutine(DrawInCost_Animation());
			DrawInCost = false;
		}
	}

	//位置リセット
	public void DrawInCost_Reset () {

		cardBoxObj.SetActive(true);
		costIcon.SetActive (false);
		if(MyTurn == true){
			cardBoxTra.localPosition = new Vector3(0, -420, 0);
		}else if(MyTurn == false){
			cardBoxTra.localPosition = new Vector3(0, 420, 0);
		}
		cardBoxTra.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		cardLightTra.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		//lastPosition = new Vector3(costIcon.transform.localPosition.x + costIconBox.localPosition.x, costIcon.transform.localPosition.y + costIconBox.localPosition.y, 0);
		lastPosition = costIcon.transform.position;
	}

	//母艦からカード射出→指定の位置に移動→チェンジエフェクトを再生→アイコン化するアニメーション
	public IEnumerator DrawInCost_Animation ()
	{   
		//Vector2 currentPos = new Vector2( costIcon.transform.localPosition.x + costIconBox.localPosition.x, costIcon.transform.localPosition.y + costIconBox.localPosition.y);
		//Debug.Log (" - - Draw CostCard : Icon Pos -> " + costIcon.transform.position + "  LocalPos -> "+ costIcon.transform.localPosition );
		//Reset
		DrawInCost_Reset();
		yield return new WaitForSeconds(0.1f);

		//母艦点滅
		StartCoroutine (MotherShip_Lighting());
		yield return new WaitForSeconds (0.5f);
		cardLightImage.DOFade (1, 0.5f);
		cardLightTra.DOScale(3f, 0.5f).SetEase(Ease.InExpo);
		yield return new WaitForSeconds (0.5f);

		//母艦からカードが射出される
		if(MyTurn == true){
			//cardBoxTra.DOLocalMove(new Vector3(0, 130f, 0f), 0.8f).SetEase(Ease.OutBack);
			cardBoxTra.DOMove(new Vector3(0, 1.4f, 0f), 0.8f).SetEase(Ease.OutBack);
			cardBoxTra.DOScale(1f, 0.8f).SetEase(Ease.InSine);
			SE_DrawCost = motherShip_Player.SE_CostDraw;
		}else if(MyTurn ==false){
			//cardBoxTra.DOLocalMove(new Vector3(0, 130f, 0f), 0.8f).SetEase(Ease.OutSine);
			cardBoxTra.DOMove(new Vector3(0, 1.4f, 0f), 0.8f).SetEase(Ease.OutSine);
			cardBoxTra.DOScale(1f, 0.8f).SetEase(Ease.OutCubic);
			SE_DrawCost = motherShip_Player.SE_CostDraw;
		}

		SE_DrawCost.Play ();

		//コストカードfadeIn、光fadeOut
		cardImage.DOFade (1, 0.3f);
		cardLightImage.DOFade (0, 0.3f);
		yield return new WaitForSeconds(0.8f);

		//余韻の動き
		//cardBoxTra.DOLocalMove(new Vector3(0, 100f, 0f), 1.4f);
		cardBoxTra.DOMove(new Vector3(0, 1.3f, 0f), 1.4f);
		yield return new WaitForSeconds(1.2f);

		//カードが向かう先の位置情報セット
		if(MyTurn == true){
			
			path = new[] {
				//new Vector3(-229, 190, 0),
				//new Vector3(-445, 124, 0),
				new Vector3(-2.5f, 1f, 0),
				//new Vector3(-5f, -2f, 0),
				lastPosition
			};
		}else if(MyTurn == false){
			path = new[] {
				new Vector3(1.5f, 0.8f),
				lastPosition
			};
		}
		/* 		if(MyTurn == true){
			path = new[] {
				//new Vector3(-229, 190, 0),
				//new Vector3(-445, 124, 0),
				new Vector3(-0.3f * lastPosition.x, 1f, 0),
				new Vector3(-0.5f * lastPosition.x, -2f, 0),
				lastPosition
			};
		}else if(MyTurn == false){
			path = new[] {
				//new Vector3(0.3f * lastPosition.x, 0.3f, 0),
				lastPosition
			};
		}
 */

		//縮まりながら手札付近に移動
		cardBoxTra.DOScale(0.3f, 0.8f).SetEase(Ease.OutQuart);
		//cardBoxTra.DOLocalPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutCubic);
		cardBoxTra.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutCubic);

		yield return new WaitForSeconds(0.6f);

		//Debug.Log (" - - END - Draw CostCard : Icon Pos -> " + costIcon.transform.position + "  LocalPos -> "+ costIcon.transform.localPosition + "  CurrentPos -> " + GetCurrentPosition(costIcon));
		//チェンジエフェクトを再生
		StartCoroutine (ChangeAnimation());
		yield return new WaitForSeconds(0.4f);

		//gameView.isPlayingAnim = false;
	}

	//母艦の光明滅
	public IEnumerator MotherShip_Lighting () {
		if(MyTurn == true){
			motherShipLight = motherShip_Player.motherShip_Light;
		}else if(MyTurn == false){
			motherShipLight = motherShip_Opp.motherShip_Light;
		}
		motherShipLight.DOFade (1, 0.3f);
		yield return new WaitForSeconds (0.3f);
		motherShipLight.DOFade (0, 0.2f);
		yield return new WaitForSeconds (0.2f);
		motherShipLight.DOFade (1, 0.3f);
		yield return new WaitForSeconds (0.3f);
		motherShipLight.DOFade (0, 0.2f);
	}

	//チェンジエフェクト
	public IEnumerator ChangeAnimation () {
		//Reset-----------------------------------------
		cardImage.DOFade (1, 0);
		cardBoxObj.SetActive(true);
		yield return new WaitForSeconds (0.1f);

		change_ef.GetComponent<Change_CardToIcon>().Effect_Play_Limit = true;
		//カードをフェードアウト
		yield return new WaitForSeconds (0.02f);
		cardImage.DOFade (0, 0.2f);
		//アイコン化されたオブジェクトを表示する
		yield return new WaitForSeconds (0.4f);
		costIcon.SetActive (true);
		//カードを非表示にする
		yield return new WaitForSeconds (1f);
		cardBoxObj.SetActive(false);
	}

}
