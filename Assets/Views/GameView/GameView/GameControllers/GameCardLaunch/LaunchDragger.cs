using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace GameView {
	public class LaunchDragger : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerClickHandler
	{
		Card card { get { return Card.Instance; }}
		GameView gameView { get { return GameView.Instance; }}
		private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

		//
		public CardLaunchManager cardLaunchManager;

		//
		[Header("Drag Drop Type")]
		public GameObject swipeParent;
		public GameObject swipIndicator;
		public BoardIconsManager boardIcons;
		public BoardIcon draggerIcon;
		public BoardIcon draggerIconLarge;

		//
		[Header("Tap Type")]
		public GameObject tapParent;
		public GameObject tapButton;

		//
		private TomeObject cardData = null;
		private BoardIcon currentDraggerIcon = null;
		private GameObject boardIconPlaceHolder = null;


		//
		public void SetData(TomeObject cardData, TomeArray boardIcons) {
			this.cardData = cardData;

			string cardId = (string)cardData["cardId"];
			if (card.IsUnitCard(cardId)) {
				// Swipe launch type
				swipeParent.SetActive(true);
				tapParent.SetActive(false);

				// Setup dragger icon
				string size = (string)cardData["size"];
				if (size == "L") {
					draggerIcon.gameObject.SetActive(false);

					draggerIconLarge.gameObject.SetActive(true);
					draggerIconLarge.SetData(cardData, false);
					currentDraggerIcon = draggerIconLarge;
				} else {
					draggerIconLarge.gameObject.SetActive(false);

					draggerIcon.gameObject.SetActive(true);
					draggerIcon.SetData(cardData, false);
					currentDraggerIcon = draggerIcon;
				}

				// Setup board icons
				this.boardIcons.SetData(boardIcons);
			} else {
				// Tap launch type
				currentDraggerIcon = null;
				swipeParent.SetActive(false);
				tapParent.SetActive(true);

			}
		}

		// Show launch dragger and correct dragger icon
		public void Open() {
			gameObject.SetActive(true);
			boardIcons.gameObject.SetActive(false);
			if (currentDraggerIcon != null) {
				currentDraggerIcon.QueueAction(currentDraggerIcon.Appear());
			}

			// チュートリアル処理
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				
				if (PlayerPrefs.GetString ("Tutorial") == "Step1"){
					var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep1> ();
					var tutorialActionData = tutorialView.GetTutorialActionData ();
					
					if (tutorialActionData.GetCurrentIndex () == 3){
						tutorialView.GetTutorialActionData ().LauchNextAction ();
					}
					if (tutorialActionData.GetCurrentIndex () == 7){
						tutorialView.ArrowActiveWithMiddleground (2, false, 2);
					}
					if (tutorialActionData.GetCurrentIndex () == 10 ||
					    tutorialActionData.GetCurrentIndex () == 14){
						tutorialView.ArrowActiveWithMiddleground (2, false, 3);
					}
				}else if (PlayerPrefs.GetString ("Tutorial") == "Step3"){
					var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep3> ();
					var tutorialActionData = tutorialView.GetTutorialActionData ();
					
					if (tutorialActionData.GetCurrentIndex () == 2){
						tutorialView.GetTutorialActionData ().LauchNextAction ();
					}
				}else if (PlayerPrefs.GetString ("Tutorial") == "Step4"){
					var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep4> ();
					var tutorialActionData = tutorialView.GetTutorialActionData ();
					
					if (tutorialActionData.GetCurrentIndex () == 1 || 
					    tutorialActionData.GetCurrentIndex () == 7){
						tutorialView.ArrowActiveWithMiddleground (12, true);
					}
				}
			}
		}

		// Hide dragger icons along with launch dragger parent
		public void Close() {
			draggerIcon.iconGroup.alpha = 0;
			draggerIconLarge.iconGroup.alpha = 0;
			gameObject.SetActive(false);

			// チュートリアル処理
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				if (PlayerPrefs.GetString ("Tutorial") == "Step4"){
					var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep4> ();
					var tutorialActionData = tutorialView.GetTutorialActionData ();
					
					if (tutorialActionData.GetCurrentIndex () == 1|| 
					    tutorialActionData.GetCurrentIndex () == 7){
						tutorialView.ArrowActiveWithMiddleground (12, false);
					}
				}
			}
		}

		// Handle begin drag events
		public void OnBeginDrag(PointerEventData eventData) {
			// Do nothing if this is not a unit card, we are expecting a tap instead
			if (!card.IsUnitCard((string)cardData["cardId"])) {
				return;
			}

			// Let the parent manager know we have begun dragging
			cardLaunchManager.BeginDrag();

			// Show board icons and hide indicator
			boardIcons.gameObject.SetActive(true);
			gameView.myBoardManager.gameObject.SetActive(false);
			swipIndicator.SetActive(false);

			// Move icon along with pointer
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
			currentDraggerIcon.GetComponent<RectTransform>().position = worldPos;

			// Allow raycasts to pass through whilst we're dragging the item.
			// Doing so allows our drop destination to receive a drop event.
			currentDraggerIcon.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
		

		// Handle drag events
		public void OnDrag(PointerEventData eventData) {
			// Do nothing if this is not a unit card, we are expecting a tap instead
			if (!card.IsUnitCard((string)cardData["cardId"])) {
				return;
			}

			// Move icon along with pointer
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
			currentDraggerIcon.GetComponent<RectTransform>().position = worldPos;
			
			// Check if we are hovering over the boardIcons
			bool hoveringBoard = false;
			foreach (GameObject item in eventData.hovered) {
				if (item.transform == boardIcons.transform) {
					hoveringBoard = true;
					break;
				}
			}
			
			if (hoveringBoard) {
				int boardPos = 0;
				
				// If there are children find the closest and detect on which side we are.
				// We then use that to determine the board position.
				if (boardIcons.transform.childCount > 0) {
					Transform closestChild = boardIcons.transform.GetChild(0);
					float closestDistanceSqr = ((Vector2)closestChild.position - worldPos).sqrMagnitude;
					
					for (int i = 1; i < boardIcons.transform.childCount; i += 1) {
						Transform child = boardIcons.transform.GetChild(i);
						float distanceSqr = ((Vector2)child.position - worldPos).sqrMagnitude;
						
						if (distanceSqr < closestDistanceSqr) {
							closestChild = child;
							closestDistanceSqr = distanceSqr;
						}
					}
					
					if (boardIconPlaceHolder != null && closestChild == boardIconPlaceHolder.transform) {
						boardPos = closestChild.GetSiblingIndex();
					} else if (worldPos.x > closestChild.position.x) {
						boardPos = closestChild.GetSiblingIndex() + 1;
					} else {
						boardPos = closestChild.GetSiblingIndex();
					}
				}

				// Create a placeholder on the board at the given board position
				if (boardIconPlaceHolder == null) {
					boardIconPlaceHolder = boardIcons.CreatePlaceholder(boardPos, 204, 240);
				} else if (boardIconPlaceHolder != null && boardIconPlaceHolder.transform.GetSiblingIndex() != boardPos) {
					GameObject.Destroy(boardIconPlaceHolder);
					boardIconPlaceHolder = boardIcons.CreatePlaceholder(boardPos, 204, 240);
				}
			} else if (boardIconPlaceHolder != null) {
				GameObject.Destroy(boardIconPlaceHolder);
			}
		}


		// Handle end drag events
		public void OnEndDrag(PointerEventData eventData) {
			// Do nothing if this is not a unit card, we are expecting a tap instead
			if (!card.IsUnitCard((string)cardData["cardId"])) {
				return;
			}

			// Let the parent manager know we have stopped dragging
			cardLaunchManager.EndDrag();

			// Hide the board icons and show indicator
			boardIcons.gameObject.SetActive(false);
			gameView.myBoardManager.gameObject.SetActive(true);
			swipIndicator.SetActive(true);

			// Make sure we have a placeholder, if there isn't one we didnt drop on the board icons
			if (boardIconPlaceHolder != null) {
				int boardPos = boardIconPlaceHolder.transform.GetSiblingIndex();

				// Call EndDrag with board position if it exists
				cardLaunchManager.LaunchCard(boardPos);
			}
			
			// Reset current dragger icon
			currentDraggerIcon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
			currentDraggerIcon.GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
		
		// Do nothing on click
		public void OnPointerClick(PointerEventData eventData) {
			// Do nothing if this is a unit card, we are expecting a drag instead
			if (card.IsUnitCard((string)cardData["cardId"])) {
				return;
			}

			// Otherwise is event / counter card
			cardLaunchManager.LaunchCard(null);

		}
	}
}