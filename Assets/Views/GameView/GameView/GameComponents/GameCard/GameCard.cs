using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameView {
	public class GameCard : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		GameView gameView { get { return GameView.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		//
		public Image imgCard;
		public TomeObject cardData;

		//
		public void SetData(TomeObject cardData, Sprite placeholder = null) {
			this.cardData = cardData;

			// Set card image
			if (placeholder != null) {
				imgCard.sprite = placeholder;
			} else if (cardData["cardId"] != null) {
				string cardId = (string)cardData["cardId"];
				loadAssetBundle.SetCardImage(cardId, (int)LoadAssetBundle.DisplayType.Card, imgCard.gameObject);
			} else {
				string enemyId = gameView.enemyUserId;
				string sleeveId = (string)game.tCurrentGame["playerData"][enemyId]["unitSleeveId"];
				loadAssetBundle.SetSleeveImage(sleeveId, (int)LoadAssetBundle.DisplayType.Card, imgCard.gameObject);
			}
		}
	}
}