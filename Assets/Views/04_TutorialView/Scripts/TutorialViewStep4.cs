using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace GameView{
	public class TutorialViewStep4 : TutorialView {
		
		TomeObject eventCard1 = null;
		TomeObject eventCard2 = null;

		//
		public override void CreateGame() {
			PlayerPrefs.SetString ("Tutorial", "Step4");
			LoadDummyData ();
			CreateStep ();
		}

		public void CreateStep (){

			// 最初行動する側のデータ 1=enemy 0=player
			string startingPlayerId = (string)game.tCurrentGame["players"][0];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));
			
			// チュートリアルのカードデータ
			userCardData = new TutorialUserCardStep4 ();
			enemyCardData = new TutorialEnemyCardStep4 ();
			dialogue = new TutoriaDialogueDataStep4 ();

			// チュートリアル行動
			userActionData = new TutorialActionStep ();

			// index 0 No 2 ~ 4 ~ 8-1
			userActionData.AddAction (() => {
				Debug.Log ("Step4 Baby!!");
			
				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;

				tutorialStepWindow.Init ("Step4", () => {

					// カードをあげる
					GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);
					DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetNextCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
					
					DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

					(game.tCurrentGame["playerStarted"][meUserId] as TomeValue).Assign(new JValue(true));
					(game.tCurrentGame["playerStarted"][enemyUserId] as TomeValue).Assign(new JValue(true));


					StartCoroutine (delayAction (() => {
						// ゲーム開始して、 Playerが最初のカードを出撃する
						TomeObject meUnitCards = game.tCurrentGame["drawnCards"][meUserId]["unitCards"] as TomeObject;
						string meInstanceId = (string)(meUnitCards.First as JProperty).Value["instanceId"];
						string meUnitCard = TutorialDataFactory.GetCurrentUnitCard (userCardData);
						JObject meHandCard = CreateCardUnit(meUserId, meUnitCard, meInstanceId);
						
						TomeObject meCrewCards = game.tCurrentGame["drawnCards"][meUserId]["crewCards"] as TomeObject;
						List<string> meCost = new List<string>();
						PlaceCard(meUserId, meHandCard, null, meCost, null);
						
						// 敵のカードを出撃する
						TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
						string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
						string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
						JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
						
						TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
						List<string> cost = GetCost (crewCards, 3);
						
						PlaceCard(enemyUserId, handCard, null, cost, null);

						StartCoroutine (delayAction (() => {
							tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
								
								DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));	
								DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
								
								eventCard1 = GameHelpers.FindCardData("8");

								StartCoroutine (delayAction (() => {
									tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
										
										ArrowActiveWithMiddleground (8, true);
										middleground.FindChild ("PlayerMe").SetAsLastSibling ();
										
										GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);
									});
								}, 2.0f));
							});
						}, 1.5f));
					}, 2.0f));
				});
			});

			// index 1 No 8-2
			userActionData.AddAction (() => {

				// Mask処理 (非表示)
				ArrowActiveWithMiddleground (8, false);

				middleground.FindChild ("PlayerMe").SetSiblingIndex (3);
			});

			// index 2 No 9-1
			userActionData.AddAction (() => {

				ArrowActiveWithMiddleground (12, false);

				//説明ポップアップ　選択Mask
				var cardTipsManager = foreground.FindChild ("cardTips").GetComponent<CardTipsManager> ();
				cardTipsManager.Show (eventCard1);

				var buttonPanel = GameObject.Find ("Canvas").transform.FindChild ("TutorialButtonPanel");
				buttonPanel.gameObject.SetActive (true);
				buttonPanel.FindChild ("Button1").gameObject.SetActive (true);

				gameIcons.FindChild ("PlayerMe/Mask").gameObject.SetActive (true);
			});

			// index 3 No 9-2 ~ No12
			userActionData.AddAction (() => {

				// ステータスを与える
				var boardIcon = gameIcons.FindChild ("PlayerMe/BoardIcons").GetChild (0).GetComponent <BoardIcon> ();
				boardIcon.iconData.Set ("currentAtk", new JValue ("4"));
				boardIcon.iconData.Set ("currentDef", new JValue ("4"));
				Debug.Log (boardIcon.iconData);
				boardIcon.UpdateFromData ();

				var buttonPanel = GameObject.Find ("Canvas").transform.FindChild ("TutorialButtonPanel");
				buttonPanel.gameObject.SetActive (false);
				buttonPanel.FindChild ("Button1").gameObject.SetActive (false);
				
				gameIcons.FindChild ("PlayerMe/Mask").gameObject.SetActive (false);

				tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
				
					tutorialArrowPanel.GetChild (3).gameObject.SetActive (true);					
				});
			});

			// index 4 No 13 ~ 14
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (3).gameObject.SetActive (false);	

				StartCoroutine (delayAction (() => {
					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);
					// Mask処理 (表示)
					middleground.FindChild ("Mask").gameObject.SetActive (true);
					middleground.FindChild ("TurnManager").SetAsLastSibling ();
				}, 3.5f));
			});

			// index 5 No 15-1 ~ 16
			userActionData.AddAction (() => {

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("TurnManager").SetSiblingIndex (4);

				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);	
				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

				// Delayアクション
				StartCoroutine (delayAction (() => {
					// 敵のカードを出撃する
					TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
					string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
					string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
					JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
					
					TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
					List<string> cost = GetCost (crewCards, 1);
					
					PlaceCard(enemyUserId, handCard, null, cost, null);
					
					// Delayアクション
					StartCoroutine (delayAction (() => {
						EndTurn ();
					}, 1.0f));
				}, 3.0f));
			});

			// index 6 No 17 ~ No20-1
			userActionData.AddAction (() => {
					
				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
										
				GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);			

				DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
				eventCard2 = GameHelpers.FindCardData("12");
				DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));

				new Timer((object state) => {
					try {

						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
				
							tutorialArrowPanel.GetChild (8).gameObject.SetActive (true);	
							// Mask処理 (表示)
							middleground.FindChild ("Mask").gameObject.SetActive (true);
							middleground.FindChild ("PlayerMe").SetAsLastSibling ();

							GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);							
						});

					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 2000, Timeout.Infinite);
			});

			// index 7 No 20-2
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (8).gameObject.SetActive (false);

				// Mask処理 (非表示)
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("PlayerMe").SetSiblingIndex (3);
			});

			// index 8 No 21
			userActionData.AddAction (() => {
				ArrowActiveWithMiddleground (12, false);
				//説明ポップアップ　選択Mask
				var cardTipsManager = foreground.FindChild ("cardTips").GetComponent<CardTipsManager> ();
				cardTipsManager.Show (eventCard2);
				
				var buttonPanel = GameObject.Find ("Canvas").transform.FindChild ("TutorialButtonPanel");
				buttonPanel.gameObject.SetActive (true);
				buttonPanel.FindChild ("Button2").gameObject.SetActive (true);

				gameIcons.FindChild ("PlayerEnemy").SetSiblingIndex (2);
				gameIcons.FindChild ("PlayerEnemy/Mask").gameObject.SetActive (true);
			});

			// index 9
			userActionData.AddAction (() => {
				
				var buttonPanel = GameObject.Find ("Canvas").transform.FindChild ("TutorialButtonPanel");
				buttonPanel.gameObject.SetActive (false);
				buttonPanel.FindChild ("Button2").gameObject.SetActive (false);
				gameIcons.FindChild ("PlayerEnemy/Mask").gameObject.SetActive (false);

				GiveDamage ("10", 3);

				new Timer((object state) => {
					try {
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
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 2000, Timeout.Infinite);
			});
		}

		public void Button1 (){
			if (userActionData.GetCurrentIndex () == 2 && 
				(string)game.tCurrentGame ["currentTurn"] == meUserId) {
				
				userActionData.LauchNextAction ();
			}
		}

		public void Button2 (){
			if (userActionData.GetCurrentIndex () == 8 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId) {
				
				userActionData.LauchNextAction ();
				return;
			}
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

			if (userActionData.GetCurrentIndex () == 7 && 
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

			if (userActionData.GetCurrentIndex () == 3 && 
			    receiverId == enemyUserId){

				tutorialErrorWindo.Init ("先に防衛ユニットを破壊してください", () => {

				});
				return;
			} 

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

			if (userActionData.GetCurrentIndex () == 3 && 
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


			if (userActionData.GetCurrentIndex () == 4 && 
			    newTurnPlayer == enemyUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 

			if (userActionData.GetCurrentIndex () == 5 && 
			    newTurnPlayer == meUserId){
				
				userActionData.LauchNextAction ();
				return;
			} 
		}

		public void ArrowActiveWithMiddleground(int index, bool active){
			tutorialArrowPanel.GetChild (index).gameObject.SetActive (active);	
			// Mask処理 (表示)
			middleground.FindChild ("Mask").gameObject.SetActive (active);
		}
	}
}
