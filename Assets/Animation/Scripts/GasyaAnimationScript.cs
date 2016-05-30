using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class GasyaAnimationScript : MonoBehaviour {

	public UIElement_GasyaAnimation gasya;
	private GameObject doorBox;
	private RectTransform doorTop, doorBot;
	private Transform gasyaCamera,sukima, doorLightWh, doorLightGr, gasyaRoom, roomLight, cardBox;
	private GameObject burstBox, resultBox, get;
	private Image bk, sukimaImage, doorLightWhImage, doorLightGrImage;
	private GameObject wh;
	private ParticleSystem holoPar, kiraPar, kira2, kira3;
	private AudioSource jingle, seHolo,seDoor;

	public UIElement_GetCard getCard;
	private Transform card;
	private ParticleSystem rareEffect;
	private GameObject newLabel;
	private Transform cardCopy;
	private Image cardImage, cardLightImage, cardCopyImage;

	public bool RareCardFlg;
	public bool NewCardFlg;
	public bool Gasya_Start, Next_Start, Appeal_Start, Result_Start, Reset_Start;
	
	void Start () {

		gasyaCamera = gasya.gasyaCamera;
		doorBox = gasya.doorBox;
		doorTop = gasya.doorTop;
		doorBot = gasya.doorBot;
		sukima = gasya.sukima;
		sukimaImage = gasya.sukimaImage;
		doorLightWh = gasya.doorLightWh;
		doorLightGr = gasya.doorLightGr;
		doorLightWhImage = gasya.doorLightWhImage;
		doorLightGrImage = gasya.doorLightGrImage;

		holoPar = gasya.holoPar;
		kiraPar = gasya.kiraPar;
		gasyaRoom = gasya.gasyaRoom;
		roomLight = gasya.roomLight;
		burstBox = gasya.burstBox;
		cardBox = gasya.cardBox;

		bk = gasya.bk;
		wh = gasya.wh;
		resultBox = gasya.resultBox;
		kira2 = gasya.kira2;
		kira3 = gasya.kira3;
		get = gasya.get;
		jingle = gasya.jingle;
		seHolo = gasya.seHolo;
		seDoor = gasya.seDoor;

	}

	void Update () {

		if (Gasya_Start == true) {
			StartCoroutine (GasyaAnimation());
			Gasya_Start = false;
		}

		if (Next_Start == true) {
			StartCoroutine (Next_Animation());
			Next_Start = false;
		}

		if (Appeal_Start == true) {
			StartCoroutine (CardAppealAnimation());
			Appeal_Start = false;
		}

		if (Result_Start == true) {
			StartCoroutine (Result_Animation());
			Result_Start = false;
		}

		if (Reset_Start == true) {
			Reset();
			Reset_Start = false;
		}
	
	}

//Reset---------------------------------------------------------------------------------------------------

	//要素リセット
	void getCardReset(){
		rareEffect = getCard.RareLight;
		newLabel = getCard.newLabel;
		card = getCard.card;
		cardCopy = getCard.cardCopy;
		cardImage = getCard.cardImage;
		cardLightImage = getCard.cardLightImage;
		cardCopyImage = getCard.cardCopyImage;
		card.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
		cardCopy.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
	}

	//リセット
	void Reset(){
		newLabel.SetActive (false);
		wh.SetActive (true);
		doorBox.SetActive (true);
		doorTop.localPosition = new Vector3 (0, 49.59986f, 0);
		doorBot.localPosition = new Vector3 (0, -166.5999f, 0);
		get.SetActive (true);
		resultBox.SetActive (false);
		kira2.Stop ();
		kira3.Stop ();
		gasyaRoom.localScale = new Vector3 (1, 1, 1);
		burstBox.GetComponent<CanvasGroup>().alpha = 0;
		cardImage.color = new Color (1, 1, 1, 0);
		cardLightImage.color = new Color (1, 1, 1, 0);
		bk.color = new Color (1, 1, 1, 0.8f);
		cardCopyImage.color = new Color (1, 1, 1, 0);
		cardBox.localPosition = new Vector3 (0, -150, 0);
		rareEffect.Stop ();
	}
	
//Gasya_Animation----------------------------------------------------------------------------------------
	
	//ゲットしたカードが現れる
	IEnumerator GasyaAnimation(){
		getCardReset ();
		Reset ();

		yield return new WaitForSeconds(0.1f);

		StartCoroutine (doorOpenAnimation ());
		yield return new WaitForSeconds(0.4f);
		seDoor.Play ();
		yield return new WaitForSeconds(1f);
	
		//カードが画面に出る
		holoPar.Play ();
		StartCoroutine(CardInAnimation ());
		StartCoroutine (GasyaCameraZoom ());
		yield return new WaitForSeconds(1.4f);

		//カードをフェードイン
		cardImage.DOFade (1, 1.2f);
		cardCopyImage.DOFade (0.5f, 1.2f);
		yield return new WaitForSeconds(1.2f);

		//カメラぐっと寄ってカードをアピール
		gasyaCamera.DOScale (1f, 0.2f).SetEase (Ease.OutQuad);
		jingle.Play ();
		StartCoroutine (CardAppealAnimation ());

		//黒みがフェードアウト
		bk.DOFade (0, 0.3f);
	}

	//ドアが開くアニメ、部屋の中のアニメ
	IEnumerator doorOpenAnimation(){
		StartCoroutine (sukimaLightAnimation ());

		yield return new WaitForSeconds(0.2f);
		//はじめ、少しだけ開く
		doorTop.DOLocalMoveY (5, 0.5f).SetRelative().SetEase (Ease.Linear);
		doorBot.DOLocalMoveY (-5, 0.5f).SetRelative().SetEase (Ease.Linear);
		StartCoroutine (sukimaLightAnimation ());
		StartCoroutine (doorLightAnimation ());
		yield return new WaitForSeconds(0.5f);
		//ゆれ
		doorBox.transform.DOShakePosition (2f, new Vector3(0,10,0), 50, 90);
		//もう少し開く
		doorTop.DOLocalMoveY (30, 0.3f).SetRelative().SetEase (Ease.Linear);
		doorBot.DOLocalMoveY (-30, 0.3f).SetRelative().SetEase (Ease.Linear);
		yield return new WaitForSeconds(0.3f);
		//ガッと開く
		doorTop.DOLocalMoveY (1030, 0.5f).SetRelative().SetEase (Ease.InExpo);
		doorBot.DOLocalMoveY (-530, 0.5f).SetRelative().SetEase (Ease.InExpo);
		yield return new WaitForSeconds(0.2f);
		//ガチャ部屋にズームする
		gasyaRoom.DOScale (1.1f, 1f).SetEase (Ease.OutCubic);
		//部屋の光が伸縮する
		roomLight.DOScaleX (3f , 3f).SetEase (Ease.OutSine).SetLoops (-1, LoopType.Yoyo);
		yield return new WaitForSeconds(3f);
		doorBox.SetActive (false);
	}

	IEnumerator sukimaLightAnimation(){
		sukimaImage.color = new Color (1, 1, 1, 0);
		sukima.transform.localScale = new Vector3 (0, 0.3f, 1);
		yield return new WaitForSeconds(0.2f);
		sukimaImage.DOFade (1, 0.2f);
		sukima.DOScale (new Vector3(40f, 1f, 1), 1f);
		yield return new WaitForSeconds(0.4f);
		sukimaImage.DOFade (0, 0.2f);
	}

	//ドアから漏れる光の表現
	IEnumerator doorLightAnimation(){
		//Reset
		doorLightWhImage.color = new Color (1, 1, 1, 0);
		doorLightGrImage.color = new Color (0.8f, 0.8f, 0.3f, 0);
		doorLightWh.localScale = new Vector3 (10, 2, 1);
		doorLightGr.localScale = new Vector3 (30, 5, 1);
		yield return new WaitForSeconds(0.1f);

		doorLightWhImage.DOFade (0.8f, 0.3f);
		doorLightGrImage.DOFade (0.5f, 0.3f);
		yield return new WaitForSeconds(0.3f);
		doorLightWh.DOScale(new Vector3 (100, 5, 1), 0.2f);
		doorLightGr.DOScale (new Vector3 (100, 20, 1), 0.4f);
		yield return new WaitForSeconds(0.2f);
		doorLightWh.DOScale(new Vector3 (100, 13, 1), 0.4f);
		doorLightWhImage.DOFade (0f, 0.4f);
		yield return new WaitForSeconds(0.2f);
		doorLightGrImage.DOFade (0f, 3f);
	}

	//カードボックスが下からやってくるアニメ（一枚目のみ使う）
	IEnumerator CardInAnimation(){
		cardBox.DOLocalMove (new Vector3 (0, 40, 0), 0.7f).SetEase (Ease.OutExpo);
		yield return new WaitForSeconds(0.7f);
		seHolo.Play ();
		cardBox.DOLocalMove (new Vector3 (0, 0, 0), 1.8f).SetEase (Ease.Linear);
		yield return new WaitForSeconds(1.8f);
	}

	//カメラズームイン、アウト
	IEnumerator GasyaCameraZoom(){
		gasyaCamera.DOScale (1.5f, 0.3f).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds(0.3f);
		gasyaCamera.DOScale (0.915f, 2.3f).SetEase (Ease.OutQuad);
		yield return new WaitForSeconds(2.3f);
	}

	//カード強調アニメ（二枚目以降も使いまわす）
	IEnumerator CardAppealAnimation(){

		yield return new WaitForSeconds (0.1f);
//		cardImage.color = new Color (1, 1, 1, 1);
		card.DOScale (1f, 0.2f).SetEase (Ease.OutBack);
		cardCopy.DOScale (4, 0.3f);
		cardCopyImage.DOFade (0, 0.3f);
		cardLightImage.DOFade (1, 0.3f);
		yield return new WaitForSeconds (0.1f);

		if (NewCardFlg == true) {
			newLabel.SetActive (true);
		}
		kiraPar.Play ();
		StartCoroutine (burstBright ());
		yield return new WaitForSeconds (0.4f);

		if (RareCardFlg == true) {
			rareEffect.Play ();
		} 
	}

	//集中線明滅アニメ
	IEnumerator burstBright(){
		for(int i = 0; i<3; i++){
			burstBox.GetComponent<CanvasGroup>().alpha = 0.8f;
			yield return new WaitForSeconds(0.05f);
			burstBox.GetComponent<CanvasGroup>().alpha = 0;
			yield return new WaitForSeconds(0.05f);
		}
	}

//Next_Animation----------------------------------------------------------------------------------------

	//タップして次のカード表示
	IEnumerator Next_Animation(){
		getCardReset ();
		yield return new WaitForSeconds(0.1f);

		cardBox.DOLocalMoveX (-1400, 0.4f).SetEase (Ease.OutQuad).SetRelative ();
		newLabel.SetActive (false);
		cardImage.color = new Color (1, 1, 1, 1);
		yield return new WaitForSeconds(0.4f);
		
	}

//Result_Animation----------------------------------------------------------------------------------------

	//リザルト画面表示
	IEnumerator Result_Animation(){

		wh.GetComponent<CanvasGroup>().DOFade (1, 0.4f);
		yield return new WaitForSeconds(0.4f);
		get.SetActive (false);
		resultBox.SetActive (true);
		kira2.Play();
		kira3.Play();
		wh.GetComponent<CanvasGroup>().DOFade (0, 0.6f);

		//カードボックスの位置リセット
		cardBox.DOLocalMoveX (0, 0.4f).SetEase (Ease.OutQuad);

		yield return new WaitForSeconds(0.4f);
		wh.SetActive (false);
	}


}
