using System.Collections;
using System.Threading;

using UnityEngine;
using UnityEngine.EventSystems;

namespace GameView {
	public class ShowCostIconInfo : MonoBehaviour,
		IPointerDownHandler,
		IPointerUpHandler
	{
		//
		GameView gameView { get { return GameView.Instance; }}

		//
		public CostIcon costIcon;
		
		//
		private Timer touchTimer;

		//
		public void CleanupTouchTimer() {
			if (touchTimer == null) {
				return;
			}
			
			touchTimer.Dispose();
			touchTimer = null;
		}
		
		//
		private IEnumerator OpenCardInfo() {
			gameView.cardInfo.Open(costIcon.iconData);
			yield break;
		}
		
		//
		public void OnPointerDown(PointerEventData eventData) {

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
			CleanupTouchTimer();
		}
	}
}