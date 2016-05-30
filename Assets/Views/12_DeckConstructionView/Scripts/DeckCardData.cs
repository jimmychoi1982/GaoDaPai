using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeckCardData : MonoBehaviour {
	private string cardId;
	private CardMaster cardMaster;
	
	public string CardId {
		get { return this.cardId; }
		set { this.cardId = value; }
	}
	
	private Color colorBlue = new Color ((float)34 / (float)255, (float)95 / (float)255, (float)152 / (float)255);
	private Color colorGreen = new Color ((float)3 / (float)255, (float)125 / (float)255, (float)50 / (float)255);
	private Color colorYellow = new Color ((float)228 / (float)255, (float)152 / (float)255, (float)14 / (float)255);
	private Color colorBlack = new Color ((float)113 / (float)255, (float)113 / (float)255, (float)113 / (float)255);
	private Color colorRed = new Color ((float)209 / (float)255, (float)0 / (float)255, (float)12 / (float)255);

	LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

	public void Init(CardMaster cardMaster) {
		this.cardMaster = cardMaster;
		this.cardId = cardMaster != null ? cardMaster.cardId : null;
		
		if (cardMaster != null) {
			transform.Find ("CardImage").gameObject.SetActive (true);
			transform.Find ("CardName").gameObject.SetActive (true);
			transform.Find ("ColorPlate").gameObject.SetActive (true);
			loadAssetBundle.SetCardImage (cardMaster.cardId, (int)LoadAssetBundle.DisplayType.Deck, transform.Find ("CardImage").gameObject);
			transform.Find ("CardName").GetComponent<Text> ().text = cardMaster.name;
			if (gameObject.GetComponent<Scale_Pop> () != null) {
				transform.localScale = gameObject.GetComponent<Scale_Pop> ().startScale;
			}
			Color tmpColor = Color.white;
			switch (cardMaster.color) {
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
			transform.Find ("ColorPlate").GetComponent<Image> ().color = tmpColor;
		} else {
			transform.Find ("CardImage").gameObject.SetActive (false);
			transform.Find ("CardName").gameObject.SetActive (false);
			transform.Find ("ColorPlate").gameObject.SetActive (false);
		}
	}
}
