using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class TapScreenBlink : MonoBehaviour {

	public Image tapScreen;
	public bool animeStart;

	void Update () {

		if (animeStart == true) {
			TapScreenBlinkAnimation();
			animeStart = false;
		}
	}

	void TapScreenBlinkAnimation () {
		tapScreen.DOFade (1, 0.4f).SetEase (Ease.OutCubic).SetLoops (-1, LoopType.Yoyo);
	}

}
