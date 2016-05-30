using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Prime31;

public class LoadingView : MonoBehaviour {
	public Text maxSize;
	public Text nowSize;
	public Text progressValue;

	private Slider downloadProgressBar;
	
	//
	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Logger logger { get { return mage.logger("LoadingView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;
	
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		downloadProgressBar = GameObject.Find ("/Main Canvas/Panel/Loading/DownloadProgressBar").GetComponent<Slider> ();

		switch (GameSettings.TutorialState) {
		case GameSettings.TutorialStates.FirstInitialRegistration:
			steward.LoadNextScene ("FirstInitialRegistrationView");
			break;
			
		case GameSettings.TutorialStates.Tutorial:
			new Task (loadAssetBundle.DownloadAllAssetBundles(() => {
				#if UNITY_ANDROID || UNITY_IOS
				Localytics.tagEvent("Tutorial: Begin");
				#endif
				// Loading done, move to next scene
				steward.LoadNextScene ("NewTutorialIntroView");
			}));
			break;
			
		case GameSettings.TutorialStates.SecondInitialRegistration:
			steward.LoadNextScene ("SecondInitialRegistrationView");
			break;

		case GameSettings.TutorialStates.Encore:
		case GameSettings.TutorialStates.Done:
		default:
			GameSettings.TutorialState = GameSettings.TutorialStates.Done;
			new Task (loadAssetBundle.DownloadAllAssetBundles(() => {
				// Loading done, move to next scene
				steward.LoadNextScene ("HomeView");
			}));
			break;
		}
	}
		
	void Update () {
		if (loadAssetBundle.assetBundleTotalCount > 0) {
			float downloadProgress = (float)loadAssetBundle.assetBundleDownloadCount/(float)loadAssetBundle.assetBundleTotalCount;
			downloadProgressBar.value = downloadProgress;

			progressValue.text = (downloadProgress * 100).ToString("F");

			maxSize.text = loadAssetBundle.assetBundleTotalCount.ToString();
			nowSize.text = loadAssetBundle.assetBundleDownloadCount.ToString();
		}
	}
}