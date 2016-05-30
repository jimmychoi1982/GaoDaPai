using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class MultiplayView : MonoBehaviour {
	public GameObject matchSelect;
	public GameObject casualButton;
	public GameObject rankedButton;
	public GameObject lowRankButton;
	public GameObject anyRankButton;
	public GameObject highRankButton;
	public GameObject rankedMatch;
	public GameObject deckView;
	public GameObject waitLobby;
	public GameObject matchingObject;
	public GameObject deckList;
	public GameObject conditionRowPrefab;
	public GameObject contributionRowPrefab;

	public GameObject conditionTab;
	public GameObject contributionTab;
	public Sprite tabActiveImage;
	public Sprite tabPassiveImage;

	public bool useBanCard;
	
	private bool isMatching;
	private JToken validateDeck;

	Matching_Manager matchingManager;
	
	private Dictionary<int, UserDeck> userDecks = new Dictionary<int, UserDeck> ();
	private GameObject[] deckPlates;
	private int currentDeckId;
	private int mainDeckId;

	private List<string> matchIds = new List<string> ();

	private ViewMode viewMode = ViewMode.MatchSelect;
	public enum ViewMode {
		MatchSelect = 0,
		RankedMatch,
		DeckSelect,
		WaitMatch,
		GotoGame
	}

	//
	const string QUEUE_TYPE_CASUAL = "casual";
	const string QUEUE_TYPE_LOW_RANK = "lowRank";
	const string QUEUE_TYPE_ANY_RANK = "anyRank";
	const string QUEUE_TYPE_HIGH_RANK = "highRank";

	const string DECK_STATUS_OK = "1";
	const string DECK_STATUS_INVALID_CARD = "2";
	const string DECK_STATUS_INVALID_COLOR = "3";

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Deck deck { get { return Deck.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Purchase purchase { get { return Purchase.Instance; }}
	MatchMaking matchMaking { get { return MatchMaking.Instance; }}
	private Logger logger { get { return mage.logger("MultiplayView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private UserDeckManager userDeckManager = new UserDeckManager();

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		userDecks = userDeckManager.GetAll ();

		steward.SetNewTask (user.getMatch (user.tUser["color"].ToString (), (Exception e, JToken result) => {
			if (e != null) {
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode (result)) return;

			foreach (var token in result ["matchIds"] as JArray) {
				matchIds.Add (token.ToString ());
			}

			if (matchIds.Contains (QUEUE_TYPE_CASUAL)) {
				casualButton.transform.Find ("Shutter").gameObject.SetActive (false);
				casualButton.GetComponent<Button> ().interactable = true;
				casualButton.GetComponent<Scale_Pop> ().isPop = true;
			}
			if (matchIds.Contains (QUEUE_TYPE_LOW_RANK) || matchIds.Contains (QUEUE_TYPE_ANY_RANK) || matchIds.Contains (QUEUE_TYPE_HIGH_RANK)) {
				rankedButton.transform.Find ("Shutter").gameObject.SetActive (false);
				rankedButton.GetComponent<Button> ().interactable = true;
				rankedButton.GetComponent<Scale_Pop> ().isPop = true;
				UnityAction errorMessage = () => {
					steward.OpenMessageWindow("確認", "出撃条件を満たしていません\n出撃制限の階級をご確認ください", "閉じる", () => {});
				};

				if (!matchIds.Contains (QUEUE_TYPE_LOW_RANK)) {
					lowRankButton.GetComponent<Button>().onClick.AddListener(errorMessage);
				} else {
					lowRankButton.GetComponent<Button>().onClick.AddListener(() => {
						EnterMatch (QUEUE_TYPE_LOW_RANK);
					});
				}
				if (!matchIds.Contains (QUEUE_TYPE_ANY_RANK)) {
					anyRankButton.GetComponent<Button>().onClick.AddListener(errorMessage);
				} else {
					anyRankButton.GetComponent<Button>().onClick.AddListener(() => {
						EnterMatch (QUEUE_TYPE_ANY_RANK);
					});
				}
				if (!matchIds.Contains (QUEUE_TYPE_HIGH_RANK)) {
					highRankButton.GetComponent<Button>().onClick.AddListener(errorMessage);
				} else {
					highRankButton.GetComponent<Button>().onClick.AddListener(() => {
						EnterMatch (QUEUE_TYPE_HIGH_RANK);
					});
				}
			}
			lowRankButton.transform.Find ("Condition").GetComponent<Text> ().text = "出撃制限：" + result ["rankMatchCondition"] [QUEUE_TYPE_LOW_RANK].ToString ();
			anyRankButton.transform.Find ("Condition").GetComponent<Text> ().text = "出撃制限：" + result ["rankMatchCondition"] [QUEUE_TYPE_ANY_RANK].ToString ();
			highRankButton.transform.Find ("Condition").GetComponent<Text> ().text = "出撃制限：" + result ["rankMatchCondition"] [QUEUE_TYPE_HIGH_RANK].ToString ();

			validateDeck = result ["validateDeck"];

			mainDeckId = int.Parse (user.tUser ["mainDeckId"].ToString ());
			currentDeckId = mainDeckId;

			deckView.SetActive (true);
			
			deckList.GetComponent<List_Instance> ().Create_Number = userDecks.Count;
			deckList.GetComponent<List_Instance> ().CreateInstance ();
			deckPlates = deckList.GetComponent<List_Instance> ().GetCreateObjects ();

			loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.LeftTop, deckView.transform.Find ("BackgroundLeft/deck_board_b1").GetComponent<Image> ().gameObject);
			loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.RightBottom, deckView.transform.Find ("BackgroundLeft/deck_board_b2").GetComponent<Image> ().gameObject);
			
			deckView.SetActive (false);

			matchingManager = matchingObject.GetComponent<Matching_Manager> ();
		}));
	}

	public void EnterMatch (string queueType) {
		steward.PlaySETap ();
		
		if ((queueType == QUEUE_TYPE_CASUAL && int.Parse(user.tUser ["battlePoint"].ToString ()) >= 3) || (queueType != QUEUE_TYPE_CASUAL && int.Parse(user.tUser ["battlePoint"].ToString ()) >= 10)) {
			GameSettings.LastQueueType = queueType;
			ChangeView (ViewMode.DeckSelect);
		} else {
			steward.OpenDialogWindow ("確認", "【BP】が不足しています\n" +
			                          "【補給物資】を使用して【BP】を回復しますか？", "はい", () => {UseReplenishingSupplies();}, "いいえ", () => {});
		}
	}
		
	public void OpenRankedMatch () {
		steward.PlaySETap ();
		ChangeView (ViewMode.RankedMatch);
	}

	public void OpenRankedMatchWebPage () {
		steward.OpenBrowser ("https://www.gundam-cw.com/app.php?id=6");
	}
	
	public void UseReplenishingSupplies () {
		if (int.Parse (user.tUser["recoveryItems"].ToString()) > 0) {
			steward.SetNewTask (user.useRecoveryItem((Exception e, JToken result) => {
				if (e != null) {
					logger.data(e).error("Error when cancelling matchmaking");
					steward.OpenExceptionPopUpWindow ();
					return;
				}
				
				if (steward.ResponseHasErrorCode(result)) return;
				
				steward.ReloadHeader ();
				steward.OpenMessageWindow ("確認", "【補給物資】を1個使用しました", "閉じる", () => {});
			}));
		} else {
			steward.OpenMessageWindow ("エラー", "【補給物資】の所持数が不足しています", "閉じる", () => {});
		}
	}

	public void SelectDeck (GameObject obj) {
		steward.PlaySETap ();

		ChangeDeck (obj.GetComponent<DeckData>().DeckId);
	}

	public void SubmitMatch () {
		var currentDeck = userDecks [currentDeckId];
		
		steward.PlaySETap ();

		steward.SetNewTask (user.validateMatch (currentDeckId, GameSettings.LastQueueType, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error when cancelling matchmaking");
				steward.OpenExceptionPopUpWindow ();
				return;
			}

			if (steward.ResponseHasErrorCode (result)) return;
			
			if (bool.Parse (result ["result"].ToString ())) {
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

				JToken record = null;
				if (GameSettings.LastQueueType == QUEUE_TYPE_CASUAL) {
					// casual match data
					record = game.tRecords ["casual"];
				} else {
					// ranked match data
					record = game.tRecords ["ranked"] [user.tUser["color"].ToString()];
				}

				int totalMatchCount = record != null ? int.Parse (record ["winCount"].ToString ()) + int.Parse (record ["loseCount"].ToString ()) : 0;
				waitLobby.transform.Find ("PlayerInformation/PlayerBox/BattleNum").GetComponent<Text> ().text = totalMatchCount.ToString ();
				waitLobby.transform.Find ("PlayerInformation/PlayerBox/WinNum").GetComponent<Text> ().text = (record  != null ? int.Parse (record ["winCount"].ToString ()) : 0).ToString ();
				waitLobby.transform.Find ("PlayerInformation/PlayerBox/RateNum").GetComponent<Text> ().text = "(" + (totalMatchCount > 0 ? ((float.Parse (record ["winCount"].ToString ()) / (float)totalMatchCount) * 100).ToString ("F") : "0.00") + "%)";

				new Task (matchMaking.queue (GameSettings.LastQueueType, currentDeckId, (Exception error) => {
					if (error != null) {
						logger.data (error).error ("Error during match making:");
						ChangeView (ViewMode.DeckSelect);
						return;
					}
				
					// Game is ready to be started, load game view
					logger.info ("Found a match! opening game view");
					foreach (var token in game.tCurrentGame["playerData"]) {
						if (token is JProperty) {
							if ((token as JProperty).Name != GameSettings.UserId) {
								var obj = (token as JProperty).Value;
								loadAssetBundle.SetAvatarImage (obj ["avatarId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/Avatar/AvatarImage").gameObject);
								loadAssetBundle.SetSleeveImage (obj ["unitSleeveId"].ToString (), (int)LoadAssetBundle.DisplayType.Card, waitLobby.transform.Find ("EnemyInformation/EnemyBox/Card").gameObject);
								loadAssetBundle.SetClassIconImage (obj ["classId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/ClassIcon").gameObject);
								loadAssetBundle.SetForcePlateImage (obj ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.LeftTop, waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/UIProfilePlate02").gameObject);
								loadAssetBundle.SetForcePlateImage (obj ["color"].ToString (), (int)LoadAssetBundle.ForcePlateType.RightBottom, waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/UIProfilePlate03").gameObject);
								loadAssetBundle.SetEmblemImage (obj ["emblemId"].ToString (), waitLobby.transform.Find ("EnemyInformation/EnemyBox/BackPlate/Emblem").gameObject);
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/NowClass").GetComponent<Text> ().text = master.staticData ["classes"] [obj ["classId"].ToString ()] ["name"].ToString ();
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/UserNameText").GetComponent<Text> ().text = obj ["name"].ToString ();
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/TitleText").GetComponent<Text> ().text = master.staticData ["titles"] [obj ["titleId"].ToString ()] ["name"].ToString ();
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/IntroText").GetComponent<Text> ().text = obj ["freeword"].ToString ();
								
								JToken enemyRecord = null;
								if (GameSettings.LastQueueType == QUEUE_TYPE_CASUAL) {
									// casual match data
									enemyRecord = obj ["casualRecords"];
								} else {
									// ranked match data
									enemyRecord = obj ["rankRecords"] [obj ["color"].ToString ()];
								}

								int enemyTotalMatchCount = enemyRecord != null ? int.Parse (enemyRecord ["winCount"].ToString ()) + int.Parse (enemyRecord ["loseCount"].ToString ()) : 0;
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/BattleNum").GetComponent<Text> ().text = enemyTotalMatchCount.ToString ();
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/WinNum").GetComponent<Text> ().text = (enemyRecord != null ? int.Parse (enemyRecord ["winCount"].ToString ()) : 0).ToString ();
								waitLobby.transform.Find ("EnemyInformation/EnemyBox/RateNum").GetComponent<Text> ().text = "(" + (enemyTotalMatchCount > 0 ? ((float.Parse (enemyRecord ["winCount"].ToString ()) / (float)enemyTotalMatchCount) * 100).ToString ("F") : "0.00") + "%)";
								break;
							}
						}
					}
					matchingManager.Matching = true;
				}));
			} else {
				steward.OpenMessageWindow ("エラー", "参加条件を満たしていません", "閉じる", () => {});
			}
		}));
	}

	public void Back(bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		switch (viewMode) {
		case ViewMode.MatchSelect:
			steward.LoadNextScene ("HomeView");
			break;

		case ViewMode.RankedMatch:
			ChangeView (ViewMode.MatchSelect);
			break;
			
		case ViewMode.DeckSelect:
			if (GameSettings.LastQueueType == QUEUE_TYPE_CASUAL) {
				ChangeView (ViewMode.MatchSelect);
			} else {
				ChangeView (ViewMode.RankedMatch);
			}
			break;

		case ViewMode.WaitMatch:
			steward.SetNewTask (matchMaking.cancel((Exception error) => {
				if (error != null) {
					logger.data(error).error("Error when cancelling matchmaking");
					return;
				}
				
				ChangeView (ViewMode.DeckSelect);
			}));
			break;
		}
	}

	private void ChangeView (ViewMode _viewMode) {
		viewMode = _viewMode;
		switch (viewMode) {
		case ViewMode.MatchSelect:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("対戦モード");
			steward.ShowLive2DOperator ();
			matchSelect.SetActive (true);
			rankedMatch.SetActive (false);
			deckView.SetActive (false);
			waitLobby.SetActive (false);
			break;

		case ViewMode.RankedMatch:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("階級戦");
			steward.HideLive2DOperator ();
			matchSelect.SetActive (false);
			rankedMatch.SetActive (true);
			deckView.SetActive (false);
			waitLobby.SetActive (false);
			break;
			
		case ViewMode.DeckSelect:
			steward.ShowHeader ();
			steward.SetHeaderViewName ("デッキ選択");
			steward.HideLive2DOperator ();
			matchSelect.SetActive (false);
			rankedMatch.SetActive (false);
			deckView.SetActive (true);
			waitLobby.SetActive (false);

			ChangeDeck (currentDeckId);
			break;

		case ViewMode.WaitMatch:
			steward.HideRankPopUp ();
			steward.HideHeader ();
			steward.HideLive2DOperator ();
			matchSelect.SetActive (false);
			rankedMatch.SetActive (false);
			deckView.SetActive (false);
			waitLobby.SetActive (true);
			break;
		}
	}

	private void ChangeDeck (int deckId) {
		var currentDeck = userDecks [deckId];
		
		currentDeckId = deckId;
		
		var index = 0;
		foreach (var kv in userDecks) {
			var userDeck = kv.Value;
			deckPlates [index].GetComponent<DeckData> ().Init (kv.Value, "multiplay", currentDeckId);
			if (validateDeck [index.ToString ()].ToString () == DECK_STATUS_INVALID_CARD || (GameSettings.LastQueueType != QUEUE_TYPE_CASUAL && validateDeck [index.ToString ()].ToString () == DECK_STATUS_INVALID_COLOR)) {
				deckPlates [index].GetComponent<DeckData> ().SetDisabled ();
			}
			index++;
		}
		
		Transform deckView = GameObject.Find ("/Main Canvas/Panel/DeckView").transform;
		loadAssetBundle.SetLoadWait ();
		loadAssetBundle.SetCardImage (currentDeck.favoriteUnit, (int)LoadAssetBundle.DisplayType.Card, deckView.Find ("UnitDeck").gameObject);
		loadAssetBundle.SetCardImage (currentDeck.favoriteCharacter, (int)LoadAssetBundle.DisplayType.Card, deckView.Find ("CharacterDeck").gameObject);
		loadAssetBundle.SetAvatarImage (currentDeck.avatarId, deckView.Find ("Avater").gameObject);
		loadAssetBundle.SetMothershipImage (currentDeck.mothershipId, deckView.Find ("Mothership").gameObject);
		loadAssetBundle.SetSleeveImage (currentDeck.unitSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckView.Find ("UnitSleve").gameObject);
		loadAssetBundle.SetSleeveImage (currentDeck.characterSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckView.Find ("CharacterSleve").gameObject);

		deckView.Find ("DeckNameWindow/DeckName").GetComponent<Text> ().text = currentDeck.deckName;

		deckView.transform.Find ("MainDeckButton").GetComponent<Button> ().onClick.RemoveAllListeners();
		deckView.transform.Find ("MainDeckButton").GetComponent<Image> ().color = new Color(1f, 1f, 1f, 0.5f);
		deckView.transform.Find ("MainDeckButton").GetComponent<Scale_Pop> ().isPop = false;

		if (validateDeck [deckId.ToString ()].ToString () == DECK_STATUS_INVALID_CARD) {
			steward.OpenMessageWindow ("確認", "出撃条件を満たしていません\nデッキに入れられないカードが\nデッキに含まれています", "閉じる", () => {});
		} else if (GameSettings.LastQueueType != QUEUE_TYPE_CASUAL && validateDeck [deckId.ToString ()].ToString () == DECK_STATUS_INVALID_COLOR) {
			steward.OpenDialogWindow ("確認", "出撃条件を満たしていません\n選択している勢力とデッキの色が\n一致している必要があります", "デッキ構築へ", () => {LoadDeckViewWithSelectedDeckId (deckId);}, "閉じる", () => {});
		} else {
			deckView.transform.Find ("MainDeckButton").GetComponent<Button> ().onClick.AddListener(() => {SubmitMatch();});
			deckView.transform.Find ("MainDeckButton").GetComponent<Image> ().color = new Color(1f, 1f, 1f, 1f);
			deckView.transform.Find ("MainDeckButton").GetComponent<Scale_Pop> ().isPop = true;
		}
	}

	private void LoadDeckViewWithSelectedDeckId (int deckId) {
		steward.SelectedDeckId = deckId;
		steward.LoadNextScene ("DeckConstructionView");
	}
}
