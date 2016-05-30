using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class HandCardsManager : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		GameView gameView { get { return GameView.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		//
		[Header("Hand Card")]
		public GameObject cardsParent;
		public GameObject cardPrefab;

		//
		[Header("Layout")]
		public HorizontalLayoutGroup layoutGroup;
		public RectTransform rectTransform;
		public float childWidth = 150;
		public float rotationIncrement = 3;
		public float liftIncrement = 6;

		//
		[Header("Animations")]
		public Animator animator;
		
		
		// Map of card objects by their instanceId
		private Dictionary<string, HandCard> handCards = new Dictionary<string, HandCard>();
		private Dictionary<string, GameCard> gameCards = new Dictionary<string, GameCard>();


		//
		void FixedUpdate() {
			// Update spacing based on number of cards
			float childCount = cardsParent.transform.childCount;
			float totalWidth = childCount * childWidth;
			float allowedWidth = rectTransform.rect.width - (childWidth / 2);
			if (totalWidth > allowedWidth) {
				layoutGroup.spacing = -1 * (totalWidth - allowedWidth) / childCount;
			}

			// Update rotation and position based on card child index
			float rotCurrent = rotationIncrement * ((childCount - 1) / 2);

			float liftPeak = liftIncrement * ((childCount - 1) / 2);
			float liftCounter = liftPeak;

			for (int i = 0; i < childCount; i += 1) {
				GameObject cardChild = cardsParent.transform.GetChild(i).gameObject;
				HandCard handCard = cardChild.GetComponent<HandCard>();
				RectTransform imgRectTransform = handCard.imgCard.GetComponent<RectTransform>();

				// Ignore cards which are being moved around
				if (handCard.GetComponent<LayoutElement>().ignoreLayout) {
					continue;
				}

				imgRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, rotCurrent));
				rotCurrent -= rotationIncrement;

				float liftCurrent = liftPeak - Mathf.Abs(liftCounter);
				imgRectTransform.anchoredPosition = new Vector2(0, liftCurrent);
				liftCounter -= liftIncrement;
			}
		}

		//
		public void SetData(TomeObject handData, bool animate = true) {
			// Reset row icons
			handCards = new Dictionary<string, HandCard>();
			gameCards = new Dictionary<string, GameCard>();
			for (int i = 0; i < cardsParent.transform.childCount; i += 1) {
				GameObject.Destroy(cardsParent.transform.GetChild(i).gameObject);
			}
			
			// Check if we should populate icons using given costIcons data
			if (handData == null) {
				return;
			}
			
			foreach (var property in handData) {
				TomeObject handCard = property.Value as TomeObject;
				AddCard(handCard, true);
			}
		}


		//
		public HandCard AddCard(TomeObject cardData, bool autoAnimate = false) {
			string instanceId = (string)cardData["instanceId"];
			if (handCards.ContainsKey(instanceId)) {
				return null;
			}

			//
			GameObject newCard = GameObject.Instantiate(cardPrefab);
			newCard.transform.SetParent(cardsParent.transform);
			newCard.transform.SetAsLastSibling();
			newCard.transform.localScale = cardPrefab.transform.localScale;
			
			GameCard newGameCard = newCard.GetComponent<GameCard>();
			newGameCard.SetData(cardData);

			HandCard newHandCard = newCard.GetComponent<HandCard>();
			
			gameCards.Add(instanceId, newGameCard);
			handCards.Add(instanceId, newHandCard);

			// Check if this is the enemy hand, if not we can attach event handlers
			bool isEnemyHand = this == gameView.enemyHandManager;
			if (!isEnemyHand) {
				newHandCard.onTap += () => {

					if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){

						if (PlayerPrefs.GetString ("Tutorial") == "Step1"){

							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep1> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();

							if (tutorialActionData.GetCurrentIndex () <= 4){
								// Swipe失敗のとき、Action Index 0に戻る
								if (tutorialActionData.GetCurrentIndex () != 1){
									tutorialActionData.SetCurrentIndex (0);
								}
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}

							if (tutorialActionData.GetCurrentIndex () == 7 ||
							    tutorialActionData.GetCurrentIndex () == 10 ||
							    tutorialActionData.GetCurrentIndex () == 14){

								return;
							}

						}else if (PlayerPrefs.GetString ("Tutorial") == "Step2"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep2> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () == 2 || 
							    tutorialActionData.GetCurrentIndex () == 5){
//								tutorialView.ArrowActiveWithMiddleground (8, animator.GetBool("IsZoomed"));
								return;
							}
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step3"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep3> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();

							if (tutorialActionData.GetCurrentIndex () == 1){
								return;
							}
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step4"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep4> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () == 0 ||
							    tutorialActionData.GetCurrentIndex () == 6){
								return;
							}
							
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step5"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep5> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () == 0){
								return;
							}

							if (tutorialActionData.GetCurrentIndex () == 4){
								return;
							}
						}
					}

					animator.SetBool("IsZoomed", !animator.GetBool("IsZoomed"));
				};
				newHandCard.onLaunch += () => {
					TomeObject costIcons = game.tCurrentGame["drawnCards"][gameView.meUserId]["crewCards"] as TomeObject;
					TomeArray boardIcons = game.tCurrentGame["board"][gameView.meUserId] as TomeArray;

//					gameView.cardLaunchManager.Open(cardData, costIcons, boardIcons);

					if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){

						if (PlayerPrefs.GetString ("Tutorial") == "Step1"){
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep1> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							var tutorialDialogue = tutorialView.GetDialogueData ();

							if (tutorialActionData.GetCurrentIndex () == 0){
								return;
							}

							if (tutorialActionData.GetCurrentIndex () <= 4){
								// Swipe失敗のとき、Action Index 0に戻る
								if (tutorialActionData.GetCurrentIndex () != 3){
									tutorialActionData.SetCurrentIndex (2);
									tutorialDialogue.SetCurrentIndex (3);
								}
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}

							if (tutorialActionData.GetCurrentIndex () == 7){
								tutorialView.ArrowActiveWithMiddleground (8, false);
								tutorialView.ArrowActiveWithMiddleground (2, true, 2);
							}

							if (tutorialActionData.GetCurrentIndex () == 10){
								tutorialView.ArrowActiveWithMiddleground (8, false);
								tutorialView.ArrowActiveWithMiddleground (2, true, 3);
							}

							if (tutorialActionData.GetCurrentIndex () == 14){
								tutorialView.ArrowActiveWithMiddleground (8, false);
								tutorialView.ArrowActiveWithMiddleground (2, true, 3);
							}

						}else if (PlayerPrefs.GetString ("Tutorial") == "Step2"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep2> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () == 2 || 
							    tutorialActionData.GetCurrentIndex () == 5){
								tutorialView.ArrowActiveWithMiddleground (8, false);
							}
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step3"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep3> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () <= 2){
								// Swipe失敗のとき
								if (tutorialActionData.GetCurrentIndex () != 1){
									tutorialActionData.SetCurrentIndex (1);
								}
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step4"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep4> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();
							
							if (tutorialActionData.GetCurrentIndex () == 0 ||
							    tutorialActionData.GetCurrentIndex () == 6){
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}
							
						}else if (PlayerPrefs.GetString ("Tutorial") == "Step5"){
							//
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep5> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();

							if (tutorialActionData.GetCurrentIndex () == 0){
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}

							if (tutorialActionData.GetCurrentIndex () == 4){
								tutorialView.GetTutorialActionData ().LauchNextAction ();
							}
							
						}
					}

					gameView.cardLaunchManager.Open(cardData, costIcons, boardIcons);
				};
			}
			
			// Check if we should automatically animate
			if (autoAnimate) {
				newHandCard.Appear();
			}
			
			return newHandCard;
		}
		
		// Destroy an icon. Will also move all icons down to fill in space
		public void DestroyCard(string instanceId, bool animate = true, Action cb = null) {
			cb = (cb != null) ? cb : () => {};
			if (!handCards.ContainsKey(instanceId)) {
				cb();
				return;
			}

			HandCard handCard = handCards[instanceId];
			if (!animate) {
				GameObject.Destroy(handCard.gameObject);
				handCards.Remove(instanceId);
				gameCards.Remove(instanceId);
				cb();
				return;
			}

			Task handCardDestroyAnim = new Task(handCard.Destroy(), false);
			handCardDestroyAnim.Finished += manual => {
				GameObject.Destroy(handCard.gameObject);
				handCards.Remove(instanceId);
				gameCards.Remove(instanceId);
				cb();
			};
			handCardDestroyAnim.Start();
		}
	}
}