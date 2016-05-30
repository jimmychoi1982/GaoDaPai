using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class CardRegistrationView : MonoBehaviour {
	public InputField registerInputField;
	public InputField releaseInputField;
	public GameObject registerCameraButton;
	public GameObject registerButtonArea;

	public GameObject cardPrefab;
	public GameObject historyRowPrefab;

	private List<string> serialCodeList = new List<string>();
	private DropDownList serialDropDownList;
	private GameObject serialDropDown;

	private Dictionary<string, string> cardNames = new Dictionary<string, string> ();

	List<string> serialCodes;

	private bool isSending = false;

	private int operateMode;
	private enum OperateMode {
		Register = 0,
		Release,
	}

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Purchase purchase { get { return Purchase.Instance; }}
	private BNID bnid { get { return BNID.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("CardRegistrationView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private CardMasterManager cardMasterManager = new CardMasterManager();

	private Steward steward;

	#if UNITY_ANDROID
	static AndroidJavaClass _cameraPermissionsClass;
	static AndroidJavaObject _cameraPermissionsInstance { get { return _cameraPermissionsClass.GetStatic<AndroidJavaObject>("instance"); } }
	#endif
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		EasyCodeScanner.Initialize();
		
		cardNames = cardMasterManager.GetCardIdNamePairs();

		//Register on Actions
		EasyCodeScanner.OnScannerMessage += onScannerMessage;
		EasyCodeScanner.OnScannerEvent += onScannerEvent;
		EasyCodeScanner.OnDecoderMessage += onDecoderMessage;

		#if UNITY_ANDROID
		// Load the native Android plugin that handles the new permissions process for Android 6+
		_cameraPermissionsClass = new AndroidJavaClass("com.wizcorp.eahkouen.cameraplugin.CameraPermissions");
		// In the start function, we need to pass the gameObject name that contains this instance
		// It is used for calling a function from the native plugin (CameraPlugin) to Unity
		_cameraPermissionsClass.CallStatic("start", "Main Canvas");
		#endif
	}
	
	void OnDestroy() {
		//Unregister
		EasyCodeScanner.OnScannerMessage -= onScannerMessage;
		EasyCodeScanner.OnScannerEvent -= onScannerEvent;
		EasyCodeScanner.OnDecoderMessage -= onDecoderMessage;
	}
	
	public void SendRegister() {
		if (String.IsNullOrEmpty (registerInputField.text) && serialCodeList.Count == 0) return;

		if (isSending) return;
		isSending = true;

		steward.PlaySETap ();

		serialCodeList.Add (registerInputField.text);
		string[] serialCodeArray = serialCodeList.ToArray ();
		List<string> registerCardIds = new List<string>();
		List<string> registerSleeveIds = new List<string>();
		string registerOperatorId = null;
		string registerBundleCode = null;
		int recoveryItemCount = 0;
		bool isLoadTitle = false;
		Action positiveAction = () => {
			steward.SetNewTask (purchase.registerSerial (serialCodeArray, (Exception e, JToken result) => {
				isSending = false;
				registerInputField.text = "";
				serialCodeList = new List<string>();
				registerCameraButton.SetActive(true);
				Destroy(serialDropDown);
				if (e != null) {
					logger.data (e).error ("Error registerSerial:");
					steward.OpenExceptionPopUpWindow ();
					return;
				}

				if (steward.ResponseHasErrorCode(result)) return;

				foreach (var token in result as JArray) {
					var obj = token as JObject;
					if (bool.Parse(obj["result"].ToString())) {
						if (obj["sleeveId"] != null && obj["sleeveId"].ToString () != "" && obj["sleeveId"].ToString ().ToLower () != "false") {
							registerSleeveIds.Add(obj["sleeveId"].ToString());
						} else if (obj["cardId"] != null && obj["cardId"].ToString () != "" && obj["cardId"].ToString ().ToLower () != "false") {
							registerCardIds.Add(obj["cardId"].ToString());
						}
						if (obj["isRecoveryItem"] != null && bool.Parse (obj["isRecoveryItem"].ToString())) {
							recoveryItemCount++;
						}
						if (obj["operatorId"] != null && obj["operatorId"].ToString () != "" && obj["operatorId"].ToString ().ToLower () != "false") {
							registerOperatorId = obj["operatorId"].ToString ();
						}
						if (obj["bundleCode"] != null && obj["bundleCode"].ToString () != "" && obj["bundleCode"].ToString ().ToLower () != "false") {
							registerBundleCode = obj["bundleCode"].ToString ();
						}
					} else {
						if (obj["error"] != null) {
							if (obj["error"].ToString () == "") {
								steward.StackPopUpWindow (Steward.PopUpType.Message, "エラー", "最大登録数を超過しているカード情報は登録できません\n\n" +
								                          "[" + obj["serial"].ToString () + "]", "閉じる", () => {});
								if (obj["isRecoveryItem"] != null && bool.Parse (obj["isRecoveryItem"].ToString())) {
									recoveryItemCount++;
								}
							} else {
								ErrorCode errorCode = steward.ErrorCodes.rows[obj["error"].ToString ()];
								steward.StackPopUpWindow (Steward.PopUpType.Message, "エラー", "[" + obj["error"].ToString () + "]\n\n" +
								                          errorCode.Description.Replace("<br />", "\n") + "\n" + (obj["serial"] != null ? obj["serial"].ToString () : ""), "閉じる", () => {});
								if (errorCode.Status == "0") isLoadTitle = true;
							}
						}
					}
				}
					
				if (registerBundleCode != null) {
					steward.OpenStackedPopUpWindow (() => {
						steward.ReloadHeader ();
						var bundleRegistrationResultWindow = steward.InitCustomWindow("BundleRegistrationResultWindow");
						var bundleName = "";
						if (master.staticData["bundle"][registerBundleCode]["description"] != null) {
							bundleName = master.staticData["bundle"][registerBundleCode]["description"].ToString();
						} else {
							bundleName = registerBundleCode;
						}
						GameObject bundleImage = bundleRegistrationResultWindow.GetObject("ImageArea/BundleImage");
						bundleRegistrationResultWindow.SetSubject ("登録成功");
						bundleRegistrationResultWindow.SetText ("SelectionName", bundleName);
						loadAssetBundle.SetBundleImage(registerBundleCode, bundleImage.gameObject);

						steward.OpenCustomWindow (bundleRegistrationResultWindow, "OK", () => {
							steward.StackPopUpWindow (Steward.PopUpType.Message, "確認", "カードバンドル特典を受け取りました", "OK", () => {});
							steward.OpenStackedPopUpWindow (() => {if (isLoadTitle) steward.LoadNextScene ("TitleView");});
						});
					});
					return;
				} else if ((registerCardIds.Count + registerSleeveIds.Count) == 0) {
					if (recoveryItemCount > 0) {
						steward.ReloadHeader ();
						steward.StackPopUpWindow (Steward.PopUpType.Message, "確認", "補給物資を" + recoveryItemCount.ToString() + "個入手しました", "OK", () => {});
					}
					steward.OpenStackedPopUpWindow (() => {steward.OpenMessageWindow ("エラー", "カード情報の登録に失敗しました", (isLoadTitle ? "タイトルへ" : "閉じる"), () => {if (isLoadTitle) steward.LoadNextScene ("TitleView");});});
					return;
				}

				steward.OpenStackedPopUpWindow (() => {
					steward.ReloadHeader ();
					var resultWindow = steward.InitCustomWindow("SelectWindow");
					resultWindow.SetSubject ("登録成功");
					resultWindow.SetText ("SelectionName", "");
					resultWindow.GetObject ("SelectionNameWindow").SetActive (false);

					List_Instance listInstance = resultWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ();
					listInstance.Create_Object = cardPrefab;
					listInstance.Create_Number = registerCardIds.Count + registerSleeveIds.Count;
					listInstance.CreateInstance ();

					var cardObjs = listInstance.GetCreateObjects ();
					int i = 0;
					foreach (var registerCardId in registerCardIds) {
						loadAssetBundle.SetCardImage(registerCardId, (int)LoadAssetBundle.DisplayType.Card, cardObjs[i].gameObject);
						i++;
					}
					foreach (var registerSleeveId in registerSleeveIds) {
						loadAssetBundle.SetSleeveImage(registerSleeveId, (int)LoadAssetBundle.DisplayType.Card, cardObjs[i].gameObject);
						i++;
					}
					resultWindow.GetObject ("ListArea/List/ListParent").GetComponent<Slide_Action_Ver2> ().Rewind ();

					steward.OpenCustomWindow (resultWindow, "OK", () => {
						listInstance.Reset ();
						if (registerOperatorId != null) {
							steward.StackPopUpWindow (Steward.PopUpType.Message, "確認", master.staticData["operators"] [registerOperatorId] ["name"].ToString () +  "\nを追加しました", "OK", () => {});
						}
						if (recoveryItemCount > 0) {
							steward.StackPopUpWindow (Steward.PopUpType.Message, "確認", recoveryItemCount.ToString() + "枚のカード情報の\n補給物資を入手しました", "OK", () => {});
						}
						if (registerCardIds.Count - recoveryItemCount > 0) {
							steward.StackPopUpWindow (Steward.PopUpType.Message, "確認", "登録した" + (registerCardIds.Count - recoveryItemCount).ToString() + "枚のカード情報の【補給物資】は、\nすでに受け取っています", "OK", () => {});
						}

						steward.OpenStackedPopUpWindow (() => {if (isLoadTitle) steward.LoadNextScene ("TitleView");});
					});
				});
			}));
		};

		steward.OpenDialogWindow ("確認", "カード情報を登録します\n" +
			"よろしいですか?\n\n" +
			"※登録枚数によって、登録する時間が長い場合があります\n" +
			"※通信環境の良い場所でお試しください", "はい", positiveAction, "いいえ", () => {
				isSending = false;
				registerInputField.text = "";
				serialCodeList = new List<string>();
				registerCameraButton.SetActive(true);
				Destroy(serialDropDown);
			});
	}
		
	public void SendRelease() {
		if (String.IsNullOrEmpty (releaseInputField.text)) return;
		if (isSending) return;
		isSending = true;
		
		steward.PlaySETap ();

		var releaseSuccessCount = 0;
		bool isLoadTitle = false;
		Action positiveAction = () => {
			steward.SetNewTask (purchase.releaseSerial (releaseInputField.text, (Exception e, JToken result) => {
				isSending = false;
				releaseInputField.text = "";
				if (e != null) {
					logger.data (e).error ("Error releaseSerial:");
					steward.OpenExceptionPopUpWindow ();
					return;
				}

				if (steward.ResponseHasErrorCode(result)) return;

				foreach (var token in result as JArray) {
					var obj = token as JObject;
					if (bool.Parse(obj["result"].ToString())) {
						releaseSuccessCount++;
					} else {
						if (obj["error"] != null) {
							ErrorCode errorCode = steward.ErrorCodes.rows[obj["error"].ToString ()];
							steward.StackPopUpWindow (Steward.PopUpType.Message, "エラー", "[" + obj["error"].ToString () + "]\n\n" +
							                           errorCode.Description.Replace("<br />", "\n") + "\n" + obj["serial"].ToString (), "閉じる", () => {});
							if (errorCode.Status == "0") isLoadTitle = true;
						}
					}
				}
				if (releaseSuccessCount == 0) {
					steward.OpenStackedPopUpWindow (() => {steward.OpenMessageWindow ("エラー", "カード情報の解除に失敗しました", (isLoadTitle ? "タイトルへ" : "閉じる"), () => {if (isLoadTitle) steward.LoadNextScene ("TitleView");});});
					return;
				}

				steward.OpenStackedPopUpWindow (() => {steward.OpenMessageWindow ("確認", "カード情報の解除に成功しました", (isLoadTitle ? "タイトルへ" : "閉じる"), () => {if (isLoadTitle) steward.LoadNextScene ("TitleView");});});
			}));
		};
		
		steward.OpenDialogWindow ("確認", "カード情報の解除をします\n" +
			                      "よろしいですか？\n\n" +
			                      "※解除したカードがデッキに含まれる場合\n" +
		                          "自動でデッキ情報から外されます", "はい", positiveAction, "いいえ", () => {
				isSending = false;
				releaseInputField.text = "";
			});
	}
	
	public void OpenCollectionScene() {
		steward.PlaySETap ();
		steward.CustomMenu = 1;
		steward.LoadNextScene ("MenuView");
	}
	
	public void OpenHistoryWindow() {
		steward.PlaySETap ();
		
		steward.SetNewTask (user.getSerialLog ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error getSerialLog:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;
			
			var historyWindow = steward.InitCustomWindow("HistoryWindow");
			historyWindow
				.SetSubject ("登録・解除履歴")
				.SetMessage ("100" + historyWindow.GetObject("Message").GetComponent<Text> ().text);
			
			JArray resultArr = result as JArray;
			int index = 0;
			foreach (var token in resultArr) {
				var obj = token as JObject;
				var clone = Instantiate (historyRowPrefab);
				clone.transform.SetParent(historyWindow.GetObject ("ScrollView/Content").transform);
				clone.transform.localPosition = historyRowPrefab.transform.localPosition;
				clone.transform.localScale = historyRowPrefab.transform.localScale;
				var itemType = cardNames.ContainsKey (obj["itemId"].ToString()) ? "card" : "sleeve";
				clone.transform.Find ("Out_Window/CardName").GetComponent<Text> ().text = itemType == "card" ? cardNames [obj["itemId"].ToString()] : master.staticData ["sleeve"] [obj["itemId"].ToString ()] ["name"].ToString ();
				clone.transform.Find ("Out_Window/DayData").GetComponent<Text> ().text = obj ["date"].ToString ();
				clone.transform.Find ("Out_Window/WhyGetMessage").GetComponent<Text>().text = (itemType == "card" ? "カード" : "スリーブ") + "情報を" + (obj["status"].ToString() == "1" ? "登録" : "解除") + "しました";
				index++;
			}
			
			historyWindow = steward.OpenCustomWindow (historyWindow, "閉じる", () => {});
		}));
	}

	public void StartCamera(int mode) {
		steward.PlaySETap ();
		operateMode = mode;

		#if UNITY_IPHONE
		launchScanner ();
		#endif

		#if UNITY_ANDROID
		using(var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
		{
			int sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
			if (sdkVersion < 23) {
				// In the old Android, the camera permissions were asked only during at the installation
				launchScanner ();
			} else {
				// In Android v6+, the camera permissions are asked at runtime
				if (_cameraPermissionsInstance.Call<bool>("hasCameraPermission")) {
					launchScanner ();
				} else {
					_cameraPermissionsInstance.Call("requestCamera");
				}
			}
		}
		#endif
	}

	#if UNITY_ANDROID
	// onRequestPermissionsResult is automatically called from the native Android plugin (CameraPlugin) when the user
	// accepts or denies the camera permission
	void onRequestPermissionsResult(string result)
	{
		bool gotPermissions = Convert.ToBoolean (result);
		if (gotPermissions) {
			launchScanner ();
		}
		// Could be nice to show a warning popup if the user denied the permissions
	}
	#endif

	void launchScanner() {
		EasyCodeScanner.launchScanner(true, "Scanning...", -1, true);
	}

	public void InputSerialCode(int mode) {
		steward.PlaySETap ();
		operateMode = mode;
		var inputField = mode == (int)OperateMode.Register ? registerInputField : releaseInputField;
		EventSystem.current.SetSelectedGameObject(inputField.gameObject);
		inputField.OnPointerClick(new PointerEventData(EventSystem.current));
	}

	public void SerialSelectCallback (GameObject obj) {
	}

	// - - - QR Code Scan - - -
	
	//Callback when returns from the scanner
	void onScannerMessage(string data) {
		//input.value = dataStr;
		if (!String.IsNullOrEmpty (data)) {
			var serial = data.Substring (data.LastIndexOf ('=') + 1);
			switch (operateMode) {
			case (int)OperateMode.Register:
				if (serialCodeList.Contains (serial)) {
					steward.OpenMessageWindow ("確認", "既に読み込んだカード情報です", "閉じる", () => {});
					if (serialCodeList.Count > 0) {
						registerInputField.text = serialCodeList.Last ();
						serialCodeList.Remove (serialCodeList.Last ());
					}
				} else {
					registerInputField.text = serial;
				}
				if (serialCodeList.Count > 0) {
					serialDropDown = UITools.AddChild (registerButtonArea, Resources.Load ("Common/Prefabs/DropDownBox") as GameObject);
					RectTransform serialListParent = registerButtonArea.transform.Find ("InputArea").GetComponent<RectTransform> ();
					serialDropDownList = serialDropDown.GetComponent<DropDownList> ().SetList (serialCodeList.ToDictionary (s => s, s => s), serialListParent.sizeDelta.x, 4, "color", (GameObject obj) => {
						SerialSelectCallback (obj);
					});
					serialDropDown.transform.localPosition = new Vector2 ((serialListParent.transform.localPosition.x - (serialListParent.sizeDelta.x * 0.5f)), (serialListParent.transform.localPosition.y - (serialListParent.sizeDelta.y * 0.5f)));
					serialDropDownList.DropDown ();
				}
				if (serialCodeList.Count >= 9) {
					registerCameraButton.SetActive (false);
				}
				break;
			case (int)OperateMode.Release:
				releaseInputField.text = serial;
				break;
			default:
				break;
			}
		}
	}
	
	//Callback which notifies an event
	//param : "EVENT_OPENED", "EVENT_CLOSED"
	void onScannerEvent(string eventStr) {
		if (operateMode == (int)OperateMode.Register) {
			switch (eventStr) {
			case "EVENT_OPENED":
				if (serialCodeList.Count > 0) {
					Destroy(serialDropDown);
				}
				if (registerInputField.text.Length > 0) {
					serialCodeList.Add (registerInputField.text);
					registerInputField.text = "";
				}
				break;
			case "EVENT_CLOSED":
				if (String.IsNullOrEmpty (registerInputField.text)) {
					if (serialCodeList.Count > 0) {
						registerInputField.text = serialCodeList.Last ();
						serialCodeList.Remove (serialCodeList.Last ());
					}
					if (serialCodeList.Count > 0) {
						serialDropDown = UITools.AddChild (registerButtonArea, Resources.Load ("Common/Prefabs/DropDownBox") as GameObject);
						RectTransform serialListParent = registerButtonArea.transform.Find ("InputArea").GetComponent<RectTransform> ();
						serialDropDownList = serialDropDown.GetComponent<DropDownList> ().SetList (serialCodeList.ToDictionary (s => s, s => s), serialListParent.sizeDelta.x, 4, "color", (GameObject obj) => {
							SerialSelectCallback (obj);
						});
						serialDropDown.transform.localPosition = new Vector2 ((serialListParent.transform.localPosition.x - (serialListParent.sizeDelta.x * 0.5f)), (serialListParent.transform.localPosition.y - (serialListParent.sizeDelta.y * 0.5f)));
						serialDropDownList.DropDown ();
					}
				}
				break;
			default:
				break;
			}
		}
	}
	
	//Callback when decodeImage has decoded the image/texture 
	void onDecoderMessage(string data) {
	}
}
