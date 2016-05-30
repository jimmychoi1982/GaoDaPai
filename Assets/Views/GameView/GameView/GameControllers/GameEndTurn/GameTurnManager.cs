using System;
using System.Collections;
using System.Threading;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class GameTurnManager : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		GameView gameView { get { return GameView.Instance; }}
		
		//
		private int TURN_WARNING_MS = 60 * 1000;
		private int TURN_TIME_LIMIT_MS = 90 * 1000;
		private int TURN_STEAL_RETRY_MS = 5 * 1000;
		private Timer turnTimeLimitTimer;

		//
		[Header("Turn Button")]
		public GameObject btnEndTurn;
		public GameObject btnEnemyTurn;
		public GameObject disableOverlay;

		//
		[Header("Turn Logo")]
		public GameObject turnContainer;
		public CanvasGroup turnCanvasGroup;
		public AudioSource turnSoundEffect;
		public GameObject logoEnemyTurn;
		public GameObject logoMyTurn;
		public GameObject lineEnemyTurn;
		public GameObject lineMyTurn;
		public LineTurnAnimation line;

		//
		[Header("Time-Out Logo")]
		public GameObject timeOutContainer;
		public CanvasGroup timeOutCanvasGroup;
		public AudioSource timeOutSoundEffect;


		//
		[Header("Timer Gauge")]
		public Slider TimerSlider;

		private int leftTime;

		// Use this for initialization
		void Awake () {
			btnEndTurn.SetActive(false);
			btnEnemyTurn.SetActive(false);
			disableOverlay.SetActive(false);
		}

		void Update () {
			leftTime -= (int)(Time.deltaTime * 1000);
			if (leftTime < 0) {
				leftTime = 0;
			}
			if (TimerSlider != null) {

				if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial ||
				    GameSettings.TutorialState == GameSettings.TutorialStates.Encore){
					return;
				}

				TimerSlider.value = (float)leftTime / (float)TURN_TIME_LIMIT_MS;
			}
		}

		//
		public void Disable() {
			disableOverlay.SetActive(true);
		}

		//
		public void Enable() {
			disableOverlay.SetActive(false);
		}

		//
		public void SetTurn() {
			//
			Enable();

			//
			string currentTurn = (string)game.tCurrentGame["currentTurn"];
			if (currentTurn == gameView.meUserId) {
				btnEndTurn.gameObject.SetActive(true);
				btnEnemyTurn.gameObject.SetActive(false);
				logoEnemyTurn.gameObject.SetActive(false);
				logoMyTurn.gameObject.SetActive(true);
				lineEnemyTurn.gameObject.SetActive (false);
				lineMyTurn.gameObject.SetActive (true);

				//
				line.StartCoroutine ("AnimeStart");
				new Task(DisplayTurnChangeLogo());
				
				//
				//animManager.ResetTimer();
				//animManager.StartTimer();
				CleanupTurnTimeLimit();
			} else {
				btnEndTurn.gameObject.SetActive(false);
				btnEnemyTurn.gameObject.SetActive(true);
				logoEnemyTurn.gameObject.SetActive(true);
				logoMyTurn.gameObject.SetActive(false);
				lineEnemyTurn.gameObject.SetActive (true);
				lineMyTurn.gameObject.SetActive (false);

				//
				line.StartCoroutine ("AnimeStart");
				new Task(DisplayTurnChangeLogo());

				//
				//animManager.ResetTimer();
				SetupTurnTimeLimit();
			}
			leftTime = TURN_TIME_LIMIT_MS;
		}
		
		//
		IEnumerator DisplayTurnChangeLogo() {

			// Reset animation objects
			turnContainer.transform.DOKill();
			turnContainer.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			
			turnCanvasGroup.DOKill();
			turnCanvasGroup.alpha = 1.0f;
			
			turnSoundEffect.Stop();
			yield return new WaitForSeconds(0.1f);
			
			// Play sound effect
			turnSoundEffect.Play();
			
			// Logo ease in
			turnContainer.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
			yield return new WaitForSeconds(0.7f);
			
			// Logo ease out
			turnCanvasGroup.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}
		
		//
		IEnumerator DisplayTimeOutLogo() {
			// Reset animation objects
			timeOutContainer.transform.DOKill();
			timeOutContainer.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			
			timeOutCanvasGroup.DOKill();
			timeOutCanvasGroup.alpha = 1.0f;
			
			timeOutSoundEffect.Stop();
			yield return new WaitForSeconds(0.1f);
			
			// Play sound effect
			timeOutSoundEffect.Play();
			
			// Logo ease in
			timeOutContainer.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
			yield return new WaitForSeconds(0.7f);
			
			// Logo ease out
			timeOutCanvasGroup.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}

		//
		public void EndTurn() {
			Disable();
			gameView.EndTurn();
		}
		
		//
		public void CleanupTurnTimeLimit() {
			if (turnTimeLimitTimer != null) {
				turnTimeLimitTimer.Dispose();
				turnTimeLimitTimer = null;
			}
		}
		
		//
		void SetupTurnTimeLimit() {
			CleanupTurnTimeLimit();
			turnTimeLimitTimer = new Timer(StealTurn, null, TURN_TIME_LIMIT_MS, Timeout.Infinite);
		}
		
		//
		void StealTurn(object state) {
			TaskManagerMainThread.Queue(game.TakeTurn((Exception error) => {
				string currentUser = (string)game.tCurrentGame["currentTurn"];
				if (error != null && currentUser != gameView.meUserId) {
					turnTimeLimitTimer = new Timer(StealTurn, null, TURN_STEAL_RETRY_MS, Timeout.Infinite);
					return;
				}
				
				CleanupTurnTimeLimit();
			}));
		}
	}
}