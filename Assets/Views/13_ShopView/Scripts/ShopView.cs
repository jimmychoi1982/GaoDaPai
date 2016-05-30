using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using Newtonsoft.Json.Linq;

public class ShopView : MonoBehaviour {
	public GameObject gashaAnimation;
	public GameObject animationManager;
	public GameObject[] GashaArea = new GameObject[2];
	public GameObject[] ShopCardList = new GameObject[2];
	public Image[] RarityTabImage = new Image[2];
	public Sprite[] ChangeButtonImage = new Sprite[4];
	
	public Text topPackName;
	public Text haveBronzeCpNum;
	public Text haveSliverCPNum;

	private int prevSilverCoin;

	private bool isDraw = false;
	
	private Dictionary<string, JObject> nomalGashas = new Dictionary<string, JObject>();
	private Dictionary<string, JObject> rareGashas = new Dictionary<string, JObject>();

	private enum GashaType {
		Normal,
		Rare,
	}
	
	private GashaType gashaType;
	private string selectedGashaId;
	private PackData packData;

	private GameObject normalFirstObject;
	private GameObject rareFirstObject;
	private Dictionary<string, string> cardNames = new Dictionary<string, string> ();

	private List<string> throwawayCards = new List<string>();

	private Mage mage { get { return Mage.Instance; }}
	private Purchase purchase { get { return Purchase.Instance; }}
	private User user { get { return User.Instance; }}
	private Logger logger { get { return mage.logger("ShopView"); } }
	
	private CardMasterManager cardMasterManager = new CardMasterManager();
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;
	
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		cardNames = cardMasterManager.GetCardIdNamePairs();
		gashaAnimation.SetActive (false);

		foreach (JProperty prop in purchase.staticData ["gashas"]) {
			var jObj = prop.Value as JObject;
			if (jObj["pointId"].ToString () == "1") {
				nomalGashas.Add (jObj["id"].ToString (), jObj);
			} else {
				rareGashas.Add (jObj["id"].ToString (), jObj);
			}
		}

		GashaArea [0].SetActive (false);

		var index = 0;
		var normalListInstance = ShopCardList[0].GetComponent<List_Instance> ();
		normalListInstance.Create_Number = nomalGashas.Count;
		normalListInstance.CreateInstance ();

		GashaArea [0].SetActive (true);
		var normalPanels = GameObject.FindGameObjectsWithTag ("NormalGasha_List");
		index = 0;
		foreach (var normalGasha in nomalGashas) {
			loadAssetBundle.SetGashaImage(normalGasha.Key, normalPanels[index].GetComponent<Image>().gameObject);
			normalPanels[index].GetComponent<PackData>().StoreData(normalGasha.Value);
			if (index == 0) {
				normalFirstObject = normalPanels[index];
			}
			index++;
		}
		GashaArea [0].SetActive (false);

		var rareListInstance = ShopCardList[1].GetComponent<List_Instance> ();
		rareListInstance.Create_Number = rareGashas.Count;
		rareListInstance.CreateInstance ();

		GashaArea [1].SetActive (true);
		var rarePanels = GameObject.FindGameObjectsWithTag ("RareGasha_List");
		index = 0;
		foreach (var rareGasha in rareGashas) {
			loadAssetBundle.SetGashaImage(rareGasha.Key, rarePanels[index].GetComponent<Image>().gameObject);
			rarePanels[index].GetComponent<PackData>().StoreData(rareGasha.Value);
			if (index == 0) {
				rareFirstObject = rarePanels[index];
			}
			index++;
		}
		GashaArea [1].SetActive (false);

		ChangeUserData ();

