using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DrawnMainCard : MonoBehaviour {
	UIElement_DrawMainCard drawCard;
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	//
	// Use this for initialization
	void Start () {
		drawCard = gameObject.GetComponent<UIElement_DrawMainCard> ();
	}
	
	public void SetData ( TomeObject cardData ) {
		string cardId = null;
		Image cardImage;
		if (cardData != null) {
			cardId = (string)cardData["cardId"];
		}
		if (drawCard.cardImage != null) {
			cardImage = drawCard.cardImage;
		} else {
			return;
		}
		if (!String.IsNullOrEmpty ( cardId )) {
			loadAssetBundle.SetCardImage( cardId, (int)LoadAssetBundle.DisplayType.Card, cardImage.gameObject);
		}
	}
}
