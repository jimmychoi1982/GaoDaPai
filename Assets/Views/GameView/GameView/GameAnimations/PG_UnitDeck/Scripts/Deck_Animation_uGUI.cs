using System;
using System.Collections;

using DG.Tweening;//DOTween

using UnityEngine;
using UnityEngine.UI;//uGUI

//カードをデッキから引いて、上部にフレームアウト.
public class Deck_Animation_uGUI : MonoBehaviour {

	public UIElement_deckBox deckBox;
	private Transform deckcardTra;
	private GameObject deckCard;
	private Transform deckCircle;
	private Image lightImage;
	private ParticleSystem particle;
	private GameObject deckAmount1, deckAmount2, deckAmount3, deckLight1, deckLight2, deckLight3;
	private Vector3[] path;
	public AudioSource SE_DrawCard;
	
	public GameObject drawCardPrefab;
	public Transform[] drawnCards;

	//カードが引かれる段階でのデッキにある残カード量（1:残り1枚, 2:2枚~20枚, 3:21枚～40枚)
	public int deckAmount;

	//カードを何枚引くか(1~5)
	public int cardCount;

	public bool drawCard_Start;

	void Awake () {
		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);
		if (deckBox != null) {
			deckcardTra = deckBox.deck_card;
			particle = deckBox.particle;
			deckCircle = deckBox.deckCircle;
			deckAmount1 = deckBox.deckAmount1;
			deckAmount2 = deckBox.deckAmount2;
			deckAmount3 = deckBox.deckAmount3;
			deckLight1 = deckBox.deckLight1;
			deckLight2 = deckBox.deckLight2;
			deckLight3 = deckBox.deckLight3;
			
			//サークル回転アニメ再生
			CircleRolling_Animation ();
		}

	}
	
#if UNITY_EDITOR	
	void Update () {
		SetCardAmountMode(deckAmount);

		if (drawCard_Start == true){
			StartCoroutine(DrawCardSet(cardCount));
			drawCard_Start = false;
		}
	}
#endif

	//
	public void SetCardAmountMode(int mode)
	{
		if (deckAmount == 1) {
			deckAmount1.SetActive (true);
			deckLight1.SetActive (true);
			deckAmount2.SetActive (false);
			deckLight2.SetActive (false);
			deckAmount3.SetActive (false);
			deckLight3.SetActive (false);
			deckCard = deckAmount1;
		} else if (deckAmount == 2) {
			deckAmount1.SetActive (false);
			deckLight1.SetActive (false);
			deckAmount2.SetActive (true);
			deckLight2.SetActive (true);
			deckAmount3.SetActive (false);
			deckLight3.SetActive (false);
			deckCard = deckAmount2;
		} else if (deckAmount == 3) {
			deckAmount1.SetActive (false);
			deckLight1.SetActive (false);
			deckAmount2.SetActive (false);
			deckLight2.SetActive (false);
			deckAmount3.SetActive (true);
			deckLight3.SetActive (true);
			deckCard = deckAmount3;
		}
	}

	//
	public void StartDraw(int count, Action cb) {
		StartCoroutine(DrawCardSet(count, cb));
	}

	//プレハブを作成、アニメーション準備
	public IEnumerator DrawCardSet (int card_number, Action cb = null) {
		StartCoroutine(Deck_Lighting());
		yield return new WaitForSeconds (1f);
		
		for (int a = 0; a < card_number; a += 1) {
			SE_DrawCard.Play ();
			//deck_cardを生成
			GameObject dcObj = (GameObject)Instantiate(drawCardPrefab, transform.position, Quaternion.identity);
			dcObj.transform.SetParent( deckcardTra, true );
			Transform dcTra = dcObj.transform;
			DrawCard_Reset(dcTra);
			yield return new WaitForSeconds (0.2f);

			if (a < card_number - 1) {
				StartCoroutine(DrawCardPlayerAnimation(dcTra, dcObj));
			} else {
				StartCoroutine(DrawCardPlayerAnimation(dcTra, dcObj, cb));
			}
		}
	}

	//光明滅アニメ
	public IEnumerator Deck_Lighting () {
		if (deckAmount == 1) {
			lightImage = deckLight1.GetComponent<Image>();
		} else if (deckAmount == 2) {
			lightImage = deckLight2.GetComponent<Image>();
		} else if (deckAmount == 3) {
			lightImage = deckLight3.GetComponent<Image>();
		}

		lightImage.DOFade (1, 0.3f);
		yield return new WaitForSeconds (0.3f);
		lightImage.DOFade (0, 0.2f);
		yield return new WaitForSeconds (0.2f);
		lightImage.DOFade (1, 0.3f);
		yield return new WaitForSeconds (0.3f);
		lightImage.DOFade (0, 0.2f);
	}

	//カード位置リセット
	public void DrawCard_Reset (Transform drawCard){
		//残り一枚なら、ドローされた瞬間デッキのカードは全て非表示にする
		if(deckAmount == 1){
			deckCard.SetActive(false);
			particle.Stop();
		}

		drawCard.gameObject.SetActive(true);
		drawCard.localPosition = new Vector3(-6, 0, 0);
		drawCard.localScale = new Vector3(1, 1, 1);
		drawCard.eulerAngles = new Vector3(0, 0, 0);
	}

	//デッキからカードを引くアニメーション
	public IEnumerator DrawCardPlayerAnimation (Transform drawCard, GameObject drawCardObj, Action cb = null)
	{
		drawCard.DOLocalMove(new Vector3(230, 1, 0), 0.4f).SetEase(Ease.InSine);
		yield return new WaitForSeconds (0.3f);

		path = new[] {
			new Vector3(156, 603, 0),
			new Vector3(-551, 1184, 0),
		};
		drawCard.DOLocalPath(path, 0.5f, PathType.CatmullRom).SetEase(Ease.InSine);
		drawCard.DOLocalRotate(new Vector3(0, 0, 140), 0.5f);
		drawCard.DOScale(new Vector3(4f, 1.6f, 1), 0.5f).SetEase(Ease.InSine);

		//アニメ再生が終わったら非表示
		yield return new WaitForSeconds (0.5f);
		GameObject.Destroy(drawCardObj);
		
		// Callback if it exists
		if (cb != null) {
			cb();
		}
	}

	//deck_circleが回るアニメーション
	void CircleRolling_Animation () {
		deckCircle.DORotate(new Vector3(0, 0, 360), 7f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
	}
}
