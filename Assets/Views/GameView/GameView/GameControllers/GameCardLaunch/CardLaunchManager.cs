using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace GameView {
	public class CardLaunchManager : MonoBehaviour,
		IPointerClickHandler
	{
		Card card { get { return Card.Instance; }}
		GameView gameView { get { return GameView.Instance; }}

		//
		[Header("Cards")]
		public GameObject cardsParent;
		public GameCard gameCard;
		public GameCard pilotCard;
		private CostIcon pilotSelectedIcon;

		//
		[Header("Cost Icons")]
		public CostIconsManager costIconManager;
		public CostCountManager costCountManager;

		//
		[Header("Launch Dragger")]
		public LaunchDragger launchDragger;

		//
		public void Open(TomeObject cardData, TomeObject costIcons, TomeArray boardIcons) {
			// Show this overlay hiding certain things underneath
			gameView.myCostManager.gameObject.SetActive(false);
			gameView.myHandManager.gameObject.SetActive(false);
			gameObject.SetActive(true);

			// Show cards
			cardsParent.SetActive(true);

			// Setup Unit Card
			gameCard.gameObject.SetActive(true);
			gameCard.SetData(cardData);

			// Setup Pilot Card
			pilotSelectedIcon = null;
			pilotCard.gameObject.SetActive(false);

			// Setup Cost Icons
			costIconManager.gameObject.SetActive(true);
			costIconManager.SetData(costIcons);
			foreach (CostIcon costIcon in costIconManager.costIcons.Values) {
				Toggle costToggle = costIcon.GetComponent<Toggle>();
				PilotDropper pilotDropper = costIcon.GetComponent<PilotDropper>();

				// Check if the cost icon can be selected as cost
				costToggle.interactable = !GameHelpers.IconIsTapped(costIcon.iconData);
				if (!GameHelpers.IconIsTapped(costIcon.iconData)) {
					costToggle.onValueChanged.AddListener(CheckCost);
				}

				// Check if the cost icon can be used as a pilot
				string cardId = (string)costIcon.iconData["cardId"];
				if (!card.IsPilotCard(cardId) || GameHelpers.PilotExistsOnMyBoard(cardId)) {
					continue;
				}

				if (GameHelpers.CardCanUsePilot(cardData, costIcon.iconData)) {
					pilotDropper.dropZone = gameCard.gameObject;
					pilotDropper.onDrop += SetPilot;
				}
			}
			costCountManager.gameObject.SetActive(true);
			costCountManager.SetData(cardData);

			// Setup Launch Dragger
			launchDragger.Close();
			launchDragger.SetData(cardData, boardIcons);

			// Check costs in case of zero cost cards
			CheckCost(false);
		}

		//
		public void Close () {
			gameView.myCostManager.gameObject.SetActive(true);
			gameView.myHandManager.gameObject.SetActive(true);
			gameObject.SetActive(false);
		}

		// Check if enough crew was selected to launch the card
		public void CheckCost(bool value) {
			// Count selected cost
			Dictionary<string, int> selectedCost = new Dictionary<string, int>();
			foreach (CostIcon costIcon in costIconManager.costIcons.Values) {
				Toggle toggleCostIcon = costIcon.GetComponent<Toggle>();

				if (costIcon.iconData != null && toggleCostIcon.isOn) {
					string color = (costIcon.iconData["color"] != null) ? (string)costIcon.iconData["color"] : "neutral";
					if (!selectedCost.ContainsKey(color)) {
						selectedCost.Add(color, 0);
					}
					selectedCost[color]++;
				}
			}

			// Update cost count pearls
			costCountManager.UpdateSelected(selectedCost);

			// Check if there is enough colour cost for launch
			int remainingCost = (selectedCost.ContainsKey("neutral")) ? selectedCost["neutral"] : 0;
			TomeObject cardCost = gameCard.cardData["currentCost"] as TomeObject;
			if (cardCost["play"] != null) {
				cardCost = cardCost["play"] as TomeObject;
			}

			foreach (var costProperty in cardCost) {
				string costColor = costProperty.Key;
				if (costColor == "neutral") {
					continue;
				}

				int costRequired = (int)costProperty.Value;
				int costHave = (selectedCost.ContainsKey(costColor)) ? selectedCost[costColor] : 0;

				if (costRequired > costHave) {
					launchDragger.Close();
					costCountManager.gameObject.SetActive(true);
					return;
				}

				remainingCost += costHave - costRequired;
			}

			// Now make sure there is just enough neutral color for launch
			if (remainingCost != (int)cardCost["neutral"]) {
				launchDragger.Close();
				costCountManager.gameObject.SetActive(true);
				return;
			}

			// Otherwise we can show the launch dragger
			launchDragger.Open();
			costCountManager.gameObject.SetActive(false);
		}


		// Grab all instanceIds for selected cost cards
		private List<string> GetSelectedCost() {
			List<string> costCards = new List<string>();
			foreach (CostIcon costIcon in costIconManager.costIcons.Values) {
				Toggle toggleCostIcon = costIcon.GetComponent<Toggle>();

				if (costIcon.iconData != null && toggleCostIcon.isOn) {
					costCards.Add((string)costIcon.iconData["instanceId"]);
				}
			}

			return costCards;
		}

		public void SetPilot (CostIcon pilotIcon) {
			// Re-enable previous pilot as cost
			if (pilotSelectedIcon != null) {
				Toggle togglePrevious = pilotSelectedIcon.GetComponent<Toggle>();
				togglePrevious.isOn = false;
				togglePrevious.interactable = true;
			}

			// Disable icon as cost as long as it doesn't have a
			// special effect allowing to be used as both
			if (!GameHelpers.CardCanUsePilot(gameCard.cardData, pilotIcon.iconData)) {
				Toggle toggleCurrent = pilotIcon.GetComponent<Toggle>();
				toggleCurrent.isOn = false;
				toggleCurrent.interactable = false;
			}

			// Set card data
			pilotSelectedIcon = pilotIcon;
			pilotCard.SetData(pilotIcon.iconData);
			pilotCard.gameObject.SetActive(true);
		}

		// Launch dragger has begun to be dragged
		public void BeginDrag () {
			cardsParent.SetActive(false);
			costIconManager.gameObject.SetActive(false);
			costCountManager.gameObject.SetActive(false);
		}

		// Launch dragger has stopped dragging
		public void EndDrag () {
			cardsParent.SetActive(true);
			costIconManager.gameObject.SetActive(true);
			costCountManager.gameObject.SetActive(true);
		}

		// Launch dragger has launched a card
		public void LaunchCard (int? boardPos) {
			//
			Close();

			//
			string pilotCardId = null;
			if (pilotSelectedIcon != null) {
				pilotCardId = (string)pilotCard.cardData["instanceId"];
			}

			string currentCardId = (string)gameCard.cardData["cardId"];
			gameView.PlaceCard(gameCard, boardPos, GetSelectedCost(), pilotCardId);
		}

		// Clost this overlay if we click on anything that doesnt have event support
		public void OnPointerClick(PointerEventData eventData) {

			// チュートリアルの時、カードを戻らないように
			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
				return;
			}
			Close();
		}
	}
}
