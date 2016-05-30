using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public partial class DeckConstructionView : MonoBehaviour {
	private void ChangeUnitDeck () {
		if (unitDeckReady) return;
		
		var currentDeck = userDecks [currentDeckId];
		
		unitVaultList = new Dictionary<string,int> (unitCards);
		eventVaultList = new Dictionary<string,int> (eventCards);
		counterVaultList = new Dictionary<string,int> (counterCards);
		
		List<string> keyList;
		foreach (string id in currentDeck.unitDeck) {
			keyList = new List<string>(unitVaultList.Keys);
			foreach(string key in keyList) {
				if (id == key) {
					unitVaultList[key] -= 1;
				}
			}
			keyList = new List<string>(eventVaultList.Keys);
			foreach(string key in keyList) {
				if (id == key) {
					eventVaultList[key] -= 1;
				}
			}
			keyList = new List<string>(counterVaultList.Keys);
			foreach(string key in keyList) {
				if (id == key) {
					counterVaultList[key] -= 1;
				}
			}
		}
		
		var unitDeckList = new Dictionary<string, int> ();
		foreach (var kv in unitTotalList) {
			foreach (string id in currentDeck.unitDeck) {
				if (kv.Key == id) {
					if (!unitDeckList.ContainsKey(kv.Key)) {
						unitDeckList.Add (kv.Key, 0);
					}
					unitDeckList[kv.Key] += 1;
				}
			}
		}

		if (cardMasters.ContainsKey (currentDeck.favoriteUnit)) {
			SetFavourite (favouriteCard [0], currentDeck.favoriteUnit);
		} else if (currentDeck.unitDeck.Count > 0) {
			SetFavourite (favouriteCard [0], currentDeck.unitDeck[0]);
		}

		CreateDeckList (unitCardView.transform.Find ("ScrollView/DeckContent").GetComponent<CreateDeckList> (), 0, unitDeckList, unitDeckLimit, deckEnterButton[0]);

		deckEnterButton[0].transform.Find ("LimitNumber").GetComponent<NumberManage> ().SetNumber (unitDeckLimit);
		
		unitDeckReady = true;
		
		unitListReady = false;
		eventListReady = false;
		counterListReady = false;
		
		OpenUnitCardTab ();
	}
	
	public void OpenUnitCardTab () {
		currentTagName = "UnitCard_List";
		if (!unitListReady) {
			CreateCardList (msEditCategory [0], unitVaultList, unitCards);
			unitListReady = true;
		}

		ChangeUnitTab (0, 1, 2, "UNIT");
		
		categolyButton [0].sprite = changeButtons [0];
		categolyButton [1].sprite = changeButtons [1];
		categolyButton [2].sprite = changeButtons [1];
		
		ExecSort ();
	}
	
	public void OpenEventCardTab () {
		currentTagName = "EventCard_List";
		if (!eventListReady) {
			CreateCardList (msEditCategory [1], eventVaultList, eventCards);
			eventListReady = true;
		}

		ChangeUnitTab (1, 0, 2, "EVENT");
		
		categolyButton [0].sprite = changeButtons [1];
		categolyButton [1].sprite = changeButtons [0];
		categolyButton [2].sprite = changeButtons [1];
		
		ExecSort ();
	}
	
	public void OpenCounterCardTab () {
		currentTagName = "CounterCard_List";
		if (!counterListReady) {
			CreateCardList (msEditCategory [2], counterVaultList, counterCards);
			counterListReady = true;
		}

		ChangeUnitTab (2, 1, 0, "COUNTER");
		
		categolyButton [0].sprite = changeButtons [1];
		categolyButton [1].sprite = changeButtons [1];
		categolyButton [2].sprite = changeButtons [0];
		
		ExecSort ();
	}
	
	private void ChangeUnitTab(int index0, int index1, int index2, string categolyName) {
		steward.PlaySETap ();
		
		msEditCategory [index0].SetActive (true);
		msEditCategory [index1].SetActive (false);
		msEditCategory [index2].SetActive (false);

		msEditCursor [index0].SetActive (true);
		msEditCursor [index1].SetActive (false);
		msEditCursor [index2].SetActive (false);

		categolyText[0].text = categolyName;
		categolyText[1].text = categolyName;
		
		categolyLabel [index0].color = new Color (1f, 1f, 1f, 1f);
		categolyLabel [index1].color = new Color (1f, 1f, 1f, 0.5f);
		categolyLabel [index2].color = new Color (1f, 1f, 1f, 0.5f);
		
		msEditCategory [index0].transform.GetComponent<List_Instance> ().CreateInstance ();
	}
	
	private void ChangeCrewDeck () {
		if (crewDeckReady) return;
		
		var currentDeck = userDecks [currentDeckId];
		
		crewVaultList = new Dictionary<string,int> (crewCards);
		pilotVaultList = new Dictionary<string,int> (pilotCards);
		
		List<string> keyList;
		foreach (string id in currentDeck.crewDeck) {
			keyList = new List<string>(crewVaultList.Keys);
			foreach(string key in keyList) {
				if (id == key) {
					crewVaultList[key] -= 1;
				}
			}
			keyList = new List<string>(pilotVaultList.Keys);
			foreach(string key in keyList) {
				if (id == key) {
					pilotVaultList[key] -= 1;
				}
			}
		}
		
		var crewDeckList = new Dictionary<string, int> ();
		foreach (var kv in crewTotalList) {
			foreach (string id in currentDeck.crewDeck) {
				if (kv.Key == id) {
					if (!crewDeckList.ContainsKey(kv.Key)) {
						crewDeckList.Add (kv.Key, 0);
					}
					crewDeckList[kv.Key] += 1;
				}
			}
		}

		if (cardMasters.ContainsKey (currentDeck.favoriteCharacter)) {
			SetFavourite (favouriteCard [1], currentDeck.favoriteCharacter);
		} else if (currentDeck.crewDeck.Count > 0) {
			SetFavourite (favouriteCard [1], currentDeck.crewDeck [0]);
		}

		CreateDeckList (characterCardView.transform.Find ("ScrollView/DeckContent").GetComponent<CreateDeckList> (), 1, crewDeckList, crewDeckLimit, deckEnterButton[1]);

		deckEnterButton[1].transform.Find ("LimitNumber").GetComponent<NumberManage> ().SetNumber (crewDeckLimit);
		
		crewDeckReady = true;
		
		crewListReady = false;
		pilotListReady = false;
		
		OpenCrewCardTab ();
	}
	
	public void OpenCrewCardTab () {
		currentTagName = "CrewCard_List";
		if (!crewListReady) {
			CreateCardList (costEditCategory [0], crewVaultList, crewCards);
			crewListReady = true;
		}

		ChangeCharacterTab(0,1,"CREW");
		
		categolyButton [3].sprite = changeButtons [0];
		categolyButton [4].sprite = changeButtons [1];
		
		ExecSort ();
	}
	
	public void OpenPilotCardTab () {
		currentTagName = "PilotCard_List";
		if (!pilotListReady) {
			CreateCardList (costEditCategory [1], pilotVaultList, pilotCards);
			pilotListReady = true;
		}

		ChangeCharacterTab(1,0,"PILOT");
		
		categolyButton [3].sprite = changeButtons [1];
		categolyButton [4].sprite = changeButtons [0];
		
		ExecSort ();
	}
	
	private void ChangeCharacterTab (int index0, int index1, string categolyName) {
		steward.PlaySETap ();
		
		costEditCategory [index0].SetActive (true);
		costEditCategory [index1].SetActive (false);

		costEditCursor [index0].SetActive (true);
		costEditCursor [index1].SetActive (false);

		categolyText[2].text = categolyName;
		categolyText[3].text = categolyName;
		
		categolyLabel [index0 + 3].color = new Color (1f, 1f, 1f, 1f);
		categolyLabel [index1 + 3].color = new Color (1f, 1f, 1f, 0.5f);
		
		
		costEditCategory[index0].transform.GetComponent<List_Instance>().CreateInstance();
	}
	
	private void CreateCardList(GameObject listBox, Dictionary<string, int> idList, Dictionary<string, int> totalList) {
		List_Instance listInstance = listBox.GetComponent<List_Instance> ();
		Slide_Action_Ver2 slideAction = listBox.transform.Find ("ParentList").GetComponent <Slide_Action_Ver2> ();
		listBox.SetActive (true);
		foreach (Transform n in listBox.transform.Find ("ParentList").transform) {
			n.gameObject.SetActive (true);
		}
		listInstance.Create_Number = idList.Count;
		listInstance.CreateInstance ();
		slideAction.Rewind ();
		var listPanel = GameObject.FindGameObjectsWithTag (currentTagName);
		var index = 0;
		var cardsCountPerPage = slideAction.One_Window_MAX_Object;
		foreach (var kv in idList) {
			listPanel[index].GetComponent<CardData>().StoreData (kv.Key);
			index++;
		}
		listBox.SetActive (false);
	}
	
	private void CreateDeckList (CreateDeckList cr, int deckKind, Dictionary<string, int> idList, int deckCount, GameObject enterButton) {
		foreach (var kv in cardMasters) {
			deckCardCount[kv.Value.cardGroupId] = 0;
		}
		
		useBanCard = false;
		int count = 0;
		foreach (var kv in idList) {
			for (int i = 0; i < kv.Value; i++) {
				var clone = UITools.AddChild (cr.parentObject, cr.createObject);
				var currentCardMaster = cardMasters[kv.Key];
				clone.GetComponent<DeckCardData> ().CardId = kv.Key;

				deckCardCount[currentCardMaster.cardGroupId]++;
				
				//デッキの中に禁止カードが見つかった
				if (prohibitCards.ContainsKey(currentCardMaster.cardId)) {
					useBanCard = true;
				}
				
				count++;
			}
		}
		
		enterButton.transform.Find ("AttachNumber").GetComponent<NumberManage> ().SetNumber (count);
		enterButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		BlockButtons [deckKind].SetActive (true);
		inDeckLimitOver = true;
		enterButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
		enterButton.GetComponent<Scale_Pop>().isPop = false;

		if (count == deckCount && !useBanCard) {
			BlockButtons [deckKind].SetActive (false);
			inDeckLimitOver = false;
			enterButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
			enterButton.GetComponent<Scale_Pop>().isPop = true;
			enterButton.GetComponent<Button> ().onClick.AddListener (() => {
				SubmitDeckCard(deckKind);
			});
		}
	}
	
	public void DispCardList () {
		var vaultList = new Dictionary<string, int>();
		var totalList = new Dictionary<string, int>();
		switch (currentTagName) {
		case "UnitCard_List":
			vaultList = unitVaultList;
			totalList = unitTotalList;
			break;
		case "EventCard_List":
			vaultList = eventVaultList;
			totalList = unitTotalList;
			break;
		case "CounterCard_List":
			vaultList = counterVaultList;
			totalList = unitTotalList;
			break;
		case "CrewCard_List":
			vaultList = crewVaultList;
			totalList = crewTotalList;
			break;
		case "PilotCard_List":
			vaultList = pilotVaultList;
			totalList = crewTotalList;
			break;
		}
		
		var listPanel = GameObject.FindGameObjectsWithTag (currentTagName);
		if (listPanel.Count () == 0) return;
		loadAssetBundle.SetLoadWait ();
		Slide_Action_Ver2 slideAction = listPanel [0].transform.parent.gameObject.GetComponent<Slide_Action_Ver2> ();
		var cardsCountPerPage = slideAction.One_Window_MAX_Object;
		var currentPage = int.Parse (slideAction.NowPage_Text.text);
		for (int i=0; i<listPanel.Count(); i++) {
			var tempCardId = listPanel[i].GetComponent<CardData>().CardId;
			if (i >= (currentPage - 2) * cardsCountPerPage && i < (currentPage + 1) * cardsCountPerPage) {
				loadAssetBundle.SetCardImage (tempCardId, (int)LoadAssetBundle.DisplayType.Card, listPanel[i].transform.Find ("CardImage").gameObject);
			} else {
				listPanel[i].transform.Find ("CardImage").GetComponent<Image>().sprite = blankImage;
			}
			var currentCardMaster = cardMasters[tempCardId];
			var totalNum = currentCardMaster.cardLimit > totalList[tempCardId] ? totalList[tempCardId] : currentCardMaster.cardLimit;
			var stockNum = (currentCardMaster.cardLimit - deckCardCount[currentCardMaster.cardGroupId]) > vaultList[tempCardId] ? vaultList[tempCardId] : currentCardMaster.cardLimit - deckCardCount[currentCardMaster.cardGroupId];
			if (stockNum < 0) stockNum = 0;
			if (prohibitCards.ContainsKey(currentCardMaster.cardId)) {
				totalNum = 0;
				stockNum = 0;
			}
			listPanel[i].transform.Find ("StockNumber").GetComponent<NumberManage>().SetNumber (stockNum);
			listPanel[i].transform.Find ("TotalNumber").GetComponent<NumberManage>().SetNumber (totalNum);
			listPanel[i].GetComponent<CardDragDrop>().isDraggable = true;
			listPanel[i].GetComponent<Scale_Pop>().isPop = true;
			listPanel[i].transform.Find ("CardImage").GetComponent<Image>().color = new Color (1f, 1f, 1f);
			if (stockNum <= 0 || deckCardCount[currentCardMaster.cardGroupId] >= currentCardMaster.cardLimit || prohibitCards.ContainsKey(currentCardMaster.cardId)) {
				listPanel[i].GetComponent<CardDragDrop>().isDraggable = false;
				listPanel[i].GetComponent<Scale_Pop>().isPop = false;
				listPanel[i].transform.Find ("CardImage").GetComponent<Image>().color = new Color (0.5f, 0.5f, 0.5f);
				if (prohibitCards.ContainsKey(currentCardMaster.cardId)) {
					listPanel[i].GetComponent<CardDragDrop>().prohibitStatus = prohibitCards[currentCardMaster.cardId];
				}
			}
			
			if (prohibitCards.ContainsKey(currentCardMaster.cardId)) {
				listPanel[i].transform.Find ("BanImage").gameObject.SetActive(true);
			} else {
				listPanel[i].transform.Find ("BanImage").gameObject.SetActive(false);
			}
		}
		Resources.UnloadUnusedAssets ();
	}

	public void ReceiveDragDrop(GameObject obj, Vector3 firstPosition, Transform firstParent, int firstSiblingIndex) {
		Transform scrollView;
		if (unitCardView.activeInHierarchy) {
			scrollView = unitCardView.transform.Find ("ScrollView");
		} else {
			scrollView = characterCardView.transform.Find ("ScrollView");
		}
		var objPosition = obj.transform.localPosition;
		var scrollViewPosition = scrollView.localPosition;
		switch (obj.GetComponent<CardDragDrop>().cardType) {
		case 0:
			if (objPosition.x + 100 < scrollViewPosition.x) {
				DeleteCardFromDeck(obj);
			} else if (objPosition.y - 60 > scrollViewPosition.y) {
				ChangeFavourite(obj);
			}
			break;
		case 1:
			if (objPosition.x + 100 > scrollViewPosition.x) {
				AddCardToDeck(obj);
			}
			break;
		}
		obj.transform.SetParent(firstParent);
		obj.transform.SetSiblingIndex (firstSiblingIndex);
		obj.transform.localPosition = firstPosition;
	}
	
	private void AddCardToDeck(GameObject obj) {
		string cardId = obj.GetComponent<CardData> ().CardId;
		CreateDeckList createDeckList;
		Dictionary<string, int> totalList;
		Dictionary<string, int> cardList;
		Dictionary<string, int> vaultList;
		string vaultCardTagName;
		GameObject[] vaultCardList;
		GameObject enterButton;
		int deckLimit;
		int deckKind;
		GameObject favourite;
		if (unitCardView.activeInHierarchy) {
			createDeckList = unitCardView.transform.Find ("ScrollView/DeckContent").GetComponent<CreateDeckList> ();
			deckLimit = unitDeckLimit;
			deckKind = 0;
			enterButton = deckEnterButton[0];
			favourite = favouriteCard[0];
			totalList = unitTotalList;
			if (unitVaultList.ContainsKey(cardId)) {
				cardList = unitCards;
				vaultList = unitVaultList;
				vaultCardTagName = "UnitCard_List";
			} else if (eventVaultList.ContainsKey(cardId)) {
				cardList = eventCards;
				vaultList = eventVaultList;
				vaultCardTagName = "EventCard_List";
			} else if (counterVaultList.ContainsKey(cardId)) {
				cardList = counterCards;
				vaultList = counterVaultList;
				vaultCardTagName = "CounterCard_List";
			} else {
				return;
			}
		} else {
			createDeckList = characterCardView.transform.Find ("ScrollView/DeckContent").GetComponent<CreateDeckList> ();
			deckLimit = crewDeckLimit;
			deckKind = 1;
			enterButton = deckEnterButton[1];
			favourite = favouriteCard[1];
			totalList = crewTotalList;
			if (crewVaultList.ContainsKey(cardId)) {
				cardList = crewCards;
				vaultList = crewVaultList;
				vaultCardTagName = "CrewCard_List";
			} else if (pilotVaultList.ContainsKey(cardId)) {
				cardList = pilotCards;
				vaultList = pilotVaultList;
				vaultCardTagName = "PilotCard_List";
			} else {
				return;
			}
		}
		if (prohibitCards.ContainsKey (cardId)) return;
		if (vaultList [cardId] <= 0 || (cardList [cardId] - vaultList [cardId]) >= cardMasterManager.Get (cardId).cardLimit) return;
		
		var deckCardList = GameObject.FindGameObjectsWithTag ("Deck_Card");
		if (deckCardList.Count() >= deckLimit) {
			steward.OpenMessageWindow ("確認", "デッキに投入できる規定枚数は" + deckLimit + "枚です", "閉じる", () => {});
			return;
		}

		editNoSave = true;
		
		vaultCardList = GameObject.FindGameObjectsWithTag (vaultCardTagName);

		var clone = UITools.AddChild (createDeckList.parentObject, createDeckList.createObject);
		clone.GetComponent<DeckCardData> ().Init (cardMasters[cardId]);
		
		int siblingIndex = -1;
		var sortedDeckCardList = new SortedList<int, GameObject> ();
		useBanCard = false;
		foreach (var deckCard in deckCardList) {
			sortedDeckCardList.Add (deckCard.transform.GetSiblingIndex(), deckCard);
			var tmpCardMaster = cardMasters [deckCard.GetComponent<DeckCardData> ().CardId];
			if (prohibitCards.ContainsKey(tmpCardMaster.cardId)) {
				useBanCard = true;
			}
		}
		
		foreach (var kv in totalList) {
			if (kv.Key == cardId) {
				clone.transform.SetSiblingIndex(siblingIndex + 1);
				break;
			}
			foreach (var kv2 in sortedDeckCardList) {
				if (kv2.Value.GetComponent<DeckCardData>().CardId == kv.Key) {
					siblingIndex = kv2.Value.transform.GetSiblingIndex();
				}
			}
		}
		
		foreach (var kv in sortedDeckCardList) {
			var tmpSiblingIndex = kv.Value.transform.GetSiblingIndex();
			if (siblingIndex < tmpSiblingIndex) {
				kv.Value.transform.SetSiblingIndex(tmpSiblingIndex + 1);
			}
		}
		
		vaultList [cardId] -= 1;
		var currentCardMaster = cardMasters[cardId];
		deckCardCount [currentCardMaster.cardGroupId] += 1;
		foreach (var vaultCard in vaultCardList) {
			var tmpCardId = vaultCard.GetComponent<CardData>().CardId;
			var tmpCardMaster = cardMasters[tmpCardId];
			var totalNum = tmpCardMaster.cardLimit > totalList[tmpCardId] ? totalList[tmpCardId] : tmpCardMaster.cardLimit;
			var stockNum = (tmpCardMaster.cardLimit - deckCardCount[tmpCardMaster.cardGroupId]) > vaultList[tmpCardId] ? vaultList[tmpCardId] : tmpCardMaster.cardLimit - deckCardCount[tmpCardMaster.cardGroupId];
			if (stockNum < 0) stockNum = 0;
			vaultCard.transform.Find ("StockNumber").GetComponent<NumberManage>().SetNumber(stockNum);
			vaultCard.GetComponent<CardDragDrop>().isDraggable = true;
			vaultCard.GetComponent<Scale_Pop>().isPop = true;
			vaultCard.transform.Find ("CardImage").GetComponent<Image>().color = new Color (1f, 1f, 1f);
			if (stockNum <= 0 || deckCardCount [tmpCardMaster.cardGroupId] >= tmpCardMaster.cardLimit) {
				vaultCard.GetComponent<CardDragDrop>().isDraggable = false;
				vaultCard.GetComponent<Scale_Pop>().isPop = false;
				vaultCard.transform.Find ("CardImage").GetComponent<Image>().color = new Color (0.5f, 0.5f, 0.5f);
			}
		}

		if (deckCardList.Length == 0) {
			SetFavourite (favourite, cardId);
		}
		
		int currentDeckCardCount = deckCardList.Length + 1;
		enterButton.transform.Find ("AttachNumber").GetComponent<NumberManage> ().SetNumber (currentDeckCardCount);
		enterButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		BlockButtons [deckKind].SetActive (true);
		inDeckLimitOver = true;
		enterButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
		enterButton.GetComponent<Scale_Pop>().isPop = false;

		if (currentDeckCardCount == deckLimit && !useBanCard) {
			BlockButtons [deckKind].SetActive (false);
			inDeckLimitOver = false;
			enterButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
			enterButton.GetComponent<Scale_Pop>().isPop = true;
			enterButton.GetComponent<Button> ().onClick.AddListener (() => {
				SubmitDeckCard(deckKind);
			});
		}
		
		
		
		Resources.UnloadUnusedAssets ();
	}
	
	private void DeleteCardFromDeck(GameObject obj) {
		string cardId = obj.GetComponent<DeckCardData> ().CardId;
		Dictionary<string, int> totalList;
		Dictionary<string, int> cardList;
		Dictionary<string, int> vaultList;
		string cardListTagName;
		GameObject[] vaultCardList;
		GameObject enterButton;
		int deckLimit;
		int deckKind;
		GameObject favourite;
		if (unitCardView.activeInHierarchy) {
			deckLimit = unitDeckLimit;
			deckKind = 0;
			enterButton = deckEnterButton[0];
			totalList = unitTotalList;
			favourite = favouriteCard[0];
			if (unitVaultList.ContainsKey(cardId)) {
				cardList = unitCards;
				vaultList = unitVaultList;
				cardListTagName = "UnitCard_List";
			} else if (eventVaultList.ContainsKey(cardId)) {
				cardList = eventCards;
				vaultList = eventVaultList;
				cardListTagName = "EventCard_List";
			} else if (counterVaultList.ContainsKey(cardId)) {
				cardList = counterCards;
				vaultList = counterVaultList;
				cardListTagName = "CounterCard_List";
			} else {
				return;
			}
		} else {
			deckLimit = crewDeckLimit;
			deckKind = 1;
			enterButton = deckEnterButton[1];
			totalList = crewTotalList;
			favourite = favouriteCard[1];
			if (crewVaultList.ContainsKey(cardId)) {
				cardList = crewCards;
				vaultList = crewVaultList;
				cardListTagName = "CrewCard_List";
			} else if (pilotVaultList.ContainsKey(cardId)) {
				cardList = pilotCards;
				vaultList = pilotVaultList;
				cardListTagName = "PilotCard_List";
			} else {
				return;
			}
		}
		if (vaultList [cardId] >= cardList [cardId]) return;
		
		editNoSave = true;
		
		vaultCardList = GameObject.FindGameObjectsWithTag (cardListTagName);
		
		var deckCardList = GameObject.FindGameObjectsWithTag ("Deck_Card");
		
		vaultList [cardId] += 1;
		var currentCardMaster = cardMasters[cardId];
		deckCardCount [currentCardMaster.cardGroupId] -= 1;
		foreach (var vaultCard in vaultCardList) {
			var tmpCardId = vaultCard.GetComponent<CardData>().CardId;
			var tmpCardMaster = cardMasters[tmpCardId];
			var totalNum = tmpCardMaster.cardLimit > totalList[tmpCardId] ? totalList[tmpCardId] : tmpCardMaster.cardLimit;
			var stockNum = (tmpCardMaster.cardLimit - deckCardCount[tmpCardMaster.cardGroupId]) > vaultList[tmpCardId] ? vaultList[tmpCardId] : tmpCardMaster.cardLimit - deckCardCount[tmpCardMaster.cardGroupId];
			if (stockNum < 0) stockNum = 0;
			if (prohibitCards.ContainsKey(tmpCardMaster.cardId)) {
				totalNum = 0;
				stockNum = 0;
			}
			vaultCard.transform.Find ("StockNumber").GetComponent<NumberManage>().SetNumber(stockNum);
			vaultCard.GetComponent<CardDragDrop>().isDraggable = true;
			vaultCard.GetComponent<Scale_Pop>().isPop = true;
			vaultCard.transform.Find ("CardImage").GetComponent<Image>().color = new Color (1f, 1f, 1f);
			if (stockNum <= 0 || deckCardCount [tmpCardMaster.cardGroupId] >= tmpCardMaster.cardLimit) {
				vaultCard.GetComponent<CardDragDrop>().isDraggable = false;
				vaultCard.GetComponent<Scale_Pop>().isPop = false;
				vaultCard.transform.Find ("CardImage").GetComponent<Image>().color = new Color (0.5f, 0.5f, 0.5f);
			}
		}
		
		var sortedDeckCardList = new SortedList<int, GameObject> ();
		useBanCard = false;
		foreach (var deckCard in deckCardList) {
			if (deckCard.Equals(obj)) continue;
			sortedDeckCardList.Add (deckCard.transform.GetSiblingIndex(), deckCard);
			var tmpCardMaster = cardMasters [deckCard.GetComponent<DeckCardData> ().CardId];
			if (prohibitCards.ContainsKey(tmpCardMaster.cardId)) {
				useBanCard = true;
			}
		}
		
		int index = 0;
		bool isFavouriteInside = false;
		string firstCardId = "";
		foreach (var kv in sortedDeckCardList) {
			if (kv.Value.Equals(obj)) continue;
			kv.Value.transform.SetSiblingIndex(index);
			if (index == 0) firstCardId = kv.Value.GetComponent<DeckCardData>().CardId;
			index++;
			if (kv.Value.GetComponent<DeckCardData>().CardId == favourite.GetComponent<DeckCardData>().CardId) {
				isFavouriteInside = true;
			}
		}
		
		Destroy(obj);
		
		if (!isFavouriteInside) {
			SetFavourite(favourite, firstCardId);
		}
		
		int currentDeckCardCount = deckCardList.Length - 1;
		enterButton.transform.Find ("AttachNumber").GetComponent<NumberManage> ().SetNumber (currentDeckCardCount);
		enterButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		BlockButtons [deckKind].SetActive (true);
		inDeckLimitOver = true;
		enterButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
		enterButton.GetComponent<Scale_Pop>().isPop = false;
		
		if (currentDeckCardCount == deckLimit && !useBanCard) {
			BlockButtons [deckKind].SetActive (false);
			inDeckLimitOver = false;
			enterButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
			enterButton.GetComponent<Scale_Pop>().isPop = true;
			enterButton.GetComponent<Button> ().onClick.AddListener (() => {
				SubmitDeckCard (deckKind);
			});
		}
		
		Resources.UnloadUnusedAssets ();
	}
	
	private void ChangeFavourite(GameObject obj) {
		GameObject favourite;
		if (unitCardView.activeInHierarchy) {
			favourite = favouriteCard[0];
		} else {
			favourite = favouriteCard[1];
		}
		SetFavourite (favourite, obj.GetComponent<DeckCardData> ().CardId);
	}

	public void OpenCardDetailPopUp(GameObject obj) {
		if (loadAssetBundle.IsWaitLoading) return;

		steward.PlaySETap ();
		loadAssetBundle.SetLoadWait ();
		var cardId = obj.GetComponent<CardData> ().CardId;
		cardDetailPopUpWindow.GetComponent<CardDetailPopUpWindow> ().Open (cardId, totalCards [cardId], steward.transform);
	}

	public void OpenUnitDeckMessageWindow () {
		string errorText = "";
		if (unitCardView.activeInHierarchy && useBanCard == true) {
			errorText = "デッキに入れられないカードが含まれています";
		} else if (unitCardView.activeInHierarchy && inDeckLimitOver == true) {
			errorText = "MSデッキの規定数(" + unitDeckLimit + "枚)を満たしていません";
		}
		steward.OpenMessageWindow ("確認", errorText, "閉じる", () => {});
	}
	
	public void OpenCharacterDeckMessageWindow () {
		string errorText = "";
		if (characterCardView.activeInHierarchy && useBanCard == true) {
			errorText = "デッキに入れられないカードが含まれています";
		} else if (characterCardView.activeInHierarchy && inDeckLimitOver == true) {
			errorText = "キャラデッキの規定数(" + crewDeckLimit + "枚)を満たしていません";
		}
		steward.OpenMessageWindow ("確認", errorText, "閉じる", () => {});
	}
	
	public void SubmitDeckCard (int index) {
		string deckType = "";
		int deckLimit = 0;
		switch (index) {
		case 0:
			deckType = "unit";
			deckLimit = unitDeckLimit;
			break;
			
		case 1:
			deckType = "crew";
			deckLimit = crewDeckLimit;
			break;
		}
		steward.SetNewTask (user.changeDeckFavoriteCard (currentDeckId, favouriteCard[index].GetComponent<DeckCardData>().CardId, deckType, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;
			
			var deckCardList = GameObject.FindGameObjectsWithTag ("Deck_Card");
			string[] cardIdArray = new string[deckLimit];
			int i = 0;
			foreach (var deckCard in deckCardList) {
				cardIdArray[i] = deckCard.GetComponent<DeckCardData>().CardId;
				i++;
			}
			steward.SetNewTask (user.changeDeckCard (currentDeckId, cardIdArray, deckType, (Exception e2, JToken result2) => {
				if (e2 != null) {
					logger.data (e2).error ("Error:");
					steward.OpenExceptionPopUpWindow ();
					return;
				}
				
				if (steward.ResponseHasErrorCode(result2)) return;
				
				editNoSave = false;

				userDecks = userDeckManager.GetAll ();
				Back ();
			}));
		}));
	}

	private void SetFavourite (GameObject favourite, string cardId) {
		favourite.SetActive (true);
		if (cardMasters.ContainsKey (cardId)) {
			favourite.GetComponent<DeckCardData> ().Init (cardMasters[cardId]);
		} else {
			favourite.GetComponent<DeckCardData> ().Init (null);
		}
	}
}
