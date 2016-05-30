using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class DeckConstructionView : MonoBehaviour {
	//
	public void OpenSortWindow () {
		steward.PlaySETap ();

		sortPopUpWindow.Open ();
	}
	
	public void ExecSort () {
		currentSort = sortPopUpWindow.currentSort;
		colorRefineKey = sortPopUpWindow.colorRefineKey;
		prohibitRefineKey = sortPopUpWindow.prohibitRefineKey;
		packRefineKey = sortPopUpWindow.packRefineKey;

		var sortMethods = currentSort.Split ('.');
		var sortKey = sortMethods [0];       
		var sortOrder = sortMethods [1];
		Dictionary<string, CardMaster> sortedCardMasters;
		List<KeyValuePair<string, CardMaster>> listedCardMasters = new List<KeyValuePair<string, CardMaster>> (cardMasters);
		listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
			return x.Value.cardId.CompareTo(y.Value.cardId);
		});
		switch (sortKey) {
		case "cost":
			listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
				return x.Value.cost.Values.Sum().CompareTo(y.Value.cost.Values.Sum());
			});
			break;
			
		case "rarity":
			listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
				return x.Value.rarity.CompareTo(y.Value.rarity);
			});
			break;
			
		case "atk":
			listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
				return x.Value.atk.CompareTo(y.Value.atk);
			});
			break;
			
		case "def":
			listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
				return x.Value.def.CompareTo(y.Value.def);
			});
			break;
			
		case "cardCode":
			listedCardMasters.Sort(delegate(KeyValuePair<string, CardMaster> x, KeyValuePair<string, CardMaster> y) {
				return x.Value.cardCode.CompareTo(y.Value.cardCode);
			});
			break;			
			
		default:
			break;			
		}
		if (sortOrder == "desc") {
			listedCardMasters.Reverse ();
		}
		sortedCardMasters = listedCardMasters.ToDictionary(d => d.Key, d=> d.Value);

		var keyList = new List<string> (sortedCardMasters.Keys);
		
		GameObject listParent = null;
		if (msEditCategory [0].activeInHierarchy) {
			listParent = msEditCategory [0];
		} else if (msEditCategory [1].activeInHierarchy) {
			listParent = msEditCategory [1];
		} else if (msEditCategory [2].activeInHierarchy) {
			listParent = msEditCategory [2];
		} else if (costEditCategory [0].activeInHierarchy) {
			listParent = costEditCategory [0];
		} else if (costEditCategory [1].activeInHierarchy) {
			listParent = costEditCategory [1];
		}
		foreach (Transform n in listParent.transform.Find ("ParentList").transform) {
			n.gameObject.SetActive (true);
		}
		
		var cardPanelList = GameObject.FindGameObjectsWithTag (currentTagName);

		if (colorRefineKey != "None") {
			sortedCardMasters = sortedCardMasters.Where (d => d.Value.color == colorRefineKey).ToDictionary (d => d.Key, d => d.Value);
		}
		if (prohibitRefineKey != "No") {
			sortedCardMasters = sortedCardMasters.Where (d => !prohibitCards.ContainsKey(d.Value.cardId)).ToDictionary (d => d.Key, d => d.Value);
		}
		if (packRefineKey != "None") {
			sortedCardMasters = sortedCardMasters.Where (d => Array.IndexOf(d.Value.packId, packRefineKey) >= 0).ToDictionary (d => d.Key, d => d.Value);
		}
		var selectedKeyList = new List<string> (sortedCardMasters.Keys);
		var excludeKeyList = new List<string> ();
		foreach (var cardId in keyList) {
			if (!selectedKeyList.Contains(cardId)) {
				excludeKeyList.Add (cardId);
			}
		}
		var mergedKeyList = selectedKeyList.Concat(excludeKeyList).ToList();
		
		int cardIndex = 0;
		int i = 0;
		string[] cardIds = new string[cardPanelList.Count ()];
		foreach (var cardId in mergedKeyList) {
			for (i=0; i<cardPanelList.Count(); i++) {
				if (cardPanelList[i].GetComponent<CardData>().CardId == cardId) {
					cardIds[cardIndex++] = cardId;
				}
			}
		}
		int cardCount = 0;
		for (i=0; i<cardPanelList.Count(); i++) {
			cardPanelList[i].GetComponent<CardData>().CardId = cardIds[i];
			if (excludeKeyList.Contains(cardIds[i])) {
				cardPanelList[i].SetActive(false);
				continue;
			}
			cardCount++;
		}
		
		listParent.GetComponent<List_Instance> ().Create_Number = cardCount;
		listParent.transform.Find ("ParentList").GetComponent<Slide_Action_Ver2> ().Rewind ();

		if (isFirstSort) {
			var deckPanelList = GameObject.FindGameObjectsWithTag ("Deck_Card");
			int deckIndex = 0;
			string[] deckIds = new string[deckPanelList.Count ()];
			foreach (var cardId in mergedKeyList) {
				for (i=0; i<deckPanelList.Count(); i++) {
					if (deckPanelList [i].GetComponent<DeckCardData> ().CardId == cardId) {
						deckIds [deckIndex++] = cardId;
					}
				}
			}
			for (i=0; i<deckPanelList.Count(); i++) {
				deckPanelList[i].GetComponent<DeckCardData>().Init (cardMasters[deckIds[i]]);
			}
			isFirstSort = false;
		}

		keyList = selectedKeyList = excludeKeyList = mergedKeyList = null;
		sortedCardMasters = null;
		DispCardList ();
	}
}
