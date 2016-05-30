using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace GameView{
	public class TutorialViewStep3 : TutorialView {

		private bool canFinalAttack = false;

		//
		public override void CreateGame() {
			
			//
			PlayerPrefs.SetString ("Tutorial", "Step3");
			LoadDummyData ();
			CreateStep ();	
		}

		public void CreateStep (){
			
			// 最初行動する側のデータ 1=enemy 0=player
			string startingPlayerId = (string)game.tCurrentGame["players"][0];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));
			
			// チュートリアルのカードデータ
			userCardData = new TutorialUserCardStep3 ();
			enemyCardData = new TutorialEnemyCardStep3 ();
			dialogue = new TutoriaDialogueDataStep3 ();

			// チュートリアル行動
			userActionData = new TutorialActionStep ();

			// No 2 ~ No 5 Index 0 
			userActionData.AddAction (() => {

				gameIcons.SetSiblingIndex (3);
				gameIcons.FindChild ("Mask").gameObject.SetActive (true);

				tutorialStepWindow.Init ("Step3", () => {

					// カードをあげる
					DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetNextCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetCurrentCrewCard (userCardData));
					DrawCardCrew (meUserId, TutorialDataFactory.GetNextCrewCard (userCardData));
					
					DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
					DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
					
					(game.tCurrentGame["playerStarted"][meUserId] as TomeValue).Assign(new JValue(true));
					(game.tCurrentGame["playerStarted"][enemyUserId] as TomeValue).Assign(new JValue(true));

					new Timer((object state) => {
						try {
							// ゲーム開始して、 Playerが最初のカードを出撃する
							TomeObject meUnitCards = game.tCurrentGame["drawnCards"][meUserId]["unitCards"] as TomeObject;
							string meInstanceId = (string)(meUnitCards.First as JProperty).Value["instanceId"];
							string meUnitCard = TutorialDataFactory.GetCurrentUnitCard (userCardData);
							JObject meHandCard = CreateCardUnit(meUserId, meUnitCard, meInstanceId);
							
							TomeObject meCrewCards = game.tCurrentGame["drawnCards"][meUserId]["crewCards"] as TomeObject;
							List<string> meCost = new List<string>();
							PlaceCard(meUserId, meHandCard, null, meCost, null);
							
							DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
							// 敵のカードを出撃する
							TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
							string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
							string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
							JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
							
							TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
							List<string> cost = GetCost (crewCards, 3);
							
							PlaceCard(enemyUserId, handCard, null, cost, null);
							
							new Timer((object state2) => {
								try {
									tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
										tutorialArrowPanel.GetChild (5).gameObject.SetActive (true);
									});
								} catch (Exception error) {
									logger.data(error).error("Failed to play enemy turn:");
								}
							}, null, 1000, Timeout.Infinite);
						} catch (Exception error) {
							logger.data(error).error("Failed to play enemy turn:");
						}
					}, null, 2000, Timeout.Infinite);
				});
			});

			// No 7 ~ 12-1 Index 1
			userActionData.AddAction (() => {
							
				tutorialArrowPanel.GetChild (5).gameObject.SetActive (false);

				tutorialErrorWindo.Init ("制圧効果で攻撃できません", () => {

					gameIcons.SetSiblingIndex (2);
					gameIcons.FindChild ("Mask").SetSiblingIndex (2);

					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

						tutorialArrowPanel.GetChild (8).gameObject.SetActive (true);

					});
				});
			});

			// index 2
			userActionData.AddAction (() => {
				tutorialArrowPanel.GetChild (8).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (2).gameObject.SetActive (true);

				// 同期処理すると、Toggleにアクセスできないので、ちょっと時間ずれて処理する（CostIconのアニメーション待ちの為）
				StartCoroutine (delayAction (() => {
					SetCostIconToggleDraggerToggleEnabled (false, 3);
				}, 0.2f));
			});

			// index 3
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (2).gameObject.SetActive (false);

				SetCostIconToggleDraggerToggleEnabled (false);

				tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

					foreground.FindChild ("panel_launchCard_overlay/LaunchDragger").SetSiblingIndex (0);
					foreground.FindChild ("panel_launchCard_overlay/Mask").gameObject.SetActive (true);

					tutorialArrowPanel.GetChild (9).gameObject.SetActive (true);
				});
			});

			// index 4 No 16-1 ~ No 16-2
			userActionData.AddAction (() => {
				tutorialArrowPanel.GetChild (9).gameObject.SetActive (false);

				// Mask処理 (表示)
				foreground.FindChild ("panel_launchCard_overlay/LaunchDragger").SetSiblingIndex (4);
				foreground.FindChild ("panel_launchCard_overlay/CostIcons").SetSiblingIndex (0);
				foreground.FindChild ("panel_launchCard_overlay/Mask").gameObject.SetActive (true);
			});

			// index 5 No 18
			userActionData.AddAction (() => {

				GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);			
				
				// Mask処理(非表示)
				foreground.FindChild ("panel_launchCard_overlay/Mask").gameObject.SetActive (false);
				// Mask処理 (表示)
				gameIcons.SetSiblingIndex (4);
				gameIcons.FindChild("Mask").SetAsFirstSibling ();
				gameIcons.FindChild("Mask").gameObject.SetActive (true);

				StartCoroutine (delayAction (() => {
					// エアマスターからハイザックへ矢印を表示
					var boardIcons = gameIcons.FindChild ("PlayerMe/BoardIcons");
					foreach (Transform child in boardIcons){
						TomeObject data = child.GetComponent<BoardIcon> ().iconData;
						if ((string)data.GetValue("instanceId") == "10"){
							if (child.GetSiblingIndex () == 0){
								tutorialArrowPanel.GetChild (11).gameObject.SetActive (true);
							}else{
								tutorialArrowPanel.GetChild (10).gameObject.SetActive (true);
							}
						}
					}
				
					GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);								
				}, 1.0f));
			});

			// index 6 No 19 No 20 ~ No 22 No 23-1
			userActionData.AddAction (() => {

				tutorialArrowPanel.GetChild (10).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (11).gameObject.SetActive (false);

				new Timer((object state) => {
					try {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

							// グレイズからハイザックへ矢印を表示
							var boardIcons = gameIcons.FindChild ("PlayerMe/BoardIcons");
							foreach (Transform child in boardIcons){
								TomeObject data = child.GetComponent<BoardIcon> ().iconData;
								if ((string)data.GetValue("instanceId") == "1"){
									if (child.GetSiblingIndex () == 0){
										tutorialArrowPanel.GetChild (6).gameObject.SetActive (true);
									}else{
										tutorialArrowPanel.GetChild (7).gameObject.SetActive (true);
									}
								}
							}

							canFinalAttack = true;
						});
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 3500, Timeout.Infinite);
			});

			// index 7 No 24 ~ No 28
			userActionData.AddAction (() => {
				tutorialArrowPanel.GetChild (6).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (7).gameObject.SetActive (false);

				gameIcons.SetSiblingIndex (2);

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

			Debug.Log (instanceId);

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

			//
			if (userActionData.GetCurrentIndex () == 4 && 
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

			if (userActionData.GetCurrentIndex () == 0 && 
				(string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				if (receiverId != enemyUserId){
					tutorialErrorWindo.Init ("制圧効果で攻撃できません", () => {

					});
					return;
				}else{
					userActionData.LauchNextAction ();
					return;
				}
			} 

			if (userActionData.GetCurrentIndex () == 5 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){
				
				if (receiverId == enemyUserId){
					Debug.Log ("母艦攻撃できないんだよ！");
					return;
				}else if (attackerId != "10"){
					Debug.Log ("エアマスタで攻撃してよ！");
					return;
				}
			} 

			if (userActionData.GetCurrentIndex () == 6 && canFinalAttack == false) {
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

			if (userActionData.GetCurrentIndex () == 5 && 
			    (string)game.tCurrentGame ["currentTurn"] == meUserId){

				userActionData.LauchNextAction ();
				
				// 強制的に敵を破壊のため、もう一度ダメージを与える
				GiveDamage(damageMap);
				return;
			} 

			if (userActionData.GetCurrentIndex () == 6 && 
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
		}
	}
}
