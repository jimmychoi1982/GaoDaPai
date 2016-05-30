using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace GameView {
	public class CardTips : MonoBehaviour {
		//
		[Header("")]
		public GameCard gameCard;
		public Text cardEffect1;
		public Text cardEffect2;

		//
		public void SetData(TomeObject cardData) {
			// Set card image
			gameCard.SetData(cardData);

			// Set card effects
			TomeArray descriptions = cardData["description"] as TomeArray;
			cardEffect1.text = (descriptions != null && descriptions.Count > 0) ? descriptions[0].ToString().Replace("<br />", "\n") : "";
			cardEffect2.text = (descriptions != null && descriptions.Count > 1) ? descriptions[1].ToString().Replace("<br />", "\n") : "";
		}

		//
		public IEnumerator DoTransition() {
			yield return StartCoroutine(WindowIn());
			yield return new WaitForSeconds(5);
			yield return StartCoroutine(WindowOut());
			GameObject.Destroy(gameObject);
		}

		//
		private IEnumerator WindowIn() {
			RectTransform rectTransform = GetComponent<RectTransform>();
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

			// Reset
			rectTransform.anchoredPosition = new Vector2(1000, 0);
			canvasGroup.alpha = 0;

			// Slide and fade in
			rectTransform.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutCubic);
			canvasGroup.DOFade (1, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}

		//
		private IEnumerator WindowOut() {
			RectTransform rectTransform = GetComponent<RectTransform>();
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

			// Reset
			rectTransform.anchoredPosition = new Vector2(0, 0);
			canvasGroup.alpha = 1;

			// Slide and fade out
			rectTransform.DOAnchorPos(new Vector2(1000, 0), 0.3f).SetEase(Ease.OutCubic);
			canvasGroup.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}
	}
}