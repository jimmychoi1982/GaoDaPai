using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;


namespace GameView {
	public class SwipeAttack : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler
	{
		//
		GameView gameView { get { return GameView.Instance; }}

		//
		[Header("Icon Info")]
		public ShowBoardIconInfo iconInfo;

		//
		[Header("Targetter")]
		public GameObject dragTargetPrefab;
		private Vector3 startPointerdPos;
		private LaserPointer currentTargetter;
		private string currentTargetId;


		//
		public void OnBeginDrag(PointerEventData eventData) {
			startPointerdPos = Camera.main.ScreenToWorldPoint(eventData.position);
			iconInfo.CleanupTouchTimer();
		}

		//
		void CreateTargetter() {
			if (currentTargetter != null) {
				return;
			}

			GameObject newTargetter = GameObject.Instantiate(dragTargetPrefab);
			newTargetter.transform.SetParent(transform);
			newTargetter.transform.localScale = dragTargetPrefab.transform.localScale;
			
			RectTransform targetterRect = newTargetter.GetComponent<RectTransform>();
			currentTargetter = newTargetter.GetComponent<LaserPointer>();
			currentTargetter.startPos = targetterRect.InverseTransformPoint(transform.position);
		}

		//
		void ResetTargetter() {
			if (currentTargetter == null) {
				return;
			}

			GameObject.Destroy(currentTargetter.gameObject);
			currentTargetter = null;
			currentTargetId = null;
		}
		
		//
		public void OnDrag(PointerEventData eventData) {
			// Make sure this is a swipe upwards
			Vector3 pointerPos = Camera.main.ScreenToWorldPoint(eventData.position);
			if (pointerPos.y < startPointerdPos.y) {
				ResetTargetter();
				return;
			}

			// Move targetter along with swipe
			CreateTargetter();
			RectTransform targetterRect = currentTargetter.GetComponent<RectTransform>();
			currentTargetter.endPos = targetterRect.InverseTransformPoint(pointerPos);

			// Find a target
			currentTargetId = null;
			foreach (GameObject item in eventData.hovered) {
				if (item.transform.parent == gameView.enemyBoardManager.transform) {
					BoardIcon boardIcon = item.GetComponent<BoardIcon>();
					currentTargetId = (string)boardIcon.iconData["instanceId"];
					currentTargetter.endPos = targetterRect.InverseTransformPoint(item.transform.position);
					break;
				} else if (item.transform == gameView.enemyMothership.transform) {
					Mothership mothership = item.GetComponent<Mothership>();
					currentTargetId = (string)mothership.data["instanceId"];
					currentTargetter.endPos = targetterRect.InverseTransformPoint(item.transform.position);
					break;
				}
			}

			// Update pointer
			if (currentTargetId != null) {
				currentTargetter.GetComponent<Animator>().SetBool("LockOn", true);
				currentTargetter.GetComponent<Animator>().SetBool("LockOnBan", false);
			} else {
				currentTargetter.GetComponent<Animator>().SetBool("LockOn", false);
				currentTargetter.GetComponent<Animator>().SetBool("LockOnBan", false);
			}
		}
		
		//
		public void OnEndDrag(PointerEventData eventData) {
			// Attack if we have a target
			if (currentTargetId != null) {
				string instanceId = null;
				BoardIcon boardIcon = GetComponent<BoardIcon>();
				Mothership mothership = GetComponent<Mothership>();
				if (boardIcon != null) {
					instanceId = (string)boardIcon.iconData["instanceId"];
				}
				if (mothership != null) {
					instanceId = (string)mothership.data["instanceId"];
				}

				gameView.AttackEnemy(instanceId, currentTargetId);
			}

			// Reset
			ResetTargetter();
		}
	}
}