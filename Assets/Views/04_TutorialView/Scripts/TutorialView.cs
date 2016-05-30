using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

using Prime31;

namespace GameView {
	public class TutorialView : MonoBehaviour {

		//
		public static Mage mage { get { return Mage.Instance; }}
		public static Game game { get { return Game.Instance; }}
		public static Card card { get { return Card.Instance; }}
		public static Master master { get { return Master.Instance; }}
		public Logger logger { get { return mage.logger("GameDummy"); }}
		
		//
		[Header("General Data")]
		public TextAsset assetBundleVersions;
		
		//
		[Header("Game Data")]
		public TextAsset gameJson;
		public TextAsset gameSecretsJson;
		
		//
		[Header("Card Data")]
		public TextAsset characteristics;
		public TextAsset cardEffects;
		public TextAsset cardsPilot;
		public TextAsset cardsCrew;
		public TextAsset cardsUnit;
		public TextAsset cardsEvent;
		public TextAsset cardsCounter;

		[Header("GameView")]
		public GameView gameView;
		//
		public string meUserId;
		public string enemyUserId;
		public int instanceIdCounter = 0;
		public System.Random random = new System.Random();

		public TutorialData userCardData;
		public TutorialData enemyCardData;
		public TutorialData dialogue;

		public TutorialStepWindow tutorialStepWindow;
		public TutorialGuideWindow tutorialGuideWindow;
		public TutorialErrorWindow tutorialErrorWindo;

		public Transform tutorialArrowPanel;
		[Header("Mask Transform")]
		public Transform gameBoard;
		public Transform gameIcons;
		public Transform middleground;
		public Transform foreground;
		//
		[Header("End Game")]
		public GameEndController_Tutorial gameEndController_Tutorial;
		
		public TutorialActionStep userActionData;

		[Header("Tutorial Test")]
		public bool tutorialTest;

		// 下記の"virtual"メソッドの実現は各子クラスで行い、TutorialViewStep1 ~ 5.cs
		public virtual void CreateGame (){	
		}
		//
		public virtual void AttackEnemy(string attackerId, string receiverId) {
		}
		//
		public virtual void PlaceCard(string userId, JObject handCard, int? boardPos, List<string> costCards, string pilotCard) {
		}
		//
		public virtual void EndTurn() {
		}

		void Awake (){
			if (tutorialTest) {
				if (GameSettings.TutorialState != GameSettings.TutorialStates.Encore){
					GameSettings.TutorialState = GameSettings.TutorialStates.Tutorial;
				}
			}
		}

		//
		public virtual void EndGame(string winnerId, bool explode = true) {

			game.tCurrentGame.Set("playerWinner", winnerId);

			if (explode) {
				// 爆発演出
				Task deathAnim = (winnerId == meUserId) ?
				new Task (gameView.enemyMothership.Explode (), false) :
					new Task (gameView.myMothership.Explode (), false);
			
				// Game Over処理開始
				deathAnim.Start ();
			} 
		}

