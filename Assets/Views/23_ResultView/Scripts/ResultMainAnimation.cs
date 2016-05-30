using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class ResultMainAnimation : MonoBehaviour 
{

	public UIElement_BattleResult battleResultElement;
	private GameObject battleResultPanel, winImage, loseImage, battleResult, addCount, addCountLabel;
	private GameObject getExp, getExpRow, totalExpRow;
	private GameObject getCoin, getCoinRow, bonusCoinRow, totalCoinRow;
	private GameObject currentClass, newClass;
	private GameObject tapScreen, tapImage;
	private float slidefloat = 0.3f;
	private float fadefloat = 0.4f;

	public bool playerWinFlg;//プレイヤー勝ったときtrue、負けたときfalse
	public bool rankMatchFlg;//ランクマッチのときtrue（獲得BCの欄が表示、演出される）
	public bool rankUpDownFlg;//ランク変動のときtrue
	public bool inAnimeStart, outAnimeStart, isAnimePlayed;

	public bool ResultEndAnimation;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("MultiplayView"); } }
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	// Use this for initialization
	void Start () 
	{
		battleResultPanel = battleResultElement.battleResultPanel;
		winImage = battleResultElement.winImage;
		loseImage = battleResultElement.loseImage;
		battleResult = battleResultElement.battleResult;
		addCount = battleResultElement.addCount;
		addCountLabel = battleResultElement.addCountLabel;
		getExp = battleResultElement.getExp;
		getExpRow = battleResultElement.getExpRow;
		totalExpRow = battleResultElement.totalExpRow;
		getCoin = battleResultElement.getCoin;
		getCoinRow = battleResultElement.getCoinRow;
		bonusCoinRow = battleResultElement.bonusCoinRow;
		totalCoinRow = battleResultElement.totalCoinRow;
		currentClass = battleResultElement.currentClass;
		newClass = battleResultElement.newClass;
		tapScreen = battleResultElement.tapScreen;
		tapImage = battleResultElement.tapImage;

		Reset ();
	}
	
	// Update is called once per frame
	void Update () {

		if (inAnimeStart == true) {
			StartCoroutine(BattleResultInAnimeSeq());
			inAnimeStart = false;
		}
		if (outAnimeStart == true) {
			StartCoroutine(battleResultOutAnimeSeq());
			outAnimeStart = false;
		}
	}

	void Reset(){
		battleResultPanel.GetComponent<CanvasGroup> ().alpha = 0;
		if (playerWinFlg == true) {
			winImage.SetActive (true);
			loseImage.SetActive (false);
		} else {
			winImage.SetActive (false);
			loseImage.SetActive (true);
		}
		battleResult.transform.localPosition = new Vector3 (587, 86, 0);
		battleResult.GetComponent<CanvasGroup> ().alpha = 0;
		addCount.SetActive (false);
		getExp.SetActive (false);
		getExpRow.transform.localPosition = new Vector3 (587, -5, 0);
		getExpRow.GetComponent<CanvasGroup> ().alpha = 0;
		totalExpRow.transform.localPosition = new Vector3 (689, -40, 0);
		totalExpRow.GetComponent<CanvasGroup> ().alpha = 0;
		getCoin.SetActive (false);
		getCoinRow.transform.localPosition = new Vector3 (587, -5, 0);
		getCoinRow.GetComponent<CanvasGroup> ().alpha = 0;
		bonusCoinRow.transform.localPosition = new Vector3 (689, -40, 0);
		bonusCoinRow.GetComponent<CanvasGroup> ().alpha = 0;
		totalCoinRow.transform.localPosition = new Vector3 (689, -75, 0);
		totalCoinRow.GetComponent<CanvasGroup> ().alpha = 0;
		tapScreen.SetActive (false);
		tapScreen.GetComponent<CanvasGroup> ().alpha = 0;
	}

	IEnumerator BattleResultInAnimeSeq(){
		if (isAnimePlayed) {
			yield break;
		}
		isAnimePlayed = true;
		Reset ();
		yield return new WaitForSeconds (0.1f);
		battleResultPanel.SetActive (true);
		battleResultPanel.GetComponent<CanvasGroup> ().DOFade (1, 0.5f);

		yield return new WaitForSeconds (0.2f);

		StartCoroutine (BattleResultAnimation ());

		yield return new WaitForSeconds (0.7f);

		if (rankMatchFlg == true) {
			getExp.SetActive (true);
			getCoin.SetActive (true);
			if (rankUpDownFlg == true) {
				newClass.SetActive (true);
			}
			StartCoroutine (PrizeAnimation());
			yield return new WaitForSeconds (1.2f);
		}

		tapScreen.SetActive (true);
		this.ResultEndAnimation = true;

		tapScreen.GetComponent<CanvasGroup> ().DOFade (1, 0.3f);

	}


	IEnumerator BattleResultAnimation(){
		battleResult.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		battleResult.transform.DOLocalMoveX(77, slidefloat).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds (0.4f);
		addCount.SetActive (true);

		if (playerWinFlg) {
			addCount.transform.localPosition = new Vector3 (70, 0, 0);
		} else {
			addCount.transform.localPosition = new Vector3 (210, 0, 0);
		}
//		addCountLabel.transform.DOLocalJump (new Vector3 (0, 0, 0), 10, 1, 0.2f, false);
	}

	IEnumerator PrizeAnimation(){
		//各要素をフェードインしながらスライドイン
		getExpRow.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		getExpRow.transform.DOLocalMoveX(220, slidefloat).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds (0.4f);

		totalExpRow.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		totalExpRow.transform.DOLocalMoveX(322, slidefloat).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds (0.4f);

		getCoinRow.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		getCoinRow.transform.DOLocalMoveX(220, slidefloat).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds (0.4f);

		bonusCoinRow.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		bonusCoinRow.transform.DOLocalMoveX (322, slidefloat).SetEase (Ease.OutCubic);
		yield return new WaitForSeconds (0.4f);

		totalCoinRow.GetComponent<CanvasGroup> ().DOFade (1, fadefloat);
		totalCoinRow.transform.DOLocalMoveX(322, slidefloat).SetEase (Ease.OutCubic);
	}

	IEnumerator battleResultOutAnimeSeq(){
		battleResultPanel.GetComponent<CanvasGroup> ().DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);
		battleResultPanel.SetActive (false);
	}

}
