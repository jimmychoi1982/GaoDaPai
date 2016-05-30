using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class GameView : SceneInstance<GameView> {
		//
		Mage mage { get { return Mage.Instance; }}
		Logger logger { get { return mage.logger("GameView"); }}

		Game game { get { return Game.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		Master master { get { return Master.Instance; }}

		//
		[HideInInspector] public string meUserId;
		[HideInInspector] public string enemyUserId;
		[HideInInspector] public Dictionary<string, Mothership> motherships = new Dictionary<string, Mothership>();
		[HideInInspector] public Dictionary<string, BoardIconsManager> boardManagers = new Dictionary<string, BoardIconsManager>();
		[HideInInspector] public Dictionary<string, CostIconsManager> costManagers = new Dictionary<string, CostIconsManager>();
		[HideInInspector] public Dictionary<string, HandCardsManager> handManagers = new Dictionary<string, HandCardsManager>();
		[HideInInspector] public Dictionary<string, Deck_Animation_uGUI> deckManagers = new Dictionary<string, Deck_Animation_uGUI>();
		[HideInInspector] public Dictionary<string, CountersManager> counterManagers = new Dictionary<string, CountersManager>();

		//
		[Header("Dummy Data")]
		public GameDummy gameDummy;
		public TutorialView tutoriallView;
		
		//
		[Header("Error")]
		public GameObject errorPrefab;
		public GameObject errorContent;
		public GameObject errorParent;
		
		//
		[Header("Turn Management")]
		public CoinToss coinToss;
		public GameTurnManager gameTurnManager;

		//
		[Header("Card Drawing")]
		public Deck_Animation_uGUI myDeckManager;
		public Deck_Animation_uGUI enemyDeckManager;
		public GameInitialDraw gameInitialDraw;

		//
		[Header("Card Launcher")]
		public CardLaunchManager cardLaunchManager;

		//
		[Header("Question Card Choice")]
		public GameChoiceCard gameChoiceCard;
		
		//
		[Header("Question Target Choice")]
		public GameChoiceTarget gameChoiceTarget;

		//
		[Header("Card Info")]
		public CardInfo cardInfo;

		//
		[Header("Card Tips")]
		public CardTipsManager cardTipsManager;

		//
		[Header("My PlayerData Manager")]
		public PlayerData myPlayerData;

		//
		[Header("Opponent PlayerData Manager")]
		public PlayerData enemyPlayerData;
		
		//
		[Header("My Icon Managers")]
		public Mothership myMothership;
		public BoardIconsManager myBoardManager;
		public CostIconsManager myCostManager;
		public HandCardsManager myHandManager;
		public CountersManager myCounterManager;

		//
		[Header("Opponent Icon Managers")]
		public Mothership enemyMothership;
		public BoardIconsManager enemyBoardManager;
		public CostIconsManager enemyCostManager;
		public HandCardsManager enemyHandManager;
		public CountersManager enemyCounterManager;

		//
		[Header("End Game")]
		public GameEndController gameEndController;

		//
		[Header("Game Options")]
		public GameOption gameOptions;

		//
		void Awake() {
			base.Awake();

			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				Debug.Log ("Creating New Tutorial game");
				tutoriallView.CreateGame();
			}else if (game.tCurrentGame == null && Application.isEditor) {
				Debug.Log("Creating dummy game");
				gameDummy.CreateGame();
			}
		}


		// Use this for initialization
		void Start () {
			// Initial cleaup / resetting
			myCostManager.SetData(null, false);
			myHandManager.SetData(null, false);
			myBoardManager.SetData(null, false);
			enemyCostManager.SetData(null, false);
			enemyHandManager.SetData(null, false);
			enemyBoardManager.SetData(null, false);

			gameInitialDraw.Close();
			cardLaunchManager.Close();
			gameChoiceTarget.Close();
			gameChoiceCard.Close();
			cardInfo.Close();

			// Check if this is a dummy game or tutoiral
			if ((string)game.tCurrentGame["gameId"] != "dummyGame") {
				SetupInitial();
				return;
			}

			// If so, we need to load the asset bundles and a few other things
			Debug.Log("Instantiating dependencies for dummy game");
			TaskManagerMainThread.Instantiate();

			loadAssetBundle.SetAssetBundleURI(GameSettings.AssetBundleURI);
			loadAssetBundle.GetVersionFromMaster(() => {
				StartCoroutine(loadAssetBundle.DownloadAllAssetBundles(() => {
					Debug.Log("Everything loaded, starting game");
					SetupInitial();
				}));
			});
		}

		//
		private void SetupInitial() {
			// Setup userIds
			meUserId = GameSettings.UserId;
			enemyUserId = (string)game.tCurrentGame["players"][0];
			if (enemyUserId == meUserId) {
				enemyUserId = (string)game.tCurrentGame["players"][1];
			}
			
			// Setup manager maps
			motherships.Add(meUserId, myMothership);
			motherships.Add(enemyUserId, enemyMothership);
			boardManagers.Add(meUserId, myBoardManager);
			boardManagers.Add(enemyUserId, enemyBoardManager);
			costManagers.Add(meUserId, myCostManager);
			costManagers.Add(enemyUserId, enemyCostManager);
			handManagers.Add(meUserId, myHandManager);
			handManagers.Add(enemyUserId, enemyHandManager);
			deckManagers.Add(meUserId, myDeckManager);
			deckManagers.Add(enemyUserId, enemyDeckManager);
			counterManagers.Add(meUserId, myCounterManager);
			counterManagers.Add(enemyUserId, enemyCounterManager);

			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore) {
				gameInitialDraw.Open ();
				return;
			}

			// Start with coin toss to determine starting player
			string currentTurn = (string)game.tCurrentGame["currentTurn"];
			if (currentTurn == meUserId) {
				coinToss.CoinTossMe(gameInitialDraw.Open);
			} else {
				coinToss.CoinTossEnemy(gameInitialDraw.Open);
			}
		}

		//
		public void ShowError(string errorText) {
			GameObject newError = GameObject.Instantiate(errorPrefab);
			GameObject newErrorContent = GameObject.Instantiate (errorContent);
			newError.transform.SetParent(errorParent.transform);
			newErrorContent.transform.SetParent (newError.transform);
			newError.transform.localScale = errorPrefab.transform.localScale;

			newError.transform.SetAsFirstSibling();
			newErrorContent.GetComponent<Text>().text = errorText;

			// Delete older errors if the list grows past 1 entries
			if (errorParent.transform.childCount > 1) {
				GameObject.DestroyObject(errorParent.transform.GetChild(0).gameObject);
			}
		}

		//
		public void StartGame(List<string> burnCards) {

			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){

				tutoriallView.StartGame(meUserId, burnCards);
				gameInitialDraw.SwapCards(burnCards);
				return;
			}else if ((string)game.tCurrentGame["gameId"] == "dummyGame") {
				gameDummy.StartGame(meUserId, burnCards);
				gameInitialDraw.SwapCards(burnCards);
				return;
			}

			new Task(game.Start(burnCards, (Exception error) => {
				if (error != null) {
					logger.data(error).error("Failed to select starting cards:");
					gameInitialDraw.RenableButtons();
				}

				gameInitialDraw.SwapCards(burnCards);
			}));
		}
		
		//
		public void SetupGameBoard() {
			gameTurnManager.SetTurn();
			
			// Set up user data(currently, only set up user name)
			myPlayerData.SetData(game.tCurrentGame["playerData"][meUserId] as TomeObject);
			enemyPlayerData.SetData(game.tCurrentGame["playerData"][enemyUserId] as TomeObject);

			// Set up mothership
			myMothership.SetData(game.tCurrentGame["motherships"][meUserId] as TomeObject);
			enemyMothership.SetData(game.tCurrentGame["motherships"][enemyUserId] as TomeObject);

			// Set up counters
			myCounterManager.SetData(game.tCurrentGameSecrets["counters"] as TomeArray);
			enemyCounterManager.SetData(game.tCurrentGame["counters"][enemyUserId] as TomeArray);

			// Setup initial cost icons
			myCostManager.SetData(game.tCurrentGame["drawnCards"][meUserId]["crewCards"] as TomeObject);
			enemyCostManager.SetData(game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject);

			// Setup initial hand cards
			myHandManager.SetData(game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject);
			enemyHandManager.SetData(game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject);
		}

		//
		public void EndTurn () {
			// Release any unused assets
			Resources.UnloadUnusedAssets();

			// Check if this is a dummy game or tutoiral
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				tutoriallView.EndTurn();
				return;
			}else if ((string)game.tCurrentGame["gameId"] == "dummyGame") {
				gameDummy.EndTurn();
				return;
			}

			// If not, call remote API
			new Task(game.EndTurn((Exception error) => {
				if (error != null) {
					gameTurnManager.Enable();
					logger.data(error).error("End Turn Failed:");
				}

				// Successfully ended turn. The rest is handled by
				// the event manager system.
			}));
		}

		public void CpuEndTurn() {
			// Release any unused assets
			Resources.UnloadUnusedAssets();

			// CPU End Turn
			new Task(Cpu.Instance.endTurn(game.tCurrentGame["gameId"].ToString(), (Exception error) => {
				if (error != null) {
					logger.data(error).error("CPU End Turn Failed:");
				}
			}));
		}

		public void CpuActionExecuted() {
			// Release any unused assets
			Resources.UnloadUnusedAssets();

			// CPU End Turn
			new Task(Cpu.Instance.executeAction(game.tCurrentGame["gameId"].ToString(), (Exception error) => {
				if (error != null) {
					logger.data(error).error("CPU Action Executed Failed:");
				}
			}));
		}

		//
		public void PlaceCard(GameCard gameCard, int? boardPos, List<string> costCards, string pilotCard) {
			// Check if this is a dummy game or tutoiral
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				tutoriallView.PlaceCard(meUserId, gameCard.cardData, boardPos, costCards, pilotCard);
				return;
			}else if ((string)game.tCurrentGame["gameId"] == "dummyGame") {
				gameDummy.PlaceCard(meUserId, gameCard.cardData, boardPos, costCards, pilotCard);
				return;
			}

			// If not, call remote API
			string instanceId = (string)gameCard.cardData["instanceId"];
			JObject placeOptions = new JObject();
			placeOptions.Add("costCards", new JArray(costCards));
			placeOptions.Add("pilotCard", new JValue(pilotCard));
			if (boardPos != null) {
				placeOptions.Add("boardPosition", new JValue((int)boardPos));
			}

			new Task(game.PlaceCard(instanceId, placeOptions, (Exception error, JToken result) => {
				// There was an error, log it, prompt it and move on
				if (error != null) {
					logger.data(error).error("Got error when placing card:");
					ShowError(error.Message);
					return;
				}

				// Successful placement, a new icon should appear on the board and be
				// removed from hand via the event manager system.
			}));
		}

		//
		public void AttackEnemy(string attackerId, string receiverId) {
			// Check if this is a dummy game or tutoiral
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				tutoriallView.AttackEnemy(attackerId, receiverId);
				return;
			}else if ((string)game.tCurrentGame["gameId"] == "dummyGame") {
				gameDummy.AttackEnemy(attackerId, receiverId);
				return;
			}

			// If not, call remote API
			new Task(game.AttackEnemy(attackerId, receiverId, (Exception error, JToken result) => {
				// There was an error, log it, prompt it and move on
				if (error != null) {
					logger.data(error).error("Got error when attacking:");

					if (master.staticData["errorCodes"][error.Message] != null) {
						ShowError(master.staticData["errorCodes"][error.Message.ToString()]["annotation"].ToString());
					} else {
						ShowError(error.Message);
					}
					return;
				}

				// Successful attck, the attack animation should be activated via the
				// event manager system.
			}));
		}


		//
		public void TapIcon(string instanceId) {
			// Check if this is a dummy game or tutoiral
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				tutoriallView.TapIcon(instanceId);
				return;
			}else if ((string)game.tCurrentGame["gameId"] == "dummyGame") {
				gameDummy.TapIcon(instanceId);
				return;
			}

			// If not, call remote API
			new Task(game.TapIcon(instanceId, (Exception error, JToken result) => {
				// There was an error, log it, prompt it and move on
				if (error != null) {
					logger.data(error).error("Got error when voluntarily tapping:");
					ShowError(error.Message);
					return;
				}

				// Successful voluntary tap, animation will be handled by event manager system
			}));
		}

		//
		public void AnswerCardQuestion(List<string> cardChoices) {
			JObject questionAnswers = new JObject();
			questionAnswers.Add("selected", new JArray(cardChoices.ToArray()));

			new Task(game.AnswerQuestion(questionAnswers, (Exception error, JToken result) => {
				// There was an error, log it, prompt it and move on
				if (error != null) {
					logger.data(error).error("Got error when answering card question:");
					ShowError(error.Message);
					return;
				}

				// Successful answer we can close the choice overlay
				gameChoiceCard.Close();
			}));
		}

		//
		public void AnswerTargetQuestion(string targetChoice) {
			JObject questionAnswers = new JObject();
			questionAnswers.Add("target", new JValue(targetChoice));

			new Task(game.AnswerQuestion(questionAnswers, (Exception error, JToken result) => {
				// There was an error, log it, prompt it and move on
				if (error != null) {
					logger.data(error).error("Got error when answering card question:");
					ShowError(error.Message);
					return;
				}
				
				// Successful answer we can close the choice overlay
				gameChoiceTarget.Close();
			}));
		}

		//
		public void ForfeitGame() {
			new Task(game.Forfeit((Exception error) => {
				if (error != null) {
					logger.data(error).error("Got error when forfeiting:");
					return;
				}

				// Succefully forfeited. Close the options overlay. The actual end of game
				// logic will follow the regular code paths via the game event system.
				gameOptions.Close();
			}));
		}

		public void AutoWinGame() {
			new Task (game.AdminAutoWin ((Exception error) => {
				if (error != null) {
					logger.data (error).error ("Got error when autoWin process:");
					return;
				}

				gameOptions.Close ();
			}));
		}

		//
		public void EndGame(string winnerId) {
			gameEndController.Open(winnerId, () => {
				// Final Cleanup
				GetComponent<GameEventManager>().gameEnded = true;
				gameTurnManager.CleanupTurnTimeLimit();

				// Move to next scene
				Application.LoadLevel("ResultView");
			});
		}
	}
}