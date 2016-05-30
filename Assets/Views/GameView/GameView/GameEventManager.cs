using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json.Linq;

using UnityEngine;


namespace GameView {
	public class GameEventManager : MonoBehaviour {
		//
		Mage mage { get { return Mage.Instance; } }
		Logger logger { get { return mage.logger("GameEventManager"); } }
		Card card { get { return Card.Instance; } }
		Game game { get { return Game.Instance; } }
		GameView gameView { get { return GameView.Instance; } }
		Master master { get { return Master.Instance; }}

		//
		[System.NonSerialized]
		public bool gameEnded = false;
		bool inCounterSequence = false;
		bool inAttackSequence = false;


		//
		void Start() {
			TomeObject currentGame = game.tCurrentGame;

			//
			(currentGame["playerStarted"] as TomeObject).onChanged += (JToken oldValue) => {
				// Prevent execution on a stale game
				if (gameEnded) {
					return;
				}

				TaskManagerMainThread.Queue(PlayerStarted());
			};

			//
			(currentGame["gameHistory"] as TomeArray).onAdd += (JToken key) => {
				// Prevent execution on a stale game
				if (gameEnded) {
					return;
				}

				TomeObject newEntry = Tome.PathValue(currentGame["gameHistory"], key) as TomeObject;
				TaskManagerMainThread.Queue(ProcessHistory(newEntry));
			};

			//
			(currentGame["currentQuestion"] as TomeValue).onChanged += (JToken oldValue) => {
				// Prevent execution on a stale game
				if (gameEnded) {
					return;
				}

				// Check if the question was removed
				JToken token = currentGame["currentQuestion"];
				if (token.Type == JTokenType.Null) {
					TaskManagerMainThread.Queue(CloseQuestions());
					return;
				}

				// Check if the question was for us
				TomeObject currentQuestion = token as TomeObject;
				if ((string)currentQuestion["userId"] != gameView.meUserId) {
					return;
				}

				string questionType = (string)currentQuestion["option"];
				switch (questionType) {
					case "SelectCards":
						TaskManagerMainThread.Queue(SelectCards((TomeArray)currentQuestion["possibleChoices"], (int)currentQuestion["choiceMin"], (int)currentQuestion["choiceMax"]));
						break;
					case "SelectTarget":
						TaskManagerMainThread.Queue(SelectTarget((TomeArray)currentQuestion["possibleTargets"]));
						break;
					default:
						logger.data(currentQuestion).debug("Unknown Question: " + questionType);
						break;
				}
			};
		}

		//
		IEnumerator PlayerStarted() {
			gameView.gameInitialDraw.StartValuesChanged();
			yield break;
		}

		//
		IEnumerator CloseQuestions() {
			gameView.gameChoiceCard.Close();
			gameView.gameChoiceTarget.Close();
			yield break;
		}

		//
		IEnumerator SelectCards(TomeArray possibleChoices, int choiceMin, int choiceMax) {
			gameView.gameChoiceCard.Open(possibleChoices, choiceMin, choiceMax);
			yield break;
		}

		//
		IEnumerator SelectTarget(TomeArray possibleTargets) {
			gameView.gameChoiceTarget.Open(possibleTargets);
			yield break;
		}

