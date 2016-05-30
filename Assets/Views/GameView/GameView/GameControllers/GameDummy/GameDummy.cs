using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json.Linq;

using UnityEngine;


namespace GameView {
	public class GameDummy : MonoBehaviour {
		//
		static Mage mage { get { return Mage.Instance; }}
		static Game game { get { return Game.Instance; }}
		static Card card { get { return Card.Instance; }}
		static Master master { get { return Master.Instance; }}
		Logger logger { get { return mage.logger("GameDummy"); }}

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

		//
		private string meUserId;
		private string enemyUserId;
		private int instanceIdCounter = 0;
		private System.Random random = new System.Random();

		//
		private List<string> allowedCrewCards = new List<string>() {
			//
			"cw21001010", "cw21001011",
			"cw21001010", "cw21001011",

			//
			//"cw11002012", "cw11001013"
			"cw12001159", "cw12001159", "cw12001159", "cw12001159"
		};
		
		//
		private List<string> allowedUnitCards = new List<string>() {
			//
			"cw32001038", "cw32001043", "cw31001007",
			"cw32001038", "cw32001043", "cw31001007",

			//
			"cw42001062", "cw42001064",

			//
			"cw52001068", "cw52001071",

			//
			"cw31001009"
		};

		//
		public void CreateGame() {
			// Load in dummy data
			JObject masterStaticData = new JObject();
			masterStaticData.Add("assetBundleVersions", (TomeObject)Tome.Conjure(JToken.Parse(assetBundleVersions.text)));
			master.staticData = masterStaticData;

			game.tCurrentGame = (TomeObject)Tome.Conjure(JToken.Parse(gameJson.text));
			game.tCurrentGameSecrets = (TomeObject)Tome.Conjure(JToken.Parse(gameSecretsJson.text));

			JObject cardStaticData = new JObject();
			cardStaticData.Add("characteristics", (TomeObject)Tome.Conjure(JToken.Parse(characteristics.text)));
			cardStaticData.Add("cardEffects", (TomeObject)Tome.Conjure(JToken.Parse(cardEffects.text)));
			cardStaticData.Add("cardsPilot", (TomeObject)Tome.Conjure(JToken.Parse(cardsPilot.text)));
			cardStaticData.Add("cardsCrew", (TomeObject)Tome.Conjure(JToken.Parse(cardsCrew.text)));
			cardStaticData.Add("cardsUnit", (TomeObject)Tome.Conjure(JToken.Parse(cardsUnit.text)));
			cardStaticData.Add("cardsEvent", (TomeObject)Tome.Conjure(JToken.Parse(cardsEvent.text)));
			cardStaticData.Add("cardsCounter", (TomeObject)Tome.Conjure(JToken.Parse(cardsCounter.text)));
			card.staticData = cardStaticData;

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
			(game.tCurrentGame["motherships"] as TomeObject).Set(enemyUserId, CreateMothership(enemyUserId, meUserId));

			// Determine starting player
			string startingPlayerId = (string)game.tCurrentGame["players"][random.Next(0, 2)];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));

