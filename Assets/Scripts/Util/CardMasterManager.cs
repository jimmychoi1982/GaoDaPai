using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public class CardMasterManager {
	public Dictionary<string, CardMaster> CardMasters = new Dictionary<string, CardMaster>();

	private string[] cardMasterNames = new string[] {"cardsPilot", "cardsCrew", "cardsUnit", "cardsEvent", "cardsCounter"};

	private Card card { get { return Card.Instance; }}

	public CardMaster Get(string cardId) {
		if (CardMasters.ContainsKey (cardId)) return CardMasters [cardId];

		foreach (var cardMasterName in cardMasterNames) {
			foreach (var prop in card.staticData[cardMasterName]) {
				if (prop is JProperty) {
					var token = (prop as JProperty).Value;
					if ((token as JObject)["cardId"].ToString() == cardId) {
						CardMaster cardMaster = new CardMaster(token);
						CardMasters.Add (cardMaster.cardId, cardMaster);
						return cardMaster;
					}
				}
			}
		}
		return null;
	}

	public Dictionary<string, string> GetCardIdCodePairs() {
		Dictionary<string, string> cardIdCodePair = new Dictionary<string, string> ();
		foreach (var cardMasterName in cardMasterNames) {
			foreach (var prop in card.staticData[cardMasterName]) {
				if (prop is JProperty) {
					var obj = ((prop as JProperty).Value) as JObject;
					cardIdCodePair.Add (obj["cardId"].ToString(), obj["cardCode"].ToString());
				}
			}
		}
		return cardIdCodePair;
	}
	
	public Dictionary<string, string> GetCardIdNamePairs() {
		Dictionary<string, string> cardIdNamePair = new Dictionary<string, string> ();
		foreach (var cardMasterName in cardMasterNames) {
			foreach (var prop in card.staticData[cardMasterName]) {
				if (prop is JProperty) {
					var obj = ((prop as JProperty).Value) as JObject;
					cardIdNamePair.Add (obj["cardId"].ToString(), obj["name"].ToString());
				}
			}
		}
		return cardIdNamePair;
	}

	public Dictionary<string, int> GetCardGroupIdRarityPairs() {
		Dictionary<string, int> cardGroupIdRarityPair = new Dictionary<string, int> ();
		foreach (var cardMasterName in cardMasterNames) {
			foreach (var prop in card.staticData[cardMasterName]) {
				if (prop is JProperty) {
					var obj = ((prop as JProperty).Value) as JObject;
					if (cardGroupIdRarityPair.ContainsKey(obj["cardGroupId"].ToString())) {
						if (cardGroupIdRarityPair[obj["cardGroupId"].ToString()] > int.Parse(obj["rarity"].ToString())) {
							cardGroupIdRarityPair[obj["cardGroupId"].ToString()] = int.Parse(obj["rarity"].ToString());
						}
					} else {
						cardGroupIdRarityPair.Add (obj["cardGroupId"].ToString(), int.Parse(obj["rarity"].ToString()));
					}
				}
			}
		}
		return cardGroupIdRarityPair;
	}
}

public class CardMaster {
	public string cardId;
	public string cardGroupId;
	public string cardTypeId;
	public string cardCode;
	public string countryCode;
	public string name;
	public int cardLimit;
	public string[] packId = new string[0];
	public string color = "";
	public int rarity;
	public string size = "";
	public Dictionary<string, int> cost = new Dictionary<string, int> ();
	public Dictionary<string, int> costActivate = new Dictionary<string, int> ();
	public string[] characteristicId = new string[0];
	public string[] description = new string[0];
	public string[] effectId = new string[0];
	public string modelDesignation = "";
	public int atk;
	public int def;

	public CardMaster(JToken token) {
		var dict = ConvertJson.ConvertJTokenToDictionary(token);

		this.cardId = dict ["cardId"].ToString();
		this.cardGroupId = dict ["cardGroupId"].ToString();
		this.cardTypeId = dict ["cardTypeId"].ToString();
		this.cardCode = dict ["cardCode"].ToString();
		this.countryCode = dict ["countryCode"].ToString();
		this.name = dict ["name"].ToString();
		this.cardLimit = int.Parse (dict["cardLimit"].ToString());
		if (dict.ContainsKey ("packId") && dict["packId"].GetType() == typeof(List<object>)) {
			this.packId = new string[(dict["packId"] as List<object>).Count];
			var index = 0;
			foreach(var obj in (dict["packId"] as List<object>)) {
				this.packId [index] = obj.ToString ();
				index++;
			}
		}
		if (dict.ContainsKey ("color")) {
			this.color = dict ["color"].ToString ();
		}
		this.rarity = int.Parse (dict ["rarity"].ToString());
		if (dict.ContainsKey ("size")) {
			this.size = dict ["size"].ToString ();
		}
		if (dict.ContainsKey ("cost") && dict["cost"].GetType() == typeof(Dictionary<string, object>)) {
			if ((dict["cost"] as Dictionary<string, object>).ContainsKey("play") && (dict["cost"] as Dictionary<string, object>).ContainsKey("activate")) {
				foreach (var kv in ((dict["cost"] as Dictionary<string, object>)["play"] as Dictionary<string, object>)) {
					cost.Add(kv.Key, int.Parse (kv.Value.ToString()));
				}
				foreach (var kv in ((dict["cost"] as Dictionary<string, object>)["activate"] as Dictionary<string, object>)) {
					costActivate.Add(kv.Key, int.Parse (kv.Value.ToString()));
				}
			} else {
				foreach (var kv in (dict["cost"] as Dictionary<string, object>)) {
					cost.Add(kv.Key, int.Parse (kv.Value.ToString()));
				}
			}
		}
		if (dict.ContainsKey ("characteristicId") && dict["characteristicId"].GetType() == typeof(List<object>)) {
			this.characteristicId = new string[(dict["characteristicId"] as List<object>).Count];
			var index = 0;
			foreach(var obj in (dict["characteristicId"] as List<object>)) {
				this.characteristicId [index] = obj.ToString ();
				index++;
			}
		}
		if (dict.ContainsKey ("description") && dict["description"].GetType() == typeof(List<object>)) {
			this.description = new string[(dict["description"] as List<object>).Count];
			var index = 0;
			foreach(var obj in (dict["description"] as List<object>)) {
				this.description [index] = obj.ToString ().Replace("<br />", '\n'.ToString());
				index++;
			}
		}
		if (dict.ContainsKey ("effectId") && dict["effectId"].GetType() == typeof(List<object>)) {
			this.effectId = new string[(dict["effectId"] as List<object>).Count];
			var index = 0;
			foreach(var obj in (dict["effectId"] as List<object>)) {
				this.effectId [index] = obj.ToString ();
				index++;
			}
		}
		if (dict.ContainsKey ("modelDesignation")) {
			this.modelDesignation = dict ["modelDesignation"].ToString ();
		}
		if (dict.ContainsKey ("atk")) {
			this.atk = int.Parse (dict ["atk"].ToString ());
		}
		if (dict.ContainsKey ("def")) {
			this.def = int.Parse (dict ["def"].ToString ());
		}
	}
}