		public void LoadDummyData (){
			// Load in dummy data
			game.tCurrentGame = (TomeObject)Tome.Conjure(JToken.Parse(gameJson.text));
			game.tCurrentGameSecrets = (TomeObject)Tome.Conjure(JToken.Parse(gameSecretsJson.text));
			
			// Add player data
			meUserId = GameSettings.UserId;
			enemyUserId = "enemyId";
			(game.tCurrentGame["players"] as TomeArray).Set(0, new JValue(meUserId));
			(game.tCurrentGame["players"] as TomeArray).Set(1, new JValue(enemyUserId));
			
			(game.tCurrentGame["playerData"] as TomeObject).Set(meUserId, CreateUserData("Me"));
			(game.tCurrentGame["playerData"] as TomeObject).Set(enemyUserId, CreateUserData("Enemy"));
			(game.tCurrentGame["playerReady"] as TomeObject).Set(meUserId, new JValue(true));
			(game.tCurrentGame["playerReady"] as TomeObject).Set(enemyUserId, new JValue(true));
			(game.tCurrentGame["playerStarted"] as TomeObject).Set(meUserId, new JValue(false));
			(game.tCurrentGame["playerStarted"] as TomeObject).Set(enemyUserId, new JValue(false));
			
			(game.tCurrentGame["board"] as TomeObject).Set(meUserId, new JArray());
			(game.tCurrentGame["board"] as TomeObject).Set(enemyUserId, new JArray());
			(game.tCurrentGame["counters"] as TomeObject).Set(meUserId, new JArray());
			(game.tCurrentGame["counters"] as TomeObject).Set(enemyUserId, new JArray());
			
			(game.tCurrentGame["drawnCards"] as TomeObject).Set(meUserId, JToken.Parse("{ \"crewCards\": {}, \"unitCards\": {} }"));
			(game.tCurrentGame["drawnCards"] as TomeObject).Set(enemyUserId, JToken.Parse("{ \"crewCards\": {}, \"unitCards\": {} }"));
			
			(game.tCurrentGame["motherships"] as TomeObject).Set(meUserId, CreateMothership(meUserId, enemyUserId));
			(game.tCurrentGame["motherships"] as TomeObject).Set(enemyUserId, CreateMothership(enemyUserId, meUserId, 5));
		}

		//
		public JObject CreateUserData(string userName) {
			JObject userData = new JObject();
			userData.Add("name", new JValue(userName));
			userData.Add("titleId", new JValue("title00001"));
			userData.Add("freeword", new JValue(userName));

			string color = getColor (userName);
			userData.Add("color", new JValue(color));

			userData.Add("emblemId", new JValue("emblem1001"));
			userData.Add("rankClass", new JValue("Chamber Pot Cleaner"));

			string motherShip = getMotherShip (userName);
			userData.Add("mothershipId", new JValue(motherShip));

			userData.Add("avatarId", new JValue("avatar1021"));
			userData.Add("characterSleeveId", new JValue("sleeve0002"));
			userData.Add("unitSleeveId", new JValue("sleeve0001"));
			return userData;
		}

		private string getMotherShip (string userName){

			if (userName == "Me") {
				return getMeMotherShip ();
			} else {
				return getEnemyMotherShip ();
			}

			return "";
		}

		private string getMeMotherShip (){
			string currentStep = PlayerPrefs.GetString ("Tutorial");

			if (currentStep == "Step1" || currentStep == "Step2") {
				
				return "mshipWhiteBase";
				
			} else if (currentStep == "Step3" || currentStep == "Step4" || currentStep == "Step5") {
				
				return "mshipArchangel";			
			}
			
			Debug.LogError ("Unknow Scene Name");
			return "";
		}

		private string getEnemyMotherShip (){
			string currentStep = PlayerPrefs.GetString ("Tutorial");
			
			if (currentStep == "Step1" || currentStep == "Step2") {
				
				return "mshipMusai";
				
			} else if (currentStep == "Step3" || currentStep == "Step4" || currentStep == "Step5") {
				
				return "mshipAlexandria";			
			}
			
			Debug.LogError ("Unknow Scene Name");
			return "";
		}

		//
		private string getColor (string userName){
			
			if (userName == "Me") {
				return getMeColor ();
			} else {
				return getEnemyColor ();
			}
			
			return "";
		}

		private string getMeColor (){
			string currentStep = PlayerPrefs.GetString ("Tutorial");
			
			if (currentStep == "Step1" || currentStep == "Step2") {
				
				return "blue";
				
			} else if (currentStep == "Step3" || currentStep == "Step4" || currentStep == "Step5") {
				
				return "yellow";			
			}
			
			Debug.LogError ("Unknow Scene Name");
			return "";
		}

		private string getEnemyColor (){
			string currentStep = PlayerPrefs.GetString ("Tutorial");
			
			if (currentStep == "Step1" || currentStep == "Step2") {
				
				return "green";
				
			} else if (currentStep == "Step3" || currentStep == "Step4" || currentStep == "Step5") {
				
				return "black";			
			}
			
			Debug.LogError ("Unknow Scene Name");
			return "";
		}
		
