using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace GameView {
	public class SwipeVoluntaryTap : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler
	{
		GameView gameView { get { return GameView.Instance; }}

		//
		[Header("Icon")]
		public GameObject icon;

		//
		[Header("Swipe")]
		public float deltaThreshold = 100f;
		public float deltaMax = 40f;
		public GameObject indicatorParent;
		public CanvasGroup textIndicator;
		private Vector3 startPointerdPos;

		//
		public void OnBeginDrag(PointerEventData eventData) {
			// Check if this icon supports voluntary tapping
			if (indicatorParent == null || !indicatorParent.activeSelf) {
				return;
			}

			startPointerdPos = Camera.main.ScreenToWorldPoint(eventData.position);
		}

		//
		void Reset() {
			icon.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
			textIndicator.alpha = 0;
		}

		//
		float GetDeltaY(Vector3 pointerPos) {
			RectTransform iconRect = GetComponent<RectTransform>();
			Vector2 startPos = iconRect.InverseTransformPoint(startPointerdPos);
			Vector2 endPos = iconRect.InverseTransformPoint(pointerPos);
			return endPos.y - (startPos.y - deltaThreshold);
		}

		//
		public void OnDrag(PointerEventData eventData) {
			// Check if this icon supports voluntary tapping
			if (indicatorParent == null || !indicatorParent.activeSelf) {
				return;
			}

			// Make sure this is a swipe upwards
			Vector3 pointerPos = Camera.main.ScreenToWorldPoint(eventData.position);
			float yDelta = GetDeltaY(pointerPos);
			if (yDelta > 0) {
				Reset();
				return;
			}
			
			// Move icon downwards by detla
			if (Mathf.Abs(yDelta) < Mathf.Abs(deltaMax)) {
				icon.GetComponent<RectTransform>().localPosition = new Vector2(0, yDelta);
			} else {
				icon.GetComponent<RectTransform>().localPosition = new Vector2(0, -deltaMax);
			}
			
			// Fade in tap indicator text
			textIndicator.alpha = (Mathf.Abs(yDelta) < deltaMax) ? Mathf.Abs(yDelta) / deltaMax : 1;
		}

		//
		public void OnEndDrag(PointerEventData eventData) {
			// Check if this icon supports voluntary tapping
			if (indicatorParent == null || !indicatorParent.activeSelf) {
				return;
			}

			// Voluntary tap if we swiped pasth the threshold
			Vector3 pointerPos = Camera.main.ScreenToWorldPoint(eventData.position);
			float yDelta = GetDeltaY(pointerPos);

			if (yDelta < 0 && Mathf.Abs(yDelta) >= Mathf.Abs(deltaMax)) {
				BoardIcon boardIcon = GetComponent<BoardIcon>();
				string instanceId = (string)boardIcon.iconData["instanceId"];
				gameView.TapIcon(instanceId);

				if (PlayerPrefs.GetString ("Tutorial") != "End"){
					
					if (PlayerPrefs.GetString ("Tutorial") == "Step2"){
						
						var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep2> ();
						var tutorialActionData = tutorialView.GetTutorialActionData ();
						
						if (tutorialActionData.GetCurrentIndex () == 5){
							tutorialView.GetTutorialActionData ().LauchNextAction ();
						}
					}
				}
			}

			// Reset
			Reset();
		}
	}
}