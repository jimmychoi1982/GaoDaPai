using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeckData : MonoBehaviour {
	private UserDeck userDeck;

	public int DeckId { get { return this.userDeck.deckId; } }

	private Color colorBlue = new Color ((float)34 / (float)255, (float)95 / (float)255, (float)152 / (float)255);
	private Color colorGreen = new Color ((float)3 / (float)255, (float)125 / (float)255, (float)50 / (float)255);
	private Color colorYellow = new Color ((float)228 / (float)255, (float)152 / (float)255, (float)14 / (float)255);
	private Color colorBlack = new Color ((float)113 / (float)255, (float)113 / (float)255, (float)113 / (float)255);
	private Color colorRed = new Color ((float)209 / (float)255, (float)0 / (float)255, (float)12 / (float)255);

	LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

	public void Init (UserDeck userDeck, string type, int currentDeckId) {
		this.userDeck = userDeck;
		
		loadAssetBundle.SetCardImage(userDeck.favoriteUnit, (int)LoadAssetBundle.DisplayType.Deck, transform.Find ("DeckImage").gameObject);
		transform.Find("DeckName").GetComponent<Text>().text = userDeck.deckName;
		SetMainDeck (userDeck.deckId == currentDeckId);

		SetEnabled ();

		Transform colorPlate = null;
		for (var i=1; i<=5; i++) {
			var tmpColorPlate = transform.Find ("ColorPlate/" + i + "Colors");
			if (i == userDeck.color.Count) {
				colorPlate = tmpColorPlate;
				tmpColorPlate.gameObject.SetActive (true);
			} else {
				tmpColorPlate.gameObject.SetActive (false);
			}
		}
		int index = 1;
		foreach (var color in userDeck.color) {
			Color tmpColor = Color.white;
			switch (color) {
			case "blue":
				tmpColor = colorBlue;
				break;

			case "green":
				tmpColor = colorGreen;
				break;
				
			case "yellow":
				tmpColor = colorYellow;
				break;
				
			case "black":
				tmpColor = colorBlack;
				break;

			case "red":
				tmpColor = colorRed;
				break;
				
			default:
				break;
			}
			colorPlate.Find ("Plate" + index).GetComponent<Image>().color = tmpColor;
			index++;
		}

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			if (type == "deck") {
				GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().SelectDeck (transform.gameObject);
			} else if (type == "multiplay") {
				GameObject.Find ("/Main Canvas").GetComponent<MultiplayView>().SelectDeck (transform.gameObject);
			} else if (type == "practice") {
				GameObject.Find ("/Main Canvas").GetComponent<PracticeView>().SelectDeck (transform.gameObject);
			}
		});
	}

	public void SetMainDeck (bool isMain) {
		transform.Find("MainDeckSign").gameObject.SetActive(isMain);
	}
	
	public void SetEnabled () {
		transform.Find("DisabledMask").gameObject.SetActive(false);
	}
	
	public void SetDisabled () {
		transform.Find("DisabledMask").gameObject.SetActive(true);
	}
}
