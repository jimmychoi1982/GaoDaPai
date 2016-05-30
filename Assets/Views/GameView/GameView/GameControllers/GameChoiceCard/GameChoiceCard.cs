using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace GameView {
	public class GameChoiceCard : MonoBehaviour {
		GameView gameView { get { return GameView.Instance; }}
		Game game { get { return Game.Instance; }}

		//
		public GameObject cardsParent;
		public GameObject cardPrefab;

		private int? choiceMin = null;
		private int? choiceMax = null;
		private Dictionary<string, Toggle> possibleChoices;
		private List<string> selectedChoices;
		private Button acceptButton;


		//
		private void CreateChoice(string instanceId) {
			// Create card instance
			GameObject newChoice = GameObject.Instantiate(cardPrefab);
			newChoice.transform.SetParent(cardsParent.transform);
			newChoice.transform.localScale = cardPrefab.transform.localScale;

			// Set card data
			TomeObject cardData = GameHelpers.FindCardData(instanceId);
			GameCard newCard = newChoice.GetComponent<GameCard>();
			newCard.SetData(cardData);

			// Setup choice handler
			Toggle toggleChoice = newChoice.GetComponent<Toggle>();
			toggleChoice.isOn = false;
			toggleChoice.onValueChanged.AddListener((bool newValue) => {
				// Add new choice to selected list
				if (toggleChoice.isOn) {
					selectedChoices.Add(instanceId);
				} else {
					selectedChoices.Remove(instanceId);
				}

				// If we have selected too many, unselect the earliest selection
				if (selectedChoices.Count > choiceMax) {
					string firstId = selectedChoices[0];
					possibleChoices[firstId].isOn = false;
					selectedChoices.Remove(firstId);
				}

				// Enable the button if enough selected
				acceptButton.interactable = selectedChoices.Count >= choiceMin;
			});

			possibleChoices.Add(instanceId, toggleChoice);
		}

		//
		public void Open(TomeArray possibleChoices, int choiceMin, int choiceMax) {
			this.choiceMin = (int?)choiceMin;
			this.choiceMax = (int?)choiceMax;
			this.possibleChoices = new Dictionary<string, Toggle>();
			this.selectedChoices = new List<string>();
			for (int i = 0; i < possibleChoices.Count; i += 1) {
				CreateChoice((string)possibleChoices[i]);
			}

			gameObject.SetActive(true);


			// Update text
			string text = "取り除くカードを" + choiceMin + "枚選択してください";
			transform.FindChild("notice_text").gameObject.GetComponent<Text>().text = text;

			// Accept button
			acceptButton = transform.FindChild("btn_confirm").gameObject.GetComponent<Button>();
			acceptButton.interactable = false;
		}

		//
		public void Close() {
			this.choiceMin = null;
			this.choiceMax = null;
			possibleChoices = null;
			for (int i = 0; i < cardsParent.transform.childCount; i += 1) {
				GameObject.Destroy(cardsParent.transform.GetChild(i).gameObject);
			}

			gameObject.SetActive(false);
		}

		//
		public void ConfirmChoices() {
			gameView.AnswerCardQuestion(selectedChoices);
		}
	}
}