using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameView {
	public class CardInfo : MonoBehaviour,
		IPointerClickHandler 
	{
		Card card { get { return Card.Instance; }}

		//
		[Header("Card")]
		public GameCard cardFront;
		public GameCard cardBack;
		public GameObject buttonChange;

		//
		[Header("Info")]
		public Text name;
		public Text characteristics;
		public Text effect1;
		public Text effect2;

		//
		public void Open(TomeObject cardData) {
			TomeObject frontCardData = cardData;
			TomeObject backCardData = (cardData["pilot"] != null) ? cardData["pilot"] as TomeObject : null;
			SetData(cardData, backCardData);
			gameObject.SetActive (true);
		}

		public void SetData(TomeObject frontCardData, TomeObject backCardData) {
			//
			cardFront.SetData(frontCardData);
			
			//
			if (backCardData != null) {
				cardBack.SetData(backCardData);
				cardBack.gameObject.SetActive(true);
				buttonChange.SetActive(true);
			} else {
				cardBack.gameObject.SetActive(false);
				buttonChange.SetActive(false);
			}
			
			//
			name.text = (string)frontCardData["name"];
			
			if (frontCardData["characteristicId"] != null) {
				List<string> characteristicsValues = new List<string>();
				TomeObject characteristicMap = card.staticData["characteristics"] as TomeObject;
				
				foreach (TomeValue value in frontCardData["characteristicId"] as TomeArray) {
					string characteristicId = (string)value;
					if (string.IsNullOrEmpty(characteristicId)) {
						continue;
					}
					
					TomeObject characteristic = characteristicMap[characteristicId] as TomeObject;
					string characteristicString = (characteristic != null) ? (string)characteristic["jpName"] : "";
					characteristicsValues.Add(characteristicString);
				}
				characteristics.text = string.Join("  ", characteristicsValues.ToArray());
			} else {
				characteristics.text = "";
			}
			
			TomeArray descriptions = frontCardData["description"] as TomeArray;
			effect1.text = (descriptions != null && descriptions.Count > 0) ? descriptions[0].ToString().Replace("<br />", "\n") : "";
			effect2.text = (descriptions != null && descriptions.Count > 1) ? descriptions[1].ToString().Replace("<br />", "\n") : "";
		}

		//
		public void Close() {
			gameObject.SetActive(false);
		}
		
		// Clost this overlay if we click on anything
		public void OnPointerClick(PointerEventData eventData) {
			Close();
		}

		//
		public void OnChangeClick() {
			SetData(cardBack.cardData, cardFront.cardData);
		}
	}
}