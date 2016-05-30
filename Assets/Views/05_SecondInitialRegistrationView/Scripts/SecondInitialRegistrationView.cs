using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Prime31;

public class SecondInitialRegistrationView : MonoBehaviour {
	public GameObject forceEnter;
	public GameObject emblemPrefab;

	private PopUpWindow emblemWindow;
	private int slideIndex;

	private string currentForceId;
	private string currentEmblemId;
	
	private bool isSelectForce = false;
	
	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("SecondInitialRegistrationView"); } }

	private List<string> selectableForceIds = new List<string> (){"blue", "green", "yellow", "black", "red"};

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;
	
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		slideIndex = 0;

		SetForceData ();
	}
	
	public void OpenMessageWindow () {
		steward.PlaySEOK ();
		steward.OpenMessageWindow ("エンブレム選択", "好きなエンブレムを選んでください\n" +
			"※ゲーム開始後でも変更できます", "はい", () => {OpenEmblemSelectWindow ();});
	}

	public void OpenEmblemSelectWindow () {
		forceEnter.SetActive (false);
		loadAssetBundle.SetLoadWait ();
		List<string> currentForceEmblem = new List<string> ();
		foreach (JProperty prop in master.staticData ["emblem"]) {
			if ((prop.Value as JObject) ["forceId"].ToString () == currentForceId) {
				currentForceEmblem.Add (prop.Name);
			}
		}

		emblemWindow = steward.OpenCustomWindow("SelectWindow", "決定", () => {OpenConfirmWindow ();});
		emblemWindow.SetSubject ("エンブレム選択");
		emblemWindow.SetText ("SelectionName", "");
		List_Instance listInstance = emblemWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ();
		listInstance.Create_Object = emblemPrefab;
		listInstance.Create_Number = currentForceEmblem.Count;
		listInstance.CreateInstance ();
		
		var panelList = listInstance.GetCreateObjects ();
		int index = 0;
		foreach (var emblemId in currentForceEmblem) {
			loadAssetBundle.SetEmblemImage (emblemId, panelList [index].GetComponent<Image> ().gameObject);
			panelList [index].GetComponent<EmblemData> ().StoreData (emblemId);
			index++;
		}
		emblemWindow.GetObject ("ListArea/List/ListParent").GetComponent<Slide_Action_Ver2> ().Rewind ();
		emblemWindow.GetSingleActionButton ().SetActive (false);
	}

	public void SelectEmblem (GameObject obj) {
		steward.PlaySETap ();
		currentEmblemId = obj.GetComponent<EmblemData> ().emblemId;
		emblemWindow.GetSingleActionButton ().SetActive (true);
		emblemWindow.SetText ("SelectionName", master.staticData ["emblem"] [currentEmblemId] ["name"].ToString ());
		
		var panelList = emblemWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<EmblemData> ().emblemId == currentEmblemId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
	}
	
	public void OpenConfirmWindow () {
		var confirmWindow = steward.OpenCustomWindow("ForceEmblemConfirmWindow", "決定", () => {SubmitForceEmblem();}, "やめる", () => {forceEnter.SetActive (true);});
		loadAssetBundle.SetForceIconImage(currentForceId, confirmWindow.GetObject ("ForceImage").GetComponent<Image>().gameObject);
		loadAssetBundle.SetEmblemImage(currentEmblemId, confirmWindow.GetObject ("EmblemImage").GetComponent<Image>().gameObject);
		confirmWindow.SetText ("ForceText", master.staticData ["forces"] [currentForceId] ["name"].ToString ());
		confirmWindow.SetText ("EmblemText", master.staticData ["emblem"] [currentEmblemId] ["name"].ToString ());
	}

	public void SubmitForceEmblem () {
		steward.SetNewTask (user.setInitialSetting (currentForceId, currentEmblemId, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setInitialSetting:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode (result))
				return;
			
			GameSettings.TutorialState = GameSettings.TutorialStates.Done;
			steward.OpenMessageWindow ("登録完了", "隊員登録が完了しました\nタイトルに戻ります", "OK", () => {
				#if UNITY_ANDROID || UNITY_IOS
				var defaultDeck = master.staticData ["defaultDecks"] [slideIndex.ToString()] as JObject;
				Localytics.tagEvent("Default Deck: " + defaultDeck["color"].ToString ());
				#endif
				steward.LoadNextScene ("TitleView");
			});
		}));
	}

	public void PushRightSlideButton () {
		if (isSelectForce) {
			steward.PlaySETap ();
			isSelectForce = false;
			slideIndex = (slideIndex + 1) % selectableForceIds.Count;
			forceEnter.transform.Find ("DescriptionMask").GetComponent<ScrollRect> ().verticalNormalizedPosition = 1.0f;
			SetForceData ();
		}
	}
	
	public void PushLeftSlideButton () {
		if (isSelectForce) {
			steward.PlaySETap ();
			isSelectForce = false;
			slideIndex = (slideIndex - 1 + selectableForceIds.Count) % selectableForceIds.Count;
			forceEnter.transform.Find ("DescriptionMask").GetComponent<ScrollRect> ().verticalNormalizedPosition = 1.0f;
			SetForceData ();
		}
	}

	private void SetForceData () {
		loadAssetBundle.SetLoadWait (() => {isSelectForce = true;});
		currentForceId = selectableForceIds [slideIndex];

		var defaultDeck = master.staticData ["defaultDecks"] [slideIndex.ToString()] as JObject;

		loadAssetBundle.SetForcePlateImage (currentForceId, (int)LoadAssetBundle.ForcePlateType.LeftTop, forceEnter.transform.Find ("ForcePlateLeftTop").gameObject);
		loadAssetBundle.SetForcePlateImage (currentForceId, (int)LoadAssetBundle.ForcePlateType.RightBottom, forceEnter.transform.Find ("ForcePlateRightBottom").gameObject);
		loadAssetBundle.SetForcePlateImage (currentForceId, (int)LoadAssetBundle.ForcePlateType.Name, forceEnter.transform.Find ("ForceColor").gameObject);
		loadAssetBundle.SetForceIconImage (currentForceId, forceEnter.transform.Find ("ForceIcon").gameObject);

		loadAssetBundle.SetCardImage (defaultDeck ["favoriteCardId"].ToString (), (int)LoadAssetBundle.DisplayType.Card, forceEnter.transform.Find ("PresentMSCard").gameObject);
		loadAssetBundle.SetCardImage (defaultDeck ["favoriteCrewId"].ToString (), (int)LoadAssetBundle.DisplayType.Card, forceEnter.transform.Find ("PresentPilotCard").gameObject);

		forceEnter.transform.Find ("ForceName").GetComponent<Text>().text = defaultDeck["name"].ToString ();
		forceEnter.transform.Find ("DescriptionMask/Description").GetComponent<Text>().text = defaultDeck["contentsInfo"].ToString ().Replace ("<br/>", "\n").Replace ("<br />", "\n");
		forceEnter.transform.Find ("Notice").GetComponent<Text>().text = defaultDeck["description"].ToString ().Replace ("<br/>", "\n").Replace ("<br />", "\n");
	}
}
