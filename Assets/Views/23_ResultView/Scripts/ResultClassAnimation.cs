using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class ResultClassAnimation : MonoBehaviour {

	public UIElement_ClassUpDown classUpDownElement;
	public GameObject classUpDown;
	private GameObject logoBox, classUpLogoBox, upBackLightOver, upLogo, upBlinkLight;
	private Image upBackLightImage, upBackLightOverImage, upLogoLightImage, upBlinkLightImage, upLogoImage;
	private GameObject classDownLogoBox;
	private GameObject battleRankBox, oldRank, newRank, arrow;
	private Image arrowImage;
	private GameObject rewardWindow;
	private GameObject tapScreen, tapImage;
	private AudioSource SE_RankUp;
	
	public bool classUp_Flg;//ユーザーが昇格したときtrue
	public bool rewardGet_Flg;//報酬ありの時true

	public bool animeStart;
	public bool rankUpEnd;

	// Use this for initialization
	void Start () {

		classUpDown = classUpDownElement.classUpDown;
		logoBox = classUpDownElement.logoBox;
		classUpLogoBox = classUpDownElement.classUpLogoBox;
		upBackLightOver = classUpDownElement.upBackLightOver;
		upLogo = classUpDownElement.upLogo;
		upBlinkLight = classUpDownElement.upBlinkLight;
		upBackLightImage = classUpDownElement.upBackLightImage;
		upBackLightOverImage = classUpDownElement.upBackLightOverImage;
		upLogoLightImage = classUpDownElement.upLogoLightImage;
		upBlinkLightImage = classUpDownElement.upBlinkLightImage;
		upLogoImage = classUpDownElement.upLogoImage;
		classDownLogoBox = classUpDownElement.classDownLogoBox;
		battleRankBox = classUpDownElement.battleRankBox;
		oldRank = classUpDownElement.oldRank;
		newRank = classUpDownElement.newRank;
		arrow = classUpDownElement.arrow;
		arrowImage = classUpDownElement.arrowImage;
		rewardWindow = classUpDownElement.rewardWindow;
		tapScreen = classUpDownElement.tapScreen;
		tapImage = classUpDownElement.tapImage;
		SE_RankUp = classUpDownElement.SE_RankUp;

		tapImage.GetComponent<TapScreenBlink> ().animeStart = true;

		Reset ();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (animeStart == true) {
			new Task (ResultClassAnimeSeq());
			animeStart = false;
		}
	}

	public void Reset(){
		classUpDown.SetActive (false);
		logoBox.transform.localPosition = new Vector3 (0, 133, 0);
		logoBox.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);

		classUpLogoBox.SetActive (false);
		upBackLightImage.color = new Color (1, 1, 1, 0);
		upBackLightOver.transform.localScale = new Vector3 (1, 1, 1);
		upBackLightOverImage.color = new Color (1, 1, 1, 0);
		upBlinkLight.transform.localScale = new Vector3 (1, 1, 1);
		upLogoLightImage.color = new Color (1, 1, 1, 0);
		upBlinkLightImage.color = new Color (1, 1, 1, 0);
		upLogoImage.color = new Color (1, 1, 1, 0);

		classDownLogoBox.SetActive (false);
		classDownLogoBox.GetComponent<CanvasGroup> ().alpha = 0;

		battleRankBox.transform.localPosition = new Vector3 (0, 0, 0);
		battleRankBox.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
		oldRank.SetActive (false);
		newRank.SetActive (false);
		arrow.transform.localPosition = new Vector3 (0, 0, 0);
		if (classUp_Flg == true) {
			arrowImage.color = new Color (0f, 1f, 1f, 0f);
		} else if (classUp_Flg == false) {
			arrowImage.color = new Color (1f, (float)(53f / 255f), (float)(53f / 255f), 0f);
		}
		rewardWindow.SetActive (false);
		rewardWindow.transform.localScale = new Vector3 (0, 0, 0);

		tapScreen.GetComponent<CanvasGroup> ().alpha = 0;
	}

	IEnumerator ResultClassAnimeSeq(){
		Reset();
		yield return new WaitForSeconds (0.1f);

		classUpDown.SetActive (true);
		battleRankBox.SetActive (true);

		if (classUp_Flg) {
			classUpLogoBox.SetActive(true);
			new Task (ClassUpLogoAnimation());
		} else {
			classDownLogoBox.SetActive(true);
			classDownLogoBox.GetComponent<CanvasGroup>().DOFade (1, 1.5f).SetEase (Ease.OutCubic);
		}


		yield return new WaitForSeconds (0.2f);

		new Task (BattleRankAnimation ());

		yield return new WaitForSeconds (0.9f);

		if (classUp_Flg) {
			if (rewardGet_Flg == true) {
				new Task (BasicElementSmallAnimation ());
				yield return new WaitForSeconds (0.5f);
			} 
		}

		tapScreen.SetActive (true);
		this.rankUpEnd = true;
		tapScreen.GetComponent<CanvasGroup> ().DOFade (1, 0.3f);
		
	}

	//Animation単体----------------------------------------------------------------

	IEnumerator ClassUpLogoAnimation(){
		SE_RankUp.Play ();
		upBlinkLight.transform.DOScale (new Vector3(3, 1, 1), 0.3f).SetEase (Ease.OutCubic);
		upBlinkLightImage.DOFade (1, 0.15f).SetLoops (2, LoopType.Yoyo);
		
		upBackLightOverImage.color = new Color (1, 1, 1, 1);
		upBackLightOver.transform.DOScale (2, 0.25f).SetEase (Ease.OutCubic);
		upBackLightOverImage.DOFade (0f, 0.25f);
		upBackLightImage.DOFade (1, 0.3f).SetEase (Ease.OutCubic);
		upLogoLightImage.DOFade (1, 0.15f);
		
		yield return new WaitForSeconds (0.2f);
		upLogoImage.DOFade (1, 0.3f);
		
	}

	IEnumerator BattleRankAnimation(){
		oldRank.SetActive (true);
		oldRank.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0.2f), 0.3f, 1, 1);
		yield return new WaitForSeconds (0.2f);
		arrowImage.DOFade (1, 0.3f);
		arrow.transform.DOPunchPosition (new Vector3 (10, 0, 0), 0.2f, 1, 1, false);
		yield return new WaitForSeconds (0.2f);
		newRank.SetActive (true);
		newRank.transform.DOPunchScale (new Vector3 (0.2f, 0.2f, 0.2f), 0.3f, 1, 1);
	}

	IEnumerator BasicElementSmallAnimation(){
		logoBox.transform.DOScale (1, 0.3f);
		logoBox.transform.DOLocalMove (new Vector3(0, 160, 0), 0.3f);
		battleRankBox.transform.DOScale (1, 0.3f);
		battleRankBox.transform.DOLocalMove (new Vector3(0, 98, 0), 0.3f);
		
		yield return new WaitForSeconds (0.2f);
		rewardWindow.SetActive (true);
		rewardWindow.transform.Find ("ScrollView/Content").transform.localPosition = new Vector3 (0f, 0f, 0f);
		rewardWindow.transform.DOScale (1, 0.4f).SetEase (Ease.OutBack);
	}

}
