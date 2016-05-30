using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public class Steward : MonoBehaviour {
	public AudioSource[] soundEffectArray = new AudioSource[3];
	public AudioSource[] backgroundMusicArray = new AudioSource[5];
	public GameObject loadingScreen;
	public GameObject loadingAnimation;
	public GameObject webViewWindow;
	public Header header;
	public AudioMixer audioMixer;

	private GameObject live2D;

	private bool isShowLoadingScreen = false;
	private Dictionary<string, int> loadingScreenCondition = new Dictionary<string, int> ();

	private bool isShowLoadingAnimation = false;
	private Dictionary<string, int> loadingAnimationCondition = new Dictionary<string, int> ();
	
	public enum SoundEffect {
		Tap = 0,
		OK,
		Cancel,
	}

	public enum BackgroundMusic {
		None = -1,
		Opening = 0,
		Home,
		InGame,
		InGame2,
		InGame3,
	}

	private List<PopUpStack> popUpStacks = new List<PopUpStack> ();

	private BackgroundMusic currentPlayBGM = BackgroundMusic.None;

	private float volumeRevise = 0.8f;

	private bool isSetVolume = false;

	public enum PopUpType {
		Message = 0,
		Dialog,
		DialogLarge,
	}
	
	public struct PopUpStack {
		public PopUpType popUpType;
		public string subject;
		public string message;
		public string buttonText1;
		public Action action1;
		public string buttonText2;
		public Action action2;
		
		public PopUpStack (PopUpType pPopUpType, string pSubject, string pMessage, string pButtonText1, Action pAction1, string pButtonText2, Action pAction2) {
			popUpType = pPopUpType;
			subject = pSubject;
			message = pMessage;
			buttonText1 = pButtonText1;
			action1 = pAction1;
			buttonText2 = pButtonText2;
			action2 = pAction2;
		}
	}
	
	private PopUpWindow popUpWindow;
	private PopUpWindow popUpWindowL;

	private ErrorCodes errorCodes;
	public ErrorCodes ErrorCodes {
		get {return errorCodes;}
		set {errorCodes = value;}
	}

	private WebViewObject webViewObject;

	private bool isWebViewOpened = false;

	private Camera[] closedCameras = new Camera[0];

	private int customMenu;
	public int CustomMenu {
		get {return customMenu;}
		set {customMenu = value;}
	}
	
	private int selectedDeckId = -1;
	public int SelectedDeckId {
		get {return selectedDeckId;}
		set {selectedDeckId = value;}
	}
	
	private bool firstHomeVisit = true;
	public bool FirstHomeVisit {
		get {return firstHomeVisit;}
		set {firstHomeVisit = value;}
	}

	private string nextSceneName = "";

	private AspectRatioFixer aspectRatioFixer;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private BNID bnid { get { return BNID.Instance; }}
	private Logger logger { get { return mage.logger("Steward"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	public BNIDManager bnidManager;

	void Awake () {
		if (GameObject.FindGameObjectsWithTag ("Steward").Length > 1) {
			Destroy (this.gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
	}

	void Start () {
		SetVolume ();
		popUpWindow = PopUpWindowManager.Init ("Common/Prefabs/PopUpWindow", transform.Find ("Panel"));
		popUpWindowL = PopUpWindowManager.Init ("Common/Prefabs/PopUpWindowLarge", transform.Find ("Panel"));

		webViewObject = (new GameObject ("WebViewObject")).AddComponent<WebViewObject>();
		webViewObject.transform.SetParent (transform);
		webViewWindow = transform.Find ("Panel/WebViewWindow").gameObject;
		CloseWebView (false);
		aspectRatioFixer = transform.GetComponent<AspectRatioFixer> ();
		aspectRatioFixer.Revise ();
		
		bnidManager = new BNIDManager ();

		if (live2D == null) {
			live2D = GameObject.FindGameObjectsWithTag ("Live2D") [0];
		}
	}

	void OnLevelWasLoaded (int level) {
		// Revise aspect ratio (ex. Game view)
		if (Application.loadedLevelName != "TitleView" && Application.loadedLevelName != "GameView") {
			aspectRatioFixer.Revise ();
		}

		// Check version
		if (Application.loadedLevelName != "TitleView" && Application.loadedLevelName != "GameView" && Application.loadedLevelName != "ResultView") {
			CheckVersion ();
		}

		// Start BGM
		if (Application.loadedLevelName == "TitleView" || Application.loadedLevelName == "FirstInitialRegistrationView") {
			PlayBGMOpening ();
		} else if (Application.loadedLevelName == "SecondInitialRegistrationView" || Application.loadedLevelName == "HomeView" || Application.loadedLevelName == "MenuView" || Application.loadedLevelName == "PracticeView") {
			PlayBGMHome ();
		} else if (Application.loadedLevelName == "TutorialView" || Application.loadedLevelName == "GameView") {
			PlayBGMInGame ();
		}

		// Hide header rank pop up
		HideRankPopUp ();

		// If views need header, show header
		if (Application.loadedLevelName == "HomeView" || Application.loadedLevelName == "DeckConstructionView" || Application.loadedLevelName == "ShopView"
			|| Application.loadedLevelName == "CardPurchaseView" || Application.loadedLevelName == "CardRegistrationView"
			|| Application.loadedLevelName == "MenuView" || Application.loadedLevelName == "PracticeView" || Application.loadedLevelName == "MultiplayView") {
			ShowHeader ();
		} else {
			HideHeader ();
		}

		// Show/Hide live2D operator
		if (Application.loadedLevelName == "HomeView" || Application.loadedLevelName == "CardPurchaseView" || Application.loadedLevelName == "CardRegistrationView"
		    || Application.loadedLevelName == "PracticeView" || Application.loadedLevelName == "MultiplayView") {
			ShowLive2DOperator ();
			ShowLive2DFirstMessage ();
		} else {
			HideLive2DOperator ();
		}
	}

	// Show header
	public void ShowHeader () {
		header.Show ();
	}
	
	// Hide header
	public void HideHeader () {
		header.Hide ();
	}
	
	// Reload header
	public void ReloadHeader () {
		header.ApplyData ();
	}
	
	// Set header view name
	public void SetHeaderViewName (string viewName) {
		header.SetViewName (viewName);
	}
	
	// Hide header rank pop up
	public void HideRankPopUp () {
		header.CloseRankPopUp ();
	}
	
	// Stack showing loading screen condition name
	// If not show loading screen, show loading screen
	public void StackLoadingScreenCondition (string conditionName) {
		loadingScreenCondition = AddCondition (loadingScreenCondition, conditionName);

		if (!isShowLoadingScreen) {
			isShowLoadingScreen = true;
			ShowLoadingScreen ();
		}
	}

	// Clear showing loading screen condition name
	// Then, if condition name list become empty, hide loading screen
	public void ClearLoadingScreenCondition (string conditionName) {
		loadingScreenCondition = RemoveCondition (loadingScreenCondition, conditionName);
				
		if (isShowLoadingScreen && loadingScreenCondition.Count == 0) {
			isShowLoadingScreen = false;
			HideLoadingScreen ();
		}
	}
	
	// Show loading screen
	private void ShowLoadingScreen () {
		if (loadingScreen.activeInHierarchy) return;

		loadingScreen.SetActive (true);
		loadingScreen.GetComponent<LoadingAnimation> ().AnimeReady = true;
	}

	// Hide loading screen
	private void HideLoadingScreen () {
		if (!loadingScreen.activeInHierarchy) return;

		loadingScreen.SetActive (false);
	}

	// Show loading screen and hide it after duration
	public void ShowAndHideLoadingScreen (int duration) {
		var conditionName = "showAndHide";
		StackLoadingScreenCondition (conditionName);
		StartCoroutine (SetSchedule (duration, () => {
			ClearLoadingScreenCondition (conditionName);
		}));
	}
	
	// Stack showing loading animation condition name
	// If not show loading animation, show loading animation
	public void StackLoadingAnimationCondition (string conditionName) {
		loadingAnimationCondition = AddCondition (loadingAnimationCondition, conditionName);

		if (!isShowLoadingAnimation) {
			isShowLoadingAnimation = true;
			ShowLoadingAnimation ();
		}
	}
	
	// Clear showing Loading Animation condition name
	// Then, if condition name list become empty, hide loading screen
	public void ClearLoadingAnimationCondition (string conditionName) {
		loadingAnimationCondition = RemoveCondition (loadingAnimationCondition, conditionName);

		if (isShowLoadingAnimation && loadingAnimationCondition.Count == 0) {
			isShowLoadingAnimation = false;
			HideLoadingAnimation ();
		}
	}

	// Do not call except io.error.network and LoadNextSceneAsync
	public void ClearLoadingAnimationConditionAll () {
		loadingAnimationCondition = new Dictionary<string, int> ();
		isShowLoadingAnimation = false;
		HideLoadingAnimation ();
	}

	// Show loading animation
	private void ShowLoadingAnimation () {
		if (Application.loadedLevelName == "GameView") return;
		loadingAnimation.SetActive (true);
	}
	
	// Hide loading animation
	private void HideLoadingAnimation () {
		if (Application.loadedLevelName == "GameView") return;
		loadingAnimation.SetActive (false);
	}
		
	// Setup live2D operator
	public void SetLive2DOperator () {
		live2D.transform.Find ("Live2D Canvas/Panel/Live2DModel").GetComponent<Live2DController> ().SetUp (audioMixer);
	}

	// Show live2D first message for load level
	public void ShowLive2DFirstMessage () {
		live2D.transform.Find ("Live2D Canvas/Panel/Live2DModel").GetComponent<Live2DController> ().ResetLoadLevelFirst ();
	}
	
	// Show live2D operator
	public void ShowLive2DOperator () {
		if (live2D != null) {
			live2D.SetActive (true);
			aspectRatioFixer.Revise ();
		}
	}
	
	// Hide live2D operator
	public void HideLive2DOperator () {
		if (live2D != null) {
			live2D.SetActive (false);
		}
	}
	
	// Load level async with loading screen
	public void LoadNextScene (string sceneName, bool force = false) {
		if (!force) {
			if (Application.loadedLevelName == sceneName) return;
			if (nextSceneName == sceneName) return;
		}
		nextSceneName = sceneName;
		new Task (LoadNextSceneAsync (sceneName));
	}

	// Set BGM and SE volumes
	public void SetVolume () {
		audioMixer.SetFloat ("MasterVolume", (GameSettings.VolumeMaster * 80f) - 80f);
		audioMixer.SetFloat ("BGMVolume", (GameSettings.VolumeBGM * volumeRevise * 80f) - 80f);
		audioMixer.SetFloat ("SEVolume", (GameSettings.VolumeSE * volumeRevise * 80f) - 80f);
		audioMixer.SetFloat ("VoiceVolume", (GameSettings.VolumeSE * 80f) - 80f);
		isSetVolume = true;
	}

	// Play sound effect
	public void PlaySE (SoundEffect seId) {
		if (!isSetVolume) return;
		soundEffectArray[(int)seId].Play ();
	}

	// Stop sound effect
	public void StopSE (SoundEffect seId) {
		soundEffectArray[(int)seId].Stop ();
	}

	// Play tap sound effect
	public void PlaySETap () {
		PlaySE (SoundEffect.Tap);
	}
	
	// Play OK sound effect
	public void PlaySEOK () {
		PlaySE (SoundEffect.OK);
	}
	
	// Play cancel sound effect
	public void PlaySECancel () {
		PlaySE (SoundEffect.Cancel);
	}
	
	// Play BGM
	public IEnumerator PlayBGM (BackgroundMusic bgmId) {
		if (bgmId.Equals (currentPlayBGM)) yield break;

		StopBGMAll ();
		while (!isSetVolume) {
			yield return null;
		}
		backgroundMusicArray[(int)bgmId].Play ();
		currentPlayBGM = bgmId;
	}
	
	// Stop BGM
	public void StopBGM (BackgroundMusic bgmId) {
		backgroundMusicArray [(int)bgmId].Stop ();
	}

	// Stop all BGMs
	public void StopBGMAll () {
		foreach (AudioSource backgroundMusic in backgroundMusicArray) {
			if (backgroundMusic != null) {
				backgroundMusic.Stop ();
			}
		}
	}

	// Play opening BGM
	public void PlayBGMOpening () {
		new Task (PlayBGM (BackgroundMusic.Opening));
	}

	// Play home BGM
	public void PlayBGMHome () {
		new Task (PlayBGM (BackgroundMusic.Home));
	}

	// Play in game BGM
	public void PlayBGMInGame () {
		switch (GameSettings.LastQueueType) {
		case "anyRank":
			new Task (PlayBGM (BackgroundMusic.InGame2));
			break;
		case "highRank":
			new Task (PlayBGM (BackgroundMusic.InGame3));
			break;
		default:
			new Task (PlayBGM (BackgroundMusic.InGame));
			break;
		}
	}

	// Open message window
	public void OpenMessageWindow (string subject, string message, string buttonText, Action action) {
		if (!popUpWindow.isClosed || !popUpWindowL.isClosed) {
			StackPopUpWindow (PopUpType.Message, subject, message, buttonText, action);
		} else {
			if (isWebViewOpened) CloseWebView ();
			PopUpWindowManager.Open (popUpWindow, subject, message, buttonText, action);
		}
	}

	// Open dialog window
	public void OpenDialogWindow (string subject, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction) {
		if (!popUpWindow.isClosed || !popUpWindowL.isClosed) {
			StackPopUpWindow (PopUpType.Dialog, subject, message, positiveButtonText, positiveAction, negativeButtonText, negativeAction);
		} else {
			if (isWebViewOpened) CloseWebView ();
			PopUpWindowManager.Open (popUpWindow, subject, message, positiveButtonText, positiveAction, negativeButtonText, negativeAction);
		}
	}
	
	// Open large dialog window
	public void OpenDialogWindowL (string subject, string message, string positiveButtonText, Action positiveAction, string negativeButtonText, Action negativeAction) {
		if (!popUpWindow.isClosed || !popUpWindowL.isClosed) {
			StackPopUpWindow (PopUpType.DialogLarge, subject, message, positiveButtonText, positiveAction, negativeButtonText, negativeAction);
		} else {
			if (isWebViewOpened) CloseWebView ();
			PopUpWindowManager.Open (popUpWindowL, subject, message, positiveButtonText, positiveAction, negativeButtonText, negativeAction);
		}
	}

	// Initialize customized window
	public PopUpWindow InitCustomWindow (string prefabName) {
		return PopUpWindowManager.Init ("Common/Prefabs/" + prefabName, transform.Find ("Panel"));
	}
	
	// Open customized window (initialize first)
	public PopUpWindow OpenCustomWindow (PopUpWindow customWindow, string buttonText1, Action action1, string buttonText2 = "", Action action2 = default(Action), bool isDestroy = true) {
		if (isWebViewOpened) CloseWebView ();
		if (action2 == default(Action)) {
			return PopUpWindowManager.Open (customWindow, buttonText1, action1, isDestroy);
		} else {
			return PopUpWindowManager.Open (customWindow, buttonText1, action1, buttonText2, action2, isDestroy);
		}
	}

	// Open customized window
	public PopUpWindow OpenCustomWindow (string prefabName, string buttonText1, Action action1, string buttonText2 = "", Action action2 = default(Action), bool isDestroy = true) {
		if (isWebViewOpened) CloseWebView ();
		if (action2 == default(Action)) {
			return PopUpWindowManager.Open ("Common/Prefabs/" + prefabName, transform.Find ("Panel"), buttonText1, action1, isDestroy);
		} else {
			return PopUpWindowManager.Open ("Common/Prefabs/" + prefabName, transform.Find ("Panel"), buttonText1, action1, buttonText2, action2, isDestroy);
		}
	}

	// Stuck pop up window data
	public void StackPopUpWindow (PopUpType popUpType, string subject, string message, string buttonText1, Action action1, string buttonText2 = "", Action action2 = default(Action)) {
		popUpStacks.Add (new PopUpStack(popUpType, subject, message, buttonText1, action1, buttonText2, action2));
	}

	// Open pop up window sequentially
	public void OpenStackedPopUpWindow (Action callback) {
		if (isWebViewOpened) CloseWebView ();
		new Task (OpenStackedPopUpWindowSequentially (callback));
	}

	// Check pop up window is stacked, then if stacked, open stacked pop up window
	public void CheckStackedPopUpWindow () {
		if (popUpStacks.Count > 0) {
			OpenStackedPopUpWindow (() => {});
		}
	}

	// Open pop up window when exception occured.
	public void OpenExceptionPopUpWindow () {
		if (Application.loadedLevelName == "TitleView") {
			OpenMessageWindow ("エラー", "予期せぬエラーが発生しました", "リロード", () => {LoadNextScene ("TitleView", true);});
		} else {
			OpenMessageWindow ("エラー", "予期せぬエラーが発生しました", "タイトルへ", () => {LoadNextScene ("TitleView");});
		}
	}

	// Open webView
	public void OpenWebView (string url, bool playSE = true) {
		if (!popUpWindow.isClosed || !popUpWindowL.isClosed) return;
		if (isWebViewOpened) return;
		if (playSE) {
			PlaySETap ();
		}
		isWebViewOpened = true;
		closedCameras = Camera.allCameras;
		foreach (var camera in Camera.allCameras) {
			if (camera.Equals (Camera.main) || camera.name == "Background Camera" || camera.name == "Steward's Camera") continue;
			camera.gameObject.SetActive (false);
		}
		webViewWindow.SetActive (true);
		webViewObject.Init ();
		webViewObject.LoadURL (url);
		webViewObject.SetMargins (0, aspectRatioFixer.webViewMarginTop , 0 , aspectRatioFixer.webViewMarginBottom);
		webViewObject.SetVisibility(true);
	}

	// Close webView
	public void CloseWebView (bool playSE = true) {
		if (playSE) {
			PlaySECancel ();
		}
		webViewObject.SetVisibility (false);
		webViewWindow.SetActive (false);
		foreach (var camera in closedCameras) {
			if (camera.Equals (Camera.main) || camera.name == "Background Camera" || camera.name == "Steward's Camera") continue;
			camera.gameObject.SetActive (true);
		}
		isWebViewOpened = false;
	}

	// Open external web browser
	public void OpenBrowser (string url) {
		PlaySETap ();
		Application.OpenURL (url);
	}

	// Toggle all buttons enabled/disabled
	public void SwitchIntaractableAllButtons (bool isEnable) {
		Transform mainCanvas = GameObject.Find ("/Main Canvas").transform;
		if (mainCanvas != null) {
			var allChildren = mainCanvas.GetComponentsInChildren<Transform>(true).Where(c => c != mainCanvas).Select(c => c.gameObject).ToArray();
			foreach (var child in allChildren) {
				if (child.GetComponent<Button>() != false) {
					child.GetComponent<Button>().interactable = isEnable;
				}
			}
		}
	}
	
	private IEnumerator OpenStackedPopUpWindowSequentially (Action callback) {
		popUpStacks.Reverse ();
		PopUpWindow prevPopUpWindow;
		for (int i = popUpStacks.Count -1; i >= 0; i--) {
			switch (popUpStacks[i].popUpType) {
			case PopUpType.Message:
				prevPopUpWindow = popUpWindow;
				OpenMessageWindow (popUpStacks[i].subject, popUpStacks[i].message, popUpStacks[i].buttonText1, popUpStacks[i].action1);
				break;
			case PopUpType.Dialog:
				prevPopUpWindow = popUpWindow;
				OpenDialogWindow (popUpStacks[i].subject, popUpStacks[i].message, popUpStacks[i].buttonText1, popUpStacks[i].action1, popUpStacks[i].buttonText2, popUpStacks[i].action2);
				break;
			case PopUpType.DialogLarge:
				prevPopUpWindow = popUpWindowL;
				OpenDialogWindowL (popUpStacks[i].subject, popUpStacks[i].message, popUpStacks[i].buttonText1, popUpStacks[i].action1, popUpStacks[i].buttonText2, popUpStacks[i].action2);
				break;
			default:
				prevPopUpWindow = popUpWindow;
				break;
			}

			popUpStacks.Remove(popUpStacks[i]);
			while(!prevPopUpWindow.isClosed) {
				yield return null;
			}
		}

		callback ();
	}
		
	// Discriminate error code in response and display pop up window
	public bool ResponseHasErrorCode (JToken result, Action onErrorAction = default(Action)) {
		if (onErrorAction == default(Action)) onErrorAction = () => {};
		if (result is JObject) {
			if (result ["error"] != null && !String.IsNullOrEmpty (result ["error"].ToString ())) {
				ErrorCode errorCode = errorCodes.rows [result ["error"].ToString ()];
				if (errorCode.Status == "0") {
					onErrorAction += () => {LoadNextScene ("TitleView");};
				}
				OpenMessageWindow ("エラー", "[" + result ["error"].ToString () + "]\n\n" +
                   errorCode.Description.Replace ("<br />", "\n"), "閉じる", onErrorAction);
				return true;
			}
		}
		return false;
	}

	// Catch Intent URI throw from native application
	public void SetIntentUri (string intent) {
		Uri uri = new Uri(intent);
		bnidManager.ExecIntent (uri);
	}
	
	// Execute after some delay time
	public void SetNewTask (IEnumerator coroutine) {
		StackLoadingAnimationCondition ("newTask");
		Task task = new Task (coroutine);
		task.Finished += (bool manual) => {
			if (!manual) {
				ClearLoadingAnimationCondition ("newTask");
			}
		};
	}

	// Execute after some delay time
	public IEnumerator SetSchedule (int delayTime, Action action) {
		yield return new WaitForSeconds(delayTime);

		action ();
	}

	// Version check and display window when newer version releases
	public void CheckVersion () {
#if !UNITY_EDITOR
		SetNewTask (user.checkVersion ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error checkVersion:");
				OpenExceptionPopUpWindow ();
				return;
			}
			if (ResponseHasErrorCode (result)) return;

			if (bool.Parse (result["isVersion"].ToString ())) {
				OpenMessageWindow ("エラー", "新しいバージョンのアプリがあります\n" +
				                   "ストアからアップデートを行ってください\n\n" +
				                   "アプリの更新ができない場合は\n" +
				                   "しばらく時間をおいてからお試しください", "ストアへ", () => {OpenBrowser (GameSettings.StoreURI);CheckVersion();});
			}
		}));
#endif
	}
	
	// Execute load level async with loading screen
	private IEnumerator LoadNextSceneAsync (string sceneName) {
		// Clear remained loading animation condition
		ClearLoadingAnimationConditionAll ();
		StackLoadingScreenCondition ("loadNextScene");
		AsyncOperation asyncOperation = Application.LoadLevelAsync (sceneName);
		asyncOperation.allowSceneActivation = false;
		if (asyncOperation.progress < 0.9f) yield return new WaitForEndOfFrame ();
		asyncOperation.allowSceneActivation = true;
		StartCoroutine (SetSchedule (2, () => {
			ClearLoadingScreenCondition ("loadNextScene");
		}));
	}

	public IEnumerator DownloadBGM (Action cb) {
		yield return new Task (DowloadBGMImpl (1, cb));
	}

	private IEnumerator DowloadBGMImpl (int index, Action cb) {
		var list = new Dictionary<int, string> () {{1, "bgm_home"}, {2, "bgm_ingame"}, {3, "bgm_ingame2"}, {4, "bgm_ingame3"}};
		if (list.ContainsKey (index)) {
			if (backgroundMusicArray [index].clip == null) {
				yield return new Task (loadAssetBundle.DownloadBGM (list [index], (AudioClip ac) => {
					backgroundMusicArray [index].clip = ac;
					new Task (DowloadBGMImpl (++index, cb));
				}));
			} else {
				yield return new Task (DowloadBGMImpl (++index, cb));
			}
		} else {
			cb ();
		}
	}

	private Dictionary<string, int> AddCondition (Dictionary<string, int> conditionDict, string conditionName) {
		if (!conditionDict.ContainsKey (conditionName)) {
			conditionDict.Add (conditionName, 0);
		}
		conditionDict [conditionName]++;

		return conditionDict;
	}

	private Dictionary<string, int> RemoveCondition (Dictionary<string, int> conditionDict, string conditionName) {
		if (conditionDict.ContainsKey (conditionName)) {
			if (conditionDict [conditionName] > 0) {
				conditionDict [conditionName]--;
			}
			if (conditionDict[conditionName] == 0) {
				conditionDict.Remove (conditionName);
			}
		}

		return conditionDict;
	}
}
