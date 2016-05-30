using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public partial class DeckConstructionView : MonoBehaviour {
	public void SelectDeck (GameObject obj) {
		steward.PlaySETap ();

		SetDeckView (obj.GetComponent<DeckData>().DeckId);
	}
	
	public void OpenDeckNameInputWindow (bool playSE = true) {
		if (playSE) steward.PlaySETap ();
		deckNameInputWindow = steward.OpenCustomWindow("InputWindow", "登録", () => {SubmitDeckName();}, "キャンセル", () => {});
		deckNameInputWindow.SetSubject("デッキ名を入力")
			.SetMessage ("デッキ名を決定してください")
			.SetText ("Notice", "※最大で" + Const.DECKNAME_IMPUT_LIMIT + "文字まで入力できます")
			.SetText ("Placeholder", "デッキ名を入力")
			.SetInputText ("InputArea/InputText", userDecks[currentDeckId].deckName);
		
		deckNameInputField = deckNameInputWindow.GetObject("InputArea/InputText").GetComponent<InputField> ();
		deckNameInputField.onEndEdit.RemoveAllListeners ();
		deckNameInputField.onEndEdit.AddListener ((string input) => {deckNameInputField.text = input.Trim ().Substring (0, input.Trim ().Length < Const.DECKNAME_IMPUT_LIMIT ? input.Trim ().Length : Const.DECKNAME_IMPUT_LIMIT);});
		deckNameInputField.characterLimit = Const.DECKNAME_IMPUT_LIMIT + Const.IMPUT_LIMIT_OVERFLOW;
		EventSystem.current.SetSelectedGameObject (deckNameInputField.gameObject);
		deckNameInputField.OnPointerClick (new PointerEventData (EventSystem.current));
		deckNameInputField.transform.parent.GetComponent<Button> ().onClick.RemoveAllListeners ();
		deckNameInputField.transform.parent.GetComponent<Button> ().onClick.AddListener (() => {
			EventSystem.current.SetSelectedGameObject(deckNameInputField.gameObject);
			deckNameInputField.OnPointerClick(new PointerEventData(EventSystem.current));
		});
	}
	
	public void SubmitDeckName () {
		var deckName = deckNameInputField.text.Trim().Substring (0, deckNameInputField.text.Trim ().Length < Const.DECKNAME_IMPUT_LIMIT ? deckNameInputField.text.Trim ().Length : Const.DECKNAME_IMPUT_LIMIT);
		if (deckName.Length == 0 || deckName.Length > Const.DECKNAME_IMPUT_LIMIT) {
			string errorText = "";
			if (deckName.Length == 0) {
				errorText = "デッキ名を入力してください";
			} else {
				errorText = "デッキ名は" + Const.DECKNAME_IMPUT_LIMIT + "文字までです";
			}
			steward.OpenMessageWindow("エラー", errorText, "閉じる", () => {OpenDeckNameInputWindow (false);});
			return;
		}
		steward.SetNewTask (user.changeDeckName (currentDeckId, deckName, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result, () => {OpenDeckNameInputWindow (false);})) return;
			
			userDecks = userDeckManager.GetAll ();
			
			var deckListPanels = GameObject.FindGameObjectsWithTag ("Deck_List");
			var index = 0;
			foreach (var kv in userDecks) {
				var userDeck = kv.Value;
				deckListPanels[index].transform.Find("DeckName").GetComponent<Text>().text = userDeck.deckName;
				index++;
			}
			
			SetDeckView(currentDeckId);			
		}));
	}
	
	public void OpenAvaterWindow () {
		steward.PlaySETap ();
		avatarWindow = steward.OpenCustomWindow(avatarWindow, "決定", () => {SubmitAvatarMothershipSleeve ("avatar");}, "", default(Action), false);
		avatarWindow
			.SetSubject ("アバター選択")
			.SetText ("SelectionName", "");
		
		avatarWindow.GetSingleActionButton ().SetActive (false);

		//set default avater
		SetAvater ();
	}
	
	public void SelectAvatar (GameObject obj) {
		steward.PlaySETap ();
		currentAvatarId = obj.GetComponent<AvatarData> ().avatarId;
		SetAvater ();
	}

	private void SetAvater () {
		var panelList = avatarWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<AvatarData> ().avatarId == currentAvatarId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
				avatarWindow.SetText ("SelectionName", master.staticData ["avatars"] [currentAvatarId] ["name"].ToString ());
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
		avatarWindow.GetSingleActionButton ().SetActive (true);
	}
	
	public void OpenMothershipWindow () {
		steward.PlaySETap ();
		mothershipWindow = steward.OpenCustomWindow(mothershipWindow, "決定", () => {SubmitAvatarMothershipSleeve ("mothership");}, "", default(Action), false);
		mothershipWindow
			.SetSubject ("母艦選択")
			.SetText ("SelectionName", "");
		
		mothershipWindow.GetSingleActionButton ().SetActive (false);

		//set default mothership
		SetMothership ();
	}
	
	public void SelectMothership (GameObject obj) {
		steward.PlaySETap ();
		currentMothershipId = obj.GetComponent<MothershipData> ().mothershipId;
		SetMothership ();
	}

	private void SetMothership () {
		var panelList = mothershipWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<MothershipData> ().mothershipId == currentMothershipId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
				mothershipWindow.SetText ("SelectionName", master.staticData ["motherships"] [currentMothershipId] ["name"].ToString ());
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
		mothershipWindow.GetSingleActionButton ().SetActive (true);
	}

	public void OpenUnitSleeveWindow () {
		steward.PlaySETap ();
		unitSleeveWindow = steward.OpenCustomWindow(unitSleeveWindow, "決定", () => {SubmitAvatarMothershipSleeve ("unitSleeve");}, "", default(Action), false);
		unitSleeveWindow
			.SetSubject ("メインスリーブ選択")
			.SetText ("SelectionName", "");
		
		unitSleeveWindow.GetSingleActionButton ().SetActive (false);

		//set default unit sleeve
		SetUnitSleeve ();
	}
	
	public void SelectUnitSleeve (GameObject obj) {
		steward.PlaySETap ();
		currentUnitSleeveId = obj.GetComponent<SleeveData> ().sleeveId;
		SetUnitSleeve ();
	}

	private void SetUnitSleeve () {
		var panelList = unitSleeveWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<SleeveData> ().sleeveId == currentUnitSleeveId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
				unitSleeveWindow.SetText ("SelectionName", master.staticData ["sleeve"] [currentUnitSleeveId] ["name"].ToString ());
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
		unitSleeveWindow.GetSingleActionButton ().SetActive (true);
	}

	public void OpenCharacterSleeveWindow () {
		steward.PlaySETap ();
		characterSleeveWindow = steward.OpenCustomWindow(characterSleeveWindow, "決定", () => {SubmitAvatarMothershipSleeve ("characterSleeve");}, "", default(Action), false);
		characterSleeveWindow
			.SetSubject ("コストスリーブ選択")
				.SetText ("SelectionName", "");
		
		characterSleeveWindow.GetSingleActionButton ().SetActive (false);

		//set default character sleeve
		SetCharacterSleeve ();
	}
	
	public void SelectCharacterSleeve (GameObject obj) {
		steward.PlaySETap ();
		currentCharacterSleeveId = obj.GetComponent<SleeveData> ().sleeveId;
		SetCharacterSleeve ();
	}

	private void SetCharacterSleeve () {
		var panelList = characterSleeveWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
		foreach (var panel in panelList) {
			if (panel.GetComponent<SleeveData> ().sleeveId == currentCharacterSleeveId) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(true);
				characterSleeveWindow.SetText ("SelectionName", master.staticData ["sleeve"] [currentUnitSleeveId] ["name"].ToString ());
			} else {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
		}
		characterSleeveWindow.GetSingleActionButton ().SetActive (true);
	}

	public void SubmitAvatarMothershipSleeve (string mode) {
		steward.PlaySEOK ();
		Action<Exception, JToken> callback = (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;

			GameObject[] panelList = null;
			switch (mode) {
			case "avatar":
				panelList = avatarWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
				break;
			case "mothership":
				panelList = mothershipWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
				break;
			case "unitSleeve":
				panelList = unitSleeveWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
				break;
			case "characterSleeve":
				panelList = characterSleeveWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ().GetCreateObjects ();
				break;
			}
			foreach (var panel in panelList) {
				panel.transform.Find ("SelectEnclose").gameObject.SetActive(false);
			}
			userDecks = userDeckManager.GetAll ();
			
			SetDeckView (currentDeckId);
		};
		
		switch (mode) {
		case "avatar":
			steward.SetNewTask (user.changeDeckAvatar (currentDeckId, currentAvatarId, callback));
			break;
		case "mothership":
			steward.SetNewTask (user.changeDeckMothership (currentDeckId, currentMothershipId, callback));
			break;
		case "unitSleeve":
			steward.SetNewTask (user.changeDeckSleeve (currentDeckId, currentUnitSleeveId, "unit", callback));
			break;
		case "characterSleeve":
			steward.SetNewTask (user.changeDeckSleeve (currentDeckId, currentCharacterSleeveId, "crew", callback));
			break;
		}
		
	}

	public void SetMainDeck () {
		if (mainDeckId == currentDeckId) return;
		steward.PlaySEOK ();
		steward.SetNewTask (user.changeDeckMain(currentDeckId, (Exception e, JToken result) => {
			if (e != null) {
				logger.data(e).error("Error changeDeckMain:");
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;
			
			mainDeckId = currentDeckId;
			
			SetDeckView(currentDeckId);
			
		}));
	}
	
	public void OpenUnitCardView () {
		deckView.SetActive (false);
		unitCardView.SetActive (true);
		
		ChangeUnitDeck ();
	}
	
	public void OpenCharacterCardView () {
		deckView.SetActive (false);
		characterCardView.SetActive (true);
		
		ChangeCrewDeck ();
	}
}