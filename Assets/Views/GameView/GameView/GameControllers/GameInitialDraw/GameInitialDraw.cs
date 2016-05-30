using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class GameInitialDraw : MonoBehaviour {
		//
		Game game { get { return Game.Instance; }}
		GameView gameView { get { return GameView.Instance; }}

		//
		public GameObject startCardsParent;
		public GameObject startCardsPrefab;
		public Button startCardsConfirm;
		public Image startCardsAlart;

		//
		private Dictionary<string, Toggle> startCards;

		// Start Timeout
		private int START_TIME_LIMIT_MS = 30 * 1000;
		private Timer startTimeLimitTimer;


		//
		public void Open() {
			ResetCards();
			startCardsConfirm.gameObject.SetActive(false);
			gameObject.SetActive(true);
			DrawCards();

			// Opponent start timeout
			SetupStartTimeLimit();
		}

		//
		public void ResetCards() {
			startCards = new Dictionary<string, Toggle>();
			for (int i = 0; i < startCardsParent.transform.childCount; i += 1) {
				GameObject.Destroy(startCardsParent.transform.GetChild(i).gameObject);
			}
		}

		//
		public void DrawCards() {

			int cardCount = (game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject).Count;

			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore) {
				StartGame ();
				return;
			}

			gameView.myDeckManager.StartDraw(cardCount, ShowCards);
		}

		//
		public void ShowCards() {
			//
			TomeObject unitHand = game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject;
			int cardCount = unitHand.Count;

			RectTransform parentRectTransform = startCardsParent.GetComponent<RectTransform>();
			HorizontalLayoutGroup parentLayoutGroup = startCardsParent.GetComponent<HorizontalLayoutGroup>();
			float parentWidth = parentRectTransform.rect.width;
			float parentSpacing = parentLayoutGroup.spacing;

			RectTransform prefabRectTransform = startCardsPrefab.GetComponent<RectTransform>();
			float itemWidth = prefabRectTransform.rect.width;
			float itemHeight = prefabRectTransform.rect.height;
			float totalItemsWidth = (cardCount * itemWidth) + (parentSpacing * (cardCount - 1));
			float xOffset = ((parentWidth - totalItemsWidth) / 2) + (itemWidth / 2);

			//
			int cardI = 0;
			float xPos = xOffset;
			float moveDelay = 0f;

			//
			foreach (var property in unitHand as TomeObject) {
				GameObject newCard = GameObject.Instantiate(startCardsPrefab);
				newCard.transform.SetParent(startCardsParent.transform);
				newCard.transform.localScale = startCardsPrefab.transform.localScale;

				TomeObject cardData = (TomeObject)property.Value;
				GameCard newGameCard = newCard.GetComponent<GameCard>();
				newGameCard.SetData(cardData);

				LayoutElement newCardLayout = newCard.GetComponent<LayoutElement>();
				newCardLayout.ignoreLayout = true;

				RectTransform newCardRectTransform = newCard.GetComponent<RectTransform>();
				newCardRectTransform.sizeDelta = prefabRectTransform.sizeDelta;
				newCardRectTransform.anchoredPosition = new Vector2(xPos, 1000f);

				Sequence mySequence = DOTween.Sequence();
				mySequence.AppendInterval(moveDelay);
				mySequence.Append(newCardRectTransform.DOAnchorPos(new Vector2(xPos, itemHeight / 2), 0.5f));
				if (cardI == cardCount - 1) {
					mySequence.AppendCallback(ShowConfirmButton);
				}

				Toggle newCardToggle = newCard.GetComponent<Toggle>();
				startCards.Add((string)cardData["instanceId"], newCardToggle);

				//
				cardI += 1;
				xPos += itemWidth + parentSpacing;
				moveDelay += 0.3f;
			}
		}

		//
		public void ShowConfirmButton() {
			startCardsConfirm.interactable = true;
			startCardsConfirm.gameObject.SetActive(true);
		}

		//
		public void StartGame() {
			List<string> burnCards = new List<string>();
			foreach (var property in startCards) {
				Toggle startCard = property.Value;
				string instanceId = property.Key;
				startCard.interactable = false;

				GameCard startGameCard = startCard.GetComponent<GameCard>();
				if (startCard.isOn) {
					burnCards.Add(instanceId);
				}
			}

			startCardsConfirm.interactable = false;

			gameView.StartGame(burnCards);
		}

		//
		public void RenableButtons() {
			startCardsConfirm.interactable = true;
			foreach (Toggle startCard in startCards.Values) {
				startCard.interactable = true;
			}
		}

		//
		public void SwapCards(List<string> burnCards) {
			TomeObject unitHand = game.tCurrentGameSecrets["hand"]["unitHand"] as TomeObject;

			float delay = 0f;
			foreach (string instanceId in burnCards) {
				Toggle startCard = startCards[instanceId];
				string replaceId = null;
				if (unitHand[instanceId] != null) {
					// We we're (un)lucky enough to redraw the same card. It will replace it self
					replaceId = instanceId;
				} else {
					// Find replacement
					foreach (var newCard in unitHand) {
						if (startCards.ContainsKey(newCard.Key)) {
							continue;
						}
						
						replaceId = newCard.Key;
						startCards.Add(replaceId, startCard);
						break;
					}
				}

				// Animate swap
				startCard.isOn = false;

				RectTransform cardRectTransform = startCard.GetComponent<RectTransform>();
				Vector2 originalPos = cardRectTransform.anchoredPosition;
				Vector2 outscreenPos = new Vector2(cardRectTransform.anchoredPosition.x, 1000f);

				Sequence sequence = DOTween.Sequence();
				sequence.AppendInterval(delay);
				sequence.Append(cardRectTransform.DOAnchorPos(outscreenPos, 0.5f));
				sequence.AppendCallback(() => {
					gameView.myDeckManager.StartDraw(1, () => {
						TomeObject cardData = (TomeObject)unitHand[replaceId];
						GameCard gameCard = startCard.GetComponent<GameCard>();
						gameCard.SetData(cardData);

						cardRectTransform.DOAnchorPos(originalPos, 0.5f);
					});
				});

				delay += 0.3f;
			}
		}

		//
		public void StartValuesChanged() {
			bool meStarted = (bool)game.tCurrentGame["playerStarted"][gameView.meUserId];
			bool enemyStarted = (bool)game.tCurrentGame["playerStarted"][gameView.enemyUserId];
			if (gameObject.activeSelf && meStarted && enemyStarted) {
				Close();
				gameView.SetupGameBoard();
			}
		}

		//
		public void Close() {
			gameObject.SetActive(false);
			CleanupStartTimeLimit();
		}

		
		// === START THEFT ===========================
		public void CleanupStartTimeLimit() {
			if (startTimeLimitTimer != null) {
				startTimeLimitTimer.Dispose();
				startTimeLimitTimer = null;
			}
		}
		
		void SetupStartTimeLimit() {
			CleanupStartTimeLimit();
			startTimeLimitTimer = new Timer(StealStart, null, START_TIME_LIMIT_MS, Timeout.Infinite);
		}
		
		void StealStart(object state) {
			TaskManagerMainThread.Queue(game.TakeStart((Exception error) => {
				CleanupStartTimeLimit();
			}));
		}

		// ALART GROW ================================
		//card change timer 5 seconds ago
		void AlartGrowAnimation(){
			startCardsAlart.DOFade (0.8f, 0.35f).SetLoops (14, LoopType.Yoyo);
		}

	}
}