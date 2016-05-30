using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class UserDeckManager {
	public UserDeck mainDeck { get { return this.Get(int.Parse (user.tUser ["mainDeckId"].ToString ())); } }

	private Deck deck { get { return Deck.Instance; }}
	private User user { get { return User.Instance; }}

	public Dictionary<int, UserDeck> GetAll() {
		Dictionary<int, UserDeck> userDecks = new Dictionary<int, UserDeck> ();
		foreach (var token in deck.tDecks) {
			var obj = token as JObject;
			var deckId = int.Parse (obj["deckId"].ToString());
			userDecks.Add (deckId, this.Get (deckId));
		}

		return userDecks;
	}

	public UserDeck Get(int deckId) {
		foreach (var token in deck.tDecks) {
			var obj = token as JObject;
			if (int.Parse (obj["deckId"].ToString()) == deckId) {
				return new UserDeck(token);
			}
		}
		return null;
	}	
}

public class UserDeck {
	public int deckId;
	public List<string> color = new List<string>();
	public string deckName;
	public string mothershipId;
	public string avatarId;
	public List<string> crewDeck = new List<string>();
	public List<string> unitDeck = new List<string>();
	public string characterSleeveId;
	public string unitSleeveId;
	public string favoriteCharacter;
	public string favoriteUnit;

	public UserDeck(JToken token) {
		var dict = ConvertJson.ConvertJTokenToDictionary(token);
		
		this.deckId =int.Parse (dict["deckId"].ToString());
		foreach (var color in (dict["color"] as List<object>)) {
			this.color.Add(color.ToString());
		}
		this.deckName = dict ["deckName"].ToString ();
		this.mothershipId = dict ["mothershipId"].ToString ();
		this.avatarId = dict ["avatarId"].ToString ();
		foreach (var cardId in (dict["crewDeck"] as List<object>)) {
			this.crewDeck.Add(cardId.ToString());
		}
		foreach (var cardId in (dict["unitDeck"] as List<object>)) {
			this.unitDeck.Add(cardId.ToString());
		}
		this.characterSleeveId = dict ["characterSleeveId"].ToString ();
		this.unitSleeveId = dict ["unitSleeveId"].ToString ();
		this.favoriteCharacter = dict ["favoriteCharacter"].ToString ();
		this.favoriteUnit = dict ["favoriteUnit"].ToString ();
	}
}
