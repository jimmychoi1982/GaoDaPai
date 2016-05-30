using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class MenuView : MonoBehaviour {
	public GameObject[] menuAreas = new GameObject[3];
	public GameObject MenuBase;

	public GameObject operatorPrefab;

	private PopUpWindow operatorWindow;

	private string selectedOperatorId;

	private bool isOpenMenuBase = true;
	private bool isOpenLifeCounter = false;

	LifeCounterView lifeCounterView;
	
	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("MenuView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private BNIDManager bnidManager;
	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		bnidManager = steward.bnidManager;

		lifeCounterView = menuAreas [2].GetComponent<LifeCounterView> ();

		if (steward.CustomMenu > 0) {
			InitView (steward.CustomMenu, false);
			steward.CustomMenu = 0;
		}
	}

	public void Back (bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		if (isOpenMenuBase) {
			steward.LoadNextScene ("HomeView");
		} else {
			MenuBase.SetActive (true);
			for (int i = 0; i < 3; i++) {
				menuAreas [i].SetActive (false);
			}
			isOpenMenuBase = true;
			if (isOpenLifeCounter) {
				steward.ShowHeader ();
				isOpenLifeCounter = false;
			}
			steward.SetHeaderViewName ("メニュー");
		}
	}

	public void OpenProfileView () {
		InitView (0);
		steward.ShowAndHideLoadingScreen (1);
		steward.SetHeaderViewName ("入隊証");
	}

	public void OpenCollectionView () {
		InitView (1, false);
		steward.SetHeaderViewName ("カードバインダー");
	}

	public void OpenLifeCounterView () {
		InitView (2);
		steward.ShowAndHideLoadingScreen (1);
		lifeCounterView.Initialize();
		isOpenLifeCounter = true;
		steward.HideHeader ();
	}

	public void OpenTutorialScene () {
		steward.PlaySETap ();
		steward.OpenDialogWindow ("確認", "もう一度チュートリアルを再生します", "OK", () => {
			GameSettings.TutorialState = GameSettings.TutorialStates.Encore;
			steward.LoadNextScene ("NewTutorialIntroView");}, "キャンセル", () => {});
	}
	
	public void OpenOptionWindow () {
		steward.PlaySETap ();
		steward.OpenCustomWindow ("OptionWindow", "閉じる", () => {});
	}

	public void OpenHelpWebView () {
		steward.OpenWebView ("http://www.gundam-cw.com/app_help.php");
	}

	public void OpenAgreementWindow () {
		steward.PlaySETap ();
		steward.OpenCustomWindow ("AgreementWindow", "閉じる", () => {})
			.SetSubject ("利用規約")
			.SetMessage ("");
	}

	public void OpenSupportWindow () {
		steward.PlaySETap ();
		steward.OpenDialogWindowL ("サポート", "各種お問い合わせは、下記メールアドレスへご連絡ください\n\n\n" +
		                           "【日本版】gcw@sup-nc.banapassport.net\n" +
		                           "【海外版】gcwe@sup-nc.banapassport.net\n\n\n" +
		                           "※サポートの受付時間は10:00～17:00（祝日、夏季・冬季休業を除く）になります\n" +
		                           "※ご返答までにお時間を頂戴する場合がございます\n" +
		                           "※返信メールが受信できるようにドメイン指定解除の設定をお願いします\n" +
		                           "※お電話でのお問い合わせは、専用のWEBサイトをご確認ください", "WEBサイト", () => {Application.OpenURL("http://www.carddass.com/contact/");}, "閉じる", () => {});
	}

	public void OpenTitleScene () {
		steward.PlaySETap ();
		steward.OpenDialogWindow ("確認", "タイトル画面へ戻ります\n" +
		                          "よろしいですか？", "タイトルへ", () => {steward.LoadNextScene ("TitleView");}, "閉じる", () => {});
	}

	public void OpenInheritWindow () {
		steward.PlaySETap ();
		bnidManager.OpenInheritWindow ();
	}

	public void OpenSelectOperatorWindow () {
		steward.PlaySETap ();
		operatorWindow = steward.InitCustomWindow("VerticalSelectWindow");

		foreach (var op in user.tOperators["operatorId"]) {
			var clone = Instantiate (operatorPrefab);
			clone.transform.SetParent(operatorWindow.GetObject ("ScrollView/Content").transform);
			clone.transform.localPosition = operatorPrefab.transform.localPosition;
			clone.transform.localScale = operatorPrefab.transform.localScale;
			clone.transform.Find ("Out_Window").GetComponent<OperatorData> ().StoreData (op.ToString ());
			clone.transform.Find ("Out_Window/Text").GetComponent<Text> ().text = master.staticData ["operators"] [op.ToString ()] ["name"].ToString ();
			clone.transform.Find ("Out_Window/SelectEnclose").gameObject.SetActive (false);
		}
		steward.OpenCustomWindow (operatorWindow, "決定", () => {ChangeOperator();});
		operatorWindow.SetSubject ("オペレーター");
		operatorWindow.GetSingleActionButton ().SetActive (false);
	}
	
	public void SelectOperator (GameObject obj) {
		selectedOperatorId = obj.transform.Find("Out_Window").GetComponent<OperatorData> ().operatorId;
		operatorWindow.GetSingleActionButton ().SetActive (true);
		
		foreach (Transform n in operatorWindow.GetObject ("ScrollView/Content").transform) {
			if (n.Find ("Out_Window").GetComponent<OperatorData> ().operatorId == selectedOperatorId) {
				n.Find ("Out_Window/SelectEnclose").gameObject.SetActive (true);
			} else {
				n.Find ("Out_Window/SelectEnclose").gameObject.SetActive (false);
			}
		}
	}
	
	public void ChangeOperator () {
		steward.SetNewTask (user.setOperator (selectedOperatorId, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setOperator:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			if (steward.ResponseHasErrorCode (result)) return;
			
			steward.SetLive2DOperator ();
			steward.OpenMessageWindow ("オペレーター変更完了", "オペレーターを変更しました\nタイトルへ戻ります", "OK", () => {steward.LoadNextScene ("TitleView");});
		}));
	}
	
	private void InitView(int Index, bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		MenuBase.SetActive (false);
		menuAreas [Index].SetActive (true);
		isOpenMenuBase = false;
		if (Index == 1) {
			steward.SetHeaderViewName ("カードバインダー");
		}
	}
}
