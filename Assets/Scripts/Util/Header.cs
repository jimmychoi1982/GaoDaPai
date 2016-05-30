using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

using Newtonsoft.Json.Linq;

public class Header : MonoBehaviour {
	public Text viewName;
	public Image classIcon;
	public Text className;
	public Text userName;
	public Text recoveryItems;
	public Text battlePoint;

	public GameObject backButton;
	public GameObject menuButton;
	public GameObject rankButton;
	public GameObject rankPrefab;

	public CanvasGroup menuCanvas;
	public GameObject menuBackground;
	public Sprite menuDefaultButton;
	public Sprite menuCloseButton;

	private DeckConstructionView deckConstructionView;
	private MenuView menuView;
	private MultiplayView multiplayView;
	private PracticeView practiceView;

	private bool isShowMenu = false;
	private bool isFade = false;

	public GameObject rankPopUp;
	private bool isShowRankPopUp = false;

	private bool isInit = false;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("Header"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;

	void OnLevelWasLoaded (int level) {
		if (isShowRankPopUp) {
			CloseRankPopUp ();
		}
	}

	public void Show () {
		gameObject.SetActive (true);
		if (!isInit) {
			steward = GameObject.Find ("Steward").GetComponent<Steward> ();

			menuCanvas.gameObject.SetActive (true);
			menuCanvas.alpha = 0f;
			menuCanvas.gameObject.SetActive (false);
			isInit = true;
		}
		if (Application.loadedLevelName == "HomeView") {
			backButton.SetActive (false);
			menuButton.SetActive (false);
		} else {
			backButton.SetActive (true);
			menuButton.SetActive (true);

			deckConstructionView = null;
			menuView = null;
			multiplayView = null;
			if (Application.loadedLevelName == "DeckConstructionView") {
				deckConstructionView = GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ();
			} else if (Application.loadedLevelName == "MenuView") {
				menuView = GameObject.Find ("/Main Canvas").GetComponent<MenuView> ();
			} else if (Application.loadedLevelName == "MultiplayView") {
				multiplayView = GameObject.Find ("/Main Canvas").GetComponent<MultiplayView> ();
			} else if (Application.loadedLevelName == "PracticeView") {
				practiceView = GameObject.Find ("/Main Canvas").GetComponent<PracticeView> ();
			}
		}
		ApplyData ();
	}

	public void Hide () {
		gameObject.SetActive (false);
	}
	
	public void ApplyData () {
		viewName.text = GetViewName ();
		loadAssetBundle.SetClassIconImage (user.tUser ["classId"].ToString (), classIcon.gameObject);
		className.text = master.staticData ["classes"] [user.tUser ["classId"].ToString ()] ["name"].ToString ();
		userName.text = user.tUser ["userName"].ToString ();
		recoveryItems.text = user.tUser ["recoveryItems"].ToString ();
		battlePoint.text = user.tUser ["battlePoint"].ToString ();
	}

	private string GetViewName () {
		switch (Application.loadedLevelName) {
		case "HomeView":
			return "メインブリッジ";
			break;

		case "DeckConstructionView":
			return "デッキ構築";
			break;
			
		case "ShopView":
			return "戦果交換所";
			break;
			
		case "CardPurchaseView":
			return "店舗情報";
			break;
			
		case "CardRegistrationView":
			return "カード登録";
			break;
			
		case "MenuView":
			return "メニュー";
			break;
			
		case "PracticeView":
			return "訓練モード";
			break;
			
		case "MultiplayView":
			return "対戦モード";
			break;
			
		default:
			return "";
			break;
			
		}
	}

	public void SetViewName (string viewName) {
		this.viewName.text = viewName;
	}

	public void LoadView (string viewName) {
		if (Application.loadedLevelName == "DeckConstructionView") {
			if (deckConstructionView.EditNoSave == true) {
				steward.OpenDialogWindow ("確認", "編集中の情報は破棄されます\n" +
				                          "よろしいですか？", "破棄", () => {HideMenuAndLoadNext (viewName, false);}, "キャンセル", () => {});
				return;
			}
		}

		if (viewName == "PracticeView") {
			GameSettings.LastQueueType = "";
		}

		HideMenuAndLoadNext (viewName);
	}

	public void Back () {
		if (Application.loadedLevelName == "DeckConstructionView") {
			deckConstructionView.Back (true);
			return;
		} else if (Application.loadedLevelName == "MenuView") {
			menuView.Back (true);
			return;
		} else if (Application.loadedLevelName == "MultiplayView") {
			multiplayView.Back (true);
			return;
		} else if (Application.loadedLevelName == "PracticeView") {
			practiceView.Back (true);
			return;
		}
		steward.PlaySETap ();
		steward.LoadNextScene ("HomeView");
	}
	
	public void ToggleMenu() {
		if (isFade) return;
		steward.PlaySETap ();
		isFade = true;
		
		if (isShowMenu == true) {
			new Task (MenuFadeOutAnimation ());
			isShowMenu = false;
		} else {
			new Task (MenuFadeInAnimation ());
			isShowMenu = true;
		}
	}

	public void OpenRankPopUp () {
		if (isShowRankPopUp)
			return;
		isShowRankPopUp = true;
		rankPopUp = new GameObject ("RankPopUp");
		rankPopUp.transform.SetParent (transform.parent.parent.Find ("BackPanel").transform);
		rankPopUp.transform.localPosition = new Vector3 (0f, 140f, 0f);
		rankPopUp.transform.localScale = new Vector3 (1f, 1f, 1f);
		rankPopUp.AddComponent<CanvasGroup> ();
		rankPopUp.GetComponent<CanvasGroup> ().alpha = 0;
		var rankPopUpRow = Instantiate (rankPrefab);
		var color = user.tUser ["color"].ToString ();
		rankPopUpRow.transform.SetParent (rankPopUp.transform);
		rankPopUpRow.transform.localPosition = new Vector3 (0f, 0f, 0f);
		rankPopUpRow.transform.localScale = new Vector3 (1f, 1f, 1f);
		loadAssetBundle.SetRankedMatchFrameImage (color, rankPopUpRow.transform.Find ("OutWindow").GetComponent<Image> ().gameObject);
		
		JObject currentClass;
		String currentExp;
		if (game.tClasses [color] != null) {
			var classOfColor = game.tClasses [color] as JObject;
			currentClass = master.staticData ["classes"] [classOfColor ["classId"].ToString ()] as JObject;
			currentExp = classOfColor ["exp"].ToString ();
		} else {
			currentClass = master.staticData ["classes"] ["class" + char.ToUpper (color [0]) + color.Substring (1) + "001"] as JObject;
			currentExp = "0";
		}
		loadAssetBundle.SetClassIconImage (currentClass ["id"].ToString (), rankPopUpRow.transform.Find ("ClassIcon").GetComponent<Image> ().gameObject);
		rankPopUpRow.transform.Find ("ClassText").GetComponent<Text> ().text = currentClass ["name"].ToString ();
		rankPopUpRow.transform.Find ("CurrentExp/ExpText").GetComponent<Text> ().text = currentExp;
		if (currentClass ["ascendClassId"] != null) {
			rankPopUpRow.transform.Find ("RankUpCondition/RankUpExp").GetComponent<Text> ().text = currentClass ["expThreshold"].ToString ();
		} else {
			rankPopUpRow.transform.Find ("RankUpCondition/RankUpExp").GetComponent<Text> ().text = "-";
			rankPopUpRow.transform.Find ("RankUpCondition/RankUpSuffix").GetComponent<Text> ().text = "";
		}
		if (currentClass ["descendClassId"] != null) {
			rankPopUpRow.transform.Find ("RankDownCondition/RankDownExp").GetComponent<Text> ().text = master.staticData ["classes"] [currentClass ["descendClassId"].ToString ()] ["expThreshold"].ToString ();
		} else {
			rankPopUpRow.transform.Find ("RankDownCondition/RankDownExp").GetComponent<Text> ().text = "-";
			rankPopUpRow.transform.Find ("RankDownCondition/RankDownSuffix").GetComponent<Text> ().text = "";
		}

		new Task (RankPopUpFadeInAnimation ());
	}

	public void CloseRankPopUp () {
		if (!isShowRankPopUp) return;
		new Task (RankPopUpFadeOutAnimation ());
	}

	private IEnumerator MenuFadeInAnimation () {
		menuButton.GetComponent<Image> ().sprite = menuCloseButton;
		menuBackground.SetActive (true);
		menuBackground.GetComponent<Image> ().DOFade (0.7f, 0.4f);
		yield return new WaitForSeconds (0.1f);
		menuCanvas.gameObject.SetActive (true);
		menuCanvas.DOFade (1f, 0.1f);
		isFade = false;
	}
	
	private IEnumerator MenuFadeOutAnimation () {
		menuButton.GetComponent<Image> ().sprite = menuDefaultButton;
		menuBackground.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
		menuBackground.SetActive (false);
		menuCanvas.DOFade (0f, 0.1f);
		menuCanvas.gameObject.SetActive (false);
		yield return new WaitForSeconds (0.2f);
		isFade = false;
	}
	
	private IEnumerator RankPopUpFadeInAnimation () {
		rankPopUp.GetComponent<CanvasGroup> ().DOFade (1f, 0.2f);
		yield return 0;
	}

	private IEnumerator RankPopUpFadeOutAnimation () {
		rankPopUp.GetComponent<CanvasGroup> ().DOFade (0f, 0.2f);
		yield return new WaitForSeconds (0.2f);
		Destroy (rankPopUp);
		rankPopUp = null;
		isShowRankPopUp = false;
	}

	private void HideMenuAndLoadNext (string nextSceneName, bool isPlaySE = true) {
		if (isPlaySE) {
			steward.PlaySETap ();
		}
		menuButton.GetComponent<Image> ().sprite = menuDefaultButton;
		menuBackground.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
		menuBackground.SetActive (false);
		menuCanvas.alpha = 0f;
		menuCanvas.gameObject.SetActive (false);
		isFade = false;
		isShowMenu = false;
		steward.LoadNextScene (nextSceneName, true);
	}
}
