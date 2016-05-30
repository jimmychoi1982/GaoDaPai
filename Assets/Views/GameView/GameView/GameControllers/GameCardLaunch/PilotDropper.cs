using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GameView {
	public class PilotDropper : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler
	{
		//
		Card card { get { return Card.Instance; }}

		//
		[Header("Cost Card")]
		public CostIcon costIcon;

		//
		[Header("Drag Drop")]
		public GameObject dropZone;
		public Action<CostIcon> onDrop;

		//
		public void OnBeginDrag(PointerEventData eventData) {
			// Check if this icon is draggable
			if (dropZone == null) {
				return;
			}

			// Allow the icon to be moved around
			GetComponent<LayoutElement>().ignoreLayout = true;
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}

		//
		public void OnDrag(PointerEventData eventData) {
			// Check if this icon is draggable
			if (dropZone == null) {
				return;
			}

			// Move the icon along with the drag
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
			GetComponent<RectTransform>().position = (Vector2)worldPos;
		}

		//
		public void OnEndDrag(PointerEventData eventData) {
			// Check if this icon is draggable
			if (dropZone == null) {
				return;
			}

			// Check if we were dropped into the drop zone
			foreach (GameObject hovered in eventData.hovered) {
				if (hovered == dropZone && onDrop != null) {

					// チュートリアル処理
					if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
						
						if (PlayerPrefs.GetString ("Tutorial") == "Step3"){
							var tutorialView = GameObject.Find ("GameView").GetComponent<TutorialViewStep3> ();
							var tutorialActionData = tutorialView.GetTutorialActionData ();

							if (tutorialActionData.GetCurrentIndex () == 2){
								// Return icon to bridge
								GetComponent<LayoutElement>().ignoreLayout = false;
								GetComponent<CanvasGroup>().blocksRaycasts = true;
								return;
							}
							if (tutorialActionData.GetCurrentIndex () == 3){
								tutorialActionData.LauchNextAction ();
							}
						}
					}
					onDrop.Invoke(costIcon);

					break;
				}
			}
			
			// Return icon to bridge
			GetComponent<LayoutElement>().ignoreLayout = false;
			GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
	}
}