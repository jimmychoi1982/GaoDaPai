using System;
using System.Collections;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;


public class FadingError : MonoBehaviour {
	//
	private Text text;
	private Image image;

	//
	[Tooltip("Setting fadeout speed when the object vanish")]
	public float fadeSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		text = GameObject.Find("error_text(Clone)").GetComponent<Text>();
		image = GameObject.Find("error_text(Clone)/error_bg").GetComponent<Image>();

		StartCoroutine (Delay (1.0f, () => {
			new Task(FadeAway());
		}));
	}

	//
	IEnumerator FadeAway() {
		Color textTargetColor = new Color(text.color.r, text.color.g, text.color.b, 0f);
		Color imageTargetColor = new Color(image.color.r, text.color.g, image.color.b, 0f);

		while (text.color.a > 0.1) {
			if (text == null) {
				yield break;
			}

			text.color = Color.Lerp(text.color, textTargetColor, fadeSpeed);
			image.color = Color.Lerp(image.color, imageTargetColor, fadeSpeed);
			yield return null;
		}

		GameObject.Destroy(gameObject);
	}

	private IEnumerator Delay(float delayTime, Action action) {
		yield return new WaitForSeconds (delayTime);
		action ();
	}
}