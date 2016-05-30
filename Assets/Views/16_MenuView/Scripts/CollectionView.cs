using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public class CollectionView : MonoBehaviour {
	public GameObject msDeckController;
	public GameObject costDeckController;
	public GameObject sortWindow;
	public Sprite blankImage;

	public GameObject[] selectButtonBox = new GameObject[6];

	public GameObject[] msEditCategory = new GameObject[3];
	public GameObject[] costEditCategory = new GameObject[2];

	public GameObject[] msEditCursor = new GameObject[3];
	public GameObject[] costEditCursor = new GameObject[2];
	
	public Text[] caterolyButtonText = new Text[9];

	public Image[] selectCursorImage = new Image[2];
	public Sprite[] selectCursorSprite = new Sprite[2];

	public Image[] categolyButtonImage = new Image[5];
	public Sprite[] categolyButtonSprite = new Sprite[2];

	private SortPopUpWindow sortPopUpWindow;
	private GameObject cardDetailPopUpWindow;

	private Dictionary<string, Dictionary<string, int>> unitCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, Dictionary<string, int>> eventCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, Dictionary<string, int>> counterCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, Dictionary<string, int>> crewCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, Dictionary<string, int>> pilotCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, Dictionary<string, int>> totalCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, CardMaster> cardMasters = new Dictionary<string, CardMaster> ();
	private Dictionary<string, string> prohibitCards = new Dictionary<string, string> ();

	private bool unitListReady = false;
	private bool eventListReady = false;
	private bool counterListReady = false;
	private bool crewListReady = false;
	private bool pilotListReady = false;

	private string currentTagName;

	private string currentSort;
	private string colorRefineKey;
	private string prohibitRefineKey;
	private string packRefineKey;

	private Dictionary<string, int> cardGroupIdRarityPair = new Dictionary<string, int>();
	
	private Mage mage { get { return Mage.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("CollectionView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private CardMasterManager cardMasterManager = new CardMasterManager();
	private UserCardManager userCardManager = new UserCardManager();

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		cardGroupIdRarityPair = cardMasterManager.GetCardGroupIdRarityPairs ();

		unitCards = userCardManager.GetCounts ("unitCard");
		eventCards = userCardManager.GetCounts ("eventCard");
		counterCards = userCardManager.GetCounts ("counterCard");
		crewCards = userCardManager.GetCounts ("crewCard");
		pilotCards = userCardManager.GetCounts ("pilotCard");
		totalCards = unitCards.Concat (eventCards).Concat (counterCards).Concat (crewCards).Concat (pilotCards).GroupBy (d => d.Key).ToDictionary (d => d.Key, d => d.First ().Value);

		foreach (var kv in totalCards) {
			cardMasters.Add (kv.Key, cardMasterManager.Get (kv.Key));
		}

		foreach (JProperty prop in master.staticData["prohibitCards"]) {
			prohibitCards.Add (prop.Name, (prop.Value as JObject) ["status"].ToString ());
		}
		
		// Create Card Detail Pop Up Window
		cardDetailPopUpWindow = PopUpWindowManager.InitAsGameObject ("Common/Prefabs/CardDetailPopUpWindow", steward.transform);

		// Create Sort Pop Up Window
		sortPopUpWindow = PopUpWindowManager.InitAsGameObject ("Common/Prefabs/SortPopUpWindow", steward.transform).GetComponent<SortPopUpWindow> ();
		sortPopUpWindow.Init (() => {
			sortPopUpWindow.SetExecAction (() => {ExecSort ();});
			OpenUnitCardView ();
		});
	}
	
	void OnDestroy () {
		Destroy (cardDetailPopUpWindow);
		Destroy (sortPopUpWindow.gameObject);
	}
	
	public void OpenUnitCardView () {
		selectButtonBox [0].GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f);
		selectButtonBox [1].GetComponent<Image> ().color = new Color (1f, 0.75f, 0f);
		selectButtonBox [2].GetComponent<Text> ().color = new Color (0.5f, 0.5f, 0.5f);
		selectButtonBox [3].GetComponent<Image> ().color = new Color (1f, 1f, 1f);
		selectButtonBox [4].GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f);
		selectButtonBox [5].GetComponent<Text> ().color = new Color (1f, 1f, 1f);
		
		selectCursorImage [0].sprite = selectCursorSprite [0];
		selectCursorImage [1].sprite = selectCursorSprite [1];
		
		msDeckController.SetActive (true);
		costDeckController.SetActive (false);
		
		msEditCategory [0].SetActive (true);

		OpenUnitCardTab ();
	}
	
	public void OpenCharacterCardView () {
		selectButtonBox [0].GetComponent<Image> ().color = new Color (1f, 1f, 1f);
		selectButtonBox [1].GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f);
		selectButtonBox [2].GetComponent<Text> ().color = new Color (1f, 1f, 1f);
		selectButtonBox [3].GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f);
		selectButtonBox [4].GetComponent<Image> ().color = new Color (1f, 0.75f, 0f);
		selectButtonBox [5].GetComponent<Text> ().color = new Color (0.5f, 0.5f, 0.5f);

		selectCursorImage [0].sprite = selectCursorSprite [1];
		selectCursorImage [1].sprite = selectCursorSprite [0];
		
		msDeckController.SetActive (false);
		costDeckController.SetActive (true);
		
		costEditCategory [0].SetActive (true);

		OpenCrewCardTab ();
	}

	public void OpenUnitCardTab () {
		currentTagName = "UnitCard_List";
		if (!unitListReady) {
			CreateCardList (msEditCategory [0], unitCards);
			unitListReady = true;
		}
		
		ChangeUnitTab (0, 1, 2, "UNIT");

		categolyButtonImage [0].sprite = categolyButtonSprite [0];
		categolyButtonImage [1].sprite = categolyButtonSprite [1];
		categolyButtonImage [2].sprite = categolyButtonSprite [1];
		
		ExecSort ();
	}
	
	public void OpenEventCardTab () {
		currentTagName = "EventCard_List";
		if (!eventListReady) {
			CreateCardList (msEditCategory [1], eventCards);
			eventListReady = true;
		}
		
		ChangeUnitTab (1, 0, 2, "EVENT");

		categolyButtonImage [0].sprite = categolyButtonSprite [1];
		categolyButtonImage [1].sprite = categolyButtonSprite [0];
		categolyButtonImage [2].sprite = categolyButtonSprite [1];
		
		ExecSort ();
	}
	
	public void OpenCounterCardTab () {
		currentTagName = "CounterCard_List";
		if (!counterListReady) {
			CreateCardList (msEditCategory [2], counterCards);
			counterListReady = true;
		}
		
		ChangeUnitTab (2, 0, 1, "COUNTER");

		categolyButtonImage [0].sprite = categolyButtonSprite [1];
		categolyButtonImage [1].sprite = categolyButtonSprite [1];
		categolyButtonImage [2].sprite = categolyButtonSprite [0];
		
		ExecSort ();
	}
	
	private void ChangeUnitTab (int index0, int index1, int index2, string categolyName) {
		steward.PlaySETap ();
		
		msEditCategory [index0].SetActive (true);
		msEditCategory [index1].SetActive (false);
		msEditCategory [index2].SetActive (false);
		
		msEditCursor [index0].SetActive (true);
		msEditCursor [index1].SetActive (false);
		msEditCursor [index2].SetActive (false);
		
		caterolyButtonText [index0].color = new Color (1f, 1f, 1f, 1f);
		caterolyButtonText [index1].color = new Color (1f, 1f, 1f, 0.5f);
		caterolyButtonText [index2].color = new Color (1f, 1f, 1f, 0.5f);
		
		caterolyButtonText [5].text = categolyName;
		caterolyButtonText [6].text = categolyName;
		
		msEditCategory [index0].transform.GetComponent<List_Instance> ().CreateInstance ();
	}

	public void OpenCrewCardTab () {
		currentTagName = "CrewCard_List";
		if (!crewListReady) {
			CreateCardList (costEditCategory [0], crewCards);
			crewListReady = true;
		}
		
		ChangeCharacterTab (0, 1, "CREW");

		categolyButtonImage [3].sprite = categolyButtonSprite [0];
		categolyButtonImage [4].sprite = categolyButtonSprite [1];
		
		ExecSort ();
	}
	
	public void OpenPilotCardTab () {
		currentTagName = "PilotCard_List";
		if (!pilotListReady) {
			CreateCardList (costEditCategory [1], pilotCards);
			pilotListReady = true;
		}
		
		ChangeCharacterTab (1, 0, "PILOT");

		categolyButtonImage [3].sprite = categolyButtonSprite [1];
		categolyButtonImage [4].sprite = categolyButtonSprite [0];
		
		ExecSort ();
	}
	
	private void ChangeCharacterTab (int index0, int index1, string categolyName) {
		steward.PlaySETap ();
		
		costEditCategory [index0].SetActive (true);
		costEditCategory [index1].SetActive (false);
		
		costEditCursor [index0].SetActive (true);
		costEditCursor [index1].SetActive (false);
		
		caterolyButtonText [index0 + 3].color = new Color (1f, 1f, 1f, 1f);
		caterolyButtonText [index1 + 3].color = new Color (1f, 1f, 1f, 0.5f);
		
		caterolyButtonText [7].text = categolyName;
		caterolyButtonText [8].text = categolyName;
		
		costEditCategory [index0].transform.GetComponent<List_Instance> ().CreateInstance ();	
	}
	
	private void CreateCardList (GameObject listBox, Dictionary<string, Dictionary<string, int>> idList) {
		List_Instance listInstance = listBox.GetComponent<List_Instance> ();
		Slide_Action_Ver2 slideAction = listBox.transform.Find ("ParentList").GetComponent <Slide_Action_Ver2> ();
		listBox.SetActive (true);
		listInstance.Create_Number = idList.Count;
		listInstance.CreateInstance();
		slideAction.Rewind ();
		var listPanel = GameObject.FindGameObjectsWithTag (currentTagName);
		var index = 0;
		var cardsCountPerPage = slideAction.One_Window_MAX_Object;
		foreach (var kv in idList) {
			listPanel[index].GetComponent<CollectionData>().StoreData(kv.Key);
			index++;
		}
		listBox.SetActive (false);
	}
	
	public void DispCardList() {
		var listPanel = GameObject.FindGameObjectsWithTag (currentTagName);
		if (listPanel.Count () == 0) return;
		loadAssetBundle.SetLoadWait ();
		Slide_Action_Ver2 slideAction = listPanel [0].transform.parent.gameObject.GetComponent<Slide_Action_Ver2> ();
		var cardsCountPerPage = slideAction.One_Window_MAX_Object;
		var currentPage = int.Parse (slideAction.NowPage_Text.text);
		for (int i=0; i<listPanel.Count(); i++) {
			if (i >= (currentPage - 2) * cardsCountPerPage && i < (currentPage + 1) * cardsCountPerPage) {
				listPanel[i].transform.Find("TotalNumber").GetComponent<NumberManage> ().SetNumber (totalCards[listPanel[i].GetComponent<CollectionData>().cardId] ["totalCount"]);
				loadAssetBundle.SetCardImage (listPanel[i].GetComponent<CollectionData>().cardId, (int)LoadAssetBundle.DisplayType.Card, listPanel[i].transform.Find ("CardImage").gameObject);
			} else {
				listPanel[i].transform.Find ("CardImage").GetComponent<Image>().sprite = blankImage;
			}
		}
		Resources.UnloadUnusedAssets ();
	}

	public void OpenSortWindow () {
		steward.PlaySETap ();
		
		sortPopUpWindow.Open ();
	}

	public void ExecSort () {
		currentSort = sortPopUpWindow.currentSort;
		colorRefineKey = sortPopUpWindow.colorRefineKey;
		prohibitRefineKey = sortPopUpWindow.prohibitRefineKey;
		packRefineKey = sortPopUpWindow.packRefineKey;

		Func<KeyValuePair<string, CardMaster>, int> intKeySelector = null;
		Func<KeyValuePair<string, CardMaster>, string> stringKeySelector = null;

		var sortMethods = currentSort.Split ('.');
		var sortKey = sortMethods [0];
		var sortOrder = sortMethods [1];
		switch (sortKey) {
		case "cost":
			intKeySelector = k => (k.Value.cost.Values.Sum());
			break;
			
		case "rarity":
			intKeySelector = k => k.Value.rarity;
			break;
			
		case "atk":
			intKeySelector = k => k.Value.atk;
			break;
			
		case "def":
			intKeySelector = k => k.Value.def;
			break;
			
		case "cardCode":
			stringKeySelector = k => k.Value.cardCode;
			break;			
			
		default:
			stringKeySelector = k => k.Value.cardId;
			break;			
		}
		
		Dictionary<string, CardMaster> sortedCardMasters;
		switch (sortKey) {
		case "cost":
		case "rarity":
		case "atk":
		case "def":
			if (sortOrder == "asc") {
				sortedCardMasters = cardMasters.OrderBy (intKeySelector).ToDictionary (d => d.Key, d => d.Value);
			} else {
				sortedCardMasters = cardMasters.OrderByDescending (intKeySelector).ToDictionary (d => d.Key, d => d.Value);
			}
			break;			
			
		case "cardCode":
		default:
			if (sortOrder == "asc") {
				sortedCardMasters = cardMasters.OrderBy (stringKeySelector).ToDictionary (d => d.Key, d => d.Value);
			} else {
				sortedCardMasters = cardMasters.OrderByDescending (stringKeySelector).ToDictionary (d => d.Key, d => d.Value);
			}
			break;			
		}
		
		var keyList = new List<string> (cardMasters.Keys);
		
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
				if (cardPanelList[i].GetComponent<CollectionData>().cardId == cardId) {
					cardIds[cardIndex++] = cardId;
				}
			}
		}
		int cardCount = 0;
		for (i=0; i<cardPanelList.Count(); i++) {
			cardPanelList[i].GetComponent<CollectionData>().cardId = cardIds[i];
			if (excludeKeyList.Contains(cardIds[i])) {
				cardPanelList[i].SetActive(false);
				continue;
			}
			cardCount++;
		}
		
		listParent.GetComponent<List_Instance> ().Create_Number = cardCount;
		listParent.transform.Find ("ParentList").GetComponent<Slide_Action_Ver2> ().Rewind ();
		keyList = selectedKeyList = excludeKeyList = mergedKeyList = null;
		sortedCardMasters = null;
		DispCardList ();
	}
	
	public void OpenCardDetailPopUp (GameObject obj) {
		if (loadAssetBundle.IsWaitLoading) return;
		
		steward.PlaySETap ();
		loadAssetBundle.SetLoadWait ();

		var cardId = obj.GetComponent<CollectionData> ().cardId;
		cardDetailPopUpWindow.GetComponent<CardDetailPopUpWindow> ().Open (cardId, totalCards [cardId], steward.transform);
	}
}
