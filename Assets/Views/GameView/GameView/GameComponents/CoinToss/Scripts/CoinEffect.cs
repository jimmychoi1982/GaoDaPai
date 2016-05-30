using System;
using System.Collections;

using UnityEngine;

public class CoinEffect : MonoBehaviour 
{
	//
	public ParticleSystem bright;

	//
	public Action onTurnDecidedMe;
	public Action onTurnDecidedEnemy;
	
	//
	public Action onComplete;

	// Called by CoinToss animations to activate sparkle particles
	void bright_Start() {
		this.bright.Play();
	}

	// Called by CoinMove animations to activate sparkle trail particles
	void bright_small() {
		bright.startSize = 0.4f;
	}

	// Called by CoinToss animations when coin stops rotating
	void TurnDecidedMe() {
		if (onTurnDecidedMe != null) {
			onTurnDecidedMe.Invoke();
		}
	}
	
	// Called by CoinToss animations when coin stops rotating
	void TurnDecidedEnemy() {
		if (onTurnDecidedEnemy != null) {
			onTurnDecidedEnemy.Invoke();
		}
	}

	// Called by CoinMove animations when animation completes
	void AnimationCompleted() {
		if (onComplete != null) {
			onComplete.Invoke();
		}
	}
}