		OpenNormalTab (false);
	}
	
	public void OpenNormalTab (bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		
		GashaArea [0].SetActive (true);
		GashaArea [1].SetActive (false);
		
		RarityTabImage [0].sprite = ChangeButtonImage [0];
		RarityTabImage [1].sprite = ChangeButtonImage [1];
		RarityTabImage [0].gameObject.transform.Find ("Text").GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
		RarityTabImage [1].gameObject.transform.Find ("Text").GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0.5f);

		gashaType = GashaType.Normal;
		
		SelectPack (normalFirstObject);
	}
	
	public void OpenRareTab (bool playSE = true) {
		if (playSE) {
			steward.PlaySETap ();
		}
		
		GashaArea [0].SetActive (false);
		GashaArea [1].SetActive (true);
		
		RarityTabImage [0].sprite = ChangeButtonImage [2];
		RarityTabImage [1].sprite = ChangeButtonImage [3];
		RarityTabImage [0].gameObject.transform.Find ("Text").GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0.5f);
		RarityTabImage [1].gameObject.transform.Find ("Text").GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);

		gashaType = GashaType.Rare;
		
		SelectPack (rareFirstObject);
	}

	public void SelectPack (GameObject obj) {
		foreach (Transform n in obj.transform.parent.transform) {
			n.Find ("Select_Enclose").gameObject.SetActive (false);
		}
		obj.transform.Find ("Select_Enclose").gameObject.SetActive(true);
		packData = obj.GetComponent<PackData> ();
		selectedGashaId = packData.id;
		topPackName.text = packData.packName;
	}

	public void Confirm () {
		steward.PlaySETap ();
		string coinLabel = "";
		string prevCoin = "";
		int calcurateResult = 0;
		if (gashaType == GashaType.Normal) {
			coinLabel = "所持BC";
			prevCoin = user.tUser ["bronzeCoin"].ToString ();
			calcurateResult = int.Parse (user.tUser ["bronzeCoin"].ToString ()) - packData.amount;
			prevSilverCoin = int.Parse (user.tUser ["silverCoin"].ToString ());
		} else {
			coinLabel = "所持SC";
			prevCoin = user.tUser ["silverCoin"].ToString ();
			calcurateResult = int.Parse (user.tUser ["silverCoin"].ToString ()) - packData.amount;
			prevSilverCoin = calcurateResult;
		}
		if (calcurateResult >= 0) {
			var confirmWindow = steward.OpenCustomWindow("GashaConfirmWindow", "購入", () => {Buy ();}, "キャンセル", () => {});

			confirmWindow.SetText ("PackName", "【" + packData.packName + "】");
			confirmWindow.SetText ("DrawCount", packData.count.ToString() + "枚");
			confirmWindow.SetText ("CoinLabel", coinLabel);
			confirmWindow.SetText ("PrevCoin", prevCoin);
			confirmWindow.SetText ("AfterCoin", calcurateResult.ToString ());
		} else {
			steward.OpenMessageWindow("エラー", "【" + coinLabel + "】が不足しています", "閉じる", () => {});
		}
	}
	
	public void Buy () {
		if (isDraw) return;
		isDraw = true;

		steward.SetNewTask (purchase.doGasha (selectedGashaId, (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error registerSerial:" + e.Message);
				steward.OpenExceptionPopUpWindow ();
				return;
			}

			if (steward.ResponseHasErrorCode(result)) return;

			steward.HideHeader ();
			gashaAnimation.SetActive(true);
			GasyaAnimationConnect animConnect = animationManager.GetComponent<GasyaAnimationConnect>();

			int index = 1;
			foreach (var card in result["cards"]) {
				if (card is JObject) {
					var JObj = card as JObject;
					loadAssetBundle.SetCardImageFromCardCode (JObj["cardCode"].ToString(), (int)LoadAssetBundle.DisplayType.Card, gashaAnimation.transform.Find ("Get/CardBox/GetCard" + index + "/Card/CardImage").GetComponent<Image>().gameObject);
					loadAssetBundle.SetCardImageFromCardCode (JObj["cardCode"].ToString(), (int)LoadAssetBundle.DisplayType.Card, gashaAnimation.transform.Find ("Result/ResultBox/GetCards/card" + index + "/cardImage").GetComponent<Image>().gameObject);
					loadAssetBundle.SetCardImageFromCardCode (JObj["cardCode"].ToString(), (int)LoadAssetBundle.DisplayType.Card, gashaAnimation.transform.Find ("Get/CardBox/GetCard" + index + "/CardImage_Copy").GetComponent<Image>().gameObject);
					CardMaster cardMaster = cardMasterManager.Get(JObj["cardId"].ToString());
					if (bool.Parse(JObj["isThrowaway"].ToString())) {
						throwawayCards.Add (JObj["cardId"].ToString());
					}
					switch (index) {
					case 1:
						animConnect.Card1_New = bool.Parse(JObj["isNew"].ToString());
						animConnect.Card1_Rare = (cardMaster.rarity >= 4);
						break;
					case 2:
						animConnect.Card2_New = bool.Parse(JObj["isNew"].ToString());
						animConnect.Card2_Rare = (cardMaster.rarity >= 4);
						break;
					case 3:
						animConnect.Card3_New = bool.Parse(JObj["isNew"].ToString());
						animConnect.Card3_Rare = (cardMaster.rarity >= 4);
						break;
					}
					animConnect.getCardNum = index;
					index++;
				}
			}

			animConnect.cardsGetAnimationStart = true;

			ChangeUserData ();
			Resources.UnloadUnusedAssets ();
		}));
	}
	
	public void EndAnimation () {
		steward.PlaySEOK ();
		steward.ShowHeader ();
		animationManager.GetComponent<GasyaAnimationScript> ().Reset_Start = true;
		gashaAnimation.SetActive (false);

		if (throwawayCards.Count > 0) {
			var surplusCardWindow = steward.OpenCustomWindow("SurplusCardWindow", "閉じる", () => {});
			
			surplusCardWindow.SetText ("PrevCoin", prevSilverCoin.ToString ());
			surplusCardWindow.SetText ("AfterCoin", user.tUser ["silverCoin"].ToString ());
			string cardNameText = "";
			foreach (var throwawayCardId in throwawayCards) {
				if (cardNameText.Length > 0) cardNameText = cardNameText + "\n";
				cardNameText = cardNameText + cardNames[throwawayCardId];
			}
			surplusCardWindow.SetText ("CardName", cardNameText);

			throwawayCards = new List<string> ();
		}
	}

	private void ChangeUserData () {
		haveBronzeCpNum.text = user.tUser ["bronzeCoin"].ToString ();
		haveSliverCPNum.text = user.tUser ["silverCoin"].ToString ();
		isDraw = false;
	}
}
