using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

using Newtonsoft.Json.Linq;

public class HomeView : MonoBehaviour {
	public GameObject presentRowPrefab;
	public GameObject cardRegistrationViewButton;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private BNID bnid { get { return BNID.Instance; }}
	private AppVersions appVersions { get { return AppVersions.Instance; }}
	private Logger logger { get { return mage.logger("HomeView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private BNIDManager bnidManager;
	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		bnidManager = steward.bnidManager;

		steward.SetNewTask (appVersions.isCommandEnable ("HomeView", (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error isCommandEnable:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			if (steward.ResponseHasErrorCode(result)) return;

			if (bool.Parse (result["result"].ToString ())) {
				cardRegistrationViewButton.SetActive (true);
			}
		}));
		
		steward.SetNewTask (user.dailySupply ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error dailySupply:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			if (steward.ResponseHasErrorCode(result)) return;

			steward.ReloadHeader ();
		}));

		if (steward.FirstHomeVisit) {
			steward.FirstHomeVisit = false;
			OpenNewsWebView (false);
		}
	}
	
	public void OpenPracticeScene () {
		GameSettings.LastQueueType = "";
		LoadScene ("PracticeView");
	}

	public void OpenMultiPlayScene () {
		LoadScene ("MultiplayView");
	}

	public void OpenDeckScene () {
		LoadScene ("DeckConstructionView");
	}

	public void OpenShopScene () {
		LoadScene ("ShopView");
	}

	public void OpenCardBuyScene () {
		LoadScene ("CardPurchaseView");
	}

	public void OpenSerialScene () {
		if (!bool.Parse (user.tUser["isAssociated"].ToString ())) {
			steward.PlaySETap ();
			steward.OpenDialogWindow ("確認", "バンダイナムコIDに登録されていないため\n" +
			                          "「カード登録」と「カメラ登録」の機能が利用できません\n\n" +
			                          "バンダイナムコIDを登録後にお試しください", "会員登録", () => {bnidManager.SetUpAssociateBNID ();}, "閉じる", () => {});
		} else {
			LoadScene ("CardRegistrationView");
		}
	}

	public void OpenMenuScene () {
		LoadScene ("MenuView");
	}

	public void OpenNewsWebView (bool playSE = true) {
		steward.OpenWebView ("http://www.gundam-cw.com/app_index.php", playSE);
	}
	
	public void OpenOfficialWeb () {
		steward.OpenBrowser ("http://www.gundam-cw.com/");
	}

	public void OpenPresentWindow () {
		steward.PlaySETap ();

		steward.SetNewTask (user.getPresentLog ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error getPresentLog:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}

			if (steward.ResponseHasErrorCode(result)) return;

			var historyWindow = steward.InitCustomWindow("HistoryWindow");
			historyWindow
				.SetSubject ("受取履歴")
				.SetMessage ("50" + historyWindow.GetObject("Message").GetComponent<Text> ().text);
			
			JArray resultArr = result as JArray;
			int index = 0;
			foreach (var token in resultArr) {
				var obj = token as JObject;
				var clone = Instantiate (presentRowPrefab);
				clone.transform.SetParent(historyWindow.GetObject ("ScrollView/Content").transform);
				clone.transform.localPosition = presentRowPrefab.transform.localPosition;
				clone.transform.localScale = presentRowPrefab.transform.localScale;
				clone.transform.Find ("Out_Window/CardName").GetComponent<Text>().text = obj["name"].ToString();
				clone.transform.Find ("Out_Window/DayData").GetComponent<Text>().text = obj["date"].ToString();
				clone.transform.Find ("Out_Window/WhyGetMessage").GetComponent<Text>().text = obj["subject"] != null ? obj["subject"].ToString() : "";
				index++;
			}

			historyWindow = steward.OpenCustomWindow (historyWindow, "閉じる", () => {});
		}));
	}

	private void LoadScene (string sceneName) {
		steward.PlaySETap ();
		steward.LoadNextScene (sceneName);
	}
}