using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardDetailPopUpWindow : MonoBehaviour {
	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Deck deck { get { return Deck.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("CardPopUpWindow"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	private CardMasterManager cardMasterManager = new CardMasterManager();

	private Steward steward;	

	void Start() {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		transform.localPosition = new Vector3 (0f, 0f, 0f);
		transform.localScale = new Vector3 (1f, 1f, 1f);
		gameObject.SetActive (false);
	}

	public void Open (string cardId, Dictionary<string, int> totalCard, Transform parentTransform) {
		var cardGroupIdRarityPair = cardMasterManager.GetCardGroupIdRarityPairs ();

		var blankImage = Resources.Load<Sprite> ("Common/blank");
		
		var rarityStarObject = transform.Find ("CardNamePart/Rarity/Star").gameObject;
		var costIconObject = transform.Find ("CardNamePart/Cost/CostIcon").gameObject;
		var activateIconObject = transform.Find ("CardNamePart/Activate/ActivateIcon").gameObject;
		var expantionCardWindow = transform.Find ("ExpantionCardWindow").gameObject;
	
		foreach (Transform n in transform.Find ("CardNamePart/Rarity")) {
			if (n.name != "Star") Destroy(n.gameObject);
		}
		
		foreach (Transform n in transform.Find ("CardNamePart/Cost")) {
			if (n.name != "CostIcon") Destroy(n.gameObject);
		}
		
		foreach (Transform n in transform.Find ("CardNamePart/Activate")) {
			if (n.name != "ActivateIcon") Destroy(n.gameObject);
		}
		
		transform.localPosition = new Vector3 (0f, 0f, 0f);
		transform.localScale = new Vector3 (1f, 1f, 1f);

		CardMaster cardMaster = cardMasterManager.Get (cardId);
		expantionCardWindow.SetActive (false);
		loadAssetBundle.SetCardImage (cardId, (int)LoadAssetBundle.DisplayType.Card, transform.Find ("CardImage").gameObject);
		transform.Find ("CardImage").GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.Find ("CardImage").GetComponent<Button> ().onClick.AddListener (() => {
			expantionCardWindow.transform.Find ("ExpantionCardImage").GetComponent<Image> ().sprite = transform.Find ("CardImage").GetComponent<Image> ().sprite;
			expantionCardWindow.GetComponent<Button> ().onClick.RemoveAllListeners ();
			expantionCardWindow.GetComponent<Button> ().onClick.AddListener (() => {expantionCardWindow.SetActive (false);});
			expantionCardWindow.SetActive (true);
		});
		if (cardMaster.size.Length > 0) {
			loadAssetBundle.SetSizeIconImage (cardMaster.size, transform.Find ("CardNamePart/SizeIcon").gameObject);
		} else {
			transform.Find ("CardNamePart/SizeIcon").GetComponent<Image> ().sprite = blankImage;
		}
		if (cardMaster.color.Length > 0 && cardMaster.color != "neutral") {
			loadAssetBundle.SetForceIconImage (cardMaster.color, transform.Find ("CardNamePart/ForceIcon").gameObject);
		} else {
			transform.Find ("CardNamePart/ForceIcon").GetComponent<Image> ().sprite = blankImage;
		}
		if (cardMaster.rarity == 101) {
			rarityStarObject.SetActive (false);
		} else {
			rarityStarObject.SetActive (true);
			for (int i=1; i<=cardGroupIdRarityPair[cardMaster.cardGroupId]; i++) {
				var copyObject = Instantiate (rarityStarObject);
				copyObject.transform.SetParent (rarityStarObject.transform.parent);
				copyObject.transform.localPosition = new Vector3 ((float)(rarityStarObject.transform.localPosition.x - (25.6 * (i - 1))), (float)rarityStarObject.transform.localPosition.y, (float)rarityStarObject.transform.localPosition.z);
				copyObject.transform.localScale = rarityStarObject.transform.localScale;
				if (cardMaster.rarity != cardGroupIdRarityPair [cardMaster.cardGroupId]) {
					copyObject.GetComponent<Image> ().color = new Color ((float)242 / (float)255, (float)207 / (float)255, (float)123 / (float)255);
				} else {
					copyObject.GetComponent<Image> ().color = new Color (1f, 1f, 1f);
				}
			}
		}
		if (cardMaster.cost.Count > 0) {
			costIconObject.SetActive (true);
			Vector3 tmpLocalposition = new Vector3 ((float)(costIconObject.transform.localPosition.x + 25.6), (float)costIconObject.transform.localPosition.y, (float)costIconObject.transform.localPosition.z);
			if (cardMaster.cost ["neutral"] > 0) {
				var copyObject = Instantiate (costIconObject);
				loadAssetBundle.SetCostIconImage ("neutral", copyObject.GetComponent<Image> ().gameObject);
				copyObject.transform.SetParent (transform.Find ("CardNamePart/Cost").transform);
				tmpLocalposition = new Vector3 ((float)(tmpLocalposition.x - 25.6), (float)tmpLocalposition.y, (float)tmpLocalposition.z);
				copyObject.transform.localPosition = tmpLocalposition;
				copyObject.transform.localScale = costIconObject.transform.localScale;
				copyObject.transform.Find ("NeutralCount").GetComponent<Text> ().text = cardMaster.cost ["neutral"].ToString ();
			}
			foreach (var kv in cardMaster.cost) {
				if (kv.Key != "neutral" && kv.Value > 0) {
					for (int i=1; i<=kv.Value; i++) {
						var copyObject = Instantiate (costIconObject);
						loadAssetBundle.SetCostIconImage (kv.Key, copyObject.GetComponent<Image> ().gameObject);
						copyObject.transform.SetParent (transform.Find ("CardNamePart/Cost").transform);
						tmpLocalposition = new Vector3 ((float)(tmpLocalposition.x - 25.6), (float)tmpLocalposition.y, (float)tmpLocalposition.z);
						copyObject.transform.localPosition = tmpLocalposition;
						copyObject.transform.localScale = costIconObject.transform.localScale;
					}
				}
			}
		} else {
			costIconObject.SetActive (false);
		}
		if (cardMaster.costActivate.Count > 0) {
			activateIconObject.SetActive (true);
			Vector3 tmpLocalposition = new Vector3 ((float)(activateIconObject.transform.localPosition.x + 25.6), (float)activateIconObject.transform.localPosition.y, (float)activateIconObject.transform.localPosition.z);
			if (cardMaster.costActivate ["neutral"] > 0) {
				var copyObject = Instantiate (activateIconObject);
				loadAssetBundle.SetActivateIconImage ("neutral", copyObject.GetComponent<Image> ().gameObject);
				copyObject.transform.SetParent (transform.Find ("CardNamePart/Activate").transform);
				tmpLocalposition = new Vector3 ((float)(tmpLocalposition.x - 25.6), (float)tmpLocalposition.y, (float)tmpLocalposition.z);
				copyObject.transform.localPosition = tmpLocalposition;
				copyObject.transform.localScale = activateIconObject.transform.localScale;
				copyObject.transform.Find ("NeutralCount").GetComponent<Text> ().text = cardMaster.costActivate ["neutral"].ToString ();
			}
			foreach (var kv in cardMaster.costActivate) {
				if (kv.Key != "neutral" && kv.Value > 0) {
					for (int i=1; i<=kv.Value; i++) {
						var copyObject = Instantiate (activateIconObject);
						loadAssetBundle.SetActivateIconImage (kv.Key, copyObject.GetComponent<Image> ().gameObject);
						copyObject.transform.SetParent (transform.Find ("CardNamePart/Activate").transform);
						tmpLocalposition = new Vector3 ((float)(tmpLocalposition.x - 25.6), (float)tmpLocalposition.y, (float)tmpLocalposition.z);
						copyObject.transform.localPosition = tmpLocalposition;
						copyObject.transform.localScale = activateIconObject.transform.localScale;
					}
				}
			}
		} else {
			activateIconObject.SetActive (false);
		}
		transform.Find ("CardNamePart/CardName").GetComponent<Text> ().text = cardMaster.name;
		transform.Find ("CardNamePart/ModelDesignation").GetComponent<Text> ().text = cardMaster.modelDesignation;
		transform.Find ("CardNamePart/CardType").GetComponent<Text> ().text = deck.staticData ["cardTypes"] [cardMaster.cardTypeId] ["cardTypeClass"].ToString ().Substring (0, deck.staticData ["cardTypes"] [cardMaster.cardTypeId] ["cardTypeClass"].ToString ().LastIndexOf ('C')).ToUpper();
		string characteristicText = "";
		if (cardMaster.characteristicId.Length > 0) {
			foreach (var characteristicId in cardMaster.characteristicId) {
				if (characteristicId.Length > 0 && master.staticData ["characteristics"] [characteristicId] != null) {
					if (characteristicText.Length > 0) characteristicText = characteristicText + " ";
					characteristicText = characteristicText + master.staticData ["characteristics"] [characteristicId] ["jpName"].ToString ();
				}
			}
		}
		transform.Find ("CardNamePart/Characteristic").GetComponent<Text> ().text = characteristicText;
		transform.Find ("CardEffect1Part/Description").GetComponent<Text> ().text = cardMaster.description.Length > 0 ? cardMaster.description[0] : "";
		transform.Find ("CardEffect2Part/Description").GetComponent<Text> ().text = cardMaster.description.Length > 1 ? cardMaster.description[1] : "";
		transform.Find ("OwnCardPart/DigitalCount").GetComponent<Text> ().text = totalCard ["digitalCount"].ToString();
		transform.Find ("OwnCardPart/DigitalLimit").GetComponent<Text> ().text = "20";
		transform.Find ("OwnCardPart/AnalogCount").GetComponent<Text> ().text = totalCard ["analogCount"].ToString();
		transform.Find ("OwnCardPart/AnalogLimit").GetComponent<Text> ().text = cardMaster.cardTypeId == "2" ? "20" : "4";

		gameObject.SetActive (true);
	}

	public void Close () {
		steward.PlaySETap ();
		gameObject.SetActive (false);
	}
}
