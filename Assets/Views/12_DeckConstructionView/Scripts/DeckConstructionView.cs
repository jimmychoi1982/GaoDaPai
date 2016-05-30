using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public partial class DeckConstructionView : MonoBehaviour {
	public GameObject deckView;
	public GameObject unitCardView;
	public GameObject characterCardView;

	public GameObject[] msEditCategory = new GameObject[3];
	public GameObject[] costEditCategory = new GameObject[2];

	public GameObject[] msEditCursor = new GameObject[3];
	public GameObject[] costEditCursor = new GameObject[2];
	
	public GameObject[] favouriteCard = new GameObject[2];
	public GameObject[] deckEnterButton = new GameObject[2];

	public GameObject[] BlockButtons = new GameObject[2];

	public GameObject avatarPrefab;
	public GameObject mothershipPrefab;
	public GameObject unitSleevePrefab;
	public GameObject characterSleevePrefab;

	public GameObject deckList;

	public Image mainDeckButton;

	public Image[] categolyButton = new Image[5];
	public Sprite[] changeButtons = new Sprite[2];
	public Text[] categolyLabel = new Text[5];
	public Text[] categolyText = new Text[4];

	private InputField deckNameInputField;

	private bool isDragging = false;
	public bool IsDragging {
		get { return this.isDragging; }
		set { this.isDragging = value; }
	}

	private PopUpWindow avatarWindow;
	private PopUpWindow mothershipWindow;
	private PopUpWindow unitSleeveWindow;
	private PopUpWindow characterSleeveWindow;
	private PopUpWindow deckNameInputWindow;

	private SortPopUpWindow sortPopUpWindow;
	private GameObject cardDetailPopUpWindow;

	private bool editNoSave = false;
	public bool EditNoSave {
		get { return this.editNoSave; }
	}
	
	private bool useBanCard;
	private bool inDeckLimitOver;

	private Dictionary<int, UserDeck> userDecks;
	private List<string> userAvatars = new List<string> ();
	private List<string> userMotherships = new List<string> ();
	private List<string> userSleeves = new List<string> ();
	private Dictionary<string, int> unitCards = new Dictionary<string, int> ();
	private Dictionary<string, int> eventCards = new Dictionary<string, int> ();
	private Dictionary<string, int> counterCards = new Dictionary<string, int> ();
	private Dictionary<string, int> crewCards = new Dictionary<string, int> ();
	private Dictionary<string, int> pilotCards = new Dictionary<string, int> ();
	private Dictionary<string, int> unitTotalList = new Dictionary<string, int> ();
	private Dictionary<string, int> crewTotalList = new Dictionary<string, int> ();
	private Dictionary<string, int> unitVaultList = new Dictionary<string, int> ();
	private Dictionary<string, int> eventVaultList = new Dictionary<string, int> ();
	private Dictionary<string, int> counterVaultList = new Dictionary<string, int> ();
	private Dictionary<string, int> crewVaultList = new Dictionary<string, int> ();
	private Dictionary<string, int> pilotVaultList = new Dictionary<string, int> ();
	private Dictionary<string, int> deckCardCount = new Dictionary<string, int> ();
	private Dictionary<string, Dictionary<string, int>> totalCards = new Dictionary<string, Dictionary<string, int>> ();
	private Dictionary<string, string> cardNames = new Dictionary<string, string> ();
	private Dictionary<string, CardMaster> cardMasters = new Dictionary<string, CardMaster> ();
	private Dictionary<string, string> prohibitCards = new Dictionary<string, string> ();
	private GameObject[] deckPlates;
	private int currentDeckId;
	private string currentAvatarId;
	private string currentMothershipId;
	private string currentUnitSleeveId;
	private string currentCharacterSleeveId;

	private bool unitDeckReady = false;
	private bool crewDeckReady = false;

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
	private bool isFirstSort = true;

	private Sprite mainDeckOn;
	private Sprite mainDeckOff;
	private int mainDeckId;

	private Sprite blankImage;

	private enum ImageType {
		Card,
		Avatar,
		Mothership,
		Sleeve,
	}

	private int unitDeckLimit;
	private int crewDeckLimit;

	private Dictionary<string, int> cardGroupIdRarityPair = new Dictionary<string, int>();

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("DeckConstructionView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private CardMasterManager cardMasterManager = new CardMasterManager();
	private UserDeckManager userDeckManager = new UserDeckManager();
	private UserCardManager userCardManager = new UserCardManager();

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		cardNames = cardMasterManager.GetCardIdNamePairs();

		userDecks = userDeckManager.GetAll ();

		unitDeckLimit = Deck.unitDeckLimit;
		crewDeckLimit = Deck.crewDeckLimit;

		foreach (var obj in user.tAvatars["avatarId"]) {
			if (obj is JToken) {
				userAvatars.Add(obj.ToString());
			}
		}

		foreach (var obj in user.tMotherships["mothershipId"]) {
			if (obj is JToken) {
				userMotherships.Add(obj.ToString());
			}
		}
		
		foreach (var obj in user.tSleeves["sleeveId"]) {
			if (obj is JToken) {
				userSleeves.Add(obj.ToString());
			}
		}

		foreach (JProperty prop in master.staticData["prohibitCards"]) {
			prohibitCards.Add (prop.Name, (prop.Value as JObject) ["status"].ToString ());
		}

		unitCards = userCardManager.GetTotalCounts ("unitCard");
		eventCards = userCardManager.GetTotalCounts ("eventCard");
		counterCards = userCardManager.GetTotalCounts ("counterCard");
		unitTotalList = unitCards.Concat (eventCards).Concat (counterCards).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);

		crewCards = userCardManager.GetTotalCounts ("crewCard");
		pilotCards = userCardManager.GetTotalCounts ("pilotCard");
		crewTotalList = crewCards.Concat (pilotCards).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);

		totalCards = userCardManager.GetCounts ("unitCard").Concat (userCardManager.GetCounts ("eventCard")).Concat (userCardManager.GetCounts ("counterCard")).Concat (userCardManager.GetCounts ("crewCard")).Concat (userCardManager.GetCounts ("pilotCard")).GroupBy (d => d.Key).ToDictionary (d => d.Key, d => d.First ().Value);

		cardGroupIdRarityPair = cardMasterManager.GetCardGroupIdRarityPairs ();

		var totalList = unitTotalList.Concat(crewTotalList).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
		foreach (var kv in totalList) {
			var currentCardMaster = cardMasterManager.Get (kv.Key);
			cardMasters.Add (kv.Key, currentCardMaster);
			if (!deckCardCount.ContainsKey(currentCardMaster.cardGroupId)) {
				deckCardCount.Add (currentCardMaster.cardGroupId, 0);
			}
		}

		mainDeckOn = Instantiate(Resources.Load<Sprite> ("Card_Floder/deck_button_3"));
		mainDeckOff = Instantiate(Resources.Load<Sprite> ("Card_Floder/deck_button_4"));
		mainDeckId = int.Parse (user.tUser ["mainDeckId"].ToString ());

		// Create Deck List
		deckList.GetComponent<List_Instance> ().Create_Number = userDecks.Count;
		deckList.GetComponent<List_Instance> ().CreateInstance ();
		deckPlates = deckList.GetComponent<List_Instance> ().GetCreateObjects ();

		// Set Deck View
		var selectedDeckId = mainDeckId;
		if (steward.SelectedDeckId >= 0) {
			selectedDeckId = steward.SelectedDeckId;
			steward.SelectedDeckId = -1;
		}
		SetDeckView (selectedDeckId);

		// Set Force Color
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.LeftTop, deckView.transform.Find ("BackgroundLeft/deck_board_b1").GetComponent<Image> ().gameObject);
		loadAssetBundle.SetForcePlateImage (user.tUser ["color"].ToString(), (int)LoadAssetBundle.ForcePlateType.RightBottom, deckView.transform.Find ("BackgroundLeft/deck_board_b2").GetComponent<Image> ().gameObject);

		// Create Avatar/Mothership/Sleeve Window
		avatarWindow = SetPopUpWindow (avatarPrefab, userAvatars, ImageType.Avatar);
		mothershipWindow = SetPopUpWindow (mothershipPrefab, userMotherships, ImageType.Mothership);
		unitSleeveWindow = SetPopUpWindow (unitSleevePrefab, userSleeves, ImageType.Sleeve, "unit");
		characterSleeveWindow = SetPopUpWindow (characterSleevePrefab, userSleeves, ImageType.Sleeve, "character");

		// Create Card Detail Pop Up Window
		cardDetailPopUpWindow = PopUpWindowManager.InitAsGameObject ("Common/Prefabs/CardDetailPopUpWindow", steward.transform);
				
		// Create Sort Pop Up Window
		sortPopUpWindow = PopUpWindowManager.InitAsGameObject ("Common/Prefabs/SortPopUpWindow", steward.transform).GetComponent<SortPopUpWindow> ();
		sortPopUpWindow.Init (() => {
			sortPopUpWindow.SetExecAction (() => {ExecSort ();});
		});

		blankImage = Resources.Load<Sprite> ("Common/blank");
	}

	void OnDestroy () {
		Destroy (cardDetailPopUpWindow);
		Destroy (sortPopUpWindow.gameObject);
	}

	private void SetDeckView (int deckId) {
		var currentDeck = userDecks [deckId];

		currentDeckId = deckId;
		currentAvatarId = currentDeck.avatarId;
		currentMothershipId = currentDeck.mothershipId;
		currentUnitSleeveId = currentDeck.unitSleeveId;
		currentCharacterSleeveId = currentDeck.characterSleeveId;
		
		var index = 0;
		foreach (var kv in userDecks) {
			var userDeck = kv.Value;
			deckPlates [index].GetComponent<DeckData> ().Init (kv.Value, "deck", currentDeckId);
			index++;
		}

		loadAssetBundle.SetLoadWait ();
		loadAssetBundle.SetCardImage (currentDeck.favoriteUnit, (int)LoadAssetBundle.DisplayType.Card, deckView.transform.Find ("UnitDeck").gameObject);
		loadAssetBundle.SetCardImage (currentDeck.favoriteCharacter, (int)LoadAssetBundle.DisplayType.Card, deckView.transform.Find ("CharacterDeck").gameObject);
		loadAssetBundle.SetAvatarImage (currentAvatarId, deckView.transform.Find ("Avater").gameObject);
		loadAssetBundle.SetMothershipImage (currentMothershipId, deckView.transform.Find ("Mothership").gameObject);
		loadAssetBundle.SetSleeveImage (currentUnitSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckView.transform.Find ("UnitSleve").gameObject);
		loadAssetBundle.SetSleeveImage (currentCharacterSleeveId, (int)LoadAssetBundle.DisplayType.Icon, deckView.transform.Find ("CharacterSleve").gameObject);
		
		if (deckId == mainDeckId) {
			deckView.transform.Find ("MainDeckButton").GetComponent<Image> ().sprite = mainDeckOn;
			deckView.transform.Find ("MainDeckButton").GetComponent<Scale_Pop> ().isPop = false;
		} else {
			deckView.transform.Find ("MainDeckButton").GetComponent<Image> ().sprite = mainDeckOff;
			deckView.transform.Find ("MainDeckButton").GetComponent<Scale_Pop> ().isPop = true;
		}
		
		deckView.transform.Find ("DeckNameWindow/DeckName").GetComponent<Text> ().text = currentDeck.deckName;

		unitDeckReady = false;
		crewDeckReady = false;
		Resources.UnloadUnusedAssets ();
	}

	private PopUpWindow SetPopUpWindow (GameObject prefab, List<string> idList, ImageType imageType, string sleeveType = "") {
		var popUpWindow = steward.InitCustomWindow ("SelectWindow");
		List_Instance listInstance = popUpWindow.GetObject ("ListArea/List").GetComponent<List_Instance> ();
		listInstance.Create_Object = prefab;
		listInstance.Create_Number = idList.Count;
		listInstance.CreateInstance();
		var listPanel = listInstance.GetCreateObjects ();
		var index = 0;
		foreach (var id in idList) {
			switch (imageType) {
			case ImageType.Avatar:
				loadAssetBundle.SetAvatarImage (id, listPanel[index].GetComponent<Image>().gameObject);
				listPanel[index].GetComponent<AvatarData>().StoreData(id);
				break;
				
			case ImageType.Mothership:
				loadAssetBundle.SetMothershipImage (id, listPanel[index].GetComponent<Image>().gameObject);
				listPanel[index].GetComponent<MothershipData>().StoreData(id);
				break;
				
			case ImageType.Sleeve:
				loadAssetBundle.SetSleeveImage (id, (int)LoadAssetBundle.DisplayType.Card, listPanel[index].GetComponent<Image>().gameObject);
				listPanel[index].GetComponent<SleeveData>().StoreData(id, sleeveType);
				break;
			}
			index++;
		}
		
		return popUpWindow;
	}

	public void Back(bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		if (deckView.activeInHierarchy) {
			steward.LoadNextScene ("HomeView");
		} else {
			if (editNoSave == true) {
				steward.OpenDialogWindow ("確認", "編集中の情報は破棄されます\n" +
					"よろしいですか？", "破棄", () => {editNoSave = false;Back (false);}, "キャンセル", () => {});
			} else {
				unitCardView.SetActive (true);
				foreach (Transform n in unitCardView.transform.Find ("ScrollView/DeckContent")) {
					Destroy (n.gameObject);
				}
				unitCardView.SetActive (false);

				characterCardView.SetActive (false);
				foreach (Transform n in characterCardView.transform.Find ("ScrollView/DeckContent")) {
					Destroy (n.gameObject);
				}
				characterCardView.SetActive (false);

				sortPopUpWindow.Reset ();
				isFirstSort = true;

				deckView.SetActive (true);
				SetDeckView (currentDeckId);
			}
		}
	}
}