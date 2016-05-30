/**
 * PopUpWindow needs following storucture.
 * Window
 *  -BackGround        -- Window's black background
 *  -Parts
 *   -Subject          -- Text object displays subject
 *   -Message          -- Text object displays message
 *   -SingleActionButton
 *    or
 *   -PositiveActionButton and NegativeActionButton
 *                     -- Button object(s) includes Button Interface Text object
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;//uGUI
using System;
using System.Collections;
using DG.Tweening;

public class PopUpWindow : MonoBehaviour {
	[HideInInspector]public bool isClosed = false;

	private CanvasGroup parts;
	private GameObject background, window;
	private bool isBackground;

	private Button singleActionButton;
	private Button positiveActionButton;
	private Button negativeActionButton;

	private Steward steward;
	
	public PopUpWindow Init (bool needBackground) {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		isBackground = needBackground;
		window = transform.Find ("Window").gameObject;
		parts = transform.Find ("Window/Parts").GetComponent<CanvasGroup> ();
		background = transform.Find ("Background").gameObject;
		background.GetComponent<Image>().color = new Color (1f, 1f, 1f, 0f);
		background.SetActive (false);

		transform.localPosition = new Vector3 (0f, 0f, 0f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
		window.transform.localScale = new Vector3 (0f, 0f, 0f);
		parts.alpha = 0;
		singleActionButton = parts.transform.Find ("SingleActionButton").GetComponent<Button> ();
		positiveActionButton = parts.transform.Find ("PositiveActionButton").GetComponent<Button> ();
		negativeActionButton = parts.transform.Find ("NegativeActionButton").GetComponent<Button> ();
		singleActionButton.gameObject.SetActive (false);
		positiveActionButton.gameObject.SetActive (false);
		negativeActionButton.gameObject.SetActive (false);
		isClosed = true;

		return this;
	}
	
	public PopUpWindow SetSubject (string text) {
		SetText("Subject", text);

		return this;
	}
	
	public PopUpWindow SetMessage (string text) {
		SetText("Message", text);
		
		return this;
	}
	
	public PopUpWindow SetText (string relativePathFromParts, string text) {
		parts.transform.Find (relativePathFromParts).GetComponent<Text> ().text = text;
		
		return this;
	}
	
	public PopUpWindow SetInputText (string relativePathFromParts, string text) {
		parts.transform.Find (relativePathFromParts).GetComponent<InputField> ().text = text;
		
		return this;
	}
	
	public GameObject GetSingleActionButton () {
		return singleActionButton.gameObject;
	}
	
	public GameObject GetPositiveActionButton () {
		return positiveActionButton.gameObject;
	}
	
	public GameObject GetNegativeActionButton () {
		return negativeActionButton.gameObject;
	}
	
	public GameObject GetObject (string relativePathFromParts) {
		return parts.transform.Find (relativePathFromParts).gameObject;
	}
	
	public PopUpWindow SetSingleActionEvent (string buttonText, Action action, bool isDestroy = false) {
		if (singleActionButton != null) {
			singleActionButton.gameObject.SetActive (true);
			singleActionButton.transform.Find ("ButtonInterface").GetComponent<Text> ().text = buttonText;
			singleActionButton.onClick.RemoveAllListeners ();
			singleActionButton.onClick.AddListener (() => {
				SingleActionButtonPush ();
				FadeOut (action, isDestroy);
			});
		}
		
		return this;
	}
	
	public PopUpWindow SetPositiveActionEvent (string buttonText, Action action, bool isDestroy = false) {
		if (positiveActionButton != null) {
			positiveActionButton.gameObject.SetActive (true);
			positiveActionButton.transform.Find ("ButtonInterface").GetComponent<Text> ().text = buttonText;
			positiveActionButton.onClick.RemoveAllListeners ();
			positiveActionButton.onClick.AddListener (() => {
				PositiveActionButtonPush ();
				FadeOut (action, isDestroy);
			});
		}
		
		return this;
	}
	
	public PopUpWindow SetNegativeActionEvent (string buttonText, Action action, bool isDestroy = false) {
		if (negativeActionButton != null) {
			negativeActionButton.gameObject.SetActive (true);
			negativeActionButton.transform.Find ("ButtonInterface").GetComponent<Text> ().text = buttonText;
			negativeActionButton.onClick.RemoveAllListeners ();
			negativeActionButton.onClick.AddListener (() => {
				NegativeActionButtonPush ();
				FadeOut (action, isDestroy);
			});
		}
		
		return this;
	}
	
	public PopUpWindow FadeIn () {
		StartCoroutine(fadeInAnimation());
		
		return this;
	}
	
	public PopUpWindow FadeOut (Action action, bool isDestroy = false) {
		StartCoroutine(fadeOutAnimation(action, isDestroy));
		
		return this;
	}

	private IEnumerator fadeInAnimation () {
		isClosed = false;
		steward.SwitchIntaractableAllButtons (false);
		if (isBackground) {
			background.SetActive (true);
			background.GetComponent<Image> ().DOFade (0.7f, 0.4f);
		}
		window.transform.DOScale (1f, 0.3f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds (0.1f);
		parts.DOFade (1f, 0.1f);
	}
	
	private IEnumerator fadeOutAnimation (Action action, bool isDestroy = false) {
		if (isBackground) {
			this.background.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
			background.SetActive (false);
		}
		parts.DOFade (0f, 0.1f);
		window.transform.DOScale (0, 0.2f).SetEase(Ease.InQuad);
		yield return new WaitForSeconds (0.2f);
		if (isDestroy) {
			Destroy (gameObject);
		}
		singleActionButton.gameObject.SetActive (false);
		positiveActionButton.gameObject.SetActive (false);
		negativeActionButton.gameObject.SetActive (false);
		steward.SwitchIntaractableAllButtons (true);
		isClosed = true;
		action ();
	}
	
	public void SingleActionButtonPush () {
		steward.PlaySETap ();
	}
	
	public void PositiveActionButtonPush () {
		steward.PlaySEOK ();
	}
	
	public void NegativeActionButtonPush () {
		steward.PlaySECancel ();
	}
}