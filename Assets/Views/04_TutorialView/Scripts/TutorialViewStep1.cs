using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Newtonsoft.Json.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace GameView{
	public class TutorialViewStep1 : TutorialView {
		
		private int canFinalAttack = -1;
		private bool canAttackAction = true;

		[Header("AtkDefButton")]
		public TutorialAtkDefButton atkDefButton;

		[Header("BridgeDescription")]
		public BridgeDescription bridgeDescription;

		//
		public override void CreateGame() {
			PlayerPrefs.SetString ("Tutorial", "Step1");
			LoadDummyData ();
			CreateStep ();
			
		}

		public void CreateStep (){

			// 最初行動する側のデータ 1=enemy 0=player
			string startingPlayerId = (string)game.tCurrentGame["players"][1];
			(game.tCurrentGame["startingPlayer"] as TomeValue).Assign(new JValue(startingPlayerId));
			(game.tCurrentGame["currentTurn"] as TomeValue).Assign(new JValue(startingPlayerId));
			
			// チュートリアルのカードデータ
			userCardData = new TutorialUserCardStep1 ();
			enemyCardData = new TutorialEnemyCardStep1 ();
			dialogue = new TutoriaDialogueDataStep1 ();
			
			// チュートリアル行動
			userActionData = new TutorialActionStep ();
			
			// Index 0 
			userActionData.AddAction (
				() => {

				// ステップポップを表示、タップしてから敵が出撃
				// ゲーム開始して、敵が出撃する
				tutorialStepWindow.Init ("Step1", () => {
					// これからゲームの基本ルールを説明します。
					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
					tutorialGuideWindow.gameObject.SetActive (true);
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						
						(game.tCurrentGame["playerStarted"][meUserId] as TomeValue).Assign(new JValue(true));
						(game.tCurrentGame["playerStarted"][enemyUserId] as TomeValue).Assign(new JValue(true));

						// Delayアクション
						StartCoroutine (delayAction (() => {
										
							// カードをあげる							
							DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
							DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

							// Delayアクション
							StartCoroutine (delayAction (() => {
								
								// カードを出撃する
								TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
								string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
								string unitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
								JObject handCard = CreateCardUnit(enemyUserId, unitCard, instanceId);
								
								TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
								List<string> cost = GetCost (crewCards, 1);
								PlaceCard(enemyUserId, handCard, null, cost, null);

								// Delayアクション敵がユニットを出撃させました。
								StartCoroutine (delayAction (() => {
									
									tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
										
										// Mask処理(表示)
										gameIcons.SetSiblingIndex (3);
										gameIcons.FindChild ("Mask").gameObject.SetActive (true);
										gameIcons.FindChild ("PlayerEnemy").SetAsLastSibling ();
										
										/// Atk Def画像を表示
										atkDefButton.Init (() => {
											
											// Mask処理(非表示)
											gameIcons.SetSiblingIndex (2);
											gameIcons.FindChild ("Mask").gameObject.SetActive (false);
											gameIcons.FindChild ("PlayerEnemy").SetSiblingIndex (0);
											
											// 先行の1ターン目は攻撃ができません。
											tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
												// End turn
												EndTurn();

												// Delayアクション
												StartCoroutine (delayAction (() => {
													
													// あなたのターンになりました
													tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
														
														DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
														DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));

														// 右下の矢印を表示まで、この間操作できないように
														GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

														// Delayアクション右下の矢印を表示
														StartCoroutine (delayAction (() => {
															middleground.FindChild ("PlayerMe").SetSiblingIndex (5);
															middleground.FindChild ("Mask").gameObject.SetActive (true);
															tutorialArrowPanel.GetChild (0).gameObject.SetActive (true);

															// 操作できるように戻る
															GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);	

														}, 2.0f));
													});
												}, 1.0f));
											});
										});
									});
								}, 2.0f));
							}, 3.0f));
						}, 1.0f));
					});
				});
			});
			
			//　Swipeの矢印 Index 1
			userActionData.AddAction ( () => {

				tutorialArrowPanel.GetChild (0).gameObject.SetActive (false);
				StartCoroutine (delayAction (() => {
					tutorialArrowPanel.GetChild (1).gameObject.SetActive (true);
				}, 0.5f));
			});
			
			//　Swipeの矢印 Index 2 (Swipe失敗時の対応)
			userActionData.AddAction ( () => {
				
				tutorialArrowPanel.GetChild (1).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (0).gameObject.SetActive (true);

				StartCoroutine (delayAction (() => {
					tutorialArrowPanel.GetChild (1).gameObject.SetActive (false);
				}, 0.5f));
			});
			
			//　Swipe矢印を消し、ポップを表示 Index 3
			userActionData.AddAction ( () => {

				tutorialArrowPanel.GetChild (0).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (1).gameObject.SetActive (false);
														
				GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

				StartCoroutine (delayAction (() => {
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

						bridgeDescription.ActiveFrame1AndAppear ();
						bridgeDescription.Init (() => {

							bridgeDescription.gameObject.SetActive (false);
							tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

								bridgeDescription.UnActiveFrame1ActionFrame2 ();								
								bridgeDescription.gameObject.SetActive (true);

								bridgeDescription.Init (() => {

									bridgeDescription.gameObject.SetActive (false);
									tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
										// Mask処理 (表示)
										foreground.FindChild ("panel_launchCard_overlay/CostIcons").SetSiblingIndex (5);
										foreground.FindChild ("panel_launchCard_overlay/Mask").gameObject.SetActive (true);
										tutorialArrowPanel.GetChild (2).gameObject.SetActive (true);
										
										GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);
									});
								});
							});
						});
					});
				}, 0.8f));
			});
			
			//　左下の矢印を消し、ポップを表示 Index 4
			userActionData.AddAction ( () => {

				// Mask処理 (非表示)
				foreground.FindChild ("panel_launchCard_overlay/CostIcons").SetSiblingIndex (1);
				foreground.FindChild ("panel_launchCard_overlay/Mask").gameObject.SetActive (false);

				tutorialArrowPanel.GetChild (2).gameObject.SetActive (false);

				// No23でコストアイコンが一回だけ押せる対応(押せない)
				SetCostIconToggleDraggerToggleEnabled (false);

			});
			
			// 攻撃矢印を表示 index 5
			userActionData.AddAction ( () => {
								
				StartCoroutine (delayAction (() => {
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						// No23でコストアイコンが一回だけ押せる対応(押せるに戻る)
						SetCostIconToggleDraggerToggleEnabled (true);
						
						tutorialArrowPanel.GetChild (3).gameObject.SetActive (true);
						gameIcons.SetSiblingIndex (3);
					});
				}, 1.0f));
			});
			
			// 攻撃矢印を非表示,ターンを終了矢印表示 index 6
			userActionData.AddAction ( () => {

				tutorialArrowPanel.GetChild (3).gameObject.SetActive (false);

				StartCoroutine (delayAction (() => {
				
					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
					
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);
					middleground.SetSiblingIndex (3);
					middleground.FindChild ("TurnManager").SetSiblingIndex (4);
				}, 3.5f));
			});
			
			// ターンを終了矢印非表示して、敵のターン index 7 
			userActionData.AddAction ( () => {
				
				// ターンを終了矢印非表示
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;
				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);
				
				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));

				StartCoroutine (delayAction (() => {
					// 攻撃を受けた......... No 28 ~ 29
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {

						StartCoroutine (delayAction (() => {
							// Place first card (randomly generated)
							TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
							string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
							string randomUnitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
							JObject handCard = CreateCardUnit(enemyUserId, randomUnitCard, instanceId);
							
							TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
							List<string> cost = GetCost (crewCards, 2);
							
							PlaceCard(enemyUserId, handCard, null, cost, null);

							StartCoroutine (delayAction (() => {
								AttackEnemy (instanceId, "3");

								StartCoroutine (delayAction (() => {
									EndTurn ();
									
									GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

									// 攻撃行動を禁止
									canAttackAction = false;

									DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
									DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));

									StartCoroutine (delayAction (() => {

										GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);
										ArrowActiveWithMiddleground (8, true);
									}, 2.0f));
								}, 4.0f));
							}, 2.0f));
						}, 1.0f));
					});
				}, 2.0f));
			});
			
			// No36 ~ 37 index 8 
			userActionData.AddAction (() => {

				new Timer((object state) => {
					try {
						// No36 ~ 37
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
							
							tutorialArrowPanel.GetChild (5).gameObject.SetActive (true);
							
							// Mask処理 (表示)
							gameIcons.SetSiblingIndex (3);
							gameIcons.FindChild ("Mask").SetSiblingIndex (0);
							gameIcons.FindChild ("Mask").gameObject.SetActive (true);

							// 攻撃行動を解除
							canAttackAction = true;
						});
					} catch (Exception error) {
						logger.data(error).error("Failed to play enemy turn:");
					}
				}, null, 800, Timeout.Infinite);
			});
			
			// No39 index 9
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (5).gameObject.SetActive (false);

				// Mask処理 (非表示)
				gameIcons.SetSiblingIndex (2);				
				gameIcons.FindChild ("Mask").SetSiblingIndex (2);
				gameIcons.FindChild ("Mask").gameObject.SetActive (false);

				StartCoroutine (delayAction (() => {
					
					// End turn Arrow & Mask
					middleground.FindChild ("Mask").gameObject.SetActive (true);
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);

					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
				}, 2.0f));
			});
			
			// No41-1 ~ No42 ~ No43 index 10
			userActionData.AddAction (() => {

				// End turn Arrow & Mask
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);

				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
				
				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;

				StartCoroutine (delayAction (() => {

					StartCoroutine (delayAction (() => {
						// Place first card (randomly generated)
						TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
						string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
						string randomUnitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
						JObject handCard = CreateCardUnit(enemyUserId, randomUnitCard, instanceId);
						
						TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
						List<string> cost = GetCost (crewCards, 1);
						
						PlaceCard(enemyUserId, handCard, null, cost, null);
						
						// 攻撃
						StartCoroutine (delayAction (() => {
							AttackEnemy (instanceId, "7");
							// ターン終了
							StartCoroutine (delayAction (() => {
								AttackEnemy ("5", "7");

								StartCoroutine (delayAction (() => {
									EndTurn ();
									
									DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
									DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));
									
									GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

									StartCoroutine (delayAction (() => {
										tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
											ArrowActiveWithMiddleground (8, true);											
											GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);
										});
									}, 2.0f));
								}, 4.0f));
							}, 4.0f));
						}, 2.0f));
					}, 1.0f));
				}, 2.0f));
				
			});
			
			// No45 ~ 47 index 11
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (3).gameObject.SetActive (true);

				// Mask処理 (表示)
				gameIcons.SetSiblingIndex (3);
				gameIcons.FindChild ("Mask").SetSiblingIndex (0);
				gameIcons.FindChild ("Mask").gameObject.SetActive (true);
			});
			
			// No48 index 12
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (3).gameObject.SetActive (false);

				// Mask処理 (非表示)
				gameIcons.SetSiblingIndex (2);				
				gameIcons.FindChild ("Mask").SetSiblingIndex (2);
				gameIcons.FindChild ("Mask").gameObject.SetActive (false);

				StartCoroutine (delayAction (() => {
					
					// End turn Arrow & Mask
					middleground.FindChild ("Mask").gameObject.SetActive (true);
					tutorialArrowPanel.GetChild (4).gameObject.SetActive (true);

					middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = true;
				}, 3.0f));
				
			});
			
			// No48 index 13
			userActionData.AddAction (() => {

				// End turn Arrow & Mask
				middleground.FindChild ("Mask").gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (4).gameObject.SetActive (false);

				DrawCardUnit (enemyUserId, TutorialDataFactory.GetNextCard (enemyCardData));
				DrawCardCrew (enemyUserId, TutorialDataFactory.GetCrewCard (enemyCardData));
				
				middleground.FindChild ("TurnManager/TurnButton/btn_endTurn").GetComponent<Button> ().enabled = false;

				StartCoroutine (delayAction (() => {

					StartCoroutine (delayAction (() => {

						// Place first card (randomly generated)
						TomeObject unitCards = game.tCurrentGame["drawnCards"][enemyUserId]["unitCards"] as TomeObject;
						string instanceId = (string)(unitCards.First as JProperty).Value["instanceId"];
						string randomUnitCard = TutorialDataFactory.GetCurrentUnitCard (enemyCardData);
						JObject handCard = CreateCardUnit(enemyUserId, randomUnitCard, instanceId);
						
						TomeObject crewCards = game.tCurrentGame["drawnCards"][enemyUserId]["crewCards"] as TomeObject;
						List<string> cost = GetCost (crewCards, 1);
						
						PlaceCard(enemyUserId, handCard, null, cost, null);

						StartCoroutine (delayAction (() => {
							AttackEnemy (instanceId, meUserId);

							StartCoroutine (delayAction (() => {
								EndTurn ();
							}, 4.0f));
						}, 2.0f));
					}, 1.0f));
				}, 3.0f));
			});
			
			// No55 ~ 56 index 14
			userActionData.AddAction (() => {

				DrawCardUnit (meUserId, TutorialDataFactory.GetNextCard (userCardData));
				DrawCardCrew (meUserId, TutorialDataFactory.GetCrewCard (userCardData));
				
				GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (true);

				StartCoroutine (delayAction (() => {
					// 
					tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
						ArrowActiveWithMiddleground (8, true);
						GameObject.Find ("Canvas").transform.FindChild ("CannotTapTutorialMask").gameObject.SetActive (false);
					});
				}, 2.0f));
			});
			
			// No 58　index 15
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (6).gameObject.SetActive (true);
				tutorialArrowPanel.GetChild (7).gameObject.SetActive (true);

				// Mask処理 (表示)
				gameIcons.SetSiblingIndex (3);
				gameIcons.FindChild ("Mask").SetSiblingIndex (0);
				gameIcons.FindChild ("Mask").gameObject.SetActive (true);
			});
			
			// 勝利になり、Step1完了 index 16
			userActionData.AddAction (() => {
				
				tutorialArrowPanel.GetChild (6).gameObject.SetActive (false);
				tutorialArrowPanel.GetChild (7).gameObject.SetActive (false);

				// Mask処理 (非表示)
				gameIcons.SetSiblingIndex (2);				
				gameIcons.FindChild ("Mask").SetSiblingIndex (2);
				gameIcons.FindChild ("Mask").gameObject.SetActive (false);
				
				new Timer((object state) => {
					try {
						tutorialGuideWindow.Init (TutorialDataFactory.GetNextDialogue (dialogue), () => {
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
				}, null, 3500, Timeout.Infinite);
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

			if ((string)game.tCurrentGame ["currentTurn"] == meUserId) {

				if (userActionData.GetCurrentIndex () == 4 ||
				    userActionData.GetCurrentIndex () == 7 ||
				    userActionData.GetCurrentIndex () == 10){

					userActionData.LauchNextAction ();
				}

				if (userActionData.GetCurrentIndex () == 14) {
					
					canFinalAttack = 0;
					userActionData.LauchNextAction ();
				}
			}
		}


		//
		public override void AttackEnemy(string attackerId, string receiverId) {

			if (CanAttack (attackerId, (string)game.tCurrentGame["currentTurn"]) == false || 
			    canAttackAction == false) {
				return;
			}

			if (userActionData.GetCurrentIndex () == 5 && 
			    receiverId == enemyUserId) {
				Debug.Log ("今戦艦に攻撃できない");
				return;
			}
			if (userActionData.GetCurrentIndex () == 8 &&
			    receiverId == meUserId) {
				Debug.Log ("今Unitに攻撃できない");
				return;
			}
			if (userActionData.GetCurrentIndex () == 11 &&
			    receiverId == enemyUserId) {
				
				Debug.Log ("今戦艦に攻撃できない");
				return;
			}
			
			if (userActionData.GetCurrentIndex () == 14 || 
			    userActionData.GetCurrentIndex () == 15) {
				
				if (canFinalAttack < 0 || receiverId != enemyUserId){
					Debug.Log ("今攻撃できない");
					return;
				}
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
			
			if (userActionData.GetCurrentIndex () == 5) {
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 8) {
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 11 && 
			    (string)game.tCurrentGame["currentTurn"] == meUserId) {
				
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 15 && 
			    (string)game.tCurrentGame["currentTurn"] == meUserId) {
				
				if (receiverId == enemyUserId){
					canFinalAttack++;

					// 矢印消す処理
					var SiblingIndex = getMeGameIconSiblingIndexByInstanceId (attackerId);

					if (SiblingIndex == 0){

						tutorialArrowPanel.GetChild (6).gameObject.SetActive (false);
					}else{
						tutorialArrowPanel.GetChild (7).gameObject.SetActive (false);
					}
				}
				if (canFinalAttack == 2){
					userActionData.LauchNextAction ();
				}
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
			
			if (userActionData.GetCurrentIndex () == 6){
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 9){
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 12){
				userActionData.LauchNextAction ();
			}
			
			if (userActionData.GetCurrentIndex () == 13 && 
			    newTurnPlayer == meUserId) {
				
				userActionData.LauchNextAction ();
			}
		}

		/// <summary>
		/// instanceIdにより該当GameIconのSiblingIndexを取得
		/// </summary>
		/// <returns>The me game icon sibling index by instance identifier.</returns>
		/// <param name="instanceId">Instance identifier.</param>
		private int getMeGameIconSiblingIndexByInstanceId (string instanceId){

			var BoardIcons = gameIcons.FindChild ("PlayerMe/BoardIcons");
			foreach (Transform icon in BoardIcons) {
				TomeObject iconData = icon.GetComponent <BoardIcon> ().iconData;

				if ((string)iconData.GetValue ("instanceId") == instanceId){
					return icon.GetSiblingIndex ();
				}
			}

			Debug.Log ("Can't find Sibling Index");
			return -1;
		}

		public void ArrowActiveWithMiddleground (int index, bool active, int arrowNum = 1){
			// Arrow1表示
			middleground.FindChild ("Mask").gameObject.SetActive (active);

			var arrowParent = tutorialArrowPanel.GetChild (index);
			arrowParent.gameObject.SetActive (active);

			if (arrowNum == 2) {
				arrowParent.GetChild (1).gameObject.SetActive (active);
			} else if (arrowNum == 3) {
				arrowParent.GetChild (1).gameObject.SetActive (active);
				arrowParent.GetChild (2).gameObject.SetActive (active);
			}
		}
	}
}
