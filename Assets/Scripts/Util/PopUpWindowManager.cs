using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class PopUpWindowManager : MonoBehaviour {
	// Initialize pop up window
	public static PopUpWindow Init(string prefabPath, Transform parentTransform) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);

		return windowPrefab.AddComponent<PopUpWindow> ()
			.Init (true);
	}

	// Initialize window, Not Pop Up Window
	public static GameObject InitAsGameObject(string prefabPath, Transform parentTransform) {
		return InstantiateWindowPrefab(prefabPath, parentTransform);
	}
	
	// Open pop up window (Init first, single action, with subject & message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string subject, string message, string buttonText, Action action, bool isDestroy = false) {
		return popUpWindow
			.SetSubject (subject)
			.SetMessage (message)
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}

	// Open pop up window (Init first, single action, without subject, with message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string message, string buttonText, Action action, bool isDestroy = false) {
		return popUpWindow
			.SetMessage (message)
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (Init first, single action, without subject & message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string buttonText, Action action, bool isDestroy = false) {
		return popUpWindow
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (Init first, positive & negative action, with subject & message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string subject, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		return popUpWindow
			.SetSubject (subject)
			.SetMessage (message)
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (Init first, positive & negative action, without subject, with message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		return popUpWindow
			.SetMessage (message)
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (Init first, positive & negative action, without subject & message)
	public static PopUpWindow Open(PopUpWindow popUpWindow, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		return popUpWindow
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, single action, with subject & message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string subject, string message, string buttonText, Action action, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetSubject (subject)
			.SetMessage (message)
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, single action, without subject, with message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string message, string buttonText, Action action, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetMessage (message)
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, single action, without subject & message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string buttonText, Action action, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetSingleActionEvent (buttonText, action, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, positive & negative action, with subject & message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string subject, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetSubject (subject)
			.SetMessage (message)
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, positive & negative action, without subject, with message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetMessage (message)
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}
	
	// Open pop up window (with Init, positive & negative action, without subject & message)
	public static PopUpWindow Open(string prefabPath, Transform parentTransform, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction, bool isDestroy = false) {
		GameObject windowPrefab = InstantiateWindowPrefab(prefabPath, parentTransform);
		
		return windowPrefab.AddComponent<PopUpWindow>()
			.Init (true)
			.SetPositiveActionEvent (positiveButtonText, positiveAction, isDestroy)
			.SetNegativeActionEvent (negativeButtonText, negativeAction, isDestroy)
			.FadeIn ();
	}

	public static PopUpWindow Close(PopUpWindow popUpWindow) {
		return popUpWindow.FadeOut (() => {});
	}

   // Instantiate pop up window prefab
	private static GameObject InstantiateWindowPrefab(string prefabPath, Transform parentTransform) {
		GameObject windowPrefab = Instantiate(Resources.Load (prefabPath)) as GameObject;
		
		windowPrefab.transform.SetParent(parentTransform);

		return windowPrefab;
	}
}
