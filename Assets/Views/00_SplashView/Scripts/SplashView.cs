using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class SplashView : MonoBehaviour {

	public Image splash1, splash2, splash3;

	private bool splashEnd;

	void Awake() {
		// Fix for multitouch to disable it
		Input.multiTouchEnabled = false;
	}

	// Use this for initialization
	void Start () {
		transform.GetComponent<AspectRatioFixer> ().Revise ();

		//DOTween初期化
		DOTween.Init (false, true, LogBehaviour.ErrorsOnly);

		splash1.color = new Color (1, 1, 1, 1);

		splashEnd = false;
	}

	void OnEnable () {
		new Task (SplashAnimation ());
	}

	void Update () {
		if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {
			EndCallback();
		}
	}

	IEnumerator SplashAnimation () {
		yield return new WaitForSeconds (5f);

		DOTween.Sequence()
			.Prepend(splash1.DOFade (0, 0.2f))
			.AppendInterval(0.5f)
			.Append(splash2.DOFade (1, 0.2f))
			.AppendInterval(1.5f)
			.Append(splash2.DOFade (0, 0.2f))
			.AppendInterval(0.5f)
			.Append(splash3.DOFade (1, 0.2f))
			.AppendInterval(1.5f)
			.Append(splash3.DOFade (0, 0.2f))
			.AppendCallback(EndCallback);
	}

	void EndCallback () {
		if (!splashEnd) {
			splashEnd = true;
			Application.LoadLevelAsync ("TitleView");
		}
	}

}
