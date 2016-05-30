using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace GameView{
	public class TutorialViewStep5 : TutorialView {

		private Mothership motherShip = null;
		
		private TomeObject counterCard1;
		private TomeObject counterCard2;

		[Header ("CountersManager")]
		public CountersManager countersManager;
		//
		public override void CreateGame() {
			PlayerPrefs.SetString ("Tutorial", "Step5");
			LoadDummyData ();
			CreateStep ();
		}

		public void CreateStep (){
			// 最初行動する側のデータ 1=enemy 0=player
			string startingPlayerId = (string)game.tCurrentGame["players"][0];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));
			
			// チュートリアルのカードデータ
			userCardData = new TutorialUserCardStep5 ();
			enemyCardData = new TutorialEnemyCardStep5 ();
			dialogue = new TutoriaDialogueDataStep5 ();

			// チュートリアル行動
			userActionData = new TutorialActionStep ();

			// index 0 No 2 ~ No 6
			userActionData.AddAction (() => {

				tutorialStepWindow.Init ("Step5", () => {

					// カードをあげる
					DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
					counterCard1 = GameHelpers.FindCardData("1");
					DrawCardCrew (meUserId, TutorialDataFactory.GetNextCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
					
					DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
					
					(game.tCurrentGame["playerStarted"][meUserId] as TomeValue).Assign(new JValue(true));
					(game.tCurrentGame["playerStarted"][enemyUserId] as TomeValue).Assign(new JValue(true));
					
					tutorialGuideWindow.gameObject.SetActive (true);
					
					new Timer((object state) => {
						try {
							tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
								tutorialArrowPanel.GetChild (8).gameObject.SetActive (true);

								// Mask処理 (表示)
								middleground.FindChild ("Mask").gameObject.SetActive (true);
								middleground.FindChild ("PlayerMe").SetAsLastSibling ();
							});
						} catch (Exception error) {
							logger.data(error).error("Failed to play enemy turn:");
						}
					}, null, 1000, Timeout.Infinite);
				});
			});

			// index 1 No 8-2
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (8).gameObject.SetActive (false);

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("PlayerMe").SetSiblingIndex (3);
			});

			// index 2 No 9 - 1
			userActionData.AddAction (() => {

				StartCoroutine (delayAction (() => {

					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);
					
					// Mask処理 (表示)
					middleground.FindChild ("Mask").gameObject.SetActive (true);
					middleground.FindChild ("TurnManager").SetAsLastSibling ();
				}, 2.0f));
			});

			// index 3 No 9 No 10 ~ No 14
			userActionData.AddAction (() => {

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("TurnManager").SetSiblingIndex (4);

				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);

				DrawCardUnit (enemyUserId, TutorialDataFactory.GetCurrentUnitCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

				// 敵のカードを出撃する

				StartCoroutine (delayAction (() => {
					for (int i = 0; i < 2; ++i){
						TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
						string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
						
						string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
						JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
						
						TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
						List<string> cost = GetCost (crewCards, 2);
						
						PlaceCard(enemyUserId, handCard, null, cost, null);
					}

					StartCoroutine (delayAction (() => {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						
							AttackEnemy ("5", meUserId);							
						});
					}, 1.0f));
				}, 3.0f));
		
			});

			// index 4 No 15 ~ 19 20 21-1
			userActionData.AddAction (() => {

				new Timer((object state) => {
					try {
						List<string> targets = new List<string> ();
						targets.Add ("5");
						targets.Add ("8");
						DestroyTargets (targets);

						new Timer((object state2) => {
							try {
								tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

									EndTurn ();
				
									DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));
									DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
									counterCard2 = GameHelpers.FindCardData("9");

									middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;

									StartCoroutine (delayAction (() => {
										tutorialArrowPanel.GetChild (8).gameObject.SetActive (true);

										// Mask処理 (表示)
										middleground.FindChild ("Mask").gameObject.SetActive (true);
										middleground.FindChild ("PlayerMe").SetAsLastSibling ();

									}, 2.0f));
								});
							} catch (Exception error) {
								logger.data(error).error("Failed to play enemy turn:");
							}
						}, null, 3000, Timeout.Infinite);

					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 1000, Timeout.Infinite);
			});

			// index 5 21-2
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (8).gameObject.SetActive (false);

				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("PlayerMe").SetSiblingIndex (3);
			});
			
			// index 6 22 -1
			userActionData.AddAction (() => {

				StartCoroutine (delayAction (() => {
					
					// Mask処理 (表示)
					middleground.FindChild ("Mask").gameObject.SetActive (true);
					middleground.FindChild ("TurnManager").SetAsLastSibling ();
					
					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);
				}, 2.0f));
			});

			// index 7 No 22-2
			userActionData.AddAction (() => {

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("TurnManager").SetSiblingIndex (4);

				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);
				motherShip = gameIcons.FindChild ("PlayerEnemy/Mothership").GetComponent<Mothership> ();
				
				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

				StartCoroutine (delayAction (() => {
					TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
					string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
					
					string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
					JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
					
					TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
					List<string> cost = GetCost (crewCards, 3);
					
					PlaceCard(enemyUserId, handCard, null, cost, null);
					//
					StartCoroutine (delayAction (() => {
						AttackEnemy (instanceId, meUserId);
						//
						StartCoroutine (delayAction (() => {
							List<string> targets = new List<string> ();
							targets.Add ("12");
							DestroyTargets (targets);
							// 敵母艦4ダメージ受ける
							GiveDamage (enemyUserId, 4);
							//
							StartCoroutine (delayAction (() => {

								tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
									EndGame (meUserId, false);
									
									// Clear演出
									gameEndController_Tutorial.Open (meUserId, () => {
										// Final Cleanup
										GetComponent<GameEventManager> ().gameEnded = true;
										gameView.gameTurnManager.CleanupTurnTimeLimit ();
										
										// Move to next scene								
										Application.LoadLevel (GetNextTutorialScene ());
									});
								});
							},2.0f));
						},0.8f));
					},2.0f));
				}, 3.0f));
			});
		}
		
		//
		public override void PlaceCard(string userId, JObject handCard, int? boardPos, List<string> costCards, string pilotCard) {
			int boardPosition = (boardPos != null) ? (int)boardPos : 0;
			string instanceId = (string)handCard["instanceId"];

			Debug.Log (instanceId);

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

				Debug.Log (userId + ";;" + (string)handCard["cardId"] + "::" + (string)handCard["instanceId"]);
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

			if (userActionData.GetCurrentIndex () == 1 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 

			if (userActionData.GetCurrentIndex () == 5 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 
		}

		//
		public override void AttackEnemy(string attackerId, string receiverId) {

			if (CanAttack (attackerId, (string)game.tCurrentGame["currentTurn"]) == false) {
				return;
			}
			TapIcon (attackerId);

			


			TomeObject attacker = GameHelpers.FindBoardIconData(attackerId);
			TomeObject receiver = GameHelpers.FindBoardIconData(receiverId);
			
			TriggerCounter((string)receiver["userId"]);

			if (userActionData.GetCurrentIndex () == 3 && 
			    (string)game.tCurrentGame ["currentTurn"] == enemyUserId){
				
				Debug.Log ("カウンターを発生するので、敵攻撃のアニメーションしない");
				
				userActionData.LauchNextAction ();
				return;
			} 
			
			if (userActionData.GetCurrentIndex () == 7 && 
			    (string)game.tCurrentGame ["currentTurn"] == enemyUserId){
				
				Debug.Log ("カウンターを発生するので、敵攻撃のアニメーションしない");
				return;
			} 
			
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
		public override void EndTurn() {
			ResetIcons();
			
			string currentPlayer = (string)game.tCurrentGame["currentTurn"];
			string newTurnPlayer = (meUserId == currentPlayer) ? enemyUserId : meUserId;
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(newTurnPlayer));
			(game.tCurrentGame["isFirstTurn"] as TomeValue).Assign(new JValue(false));
			
			JObject historyItem = new JObject();
			historyItem.Add("action", new JValue("TurnChanged"));
			historyItem.Add("newTurnPlayer", new JValue(newTurnPlayer));
			(game.tCurrentGame["gameHistory"] as TomeArray).Push(historyItem);


			if (userActionData.GetCurrentIndex () == 2 && 
			    newTurnPlayer == enemyUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 

			if (userActionData.GetCurrentIndex () == 6 && 
			    newTurnPlayer == enemyUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 
		}
	}
}