			// Do initial card draws
			for (int i = 0; i < 5; i += 1) {
				if (i < 3) {
					DrawCardUnit(meUserId);
					DrawCardUnit(enemyUserId);
				} else if (i >= 3 && startingPlayerId == meUserId) {
					DrawCardUnit(enemyUserId);
				} else if (i >= 3 && startingPlayerId == enemyUserId) {
					DrawCardUnit(meUserId);
				}
			}
		}

		//
		JObject CreateUserData(string userName) {
			JObject userData = new JObject();
			userData.Add("name", new JValue(userName));
			userData.Add("titleId", new JValue("title00001"));
			userData.Add("freeword", new JValue(userName));
			userData.Add("color", new JValue("blue"));
			userData.Add("emblemId", new JValue("emblem1001"));
			userData.Add("rankClass", new JValue("Chamber Pot Cleaner"));
			userData.Add("mothershipId", new JValue("mshipWhiteBase"));
			userData.Add("avatarId", new JValue("avatar1021"));
			userData.Add("characterSleeveId", new JValue("sleeve0002"));
			userData.Add("unitSleeveId", new JValue("sleeve0001"));
			return userData;
		}

		//
		JObject CreateMothership(string userId, string enemyId) {
			JObject mothership = new JObject();
			mothership.Add("currentAtk", new JValue(0));
			mothership.Add("currentDef", new JValue(30));
			mothership.Add("instanceId", new JValue(userId));
			mothership.Add("userId", new JValue(userId));
			mothership.Add("enemyId", new JValue(enemyId));
			mothership.Add("tapped", new JValue(0));
			mothership.Add("currentEffects", new JArray());
			return mothership;
		}

		//
		JObject CreateCardCrew(string userId, string cardId, string instanceId) {
			TomeObject cardData = card.GetCrewCard(cardId);

			JObject newCard = (JObject)JToken.Parse(cardData.ToString());
			newCard.Add("instanceId", new JValue(instanceId));
			newCard.Add("tapped", new JValue(0));
			newCard.Add("userId", new JValue(userId));

			return newCard;
		}
		
		//
		JObject CreateCardUnit(string userId, string cardId, string instanceId) {
			TomeObject cardData = card.GetUnitCard(cardId);
			
			JObject newCard = (JObject)JToken.Parse(cardData.ToString());
			newCard.Add("instanceId", new JValue(instanceId));
			newCard.Add("userId", new JValue(userId));

			newCard.Add("currentCost", (JObject)JToken.Parse(newCard["cost"].ToString()));
			
			return newCard;
		}

		//
		JObject CreateBoardIcon(string userId, string cardId, string pilotId, string instanceId) {
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
		string CreateInstanceId() {
			instanceIdCounter++;
			return instanceIdCounter.ToString();
		}

		//
		void CardGiveCrew(string userId, string cardId, string instanceId, string actionType) {
			JObject card = CreateCardCrew(userId, cardId, instanceId);
			(game.tCurrentGame["drawnCards"][userId]["crewCards"] as TomeObject).Set(instanceId, card);

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue(actionType));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
		}

		//
		void CardGiveUnit(string userId, string cardId, string instanceId, string actionType) {
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
		
		//
		public void DrawCardCrew(string userId) {
			string randomCrewCard = allowedCrewCards[random.Next(0, allowedCrewCards.Count)];
			CardGiveCrew(userId, randomCrewCard, CreateInstanceId(), "CardDrawnCrew");
		}

		//
		public void DrawCardUnit(string userId) {
			string randomUnitCard = allowedUnitCards[random.Next(0, allowedUnitCards.Count)];
			CardGiveUnit(userId, randomUnitCard, CreateInstanceId(), "CardDrawnUnit");
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
		public void StartGame(string userId, List<string> burnCards) {
			//
			(game.tCurrentGame["playerStarted"][userId] as TomeValue).Assign(new JValue(true));

			//
			foreach (string instanceId in burnCards) {
				BurnCardUnit(userId, instanceId);
			}

			//
			foreach (string instanceId in burnCards) {
				DrawCardUnit(userId);
			}

			//
			string currentTurn = (string)game.tCurrentGame["currentTurn"];
			if (userId == currentTurn) {
				DrawCardCrew(userId);
				DrawCardUnit(userId);
			}

			// Check if enemy player, if not then process StartGame for enemy after some time
			if (userId != meUserId) {
				return;
			}

			new Timer((object state) => {
				try {
					List<string> enemyBurnCards = new List<string>();
					foreach (var property in game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject) {
						if (random.Next(0, 100) > 50) {
							enemyBurnCards.Add((string)property.Value["instanceId"]);
						}
					}

					//
					StartGame(enemyUserId, enemyBurnCards);

					//
					if (enemyUserId != currentTurn) {
						return;
					}

					new Timer((object state2) => {
						EndTurn();
					}, null, 3000, Timeout.Infinite);
				} catch (Exception error) {
					logger.data(error).error("Enemy could not start game:");
				}
			}, null, 5000, Timeout.Infinite);
		}

		//
		void CreateUnit(string userId, JObject cardData, int boardPosition, string pilotId) {
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
		void CreateCounter(string userId, string cardId, string instanceId) {
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
		public void PlaceCard(string userId, JObject handCard, int? boardPos, List<string> costCards, string pilotCard) {
			int boardPosition = (boardPos != null) ? (int)boardPos : 0;
			string instanceId = (string)handCard["instanceId"];

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("CardPlaced"));
			historyItem.Add("userId", new JValue(userId));
			historyItem.Add("instanceId", new JValue(instanceId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);

			if (card.IsUnitCard((string)handCard["cardId"])) {
				CreateUnit(userId, handCard, boardPosition, pilotCard);
				if (pilotCard != null) {
					DestroyCardCrew(userId, pilotCard);
				}
			}
			if (card.IsCounterCard((string)handCard["cardId"])) {
				CreateCounter(userId, (string)handCard["cardId"], (string)handCard["instanceId"]);
			}

			TomeObject drawnCards = game.tCurrentGame["drawnCards"][userId] as TomeObject;
			foreach (string crewId in costCards) {
				TomeObject crewCard = drawnCards["crewCards"][crewId] as TomeObject;
				if (crewCard != null) {
					crewCard.Set("tapped", (int)crewCard["tapped"] + 1);
				}
			}

			(drawnCards["unitCards"] as TomeObject).Del(instanceId);
			if (userId == meUserId) {
				(game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject).Del(instanceId);
			}
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

		//
		public void EndGame(string winnerId) {
			game.tCurrentGame.Set("playerWinner", winnerId);

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("GameEnded"));
			historyItem.Add("winnerId", new JValue(winnerId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
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
					EndGame((string)boardIcon["enemyId"]);
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

		//
		public void AttackEnemy(string attackerId, string receiverId) {
			TomeObject attacker = GameHelpers.FindBoardIconData(attackerId);
			TomeObject receiver = GameHelpers.FindBoardIconData(receiverId);
			
			TriggerCounter((string)receiver["userId"]);

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("TargetAttacked"));
			historyItem.Add("attackerId", new JValue(attackerId));
			historyItem.Add("receiverId", new JValue(receiverId));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);

			Dictionary<string, int> damageMap = new Dictionary<string, int>();
			damageMap.Add(attackerId, (int)receiver["currentAtk"]);
			damageMap.Add(receiverId, (int)attacker["currentAtk"]);
			GiveDamage(damageMap);
		}

		//
		public void TapIcon(string instanceId) {
			TomeObject boardIcon = GameHelpers.FindBoardIconData(instanceId);
			boardIcon.Set("tapped", (int)boardIcon["tapped"] + 1);

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("UnitTapChange"));
			historyItem.Add("instanceId", new JValue(instanceId));
			historyItem.Add("effectData", JValue.CreateNull());
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);
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
		public void EndTurn() {
			ResetIcons();

			string currentPlayer = (string)game.tCurrentGame["currentTurn"];
			string newTurnPlayer = (meUserId == currentPlayer) ? enemyUserId : meUserId;
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(newTurnPlayer));
			(game.tCurrentGame["isFirstTurn"] as TomeValue).Assign(new JValue(false));

			DrawCardCrew(newTurnPlayer);
			DrawCardUnit(newTurnPlayer);

			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("TurnChanged"));
			historyItem.Add("newTurnPlayer", new JValue(newTurnPlayer));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);

			//
			if (newTurnPlayer == meUserId) {
				return;
			}

			new Timer((object state) => {
				try {
					// Place first card (randomly generated)
					TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
					string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
					string randomUnitCard = allowedUnitCards[random.Next(0, allowedUnitCards.Count)];
					JObject handCard = CreateCardUnit(enemyUserId, randomUnitCard, instanceId);

					TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
					List<string> cost = new List<string>();
					foreach (var property in crewCards) {
						if (random.Next(0, 100) > 50) {
							cost.Add((string)property.Value["instanceId"]);
						}
					}

					PlaceCard(enemyUserId, handCard, null, cost, null);

					// End turn
					EndTurn();
				} catch (Exception error) {
					logger.data(error).error("Failed to play enemy turn:");
				}
			}, null, 2000, Timeout.Infinite);
		}
	}
}