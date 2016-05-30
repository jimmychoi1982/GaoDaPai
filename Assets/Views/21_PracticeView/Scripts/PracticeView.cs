using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class PracticeView : MonoBehaviour {
	public GameObject difficultySelect;
	public GameObject forceSelectEasy;
	public GameObject forceSelectNormal;
	public GameObject canvasLive2D;
	public GameObject canvasWindow;
	public GameObject soliderContent;
	public GameObject matchingObject;
	public GameObject deckSelect;
	public GameObject BP_ShortageDialog;
	public GameObject decisionDialog;
	public GameObject waitLobby;
	public GameObject normalMask;

	public Text BP;
	public Text ReplenishingSupplies;

	public bool useBanCard;

	public GameObject deckList;
	
	private Dictionary<int, UserDeck> userDecks = new Dictionary<int, UserDeck> ();
	private GameObject[] deckPlates;
	private int currentDeckId;
	private int mainDeckId;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Deck deck { get { return Deck.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Logger logger { get { return mage.logger("PracticeView"); } }
	private Master master { get { return Master.Instance; }}
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private UserDeckManager userDeckManager = new UserDeckManager();

	private Steward steward;
	Matching_Manager matchingManager;

	private ViewMode viewMode = ViewMode.DifficultySelect;
	public enum ViewMode {
		DifficultySelect,
		ForceSelectEasy,
		ForceSelectNormal,
		DeckSelect,
		WaitMatch,
		GotoGame
	}
	
	const string SELECTED_DIFFICULTY_EASY = "easy";
	const string SELECTED_DIFFICULTY_NORMAL = "normal";
	Dictionary<string, string> DifficultyList = new Dictionary<string, string> ();
	
	const string SELECTED_FORCE_BLUE = "blue";
	const string SELECTED_FORCE_GREEN = "green";
	const string SELECTED_FORCE_YELLOW = "yellow";
	const string SELECTED_FORCE_BLACK = "black";
	const string SELECTED_FORCE_RED = "red";
	Dictionary<string, string> ForceList = new Dictionary<string, string> ();

	const string QUEUE_TYPE_PRACTICE = "practice";

	const string DECK_STATUS_OK = "1";
	const string DECK_STATUS_INVALID_CARD = "2";

	public JToken tUserCpuRecords;

	Cpu cpu { get { return Cpu.Instance; }}

	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		DifficultyList.Add (SELECTED_DIFFICULTY_EASY, "初級");
		DifficultyList.Add (SELECTED_DIFFICULTY_NORMAL, "中級");

		ForceList.Add (SELECTED_FORCE_BLUE, "青");
		ForceList.Add (SELECTED_FORCE_GREEN, "緑");
		ForceList.Add (SELECTED_FORCE_YELLOW, "黄");
		ForceList.Add (SELECTED_FORCE_BLACK, "黒");
		ForceList.Add (SELECTED_FORCE_RED, "赤");

		userDecks = userDeckManager.GetAll ();

		mainDeckId = int.Parse (user.tUser ["mainDeckId"].ToString ());

		deckSelect.SetActive (true);

		deckList.GetComponent<List_Instance> ().Create_Number = userDecks.Count;
		deckList.GetComponent<List_Instance> ().CreateInstance ();
		deckPlates = deckList.GetComponent<List_Instance> ().GetCreateObjects ();

		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.LeftTop, deckSelect.transform.Find ("Back_Ground_Left/deck_board_b1").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.RightBottom, deckSelect.transform.Find ("Back_Ground_Left/deck_board_b2").GetComponent<Image> ().gameObject);

		ChangeDeck (mainDeckId);

		deckSelect.SetActive (false);

		matchingManager = matchingObject.GetComponent<Matching_Manager> ();

		if (GameSettings.LastQueueType == QUEUE_TYPE_PRACTICE) {
			OpenForceSelect(GameSettings.CPUDifficultyType);
			GameSettings.LastQueueType = "";
		}
		ResetAllClearIcons ();

		tUserCpuRecords = game.tRecords ["cpu"];

		foreach (string difficulty in DifficultyList.Keys) {
			foreach (string color in ForceList.Keys) {
				if (tUserCpuRecords[difficulty][color].Value<int>("winCount") >= 1) {
					ActiveClearIcon(difficulty, color);
				}
			}

			var difficultyClearFlag = CheckDifficultyClearStatus(tUserCpuRecords, difficulty);
			if (difficultyClearFlag == true) {
				ActiveCompleteIcon(difficulty);

				if (difficulty == "easy") {
					Transform difficultySelectButtonNormal = GameObject.Find ("/Main Canvas/Panel/Difficulty_Select/SelectButton_normal").transform;
					difficultySelectButtonNormal.GetComponent<Button> ().onClick.AddListener(() => {OpenForceSelect("normal");});
					difficultySelectButtonNormal.GetComponent<Scale_Pop> ().isPop = true;
					normalMask.SetActive(false);
				}
			}
		}
	}

	private void GetRewards () {
		steward.OpenMessageWindow ("攻略報酬", 
		                           "初めて" + DifficultyList [GameSettings.CPUDifficultyType] + "の" + ForceList [GameSettings.CPUForceColorType] + "勢力を撃破しました！\n" + 
		                           "以下を報酬としてアイテムボックスに支給しました。\n" + 
		                           "item1\n" + 
		                           "item2", 
		                         "確認", () => {});
	}

	public void OpenForceSelect (string difficulty) {
		if (GameSettings.LastQueueType != QUEUE_TYPE_PRACTICE) {
			steward.PlaySETap();
		}

		GameSettings.CPUDifficultyType = difficulty;

		if (GameSettings.CPUDifficultyType == SELECTED_DIFFICULTY_EASY) {
			ChangeView (ViewMode.ForceSelectEasy);

		} else if (GameSettings.CPUDifficultyType == SELECTED_DIFFICULTY_NORMAL) {
			ChangeView (ViewMode.ForceSelectNormal);

		}
	}

	public void ResetAllClearIcons () {
		foreach (string difficulty in DifficultyList.Keys) {
			Transform completeIcon = GameObject.Find ("/Main Canvas/Panel/Difficulty_Select/SelectButton_"
			                                          + (string)difficulty
			                                          + "/BackImage/Clear_Icon").transform;
			completeIcon.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);

			foreach (string color in ForceList.Keys) {
				Transform clearIcon = GameObject.Find ("/Main Canvas/Panel/Force_Select_" 
					+ (string)difficulty 
					+ "/ScrollView/SoliderContent/Solider_Panel_"
					+ (string)color
					+ "/Window/Clear_Icon").transform;
				clearIcon.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
			}
		}
	}

	public bool CheckDifficultyClearStatus (JToken records,string difficulty) {
		foreach (string color in ForceList.Keys) {
			if (records[difficulty][color].Value<int>("winCount") == 0) {
				return false;
			}
		}

		return true;
	}

	public void ActiveClearIcon (string difficulty, string color) {
		Transform clearIcon = GameObject.Find ("/Main Canvas/Panel/Force_Select_" 
		                                       + (string)difficulty 
		                                       + "/ScrollView/SoliderContent/Solider_Panel_"
		                                       + (string)color
		                                       + "/Window/Clear_Icon").transform;
		clearIcon.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
	}

	public void ActiveCompleteIcon (string difficulty) {
		Transform completeIcon = GameObject.Find ("/Main Canvas/Panel/Difficulty_Select/SelectButton_"
		                                          + (string)difficulty
		                                          + "/BackImage/Clear_Icon").transform;
		completeIcon.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
	}

	public void OpenDeckSelect (string force) {
		steward.PlaySETap ();
		GameSettings.CPUForceColorType = force;
		ChangeView (ViewMode.DeckSelect);
	}

	public void Back(bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		switch (viewMode) {
		case ViewMode.DifficultySelect:
			steward.LoadNextScene ("HomeView");
			break;
			
		case ViewMode.ForceSelectEasy:
			ChangeView (ViewMode.DifficultySelect);
			break;

		case ViewMode.ForceSelectNormal:
			ChangeView (ViewMode.DifficultySelect);
			break;

		case ViewMode.DeckSelect:
			if (GameSettings.CPUDifficultyType == SELECTED_DIFFICULTY_EASY) {
				ChangeView (ViewMode.ForceSelectEasy);
				break;
			
			} else if (GameSettings.CPUDifficultyType == SELECTED_DIFFICULTY_NORMAL) {
				ChangeView (ViewMode.ForceSelectNormal);
				break;
			}

			break;
			
/*		case ViewMode.WaitMatch:
			steward.SetNewTask (matchMaking.cancel((Exception error) => {
				if (error != null) {
					logger.data(error).error("Error when cancelling matchmaking");
					return;
				}
				
				ChangeView (ViewMode.DeckSelect);
			}));
			break;*/
		}
	}

	public void BattleStart () {
		steward.PlaySEOK ();
		if (useBanCard == false) {
			steward.LoadNextScene ("GameView");
		} else {
			steward.OpenMessageWindow ("エラー", "参加条件を満たしていません", "閉じる", () => {});
		}
	}

	public void SelectDeck (GameObject obj) {
		steward.PlaySETap ();
		
		ChangeDeck (obj.GetComponent<DeckData>().DeckId);
	}

	public void ChangeDeck (int deckId) {
		var currentDeck = userDecks [deckId];
		
		currentDeckId = deckId;

		var index = 0;
		foreach (var kv in userDecks) {
			var userDeck = kv.Value;
			deckPlates [index].GetComponent<DeckData> ().Init (kv.Value, "practice", currentDeckId);
			index++;
		}
		
		Transform deckSelect = GameObject.Find ("/Main Canvas/Panel/Deck_Select").transform;
		loadAssetBundle.SetLoadWait ();
		loadAssetBundle.SetAvatarImage (currentDeck.avatarId, deckSelect.Find ("Avater").gameObject);
		loadAssetBundle.SetMothershipImage (currentDeck.mothershipId, deckSelect.Find ("Mother_Ship").gameObject);
		loadAssetBundle.SetSleeveImage (currentDeck.unitSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckSelect.Find ("MS_Sleve").gameObject);
		loadAssetBundle.SetSleeveImage (currentDeck.characterSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckSelect.Find ("Cost_Sleve").gameObject);
		loadAssetBundle.SetCardImage (currentDeck.favoriteCharacter, (int)LoadAssetBundle.DisplayType.Card, deckSelect.Find ("Cost_Deck").gameObject);
		loadAssetBundle.SetCardImage (currentDeck.favoriteUnit, (int)LoadAssetBundle.DisplayType.Card, deckSelect.Find ("MS_Deck").gameObject);
		
		deckSelect.Find ("Deck_Name_Window/This_Deck_Name").GetComponent<Text> ().text = currentDeck.deckName;

		deckSelect.transform.Find ("MainDeck_Button").GetComponent<Button> ().onClick.RemoveAllListeners();
		deckSelect.transform.Find ("MainDeck_Button").GetComponent<Image> ().color = new Color(1f, 1f, 1f, 0.5f);
		deckSelect.transform.Find ("MainDeck_Button").GetComponent<Scale_Pop> ().isPop = false;
		
		// TODO: Do a real validateDeck on the server side
		string validateDeck = DECK_STATUS_OK;
		
		if (validateDeck == DECK_STATUS_INVALID_CARD) {
			steward.OpenMessageWindow ("確認", "出撃条件を満たしていません\nデッキに入れられないカードが\nデッキに含まれています", "閉じる", () => {});
		} else {
			deckSelect.transform.Find ("MainDeck_Button").GetComponent<Button> ().onClick.AddListener(() => {SubmitMatch();});
			deckSelect.transform.Find ("MainDeck_Button").GetComponent<Image> ().color = new Color(1f, 1f, 1f, 1f);
			deckSelect.transform.Find ("MainDeck_Button").GetComponent<Scale_Pop> ().isPop = true;
		}
	}

	private void ChangeView (ViewMode _viewMode) {
		viewMode = _viewMode;
		switch (viewMode) {
		case ViewMode.DifficultySelect:
			steward.ShowHeader();
			steward.SetHeaderViewName("訓練モード");
			steward.ShowLive2DOperator();
			forceSelectEasy.SetActive(false);
			forceSelectNormal.SetActive(false);
			difficultySelect.SetActive(true);
			deckSelect.SetActive (false);
			break;

		case ViewMode.ForceSelectEasy:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("敵勢力選択");
			steward.ShowLive2DOperator ();
			forceSelectEasy.SetActive (true);
			difficultySelect.SetActive (false);
			deckSelect.SetActive (false);
			waitLobby.SetActive (false);
			break;

		case ViewMode.ForceSelectNormal:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("敵勢力選択");
			steward.ShowLive2DOperator ();
			forceSelectNormal.SetActive (true);
			difficultySelect.SetActive (false);
			deckSelect.SetActive (false);
			waitLobby.SetActive (false);
			break;

		case ViewMode.DeckSelect:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("デッキ選択");
			steward.HideLive2DOperator ();
			forceSelectEasy.SetActive (false);
			forceSelectNormal.SetActive (false);
			difficultySelect.SetActive (false);
			deckSelect.SetActive (true);
			waitLobby.SetActive (false);
			break;
		
		case ViewMode.WaitMatch:
			steward.HideRankPopUp ();
			steward.HideHeader ();
			steward.HideLive2DOperator ();
			forceSelectEasy.SetActive (false);
			forceSelectNormal.SetActive(false);
			difficultySelect.SetActive (false);
			deckSelect.SetActive (false);
			waitLobby.SetActive (true);
			break;
		}
	}

	public void SubmitMatch () {
		var currentDeck = userDecks [currentDeckId];

		Debug.Log ("SubmitMatch");
		
		steward.PlaySETap ();

		ChangeView (ViewMode.WaitMatch);

		loadAssetBundle.SetAvatarImage (currentDeck.avatarId, waitLobby.transform.Find ("PlayerInformation/PlayerBox/Avatar/AvatarImage").gameObject);
		loadAssetBundle.SetCardImage (currentDeck.favoriteUnit, (int)LoadAssetBundle.DisplayType.Card, waitLobby.transform.Find ("PlayerInformation/PlayerBox/Card").gameObject);
		loadAssetBundle.SetClassIconImage (user.tUser ["classId"].ToString (), waitLobby.transform.Find ("PlayerInformation/PlayerBox/ClassIcon").gameObject);
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.LeftTop, waitLobby.transform.Find ("PlayerInformation/PlayerBox/BackPlate/UIProfilePlate02").gameObject);
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.RightBottom, waitLobby.transform.Find ("PlayerInformation/PlayerBox/BackPlate/UIProfilePlate03").gameObject);
		loadAssetBundle.SetEmblemImage (user.tUser ["emblemId"].ToString (), waitLobby.transform.Find ("PlayerInformation/PlayerBox/BackPlate/Emblem").gameObject);
		waitLobby.transform.Find ("PlayerInformation/PlayerBox/NowClass").GetComponent<Text> ().text = master.staticData ["classes"] [user.tUser ["classId"].ToString ()] ["name"].ToString ();
		waitLobby.transform.Find ("PlayerInformation/PlayerBox/UserNameText").GetComponent<Text> ().text = user.tUser ["userName"].ToString ();
		waitLobby.transform.Find ("PlayerInformation/PlayerBox/TitleText").GetComponent<Text> ().text = master.staticData ["titles"] [user.tUser ["titleId"].ToString ()] ["name"].ToString ();
		waitLobby.transform.Find ("PlayerInformation/PlayerBox/IntroText").GetComponent<Text> ().text = user.tUser ["freeword"].ToString ();
		
		new Task (cpu.queue (currentDeckId, GetCpuId(), (Exception error) => {
			if (error != null) {
				logger.data (error).error ("Error during match making:");
				ChangeView (ViewMode.DeckSelect);
				return;
			}

			// Game is ready to be started, load game view
			GameSettings.LastQueueType = QUEUE_TYPE_PRACTICE;
			logger.info ("Found a match! opening game view");

			foreach (var token in game.tCurrentGame["playerData"]) {
				if (token is JProperty) {
					if ((token as JProperty).Name != GameSettings.UserId) {
						var obj = (token as JProperty).Value;

						loadAssetBundle.SetAvatarImage (obj ["avatarId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/Avatar/AvatarImage").gameObject);
						loadAssetBundle.SetCardImage (obj ["favoriteCardId"].ToString (), (int)LoadAssetBundle.DisplayType.Card, waitLobby.transform.Find ("EnemyInformation/EnemyBox/Card").gameObject);
						loadAssetBundle.SetClassIconImage (obj ["classId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/ClassIcon").gameObject);
						loadAssetBundle.SetForcePlateImage (obj ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.LeftTop, waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/UIProfilePlate02").gameObject);
						loadAssetBundle.SetForcePlateImage (obj ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.RightBottom, waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/UIProfilePlate03").gameObject);
						loadAssetBundle.SetEmblemImage (obj ["emblemId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/Emblem").gameObject);
						waitLobby.transform.Find ("EnemyInformation/EnemyBox/NowClass").GetComponent<Text> ().text = master.staticData ["classes"] [obj ["classId"].ToString ()] ["name"].ToString ();
						waitLobby.transform.Find ("EnemyInformation/EnemyBox/UserNameText").GetComponent<Text> ().text = obj ["name"].ToString ();
						waitLobby.transform.Find ("EnemyInformation/EnemyBox/TitleText").GetComponent<Text> ().text = master.staticData ["titles"] [obj ["titleId"].ToString ()] ["name"].ToString ();
						waitLobby.transform.Find ("EnemyInformation/EnemyBox/IntroText").GetComponent<Text> ().text = obj ["freeword"].ToString ();

						break;
					}
				}
			}

			matchingManager.Matching = true;
		}));
	}
	
	private string GetCpuId() {
		string cpuId = "cpu:";
		//
		if (GameSettings.CPUDifficultyType == SELECTED_DIFFICULTY_EASY) {
			cpuId += "easy";
		} else {
			cpuId += "normal";
		}
		//
		cpuId += "-";
		cpuId += GameSettings.CPUForceColorType;
		return cpuId;
	}
}