		//
		public JObject CreateMothership(string userId, string enemyId, int def = 30) {
			JObject mothership = new JObject();
			mothership.Add("currentAtk", new JValue(0));
			mothership.Add("currentDef", new JValue(def));
			mothership.Add("instanceId", new JValue(userId));
			mothership.Add("userId", new JValue(userId));
			mothership.Add("enemyId", new JValue(enemyId));
			mothership.Add("tapped", new JValue(0));
			mothership.Add("currentEffects", new JArray());
			return mothership;
		}
		
		//
		public JObject CreateCardCrew(string userId, string cardId, string instanceId) {
			TomeObject cardData = card.GetCrewCard(cardId);
			
			JObject newCard = (JObject)JToken.Parse(cardData.ToString());
			newCard.Add("instanceId", new JValue(instanceId));
			newCard.Add("tapped", new JValue(0));
			newCard.Add("userId", new JValue(userId));
			
			return newCard;
		}
		
		//
		public JObject CreateCardUnit(string userId, string cardId, string instanceId) {
			TomeObject cardData = card.GetUnitCard(cardId);
			
			JObject newCard = (JObject)JToken.Parse(cardData.ToString());
			newCard.Add("instanceId", new JValue(instanceId));
			newCard.Add("userId", new JValue(userId));
			
			newCard.Add("currentCost", (JObject)JToken.Parse(newCard["cost"].ToString()));
			
			return newCard;
		}
		
		//
		public JObject CreateBoardIcon(string userId, string cardId, string pilotId, string instanceId) {
			TomeObject cardData = card.GetUnitCard(cardId);
			
			JObject newIcon = (JObject)JToken.Parse(cardData.ToString());
			newIcon.Add("instanceId", new JValue(instanceId));
			newIcon.Add("userId", new JValue(userId));
			
			newIcon.Add("tapped", new JValue(0));
			
			newIcon.Add("currentAtk", newIcon["atk"]);
			newIcon.Add("currentDef", newIcon["def"]);
			newIcon.Add("currentCost", (JObject)JToken.Parse(newIcon["cost"].ToString()));
			newIcon.Add("currentEffects", (JArray)JToken.Parse((newIcon["effects"] != null) ? newIcon["effects"].ToString() : "[]"));
			newIcon.Add("currentFlags", new JObject());
			
			if (pilotId != null) {
				string pilotCardId = (string)game.tCurrentGame["drawnCards"][userId]["crewCards"][pilotId]["cardId"];
				newIcon.Add("pilot", (JObject)JToken.Parse(card.GetCrewCard(pilotCardId).ToString()));
				(newIcon["pilot"] as JObject).Add("instanceId", new JValue(pilotId));
			}
			
			return newIcon;
		}
		
		//
		public string CreateInstanceId() {
			instanceIdCounter++;
			return instanceIdCounter.ToString();
		}