		//
		IEnumerator ProcessHistory(TomeObject historyItem) {

			if (historyItem["userId"] != null && ((string) historyItem["userId"]).Contains("cpu:")) {
				if (historyItem["delay"] != null) {
					yield return new WaitForSeconds((int)historyItem["delay"]);
				}
			}

			//
			string action = (string)historyItem["action"];

			//
			string effectId = null;
			string effectParentId = null;
			if (historyItem["effectData"] != null && historyItem["effectData"].Type != JTokenType.Null) {
				effectId = (string)historyItem["effectData"]["effectId"];
				effectParentId = (string)historyItem["effectData"]["parentId"];
			}

			switch (action) {
				case "CardDrawnCrew":
					CardDrawnCrew((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardDrawnUnit":
					CardDrawnUnit((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardPilotReturned":
					CardPilotReturned((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardBurntUnit":
					CardBurnUnit((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardDestroyCrew":
					CardDestroyCrew((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardDestroyUnit":
					CardDestroyUnit((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardPlaced":
					CardPlaced((string)historyItem["userId"], (string)historyItem["cardId"], (string)historyItem["instanceId"]);
					break;
				case "CardDrawnAndThrownAway":
					CardDrawnAndThrownAway((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CardUnitReturned":
					CardUnitReturned((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "UnitCreated":
					UnitCreated((string)historyItem["userId"], (string)historyItem["instanceId"], (int)historyItem["boardPosition"]);
					break;
				case "CounterCreated":
					CounterCreated((string)historyItem["userId"], (string)historyItem["instanceId"]);
					break;
				case "CounterActivated":
					CounterActivated((string)historyItem["userId"], (string)historyItem["instanceId"], (string)historyItem["cardId"], (bool)historyItem["wasEffective"]);
					break;
				case "CounterCancelled":
					CounterCancelled ();
					break;
				case "TargetAttacked":
					TargetAttacked((string)historyItem["attackerId"], (string)historyItem["receiverId"]);
					break;
				case "UnitTapChange":
					UnitTapChange((string)historyItem["instanceId"], effectId, effectParentId);
					break;
				case "TargetsStatChange":
					TargetsStatChange((TomeArray)historyItem["targetIds"], effectId, effectParentId);
					break;
				case "TargetsHealed":
					TargetsHealed((TomeArray)historyItem["targetIds"], effectId, effectParentId);
					break;
				case "TargetsDamaged":
					TargetsDamaged((TomeArray)historyItem["targetIds"], effectId, effectParentId);
					break;
				case "TargetsDestroyed":
					TargetsDestroyed((TomeArray)historyItem["targetIds"], effectId, effectParentId);
					break;
				case "TurnChanged":
					TurnChanged((string)historyItem["newTurnPlayer"]);
					if (((string)historyItem["newTurnPlayer"]).Contains("cpu:")) {
						if (historyItem["delay"] != null)
						{
							yield return new WaitForSeconds((int)historyItem["delay"]);
						}

						CpuActionExecuted();
					}
					break;
				case "CpuTurnExecuted":
					CpuTurnExecuted();
					break;
				case "CpuActionExecuted":
					CpuActionExecuted();
					break;
				case "GameEnded":
					GameEnded((string)historyItem["winnerId"]);
					break;
				default:
					logger.data(historyItem).debug("Unknown History Action: " + action);
					break;
			}
			yield break;
		}

		//
		private void CardDrawnCrew(string userId, string instanceId) {
			// We cannot draw cards whilst inside initial draw phase
			if (gameView.gameInitialDraw.gameObject.activeSelf) {
				return;
			}

			// Grab newly instantiated cost object and add it to the bridge
			TomeObject newCost = game.tCurrentGame["drawnCards"][userId]["crewCards"][instanceId] as TomeObject;
			CostIcon costIcon = gameView.costManagers[userId].AddIcon(newCost);
			if (costIcon == null) {
				return;
			}

			// Perform animation
			gameView.motherships[userId].CostIconAdded(costIcon);
		}

		//
		private void CardDrawnUnit(string userId, string instanceId) {
			// We cannot draw cards whilst inside initial draw phase
			if (gameView.gameInitialDraw.gameObject.activeSelf) {
				return;
			}

			// Grab newly instantiated hand card and add it to the hand
			TomeObject newCard;
			if (userId == gameView.meUserId) {
				newCard = game.tCurrentGameSecrets["hand"]["unitHand"][instanceId] as TomeObject;
			} else {
				newCard = game.tCurrentGame["drawnCards"][userId]["unitCards"][instanceId] as TomeObject;
			}

			HandCard handCard = gameView.handManagers[userId].AddCard(newCard);
			if (handCard == null) {
				return;
			}

			// Perform animation
			gameView.deckManagers[userId].StartDraw(1, handCard.Appear);
		}

		//
		private void CardPilotReturned(string userId, string instanceId) {
			// Put card back on the bridge
			TomeObject newCost = game.tCurrentGame["drawnCards"][userId]["crewCards"][instanceId] as TomeObject;
			CostIcon costIcon = gameView.costManagers[userId].AddIcon(newCost);
			if (costIcon == null) {
				return;
			}

			// Perform return animation
			costIcon.Return();
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="instanceId"></param>
		private void CardUnitReturned(string userId, string instanceId) {
			if (!inAttackSequence) {
				UnitReturned(userId, instanceId);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				UnitReturned(userId, instanceId);
			};
			waitAttack.Start();
		}

		private void UnitReturned(string userId, string instanceId) {
			// Put card back in the hand
			TomeObject newCard = GameHelpers.FindCardData(instanceId);
			HandCard handCard = gameView.handManagers[userId].AddCard(newCard);
			if (handCard == null) {
				return;
			}

			// Perform animation
			gameView.boardManagers[userId].DestroyIcon(instanceId, null, () => {
				handCard.Appear();
			});
		}

		//
		private void CardBurnUnit(string userId, string instanceId) {
			gameView.handManagers[userId].DestroyCard(instanceId);
		}


		//
		private void CardDestroyCrew(string userId, string instanceId) {
			gameView.costManagers[userId].DestroyIcon(instanceId);
		}

		//
		private void CardDestroyUnit(string userId, string instanceId) {
			gameView.handManagers[userId].DestroyCard(instanceId);
		}

		//
		private void CardPlaced(string userId, string cardId, string instanceId) {
			gameView.handManagers[userId].DestroyCard(instanceId, false);

			// Show card tips window for placed event cards
			if (cardId != null && card.IsEventCard(cardId)) {
				gameView.cardTipsManager.Show(card.GetCard(cardId));
			}

			// Update all cost icons for user. This will make tapped cost icons appear so
			gameView.costManagers[userId].UpdateIcons();
		}

		//
		private void CardDrawnAndThrownAway(string userId, string instanceId) {
			// TODO: need to do the animation
			gameView.deckManagers[userId].StartDraw(1, () => {
			});
		}

		//
		private void UnitCreated(string userId, string instanceId, int boardPosition) {
			// TODO: check if this is a token instance, if so use tokenLight animation instead of default appear 

			// Grab newly instantiated unit and add it to the board
			TomeObject newUnit = GameHelpers.FindBoardIconData(instanceId);
			BoardIcon boardIcon = gameView.boardManagers[userId].AddIcon(newUnit, boardPosition);
			if (boardIcon == null) {
				return;
			}

			// Perform animation
			boardIcon.SetIconImg(true);
			boardIcon.QueueAction(boardIcon.Appear());
			boardIcon.UpdateFromData();
		}

		//
		private void CounterCreated(string userId, string instanceId) {
			TomeObject counter = GameHelpers.FindCounterData(instanceId);
			gameView.counterManagers[userId].AddCounter(counter);
		}

		//
		private void CounterActivated(string userId, string instanceId, string cardId, bool wasEffective) {
			// All animations should wait for this to complete before continuing
			inCounterSequence = true;

			// Trigger counter sequence
			Task triggerCounter = new Task(gameView.counterManagers[userId].TriggerCounter(cardId, wasEffective), false);

			triggerCounter.Finished += (manual) => {
				inCounterSequence = false;
			};

			triggerCounter.Start();
		}

		//
		private void CounterCancelled() {
			if (master.staticData["errorCodes"]["G0007"] != null) {
				gameView.ShowError(master.staticData["errorCodes"]["G0007"]["annotation"].ToString());
			} else {
				gameView.ShowError("Counter was cancelled by the unit's effect");
			}
		}

		//
		private void TargetAttacked(string attackerId, string receiverId) {
			// All animations should wait for this to complete before continuing
			inAttackSequence = true;

			Task waitCounter = new Task(WaitCounterFinished(), false);

			waitCounter.Finished += (manual) => {
				// Trigger attack sequence
				BoardIcon attacker = GameHelpers.FindBoardIcon(attackerId);
				attacker.QueueAction(attacker.Attack(receiverId), () => {
					inAttackSequence = false;
				});
			};

			waitCounter.Start();
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		private void UnitTapChange(string instanceId, string effectId, string effectParentId) {
			BoardIcon boardIcon = GameHelpers.FindBoardIcon((string)instanceId);
			bool iconTapped = GameHelpers.IconIsTapped(boardIcon.iconData);

			if (!inAttackSequence) {
				boardIcon.SetTap(iconTapped);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				boardIcon.SetTap(iconTapped);
			};
			waitAttack.Start();
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		private void TargetsStatChange(TomeArray targetIds, string effectId, string effectParentId) {
			if (!inAttackSequence) {
				StatChange(targetIds, effectId, effectParentId);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				StatChange(targetIds, effectId, effectParentId);
			};
			waitAttack.Start();
		}

		/// <summary>
		/// TODO: rewrite based inside targets
		/// </summary>
		private void StatChange(TomeArray targetIds, string effectId, string effectParentId) {
			string effectParentUserId = effectParentId;
			BoardIcon effectBoardIcon = GameHelpers.FindBoardIcon(effectParentId);
			if (effectBoardIcon != null) {
				effectParentUserId = (string)effectBoardIcon.iconData["userId"];
			}

			foreach (TomeValue targetId in targetIds) {
				BoardIcon boardIcon = GameHelpers.FindBoardIcon((string)targetId);
				if (boardIcon != null) {
					if ((string)boardIcon.iconData["userId"] == effectParentUserId) {
						boardIcon.QueueAction(boardIcon.PositiveStatChange());
					}
					else {
						boardIcon.QueueAction(boardIcon.NegativeStatChange());
					}
				}

				Mothership mothership = GameHelpers.FindMothership((string)targetId);
				if (mothership != null) {
					mothership.Attacked();
				}
			}
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		private void TargetsHealed(TomeArray targetIds, string effectId, string effectParentId) {
			if (!inAttackSequence) {
				Heal(targetIds, effectId, effectParentId);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				Heal(targetIds, effectId, effectParentId);
			};
			waitAttack.Start();
		}

		/// <summary>
		/// TODO: rewrite based on targets
		/// </summary>
		private void Heal(TomeArray targetIds, string effectId, string effectParentId) {
			foreach (TomeValue targetId in targetIds) {
				BoardIcon boardIcon = GameHelpers.FindBoardIcon((string)targetId);
				if (boardIcon != null) {
					boardIcon.QueueAction(boardIcon.PositiveStatChange());
				}

				Mothership mothership = GameHelpers.FindMothership((string)targetId);
				if (mothership != null) {
					mothership.Attacked();
				}
			}
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		private void TargetsDamaged(TomeArray targetIds, string effectId, string effectParentId) {
			// Attack event will handle attack stat changes
			if (effectId == null) {
				return;
			}

			if (!inAttackSequence) {
				Damage(targetIds, effectId, effectParentId);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				Damage(targetIds, effectId, effectParentId);
			};
			waitAttack.Start();
		}

		/// <summary>
		/// TODO: rewrite based on targets
		/// </summary>
		private void Damage(TomeArray targetIds, string effectId, string effectParentId) {
			// Perform animation on each target
			// TODO: if shelling shoot effect use shellingShoot animation
			// TODO: create mothership animation
			foreach (TomeValue targetId in targetIds) {
				BoardIcon boardIcon = GameHelpers.FindBoardIcon((string)targetId);
				if (boardIcon != null) {
					boardIcon.QueueAction(boardIcon.EffectDamage());
				}

				Mothership mothership = GameHelpers.FindMothership((string)targetId);
				if (mothership != null) {
					mothership.Attacked();
				}
			}
		}

		/// <summary>
		/// TODO: Move to queue system
		/// </summary>
		private void TargetsDestroyed(TomeArray targetIds, string effectId, string effectParentId) {
			if (!inAttackSequence) {
				DestroyTargets(targetIds, effectId, effectParentId);
				return;
			}

			Task waitAttack = new Task(WaitAttackFinished(), false);
			waitAttack.Finished += (manual) => {
				DestroyTargets(targetIds, effectId, effectParentId);
			};
			waitAttack.Start();
		}

		/// <summary>
		/// TODO: rewrite based on targets
		/// </summary>
		private void DestroyTargets(TomeArray targetIds, string effectId, string effectParentId) {
			// Perform animation on each target
			foreach (TomeValue instanceId in targetIds) {
				if (gameView.myBoardManager.boardIcons.ContainsKey((string)instanceId)) {
					gameView.myBoardManager.DestroyIcon((string)instanceId);
				}
				if (gameView.enemyBoardManager.boardIcons.ContainsKey((string)instanceId)) {
					gameView.enemyBoardManager.DestroyIcon((string)instanceId);
				}
			}
		}

		//
		private void TurnChanged(string newTurnPlayer) {
			// Change turn
			gameView.gameTurnManager.SetTurn();

			// Update all board icon stats
			foreach (BoardIcon boardIcon in gameView.myBoardManager.boardIcons.Values) {
				boardIcon.UpdateFromData();
			}
			foreach (BoardIcon boardIcon in gameView.enemyBoardManager.boardIcons.Values) {
				boardIcon.UpdateFromData();
			}

			// Update all cost icon states
			gameView.myCostManager.UpdateIcons();
			gameView.enemyCostManager.UpdateIcons();
		}

		private void CpuTurnExecuted() {
			gameView.CpuEndTurn();
		}

		private void CpuActionExecuted() {
			gameView.CpuActionExecuted();
		}

		/// <summary>
		/// TODO: Move to mothership code
		/// </summary>
		/// <param name="winnerId"></param>
		private void GameEnded(string winnerId) {
			Task deathAnim = (winnerId == gameView.meUserId) ?
				new Task(gameView.enemyMothership.Explode(), false) :
				new Task(gameView.myMothership.Explode(), false);

			deathAnim.Finished += (manual) => {
				gameView.EndGame(winnerId);
			};

			deathAnim.Start();
		}

		/// <summary>
		/// TODO: Remove this, will be part of the queue system
		/// </summary>
		private IEnumerator WaitAttackFinished() {
			// Wait for attack sequence if it is running
			while (inAttackSequence) {
				yield return null;
			}
		}

		/// <summary>
		/// TODO: Remove this, will be part of the queue system
		/// </summary>
		private IEnumerator WaitCounterFinished() {
			// Wait for counter sequence to end
			while (inCounterSequence) {
				yield return null;
			}
		}
	}
}