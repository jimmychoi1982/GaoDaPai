using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace GameView{
	public class TutorialViewStep2 : TutorialView {

		private bool canAttackMotherShip = false;

		//
		public override void CreateGame() {
			PlayerPrefs.SetString ("Tutorial", "Step2");
			LoadDummyData ();
			CreateStep ();
			
		}

		public void CreateStep (){

			// 最初行動する側のデータ 1=enemy 0=player
			string startingPlayerId = (string)game.tCurrentGame["players"][0];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));
			
			// チュートリアルのカードデータ
			userCardData = new TutorialUserCardStep2 ();
			enemyCardData = new TutorialEnemyCardStep2 ();
			dialogue = new TutoriaDialogueDataStep2 ();

			// チュートリアル行動
			userActionData = new TutorialActionStep ();

			// Index 0 
			userActionData.AddAction (
				() => {
				tutorialStepWindow.Init ("Step2", () => {
					gameIcons.SetSiblingIndex (3);
					gameIcons.FindChild ("Mask").gameObject.SetActive (true);
					
					(game.tCurrentGame["playerStarted"][meUserId] as TomeValue).Assign(new JValue(true));
					(game.tCurrentGame["playerStarted"][enemyUserId] as TomeValue).Assign(new JValue(true));
					
					// カードをあげる
					DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));

					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

					StartCoroutine (delayAction (() => {
						// ゲーム開始して、 Playerが最初のカードを出撃する
						TomeObject meUnitCards = game.tCurrentGame["drawnCards"][meUserId]["unitCards"] as TomeObject;
						string meInstanceId = (string)(meUnitCards.First as JProperty).Value["instanceId"];
						string meUnitCard = TutorialDataFactory.GetCurrentUnitCard (userCardData);
						JObject meHandCard = CreateCardUnit(meUserId, meUnitCard, meInstanceId);
						
						TomeObject meCrewCards = game.tCurrentGame["drawnCards"][meUserId]["crewCards"] as TomeObject;
						List<string> meCost = GetCost (meCrewCards, 1);
						
						PlaceCard(meUserId, meHandCard, null, meCost, null);

						StartCoroutine (delayAction (() => {
							tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
								
								EndTurn ();

								StartCoroutine (delayAction (() => {
									DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
									DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

									StartCoroutine (delayAction (() => {
										// カードを出撃する
										TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
										string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
										string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
										JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
										
										TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
										List<string> cost = GetCost (crewCards, 2);
										
										PlaceCard(enemyUserId, handCard, null, cost, null);
										
										StartCoroutine (delayAction (() => {
											tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
												
												EndTurn ();
												middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
												DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
												DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));
												
												StartCoroutine (delayAction (() => {
													tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
														tutorialArrowPanel.GetChild (5).gameObject.SetActive (true);
														canAttackMotherShip = true;
													});
												}, 2.5f));
											});
										}, 2.0f));
									}, 2.5f));
								}, 2.0f));
							});
						}, 2.0f));
					}, 1.0f));
				});
			});

			// index 1
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (5).gameObject.SetActive (false);

				tutorialErrorWindo.Init ("先に防衛ユニットを破壊してください", () => {
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						tutorialArrowPanel.GetChild (3).gameObject.SetActive (true);
					});
				});
			});

			//No No 16 ~ No 18 index 2
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (3).gameObject.SetActive (false);
				new Timer((object state) => {
					try {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
							gameIcons.SetSiblingIndex (2);
							gameIcons.FindChild ("Mask").gameObject.SetActive (false);

							// 矢印表示
							ArrowActiveWithMiddleground (8, true);
						});
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 3500, Timeout.Infinite);
			});

			// No 20 ~ No 22 No 25 index 3
			userActionData.AddAction (() => {

				gameIcons.SetSiblingIndex (2);
				ArrowActiveWithMiddleground (5, false);

				new Timer((object state) => {
					try {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

							tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

								List<string> ids = new List<string> ();
								ids.Add ("6");
								DestroyTargets (ids);
														
								tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);

								// Mask処理 (表示)
								middleground.FindChild ("Mask").SetSiblingIndex (0);
								middleground.FindChild ("Mask").gameObject.SetActive (true);

								StartCoroutine (delayAction (() => {
									middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
								}, 1.8f));
							});
						});
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 2000, Timeout.Infinite);
			});

			// No 28 index 4
			userActionData.AddAction (() => {

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").SetSiblingIndex (5);
				middleground.FindChild ("Mask").gameObject.SetActive (false);

				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);
				
				// 敵のカードを出撃
				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

				StartCoroutine (delayAction (() => {

					// カードを出撃する
					TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
					string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
					string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
					JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
					
					TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
					List<string> cost = GetCost (crewCards, 2);
					
					PlaceCard(enemyUserId, handCard, null, cost, null);

					StartCoroutine (delayAction (() => {
						EndTurn();
					}, 2.0f));
				}, 3.0f));
			});

			// No 31 index 5
			userActionData.AddAction (() => {

				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;

				DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
				DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));
				
				GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

				new Timer((object state) => {
					try {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
							ArrowActiveWithMiddleground (8, true);
							GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);							
						});
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 2000, Timeout.Infinite);
			});


			// No 35 ~ No 36 index 6
			userActionData.AddAction (() => {
								
				// 敵母艦4ダメージ受ける
				GiveDamage (enemyUserId, 1);

				StartCoroutine (delayAction (() => {
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						// 敵母艦4ダメージ受けて(line 245)爆発、クリア
						// Clear演出
						gameEndController_Tutorial.Open (meUserId, () => {
							// Final Cleanup
							GetComponent<GameEventManager> ().gameEnded = true;
							gameView.gameTurnManager.CleanupTurnTimeLimit ();
							
							// Move to next scene								
							Application.LoadLevel (GetNextTutorialScene ());
						});
					});
				}, 1.0f));
			});
		}
		
		//
		public override void PlaceCard(string userId, JObject handCard, int? boardPos, List<string> costCards, string pilotCard) {
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

			if (userActionData.GetCurrentIndex () == 2 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				gameIcons.SetSiblingIndex (3);
				ArrowActiveWithMiddleground (5, true);
			} 
		}

		//
		public override void AttackEnemy(string attackerId, string receiverId) {

			if (CanAttack (attackerId, (string)game.tCurrentGame["currentTurn"]) == false) {
				return;
			}

			if (canAttackMotherShip == false) {
				return;
			}

			if (userActionData.GetCurrentIndex () == 0 && 
				(string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				if (receiverId != enemyUserId){
					return;
				}else{
					userActionData.LauchNextAction ();
					return;
				}
			} 

			if (userActionData.GetCurrentIndex () == 1 && 
			    receiverId == enemyUserId){

				Debug.Log ("母艦に攻撃ができない！！");
				return;
			} 

			if (userActionData.GetCurrentIndex () == 5 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				Debug.Log ("何やってるんだよ、砲撃で攻撃しでよ！！");
				return;
			} 

			// 攻撃前グレイアウトする			
			TapIcon (attackerId);
			
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

			if (userActionData.GetCurrentIndex () == 1 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 

			if (userActionData.GetCurrentIndex () == 2 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 
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

			if (userActionData.GetCurrentIndex () == 3 && 
			    newTurnPlayer == enemyUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 

			if (userActionData.GetCurrentIndex () == 4 && 
			    newTurnPlayer == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 
		}

		public void ArrowActiveWithMiddleground (int index, bool active){
			tutorialArrowPanel.GetChild (index).gameObject.SetActive (active);
			middleground.FindChild ("Mask").SetAsFirstSibling ();						
			middleground.FindChild ("Mask").gameObject.SetActive (active);
		}

	}
}