		//
		public void CardGiveCrew(string userId, string cardId, string instanceId, string actionType) {
			JObject card = CreateCardCrew(userId, cardId, instanceId);
			(game.tCurrentGame["drawnCards"][userId]["crewCards"] as TomeObject).Set(instanceId, card);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue(actionType));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public void CardGiveUnit(string userId, string cardId, string instanceId, string actionType) {
			if (userId == meUserId) {
				JObject card = CreateCardUnit(userId, cardId, instanceId);
				(game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject).Set(instanceId, card);
			}
			
			JObject secretCard = new JObject();
			secretCard.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["drawnCards"][userId]["unitCards"] as TomeObject).Set(instanceId, secretCard);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue(actionType));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}

		private List<string> userInstanceIdList = new List<string> ();
		private List<string> enemyInstanceIdList = new List<string> ();

		//
		public void DrawCardCrew(string userId, string crewCard) {

			var instanceId = CreateInstanceId ();
			CardGiveCrew(userId, crewCard, instanceId, "CardDrawnCrew");

			if (userId == meUserId) {
				userInstanceIdList.Add (instanceId);
			} else {
				enemyInstanceIdList.Add (instanceId);
			}
		}

		//
		public void DrawCardUnit(string userId, string unitCard) {

			var instanceId = CreateInstanceId ();
			CardGiveUnit(userId, unitCard, instanceId, "CardDrawnUnit");

			if (userId == meUserId) {
				userInstanceIdList.Add (instanceId);
			} else {
				enemyInstanceIdList.Add (instanceId);
			}
		}
		
		//
		public void DestroyCardCrew(string userId, string instanceId) {
			(game.tCurrentGame["drawnCards"][userId]["crewCards"] as TomeObject).Del(instanceId);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("CardDestroyCrew"));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public void BurnCardUnit(string userId, string instanceId) {
			if (userId == meUserId) {
				(game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject).Del(instanceId);
			}
			
			(game.tCurrentGame["drawnCards"][userId]["unitCards"] as TomeObject).Del(instanceId);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("CardBurntUnit"));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public virtual void StartGame(string userId, List<string> burnCards) {
			// InGameのBgmを再生
			if (GameObject.Find ("Steward") != null) {
				var steward = GameObject.Find ("Steward").GetComponent<Steward> ();
				steward.PlayBGMInGame ();
			}
			userActionData.LauchNextAction ();
		}

		public TutorialActionStep GetTutorialActionData (){
			return userActionData;
		}

		public TutorialData GetDialogueData (){
			return dialogue;
		}

		//
		public void CreateUnit(string userId, JObject cardData, int boardPosition, string pilotId) {
			string cardId = (string)cardData["cardId"];
			string instanceId = (string)cardData["instanceId"];
			JObject newUnit = CreateBoardIcon(userId, cardId, pilotId, instanceId);
			
			JArray insertItems = new JArray();
			insertItems.Add(newUnit);
			(game.tCurrentGame["board"][userId] as TomeArray).Splice(boardPosition, 0, insertItems);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("UnitCreated"));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			historyItem.Add("boardPosition", new JValue(boardPosition));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public void CreateCounter(string userId, string cardId, string instanceId) {
			TomeObject cardData = card.GetUnitCard(cardId);
			
			if (userId == meUserId) {
				JObject secretCounter = (JObject)JToken.Parse(cardData.ToString());
				secretCounter.Add("instanceId", new JValue(instanceId));
				secretCounter.Add("userId", new JValue(userId));
				(game.tCurrentGameSecrets["counters"] as TomeArray).Push(secretCounter);
			}
			
			JObject newCounter = new JObject();
			newCounter.Add("instanceId", instanceId);
			(game.tCurrentGame["counters"][userId] as TomeArray).Push(newCounter);
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("CounterCreated"));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public void GiveDamage(Dictionary<string, int> damageMap) {
			List<string> destroyTargets = new List<string>();
			foreach (var property in damageMap) {
				TomeObject boardIcon = GameHelpers.FindBoardIconData(property.Key);
				boardIcon.Set("currentDef", (int)boardIcon["currentDef"] - property.Value);

				if (GameHelpers.UnitGetDef(boardIcon) <= 0) {
					destroyTargets.Add(property.Key);
				}
			}

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("TargetsDamaged"));
			historyItem.Add("targetIds", new JArray(damageMap.Keys));
			historyItem.Add("effectData", JValue.CreateNull());
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);

			if (destroyTargets.Count > 0) {
				DestroyTargets(destroyTargets);
			}
		}

		/// <summary>
		/// Gives the damage.
		/// </summary>
		/// <param name="instanceId">Instance identifier.</param>
		/// <param name="damage">Damage.</param>
		public void GiveDamage(string instanceId, int damage){

			Dictionary<string, int> damageMap = new Dictionary<string, int>();
			damageMap.Add(instanceId, damage);
			GiveDamage(damageMap);

			BoardIcon boardIcon = GameHelpers.FindBoardIcon (instanceId);
			if (boardIcon != null) {
				boardIcon.Attacked ();
			}
		
			Mothership mothership = GameHelpers.FindMothership (instanceId);
			if (mothership != null) {
				mothership.Attacked ();
			}
		}
		
		//
		public void DestroyTargets(List<string> targets) {
			foreach (string instanceId in targets) {
				TomeObject boardIcon = GameHelpers.FindBoardIconData(instanceId);
				if (boardIcon["pilot"] != null) {
					string pilotCardId = (string)boardIcon["pilot"]["cardId"];
					string pilotInstanceId = (string)boardIcon["pilot"]["instanceId"];
					CardGiveCrew((string)boardIcon["userId"], pilotCardId, pilotInstanceId, "CardPilotReturned");
				}
				
				if ((string)boardIcon["userId"] != (string)boardIcon["instanceId"]) {
					TomeArray ownerBoard = game.tCurrentGame["board"][(string)boardIcon["userId"]] as TomeArray;
					ownerBoard.Splice(ownerBoard.IndexOf(boardIcon), 1, new JArray());
				} else {

					StartCoroutine (delayAction (() => {
						EndGame((string)boardIcon["enemyId"]);
					}, 2.0f));
				}
			}
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("TargetsDestroyed"));
			historyItem.Add("targetIds", new JArray(targets));
			historyItem.Add("effectData", JValue.CreateNull());
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}
		
		//
		public void TriggerCounter(string userId) {
			TomeArray counters = game.tCurrentGame["counters"][userId] as TomeArray;
			if (counters.Count > 0) {
				TomeObject counter = counters.Pop() as TomeObject;
				
				string cardId = "cw52001071";
				if (userId == meUserId) {
					counter = (game.tCurrentGameSecrets["counters"] as TomeArray).Pop() as TomeObject;
					cardId = (string)counter["cardId"];
				}
				
				JObject historyItem = new JObject();
				historyItem.Add("action", new JValue("CounterActivated"));
				historyItem.Add("userId", new JValue(userId));
				historyItem.Add("instanceId", new JValue((string)counter["instanceId"]));
				historyItem.Add("cardId", new JValue((string)counter["cardId"]));
				historyItem.Add("wasEffective", new JValue(true));
				(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
			}
		}
		

		public IEnumerator delayAction (Action action, float delayTime){

			yield return new WaitForSeconds (delayTime);
			action ();
		}

		//
		public void ResetIcons() {

			foreach (var property in game.tCurrentGame["drawnCards"][meUserId]["crewCards"] as TomeObject) {
				TomeObject costIcon = property.Value as TomeObject;
				if ((int)costIcon["tapped"] > 0) {
					costIcon.Set("tapped", (int)costIcon["tapped"] - 1);
				}
			}
			foreach (var property in game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject) {
				TomeObject costIcon = property.Value as TomeObject;
				if ((int)costIcon["tapped"] > 0) {
					costIcon.Set("tapped", (int)costIcon["tapped"] - 1);
				}
			}
			foreach (TomeObject boardIcon in game.tCurrentGame["board"][meUserId] as TomeArray) {
				if ((int)boardIcon["tapped"] > 0) {
					boardIcon.Set("tapped", (int)boardIcon["tapped"] - 1);
				}
				
				boardIcon.Set("currentAtk", (int)boardIcon["atk"]);
				boardIcon.Set("currentDef", (int)boardIcon["def"]);
			}
			foreach (TomeObject boardIcon in game.tCurrentGame["board"][enemyUserId] as TomeArray) {
				if ((int)boardIcon["tapped"] > 0) {
					boardIcon.Set("tapped", (int)boardIcon["tapped"] - 1);
				}
				
				boardIcon.Set("currentAtk", (int)boardIcon["atk"]);
				boardIcon.Set("currentDef", (int)boardIcon["def"]);
			}
		}

		//
		public void TapIcon(string instanceId) {
			try{
				TomeObject boardIcon = GameHelpers.FindBoardIconData(instanceId);
				
				if (boardIcon != null){
					boardIcon.Set("tapped", (int)boardIcon["tapped"] + 1);
					
					JObject historyItem = new JObject();
					historyItem.Add("action", new JValue("UnitTapChange"));
					historyItem.Add("instanceId", new JValue(instanceId));
					historyItem.Add("effectData", JValue.CreateNull());
					(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
				}
			}catch (Exception ex){
				Debug.LogError (ex);
			}
		}

		/// <summary>
		/// 次のチュートリアルシーン名を取得
		/// </summary>
		/// <returns>The next tutorial scene.</returns>
		public string GetNextTutorialScene (){
			string currentStep = PlayerPrefs.GetString ("Tutorial");
			
			if (currentStep == "Step1") {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Step 1 Done");
				#endif
				return "NewTutorialViewStep2";
				
			} else if (currentStep == "Step2") {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Step 2 Done");
				#endif
				return "NewTutorialViewStep3";
				
			} else if (currentStep == "Step3") {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Step 3 Done");
				#endif
				return "NewTutorialViewStep4";		
				
			} else if (currentStep == "Step4") {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Step 4 Done");
				#endif
				return "NewTutorialViewStep5";	
				
			} else if (currentStep == "Step5") {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Step 5 Done");
				#endif
				return "NewTutorialEndView";
			}
			
			Debug.LogError ("Unknow Scene Name");
			return "";
		}

		/// <summary>
		/// 該当ユニットが攻撃を実装したかどうか
		/// </summary>
		/// <returns><c>true</c> グレイアウトの状態なら 
		/// <c>false</c>.</returns>
		/// <param name="instanceId">Instance identifier.</param>
		public bool IsTapped (string instanceId){
			BoardIcon boardIcon = GameHelpers.FindBoardIcon((string)instanceId);
			bool iconTapped = GameHelpers.IconIsTapped(boardIcon.iconData);

			return iconTapped;
		}

		/// <summary>
		/// このInstanceIdの所属
		/// </summary>
		/// <returns><c>true</c> if this instance is user instance identifier the specified instanceId; otherwise, <c>false</c>.</returns>
		/// <param name="instanceId">Instance identifier.</param>
		public string InstanceIdBelongTo (string instanceId){

			if (userInstanceIdList.Contains (instanceId)) {
				return meUserId;
			} else {
				return enemyUserId;
			}
		}

		/// <summary>
		/// 攻撃できるかどうか
		/// </summary>
		/// <returns><c>true</c> if this instance can attack the specified instanceId currentTurn; otherwise, <c>false</c>.</returns>
		/// <param name="instanceId">Instance identifier.</param>
		/// <param name="currentTurn">Current turn.</param>
		public bool CanAttack (string instanceId, string currentTurn){

			if (IsTapped (instanceId)){
				return false;
			}

			if (currentTurn != InstanceIdBelongTo (instanceId)) {
				return false;
			}

			return true;
		}

		public List<string> GetCost (TomeObject crewCards, int count){

			List<string> cost = new List<string> ();
			int index = 0;

			foreach (var property in crewCards) {

				if (index == count){
					break;
				}else{
					cost.Add((string)property.Value["instanceId"]);
					index++;
				}
			}

			return cost;
		}

		/// <summary>
		/// Sets the cost icon toggle dragger toggle enabled.
		/// </summary>
		/// <param name="active">If set to <c>true</c> active.</param>
		/// <param name="index">Index.</param>
		public void SetCostIconToggleDraggerToggleEnabled (bool active, int index = -1){

			var row1 = foreground.FindChild ("panel_launchCard_overlay/CostIcons/row1");

			if (index == -1) {
				foreach (Transform icon in row1) {
					icon.GetComponent<Toggle> ().enabled = active;
				}
			} else {
				var CostIcon = row1.GetChild (index);
				CostIcon.GetComponent<Toggle> ().enabled = active;
			}
		}
	}
}
