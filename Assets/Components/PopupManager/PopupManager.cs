using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class PopupManager : MonoBehaviour {
	//
	public GameObject debugServerSelect;
	public GameObject debugInitFailed;

	//
	public GameObject connectionError;
	public GameObject authError;
	public GameObject maintenanceError;


	//
	private Dictionary<string, GameObject> popupList;

	//
	private string currentPopupId = null;
	private Action<object> currentPopupCb = null;


	// 
	void Awake () {
		//
		debugServerSelect.SetActive(false);
		debugInitFailed.SetActive (false);
		connectionError.SetActive (false);

		//
		gameObject.SetActive (false);

		//
		popupList = new Dictionary<string, GameObject>()
		{
			// DEBUG only popups, these should never be used in production
			{"DebugServerSelect", debugServerSelect},
			{"DebugInitFailed", debugInitFailed},

			// Non-Debug popups, these are safe to user where ever required
			{"ConnectionError", connectionError},
			{"AuthError", authError},
			{"MaintenanceError", maintenanceError}
		};
	}

	// Opens up a popups inside the popupList
	public void open(string popupId, Action<object> cb = null) {
		if (!popupList.ContainsKey(popupId) || currentPopupId != null) {
			return;
		}

		//
		currentPopupId = popupId;
		currentPopupCb = (cb != null) ? cb : (object result) => {};

		//
		gameObject.SetActive (true);
		popupList[popupId].SetActive (true);
	}

	//
	private void close(object result) {
		//
		Action<object> toBeCalled = currentPopupCb;

		//
		gameObject.SetActive (false);
		popupList[currentPopupId].SetActive (false);

		//
		currentPopupId = null;
		currentPopupCb = null;

		//
		toBeCalled(result);
	}


	/**
	 * Select Server (DEBUG)
	 **/

	//
	public void debugServerSelectLocal() {
		close("LOCAL");
	}
	
	//
	public void debugServerSelectTest() {
		close("TESTING");
	}
	
	//
	public void debugServerSelectQA() {
		close("QA");
	}
	
	//
	public void debugServerSelectRankMatch() {
		close("RANKMATCH");
	}

	//
	public void debugServerSelectProd() {
		close("PRODUCTION");
	}


	/**
	 * Init Failed Popup (DEBUG)
	 **/

	//
	public void debugInitFailedRetry() {
//		Application.LoadLevel("TitleView");
		GameObject.Find ("/Steward").GetComponent<Steward> ().LoadNextScene ("TitleView", true);
		close(null);
	}

	//
	public void debugInitFailedResetUserId() {
		GameSettings.UserId = null;
//		Application.LoadLevel("TitleView");
		GameObject.Find ("/Steward").GetComponent<Steward> ().LoadNextScene ("TitleView", true);
		close(null);
	}
	
	
	/**
	 * Connection Error Popup
	 **/
	
	//
	public void debugConnecitonErrorReload() {
//		Application.LoadLevel("TitleView");
		GameObject.Find ("/Steward").GetComponent<Steward> ().LoadNextScene ("TitleView", true);
		close(null);
	}
	
	
	/**
	 * Auth Error Popup
	 **/
	
	//
	public void debugAuthErrorReload() {
//		Application.LoadLevel("TitleView");
		GameObject.Find ("/Steward").GetComponent<Steward> ().LoadNextScene ("TitleView", true);
		close(null);
	}
	
	
	/**
	 * Maintenance Error Popup
	 **/
	
	//
	public void debugMaintenanceErrorReload() {
//		Application.LoadLevel("TitleView");
		GameObject.Find ("/Steward").GetComponent<Steward> ().LoadNextScene ("TitleView", true);
		close(null);
	}
}
