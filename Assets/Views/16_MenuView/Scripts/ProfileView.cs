using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Newtonsoft.Json.Linq;

public class ProfileView : MonoBehaviour {
	public GameObject forcePrefab;
	public GameObject emblemPrefab;
	public GameObject titlePrefab;
	public GameObject rankPrefab;
	public GameObject recordPrefab;
	public GameObject matchingRowPrefab;

	private InputField nameInputField;
	private InputField commentInputField;
	private PopUpWindow nameInputWindow;
	private PopUpWindow commentInputWindow;
	private PopUpWindow forceWindow;
	private PopUpWindow emblemWindow;
	private PopUpWindow titleWindow;

	private string selectedForceId;
	private string selectedEmblemId;
	private string selectedTitleId;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Deck deck { get { return Deck.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("ProfileView"); } }
	
	private UserDeckManager userDeckManager = new UserDeckManager();
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		SetProfile ();
	}

	public void OpenUserNameInputWindow (bool playSE = true) {
		if (playSE) steward.PlaySETap ();
		nameInputWindow = steward.OpenCustomWindow("InputWindow", "登録", () => {ChangeUserName();}, "キャンセル", () => {});
		nameInputWindow.SetSubject ("隊員名を入力")
			.SetMessage ("隊員名を登録してください")
			.SetText ("Notice", "※最大で" + Const.USERNAME_IMPUT_LIMIT + "文字まで入力できます")
			.SetText ("Placeholder", "隊員名を入力")
			.SetInputText ("InputArea/InputText", user.tUser ["userName"].ToString ());
		
		nameInputField = nameInputWindow.GetObject("InputArea/InputText").GetComponent<InputField> ();
		nameInputField.onEndEdit.RemoveAllListeners ();
		nameInputField.onEndEdit.AddListener ((string input) => {nameInputField.text = input.Trim ().Substring (0, input.Trim ().Length < Const.USERNAME_IMPUT_LIMIT ? input.Trim ().Length : Const.USERNAME_IMPUT_LIMIT);});
		nameInputField.characterLimit = Const.USERNAME_IMPUT_LIMIT + Const.IMPUT_LIMIT_OVERFLOW;
		EventSystem.current.SetSelectedGameObject (nameInputField.gameObject);
		nameInputField.OnPointerClick (new PointerEventData (EventSystem.current));
		nameInputField.transform.parent.GetComponent<Button> ().onClick.RemoveAllListeners ();
		nameInputField.transform.parent.GetComponent<Button> ().onClick.AddListener (() => {
			EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
			nameInputField.OnPointerClick(new PointerEventData(EventSystem.current));
		});
	}

	public void ChangeUserName () {
		var userName = nameInputField.text.Trim ().Substring (0, nameInputField.text.Trim ().Length < Const.USERNAME_IMPUT_LIMIT ? nameInputField.text.Trim ().Length : Const.USERNAME_IMPUT_LIMIT);
		if (userName.Length == 0 || userName.Length > Const.USERNAME_IMPUT_LIMIT) {
			string errorText = "";
			if (userName.Length == 0) {
				errorText = "隊員名を入力してください";
			} else {
				errorText = "隊員名は" + Const.USERNAME_IMPUT_LIMIT + "文字までです";
			}
			steward.OpenMessageWindow ("エラー", errorText, "閉じる", () => {OpenUserNameInputWindow (false);});
			return;
		}
		steward.SetNewTask (user.setName (userName, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setName:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode (result, () => {OpenUserNameInputWindow (false);})) return;

			steward.ReloadHeader ();
			SetProfile ();
		}));
	}

	public void OpenCommentInputWindow (bool playSE = true) {
		if (playSE) steward.PlaySETap ();
		commentInputWindow = steward.OpenCustomWindow("InputWindow", "登録", () => {ChangeComment();}, "キャンセル", () => {});
		commentInputWindow.SetSubject ("コメントを入力")
			.SetMessage ("コメントを入力してください")
				.SetText ("Notice", "※最大で" + Const.FREEWORD_IMPUT_LIMIT + "文字まで入力できます")
			.SetText ("Placeholder", "コメントを入力")
			.SetInputText ("InputArea/InputText", user.tUser ["freeword"].ToString ());

		commentInputField = commentInputWindow.GetObject("InputArea/InputText").GetComponent<InputField> ();
		commentInputField.onEndEdit.RemoveAllListeners ();
		commentInputField.onEndEdit.AddListener ((string input) => {commentInputField.text = input.Trim ().Substring (0, input.Trim ().Length < Const.FREEWORD_IMPUT_LIMIT ? input.Trim ().Length : Const.FREEWORD_IMPUT_LIMIT);});
		commentInputField.characterLimit = Const.FREEWORD_IMPUT_LIMIT + Const.IMPUT_LIMIT_OVERFLOW;
		EventSystem.current.SetSelectedGameObject (commentInputField.gameObject);
		commentInputField.OnPointerClick (new PointerEventData (EventSystem.current));
		commentInputField.transform.parent.GetComponent<Button> ().onClick.RemoveAllListeners ();
		commentInputField.transform.parent.GetComponent<Button> ().onClick.AddListener (() => {
			EventSystem.current.SetSelectedGameObject(commentInputField.gameObject);
			commentInputField.OnPointerClick(new PointerEventData(EventSystem.current));
		});
	}

	public void ChangeComment () {
		var comment = commentInputField.text.Trim().Substring (0, commentInputField.text.Trim ().Length < Const.FREEWORD_IMPUT_LIMIT ? commentInputField.text.Trim ().Length : Const.FREEWORD_IMPUT_LIMIT);
		if (comment.Length == 0 || comment.Length > Const.FREEWORD_IMPUT_LIMIT) {
			string errorText = "";
			if (comment.Length == 0) {
				errorText = "コメントを入力してください";
			} else {
				errorText = "コメントは" + Const.FREEWORD_IMPUT_LIMIT + "文字までです";
			}
			steward.OpenMessageWindow("エラー", errorText, "閉じる", () => {OpenCommentInputWindow(false);});
			return;
		}
		steward.SetNewTask (user.setFreeword (comment, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setFreeword:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result, () => {OpenCommentInputWindow(false);})) return;

			SetProfile();
		}));
	}

	public void OpenForceSelectWindow () {
		steward.PlaySETap ();
		List<string> forceIds = new List<string> ();
		foreach (JProperty prop in master.staticData ["forces"]) {
			forceIds.Add (prop.Name);
		}

		forceWindow = steward.OpenCustomWindow("SelectWindow", "決定", () => {OpenEmblemSelectWindow();});
		forceWindow.SetSubject ("勢力選択");
		forceWindow.SetText ("SelectionName", "");
		List_Instance listInstance = forceWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ();
		listInstance.Create_Object = forcePrefab;
		listInstance.Create_Number = forceIds.Count;
		listInstance.CreateInstance ();
		
		var panelList = listInstance.GetCreateObjects ();
		int index = 0;
		foreach (var forceId in forceIds) {
			loadAssetBundle.SetForceIconImage (forceId, panelList [index].GetComponent<Image> ().gameObject);
			panelList [index].GetComponent<ProfileForceData> ().StoreData (forceId);
			index++;
		}
		forceWindow.GetObject ("ListArea/List/ListParent").GetComponent<Slide_Action_Ver2> ().Rewind ();
		forceWindow.GetSingleActionButton ().SetActive (false);

		//set default force
		selectedForceId = user.tUser ["color"].ToString ();
		SetForce ();
	}

	public void SelectForce (GameObject obj) {
		selectedForceId = obj.GetComponent<ProfileForceData> ().forceId;
		SetForce ();
	}

	private void SetForce(){
		forceWindow.GetSingleActionButton ().SetActive (true);
		forceWindow.SetText ("SelectionName", master.staticData ["forces"] [selectedForceId] ["name"].ToString ());
		
		var panelList = forceWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<ProfileForceData> ().forceId == selectedForceId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
	}

	public void OpenEmblemSelectWindow () {
		List<string> currentForceEmblem = new List<string> ();
		foreach (JProperty prop in master.staticData ["emblem"]) {
			if ((prop.Value as JObject) ["forceId"].ToString () == selectedForceId) {
				currentForceEmblem.Add (prop.Name);
			}
		}
		
		emblemWindow = steward.OpenCustomWindow("SelectWindow", "決定", () => {ChangeForceEmblem();});
		emblemWindow.SetSubject ("勢力選択");
		emblemWindow.SetText ("SelectionName", "");
		List_Instance listInstance = emblemWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ();
		listInstance.Create_Object = emblemPrefab;
		listInstance.Create_Number = currentForceEmblem.Count;
		listInstance.CreateInstance ();
		
		var panelList = listInstance.GetCreateObjects ();
		int index = 0;
		foreach (var emblemId in currentForceEmblem) {
			loadAssetBundle.SetEmblemImage (emblemId, panelList [index].GetComponent<Image> ().gameObject);
			panelList [index].GetComponent<ProfileEmblemData> ().StoreData (emblemId);
			index++;
		}
		emblemWindow.GetObject ("ListArea/List/ListParent").GetComponent<Slide_Action_Ver2> ().Rewind ();
		emblemWindow.GetSingleActionButton ().SetActive (false);

		//set default emblem
		if (selectedForceId == user.tUser ["color"].ToString ()) {
			selectedEmblemId = user.tUser ["emblemId"].ToString ();
			SetEmblem ();
		}
	}

	public void SelectEmblem (GameObject obj) {
		selectedEmblemId = obj.GetComponent<ProfileEmblemData> ().emblemId;
		SetEmblem ();
	}

	private void SetEmblem(){
		emblemWindow.GetSingleActionButton ().SetActive (true);
		emblemWindow.SetText ("SelectionName", master.staticData ["emblem"] [selectedEmblemId] ["name"].ToString ());
		
		var panelList = emblemWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<ProfileEmblemData> ().emblemId == selectedEmblemId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
	}

	public void ChangeForceEmblem () {
		var confirmWindow = steward.InitCustomWindow("ForceEmblemConfirmWindow");
		loadAssetBundle.SetForceIconImage(selectedForceId, confirmWindow.GetObject ("ForceImage").GetComponent<Image>().gameObject);
		loadAssetBundle.SetEmblemImage(selectedEmblemId, confirmWindow.GetObject ("EmblemImage").GetComponent<Image>().gameObject);
		confirmWindow.SetText ("ForceText", master.staticData ["forces"] [selectedForceId] ["name"].ToString ());
		confirmWindow.SetText ("EmblemText", master.staticData ["emblem"] [selectedEmblemId] ["name"].ToString ());
		steward.OpenCustomWindow (confirmWindow, "決定", () => {
			steward.SetNewTask (user.setColor (selectedForceId, selectedEmblemId, (Exception e, JToken result) => {
				if (e != null) {
					logger.data (e).error ("Error setColor:");
					steward.OpenExceptionPopUpWindow ();
					return;
				}
				if (steward.ResponseHasErrorCode (result))
					return;
				
				steward.ReloadHeader ();
				SetProfile ();
			}));
		}, "やめる", () => {});
	}

	public void OpenTitleSelectWindow () {
		steward.PlaySETap ();
		titleWindow = steward.InitCustomWindow("VerticalSelectWindow");

		foreach (var title in user.tTitles["titleId"]) {
			var clone = Instantiate (titlePrefab);
			clone.transform.SetParent(titleWindow.GetObject ("ScrollView/Content").transform);
			clone.transform.localPosition = titlePrefab.transform.localPosition;
			clone.transform.localScale = titlePrefab.transform.localScale;
			clone.transform.Find ("Out_Window").GetComponent<ProfileTitleData> ().StoreData (title.ToString ());
			clone.transform.Find ("Out_Window/Text").GetComponent<Text> ().text = master.staticData ["titles"] [title.ToString ()] ["name"].ToString ();
			clone.transform.Find ("Out_Window/SelectEnclose").gameObject.SetActive (false);
		}
		steward.OpenCustomWindow (titleWindow, "決定", () => {ChangeTitle();});
		titleWindow.GetSingleActionButton ().SetActive (false);
	}
	
	public void SelectTitle (GameObject obj) {
		selectedTitleId = obj.transform.Find("Out_Window").GetComponent<ProfileTitleData> ().titleId;
		titleWindow.GetSingleActionButton ().SetActive (true);

		foreach (Transform n in titleWindow.GetObject ("ScrollView/Content").transform) {
			if (n.Find ("Out_Window").GetComponent<ProfileTitleData> ().titleId == selectedTitleId) {
				n.Find ("Out_Window/SelectEnclose").gameObject.SetActive (true);
			} else {
				n.Find ("Out_Window/SelectEnclose").gameObject.SetActive (false);
			}
		}
	}

	public void ChangeTitle () {
		steward.SetNewTask (user.setTitle (selectedTitleId, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setTitle:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			if (steward.ResponseHasErrorCode (result))
				return;
			
			SetProfile ();
		}));
	}
	
	public void OpenRankWindow () {
		steward.PlaySETap ();
		var rankWindow = steward.InitCustomWindow ("RankWindow");
		foreach (JProperty prop in master.staticData ["forces"]) {
			var rankRow = Instantiate (rankPrefab);
			var color = prop.Name;
			rankRow.transform.SetParent(rankWindow.GetObject ("ScrollView/Content").transform);
			rankRow.transform.localPosition = rankPrefab.transform.localPosition;
			rankRow.transform.localScale = rankPrefab.transform.localScale;
			loadAssetBundle.SetRankedMatchFrameImage(color, rankRow.transform.Find ("OutWindow").GetComponent<Image> ().gameObject);

			JObject currentClass;
			String currentExp;
			if (game.tClasses[color] != null) {
				var classOfColor = game.tClasses[color] as JObject;
				currentClass = master.staticData ["classes"] [classOfColor ["classId"].ToString()] as JObject;
				currentExp = classOfColor ["exp"].ToString();
			} else {
				currentClass = master.staticData ["classes"] ["class" + char.ToUpper(color[0]) + color.Substring(1) + "001"] as JObject;
				currentExp = "0";
			}
			loadAssetBundle.SetClassIconImage (currentClass ["id"].ToString (), rankRow.transform.Find ("ClassIcon").GetComponent<Image> ().gameObject);
			rankRow.transform.Find ("ClassText").GetComponent<Text> ().text = currentClass ["name"].ToString ();
			rankRow.transform.Find ("CurrentExp/ExpText").GetComponent<Text> ().text = currentExp;
			if (currentClass ["ascendClassId"] != null) {
				rankRow.transform.Find ("RankUpCondition/RankUpExp").GetComponent<Text> ().text = currentClass ["expThreshold"].ToString ();
			} else {
				rankRow.transform.Find ("RankUpCondition/RankUpExp").GetComponent<Text> ().text = "-";
				rankRow.transform.Find ("RankUpCondition/RankUpSuffix").GetComponent<Text> ().text = "";
			}
			if (currentClass ["descendClassId"] != null) {
				rankRow.transform.Find ("RankDownCondition/RankDownExp").GetComponent<Text> ().text = master.staticData ["classes"] [currentClass ["descendClassId"].ToString ()] ["expThreshold"].ToString ();
			} else {
				rankRow.transform.Find ("RankDownCondition/RankDownExp").GetComponent<Text> ().text = "-";
				rankRow.transform.Find ("RankDownCondition/RankDownSuffix").GetComponent<Text> ().text = "";
			}
		}
		steward.OpenCustomWindow (rankWindow, "閉じる", () => {});
	}

	public void OpenRecordWindow () {
		steward.PlaySETap ();
		var recordWindow = steward.InitCustomWindow ("RecordWindow");
		// display casual match data
		var casualMatch = Instantiate (recordPrefab);
		casualMatch.transform.SetParent(recordWindow.GetObject ("ScrollView/Content").transform);
		casualMatch.transform.localPosition = recordPrefab.transform.localPosition;
		casualMatch.transform.localScale = recordPrefab.transform.localScale;
		int totalMatchCount = int.Parse (game.tRecords ["casual"] ["winCount"].ToString ()) + int.Parse (game.tRecords ["casual"] ["loseCount"].ToString ());
		casualMatch.transform.Find ("Out_Window/MatchName").GetComponent<Text> ().text = "演習";
		casualMatch.transform.Find ("Out_Window/TotalBattle_Num").GetComponent<Text> ().text = totalMatchCount.ToString();
		casualMatch.transform.Find ("Out_Window/Win_Num").GetComponent<Text> ().text = (int.Parse (game.tRecords ["casual"] ["winCount"].ToString ())).ToString();
		casualMatch.transform.Find ("Out_Window/Win_Parsent_Num").GetComponent<Text> ().text = (totalMatchCount > 0 ? ((float.Parse (game.tRecords ["casual"] ["winCount"].ToString ()) / (float)totalMatchCount) * 100).ToString("F") : "0.00") + "%";

		// display casual match data
		foreach (JProperty prop in game.tRecords ["ranked"]) {
			var obj = prop.Value as JObject;
			var rankedMatch = Instantiate (recordPrefab);
			var color = prop.Name;
			rankedMatch.transform.SetParent(recordWindow.GetObject ("ScrollView/Content").transform);
			rankedMatch.transform.localPosition = recordPrefab.transform.localPosition;
			rankedMatch.transform.localScale = recordPrefab.transform.localScale;
			loadAssetBundle.SetRankedMatchFrameImage(color, rankedMatch.transform.Find ("Out_Window").GetComponent<Image> ().gameObject);

			totalMatchCount = int.Parse (obj ["winCount"].ToString ()) + int.Parse (obj ["loseCount"].ToString ());
			rankedMatch.transform.Find ("Out_Window/MatchName").GetComponent<Text> ().text = "階級戦 (" + master.staticData["forces"] [color] ["name"].ToString () + ")";
			rankedMatch.transform.Find ("Out_Window/TotalBattle_Num").GetComponent<Text> ().text = totalMatchCount.ToString();
			rankedMatch.transform.Find ("Out_Window/Win_Num").GetComponent<Text> ().text = (int.Parse (obj ["winCount"].ToString ())).ToString();
			rankedMatch.transform.Find ("Out_Window/Win_Parsent_Num").GetComponent<Text> ().text = (totalMatchCount > 0 ? ((float.Parse (obj ["winCount"].ToString ()) / (float)totalMatchCount) * 100).ToString("F") : "0.00") + "%";
		}
		steward.OpenCustomWindow (recordWindow, "閉じる", () => {});
	}
	
	public void OpenMatchingLogWindow () {
		steward.PlaySETap ();
		steward.SetNewTask (user.getMatchingLog ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error getSerialLog:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;
			
			var historyWindow = steward.InitCustomWindow("HistoryWindow");
			historyWindow
				.SetSubject ("対戦履歴")
				.SetMessage ("10" + historyWindow.GetObject("Message").GetComponent<Text> ().text);
			
			JArray resultArr = result as JArray;
			int index = 0;
			foreach (var token in resultArr) {
				var obj = token as JObject;
				var clone = Instantiate (matchingRowPrefab);
				clone.transform.SetParent(historyWindow.GetObject ("ScrollView/Content").transform);
				clone.transform.localPosition = matchingRowPrefab.transform.localPosition;
				clone.transform.localScale = matchingRowPrefab.transform.localScale;
				clone.transform.Find ("Out_Window/EnemyUserName").GetComponent<Text>().text = obj["enemyName"].ToString();
				clone.transform.Find ("Out_Window/Date").GetComponent<Text>().text = obj["date"].ToString();
				clone.transform.Find ("Out_Window/BattleResult").GetComponent<Text>().text = bool.Parse (obj["isWin"].ToString()) ? "WIN" : "LOSE";
				index++;
			}
			
			historyWindow = steward.OpenCustomWindow (historyWindow, "閉じる", () => {});
		}));
	}

	private void SetProfile () {	
		var mainDeck = userDeckManager.mainDeck;
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.LeftTop, transform.Find ("ForcePlate1").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.RightBottom, transform.Find ("ForcePlate2").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetEmblemImage (user.tUser ["emblemId"].ToString (), transform.Find ("ForceEmblem").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetAvatarImage (mainDeck.avatarId, transform.Find ("AvaterImage").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetClassIconImage (user.tUser ["classId"].ToString (), transform.Find ("ClassIcon").GetComponent<Image> ().gameObject);
		transform.Find ("ClassText").GetComponent<Text> ().text = master.staticData ["classes"] [user.tUser ["classId"].ToString ()] ["name"].ToString ();
		transform.Find ("TitleText").GetComponent<Text> ().text = master.staticData ["titles"] [user.tUser ["titleId"].ToString ()] ["name"].ToString ();
		transform.Find ("NameText").GetComponent<Text> ().text = user.tUser ["userName"].ToString ();
		transform.Find ("CommentText").GetComponent<Text> ().text = user.tUser ["freeword"].ToString ();
		transform.Find ("DogTagText").GetComponent<Text> ().text = user.tUser ["dogTag"].ToString ();
		transform.Find ("DeckNameText").GetComponent<Text> ().text = mainDeck.deckName;
		transform.Find ("Exp").GetComponent<Text> ().text = game.tClasses [user.tUser ["color"].ToString ()] != null && game.tClasses [user.tUser ["color"].ToString ()] ["exp"] != null ? game.tClasses [user.tUser ["color"].ToString ()] ["exp"].ToString () : "0";
	}
}
