using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using DG.Tweening;

using Newtonsoft.Json.Linq;

using Prime31;

public class TitleView : MonoBehaviour {
	public GameObject background, white;
	public GameObject startButton;
	public GameObject titleAnimation;
	public GameObject lodingAnimation;
	public Text versionText;
	public Sprite continueButtton;

	public bool firstStartFlg;

	public GameObject[] alphaButton = new GameObject[6];

	private bool isShowTitle = false;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private BNID bnid { get { return BNID.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("TitleView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private BNIDManager bnidManager;
	private Steward steward;
	
	//
	public PopupManager popupManager;

	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		bnidManager = new BNIDManager ();

		versionText.text = "Ver." + BundleVersion.Get ();

		startButton.GetComponent<Image> ().DOFade (1f, 1f).SetLoops (-1, LoopType.Yoyo);

		// Instantiate TaskManager for main thread
		TaskManagerMainThread.Instantiate();

		new Task (TweenController ());

		for (int i = 0; i < 4; i++) {
			alphaButton [i].SetActive (false);
		}
		if (GameSettings.IsMigrated == 1) {
			startButton.SetActive (false);
			bnidManager.OpenMigratedWindow ();
		} else {
			//
			Async.series (new List<Action<Action<Exception>>> {
				// Set JSONRPC endpoint URL for mage usercommands and get related userId
				(Action<Exception> callback) => {
					if (!Application.isEditor && !Debug.isDebugBuild) {
						GameSettings.Environment = "PRODUCTION";
						GameSettings.SetMageEndpoint();
						GameSettings.SetMageLoggerConfig();
						logger.debug("Using default endpoint");

						callback(null);
						return;
					}
					
					if (GameSettings.Environment != "NONE") {
						GameSettings.SetMageEndpoint();
						GameSettings.SetMageLoggerConfig();
						logger.debug("Environment already choosen");

						callback(null);
						return;
					}
					
					popupManager.open("DebugServerSelect", (object result) => {
						GameSettings.Environment = (string)result;
						GameSettings.SetMageEndpoint();
						GameSettings.SetMageLoggerConfig();
						logger.debug("Set endpoint for environment: " + (string)result);

						callback(null);
					});
				},

				// Setup mage internals
				(Action<Exception> callback) => {
					new Task(mage.SetupTask(callback));
				},

				// Setup error handlers
				(Action<Exception> callback) => {
					//
					mage.eventManager.on("session.unset", (object sender, JToken info) => {
						TaskManagerMainThread.Queue(OnAuthError());
					});

					//
					mage.eventManager.on("io.error.maintenance", (object sender, JToken info) => {
						TaskManagerMainThread.Queue(OnMaintenanceError());
					});

					//
					mage.eventManager.on("io.error.network", (object sender, JToken info) => {
						steward.OpenDialogWindow ("エラー", "通信エラーが発生しました", "リトライ", () => {
							steward.ClearLoadingAnimationConditionAll ();
							steward.StackLoadingAnimationCondition ("errorRetry");
							new Timer((object state) => {
								mage.commandCenter.Resend();
								steward.ClearLoadingAnimationCondition ("errorRetry");
							}, null, 2000, Timeout.Infinite);
						}, (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {
							steward.ClearLoadingAnimationConditionAll ();
							steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);
						});
					});

					callback(null);
				},
				
				// Setup application mage modules
				(Action<Exception> callback) => {
					new Task(mage.SetupModulesTask(new List<string> () {
						"AppVersions",
						"BNID",
						"Card",
						"Cpu",
						"Deck",
						"Game",
						"Master",
						"MatchMaking",
						"Purchase",
						"User"
					}, callback));
				},
				
				// Attempt to log the user in
				(Action<Exception> callback) => {
					new Task(user.loginOrRegister(callback));
				},

				// Gat user data(s)
				(Action<Exception> callback) => {
					new Task(user.GetUserData(callback));
				},

				// Fix CPU User records if needed
				(Action<Exception> callback) => {
					new Task(game.FixCpuUserRecords(callback));
				},

				// Gat game records
				(Action<Exception> callback) => {
					new Task(game.GetUserRecords(callback));
				},
				
				// Gat game class data
				(Action<Exception> callback) => {
					new Task(game.GetUserClasses(callback));
				},

				// Init localytics and call logged in
				(Action<Exception> callback) => {
					try {

						// This current code assumes that we are not deploying localytics
						// on any other platform

						#if UNITY_ANDROID
						Localytics.init();
						#endif

						#if UNITY_IOS
						Localytics.init("09c18c6dfe15f802d9ce5b8-c3ca0afc-e66a-11e5-5711-0042876ec363");
						#endif

						#if UNITY_ANDROID || UNITY_IOS
						Localytics.startSession();
						Localytics.setCustomerId(user.tUser["userId"].ToString());
						Localytics.tagEvent("Logged In");
						#endif
					} catch (Exception error) {
						// Log failure
						logger.debug("localytics failed to start:" + error.Message);

						// DO NOT THROW ON LOCALYTICS ERRORS
						callback(null);
					}

					callback(null);
				}
			}, (Exception error) => {
				if (error != null) {
					logger.data (error).critical ("Failed to initialize applicaiton: ");
					
					if (Application.isEditor || Debug.isDebugBuild) {
						// Show debug popup if we are in the editor
						popupManager.open ("DebugInitFailed");
					}
					
					return;
				}
				steward.CheckVersion();

				loadAssetBundle.SetAssetBundleURI(GameSettings.AssetBundleURI);

				if (GameSettings.TutorialState == GameSettings.TutorialStates.Done || GameSettings.TutorialState == GameSettings.TutorialStates.Encore) {
					steward.SetLive2DOperator ();
					steward.HideLive2DOperator ();
				}

				loadAssetBundle.GetVersionFromMaster(() => {
					new Task (steward.DownloadBGM (() => {
						new Task (loadAssetBundle.DownloadLive2DOperator (("Live2D" + user.tUser ["operatorId"].ToString ().Substring (8)).ToLower (), transform, (Live2DElements elements) => {
							new Task (ShowButtons ());
					
							if (GameSettings.TutorialState == GameSettings.TutorialStates.FirstInitialRegistration) {
								firstStartFlg = true;
							} else {
								firstStartFlg = false;
							}
							
							if (firstStartFlg) {
								OpenFirstVisitWindow ();
							}

							steward.ErrorCodes = new ErrorCodes (master.staticData["errorCodes"]);
						}));
					}));
				});
			});
		}
	}

	void Update () {
		if (isShowTitle == false) {
			if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) ShowTitle ();
		}
	}

	public void PushStart () {
		steward.PlaySEOK ();
		steward.LoadNextScene ("LoadingView");
	}

	public void OpenInheritWindow () {
		steward.PlaySETap ();
		if (firstStartFlg) {
			bnidManager.OpenContinueWindow ();
		} else {
			bnidManager.OpenInheritWindow ();
		}
	}

	public void OpenCacheClearWindow () {
		steward.PlaySETap ();
		steward.OpenDialogWindow ("キャッシュクリア", "キャッシュを全てクリアします\n実行しますか？", "はい", () => {ExecCacheClear ();}, "いいえ", () => {});
	}
	
	public void ExecCacheClear () {
		Caching.CleanCache ();
		steward.OpenMessageWindow ("キャッシュクリア", "キャッシュをクリアしました", "閉じる", () => {});
	}
	
	public void OpenNewsWebView () {
		steward.OpenWebView ("http://www.gundam-cw.com/app_index.php");
	}

	public void OpenOfficialWeb () {
		steward.OpenBrowser ("http://www.gundam-cw.com/");
	}
	
	private void OpenFirstVisitWindow () {
		steward.PlaySETap ();
		steward.OpenDialogWindow ("ゲームスタート", "データを引き継いでゲームをプレイする場合は\n" +
		                          "「データ引継ぎ」を選択してください\n" +
		                          "「新しく始める」を選択した後に、\n" +
		                          "以前のゲームを引き継ぐには\n" +
		                          "アプリを再インストールする必要があります", "データ引継", () => {bnidManager.OpenContinueWindow ();}, "新しく始める", () => {});
	}

	private IEnumerator TweenController () {
		white.GetComponent<Image>().DOFade (0, 1f);
		yield return new WaitForSeconds (0.4f);
		
		alphaButton[5].GetComponent<Image>().DOFade(1f, 0.5f);
		yield return new WaitForSeconds (0.4f);
		
		alphaButton[4].GetComponent<Image>().DOFade(1f, 1.5f);
		yield return new WaitForSeconds (0.6f);
		
		titleAnimation.SetActive (true);
		white.SetActive (false);
		yield return new WaitForSeconds (0.8f);
		
		ShowTitle ();
	}

	private void ShowTitle () {
		if (isShowTitle) return;
		isShowTitle = true;
		white.SetActive (false);
		steward.StackLoadingAnimationCondition ("setUp");
	}

	private IEnumerator ShowButtons () {
		if (!isShowTitle) {
			ShowTitle ();
			yield return new WaitForEndOfFrame ();
		}
		steward.ClearLoadingAnimationCondition ("setUp");
		startButton.SetActive (true);
		if (GameSettings.TutorialState == GameSettings.TutorialStates.Done || GameSettings.TutorialState == GameSettings.TutorialStates.Encore) {
			for (int i = 0; i < 4; i++) {
				alphaButton [i].SetActive (true);
			}
			for (int i = 0; i < 6; i++) {
				alphaButton [i].GetComponent<Image> ().DOFade (1f, 0.5f);
			}
		}

		yield break;
	}

	IEnumerator OnAuthError() {
		steward.OpenMessageWindow ("エラー", "セッションがタイムアウトしました", (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {
			steward.ClearLoadingAnimationConditionAll ();
			steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);});
		yield break;
	}
	
	IEnumerator OnMaintenanceError() {
		steward.OpenMessageWindow ("エラー", "メンテナンス中です", (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {
			steward.ClearLoadingAnimationConditionAll ();
			steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);});
		yield break;
	}
}