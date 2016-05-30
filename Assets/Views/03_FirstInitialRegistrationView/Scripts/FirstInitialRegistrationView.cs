using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Collections;

using Newtonsoft.Json.Linq;

public class FirstInitialRegistrationView : MonoBehaviour {
	private InputField inputField;

	private string userName = "";

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("FirstInitialRegistrationView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;

	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		OpenAgreementWindow ();
	}
	
	public void OpenAgreementDenyMessageWindow () {
		steward.PlaySETap ();
		steward.OpenMessageWindow ("利用規約同意確認", "利用規約に同意していただかないと\nゲームをプレイできません", "戻る", () => {OpenAgreementWindow ();});
	}
			
	public void OpenAgreementWindow () {
		steward.OpenCustomWindow ("AgreementWindow", "同意", () => {OpenInputWindow ();}, "閉じる", () => {OpenAgreementDenyMessageWindow ();});
	}

	public void OpenInputWindow () {
		var inputWindow = steward.OpenCustomWindow ("InputWindow", "登録", () => {OpenInputConfirmMessageWindow ();});
		inputWindow.SetSubject ("隊員名を入力")
			.SetMessage ("隊員名を登録してください")
			.SetText ("Notice", "※最大で" + Const.USERNAME_IMPUT_LIMIT + "文字まで入力できます\n※隊員名は後からでも変更できます")
			.SetText ("Placeholder", "隊員名を入力")
			.SetInputText ("InputArea/InputText", userName);

		inputField = inputWindow.GetObject ("InputArea/InputText").GetComponent<InputField> ();
		inputField.onEndEdit.RemoveAllListeners ();
		inputField.onEndEdit.AddListener ((string input) => {inputField.text = input.Trim ().Substring (0, input.Trim ().Length < Const.USERNAME_IMPUT_LIMIT ? input.Trim ().Length : Const.USERNAME_IMPUT_LIMIT);});
		inputField.characterLimit = Const.USERNAME_IMPUT_LIMIT + Const.IMPUT_LIMIT_OVERFLOW;
		EventSystem.current.SetSelectedGameObject (inputField.gameObject);
		inputField.OnPointerClick (new PointerEventData (EventSystem.current));
		inputField.transform.parent.GetComponent<Button> ().onClick.RemoveAllListeners ();
		inputField.transform.parent.GetComponent<Button> ().onClick.AddListener (() => {
			EventSystem.current.SetSelectedGameObject(inputField.gameObject);
			inputField.OnPointerClick(new PointerEventData(EventSystem.current));
		});
	}

	public void OpenInputConfirmMessageWindow () {
		userName = inputField.text.Trim ().Substring (0, inputField.text.Trim ().Length < Const.USERNAME_IMPUT_LIMIT ? inputField.text.Trim ().Length : Const.USERNAME_IMPUT_LIMIT);
		if (userName.Length == 0 || userName.Length > Const.USERNAME_IMPUT_LIMIT) {
			string errorText;
			if (userName.Length == 0) {
				errorText = "隊員名を入力してください";
			} else {
				errorText = "隊員名は" + Const.USERNAME_IMPUT_LIMIT + "文字までです";
			}
			steward.OpenMessageWindow ("エラー", errorText, "閉じる", () => {OpenInputWindow ();});
			return;
		}

		steward.OpenDialogWindow ("隊員名登録", "[" + userName + "]\nでよろしいですか？", "OK", () => {OpenSubmitMessageWindow ();}, "やめる", () => {OpenInputWindow ();});
	}
			
	public void OpenSubmitMessageWindow () {
		steward.SetNewTask (user.setName (userName, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error setName:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}

			if (steward.ResponseHasErrorCode(result, () => {OpenInputWindow ();})) return;

			GameSettings.TutorialState = GameSettings.TutorialStates.Tutorial;

			steward.OpenMessageWindow ("隊員名登録", "隊員名が登録されました", "閉じる", () => {steward.LoadNextScene ("LoadingView");});
		}));
	}
}
