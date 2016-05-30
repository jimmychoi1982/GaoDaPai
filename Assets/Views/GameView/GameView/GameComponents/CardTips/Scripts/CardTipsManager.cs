using UnityEngine;
using System.Collections;

namespace GameView {
	public class CardTipsManager : MonoBehaviour {
		//
		[Header("Card Tips Instance")]
		public GameObject parent;
		public GameObject prefab;

		//
		private CardTips currentTips = null;

		//
		public void Show(TomeObject cardData) {
			// Destroy existing tips window if there is one
			if (currentTips) {
				GameObject.Destroy(currentTips.gameObject);
				currentTips = null;
			}

			// Create new tip object
			GameObject tipsObject = GameObject.Instantiate(prefab);
			tipsObject.transform.SetParent(parent.transform);
			tipsObject.transform.localScale = prefab.transform.localScale;
			tipsObject.transform.SetAsLastSibling();

			// Setup card data
			CardTips cardTips = tipsObject.GetComponent<CardTips>();
			cardTips.SetData(cardData);

			// Set current tip and transition it
			currentTips = cardTips;
			new Task(cardTips.DoTransition());
		}
	}
}