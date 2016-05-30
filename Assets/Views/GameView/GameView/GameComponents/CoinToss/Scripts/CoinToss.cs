using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


public class CoinToss : MonoBehaviour {
	//
	Game game { get { return Game.Instance; }}

	//
	[Header("Coin Object")]
	public GameObject coin;
	private Animator coinAnimator;
	private CoinEffect coinEffect;

	//
	[Header("Move Turn")]
	public GameObject moveTurnBox;
	public GameObject firstmove, secondmove;
	public CanvasGroup logoCanvasGroup;
	public AudioSource SE_TurnFix;

	//
	[Header("Overlay")]
	public Image overlay;
	
	//
	void Awake() {
		//
		overlay.gameObject.SetActive(false);

		//
		coinAnimator = coin.GetComponent<Animator>();
		coinEffect = coin.GetComponent<CoinEffect>();

		//
		coinEffect.onTurnDecidedMe += DisplayTurnMe;
		coinEffect.onTurnDecidedEnemy += DisplayTurnEnemy;

		//
		DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);
	}

	//
	public void CoinTossMe(Action cb) {
		coinEffect.onComplete = () => {
			coinEffect.onComplete = null;
			cb();
		};

		overlay.gameObject.SetActive(true);
		coinAnimator.SetTrigger("TossCoinMe");
	}

	//
	public void CoinTossEnemy(Action cb) {
		coinEffect.onComplete = () => {
			coinEffect.onComplete = null;
			cb();
		};

		overlay.gameObject.SetActive(true);
		coinAnimator.SetTrigger("TossCoinEnemy");
	}

	//
	void DisplayTurnMe() {
		FadeOverlay();
		new Task(DisplayTurnLogo(firstmove));
	}

	//
	void DisplayTurnEnemy() {
		FadeOverlay();
		new Task(DisplayTurnLogo(secondmove));
	}

	//
	IEnumerator DisplayTurnLogo(GameObject turnLogo) {
		// Reset animation objects
		moveTurnBox.transform.DOKill();
		moveTurnBox.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		
		logoCanvasGroup.DOKill();
		logoCanvasGroup.alpha = 1.0f;

		firstmove.SetActive(false);
		secondmove.SetActive(false);
		turnLogo.SetActive(true);

		SE_TurnFix.Stop();
		yield return new WaitForSeconds(0.1f);

		// Play sound effect
		SE_TurnFix.Play();
		
		// Logo ease in
		moveTurnBox.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(0.7f);
		
		// Logo ease out
		logoCanvasGroup.DOFade(0, 0.3f);
		yield return new WaitForSeconds(0.3f);
	}

	//
	void FadeOverlay() {
		overlay.DOFade(0, 0.6f).OnComplete(() => {
			overlay.gameObject.SetActive(false);
		});
	}
}
