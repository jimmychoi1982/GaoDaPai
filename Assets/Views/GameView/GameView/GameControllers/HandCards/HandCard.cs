using System;
using System.Collections;
using System.Threading;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace GameView {
	public class HandCard : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerClickHandler,
		IPointerDownHandler,
		IPointerUpHandler
	{
		//
		GameView gameView { get { return GameView.Instance; }}

		//
		public Image imgCard;
		public CanvasGroup imgGroup;
		public GameCard gameCard;

		//
		public delegate void OnTap();
		public OnTap onTap;
		public delegate void OnLaunch();
		public OnLaunch onLaunch;

		//
		private RectTransform rectTransform;
		private LayoutElement layoutElement;
		private Vector2 dragStartPos;

		//
		private Timer touchTimer;


		void Awake() {
			rectTransform = GetComponent<RectTransform>();
			layoutElement = GetComponent<LayoutElement>();

			// Set up in the awake because Appear is called in a Start()
			imgGroup.alpha = 0;
		}

		public void Appear() {
			imgGroup.alpha = 1;
		}

		//
		public IEnumerator Destroy() {
			// Pull card out
			layoutElement.ignoreLayout = true;
			rectTransform.DOLocalMoveY(300, 0.3f).SetEase(Ease.OutExpo);
			rectTransform.DOScale(new Vector3(2, 2, 0), 0.3f).SetEase(Ease.OutExpo);
			imgCard.GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.OutExpo);
			yield return new WaitForSeconds(1f);

			// Translout up and out
			rectTransform.DOLocalMoveY(2000, 0.3f).SetEase(Ease.InExpo);
			yield return new WaitForSeconds(0.3f);
		}
		
		public void OnBeginDrag(PointerEventData eventData) {
			if (onLaunch == null) {
				return;
			}

			if (!GameHelpers.IsUsersTurn(gameView.meUserId)) {
				return;
			}

			//
			CleanupTouchTimer();

			// Allow the item to be moved around
			layoutElement.ignoreLayout = true;
			dragStartPos = Camera.main.ScreenToWorldPoint(eventData.position);
		}
		
		public void OnDrag (PointerEventData eventData) {
			if (onLaunch == null) {
				return;
			}

			if (!GameHelpers.IsUsersTurn(gameView.meUserId))
			{
				return;
			}

			// Drag the card around
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
			rectTransform.position = worldPos;
		}
		
		public void OnEndDrag (PointerEventData eventData) {
			if (onLaunch == null) {
				return;
			}

			if (!GameHelpers.IsUsersTurn(gameView.meUserId))
			{
				return;
			}

			// Calculate drag distance and see if we went far enough to call onLaunch
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
			float yDistance = worldPos.y - dragStartPos.y;
			if (yDistance > 1) {
				onLaunch.Invoke();
			}

			// Reset
			dragStartPos = Vector2.zero;
			layoutElement.ignoreLayout = false;
		}
		
		//
		public void OnPointerClick(PointerEventData eventData) {
			// Ignore click events if we are dragging
			if (layoutElement.ignoreLayout == true) {
				return;
			}

			if (onTap != null) {
				onTap.Invoke();
			}
		}

		//
		private void CleanupTouchTimer() {
			if (touchTimer == null) {
				return;
			}

			touchTimer.Dispose();
			touchTimer = null;
		}

		//
		private IEnumerator OpenCardInfo() {
			gameView.cardInfo.Open(gameCard.cardData);
			yield break;
		}

		//
		public void OnPointerDown(PointerEventData eventData) {
			if (onLaunch == null) {
				return;
			}

			if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial || GameSettings.TutorialState == GameSettings.TutorialStates.Encore) {
				if (PlayerPrefs.GetString ("Tutorial") != "Step1"){
					return;
				}
			}

			touchTimer = new Timer((object state) => {
				CleanupTouchTimer();
				TaskManagerMainThread.Queue(OpenCardInfo());
			}, null, 400, Timeout.Infinite);
		}

		//
		public void OnPointerUp(PointerEventData eventData) {
			if (onLaunch == null) {
				return;
			}

			CleanupTouchTimer();
		}
	}
}