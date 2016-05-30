using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class UserCardManager {
	private User user { get { return User.Instance; }}

	public Dictionary<string, int> GetTotalCounts(string masterName, bool ignoreEmpty = true) {
		Dictionary<string, int> cards = new Dictionary<string, int> ();
		foreach (var obj in user.tCards[masterName]) {
			if (obj is JProperty) {
				var jObj = (obj as JProperty).Value as JObject;
				var cnt = int.Parse (jObj ["analogCount"].ToString ()) + int.Parse (jObj ["digitalCount"].ToString ());
				if (ignoreEmpty && cnt <= 0) continue;
				cards.Add (jObj ["cardId"].ToString (), cnt);
			}
		}
		return cards;
	}
	
	public Dictionary<string, Dictionary<string, int>> GetCounts(string masterName, bool ignoreEmpty = true) {
		Dictionary<string, Dictionary<string, int>> cards = new Dictionary<string, Dictionary<string, int>> ();
		foreach (var obj in user.tCards[masterName]) {
			if (obj is JProperty) {
				var jObj = (obj as JProperty).Value as JObject;
				var cnt = int.Parse (jObj ["analogCount"].ToString ()) + int.Parse (jObj ["digitalCount"].ToString ());
				if (ignoreEmpty && cnt <= 0) continue;
				cards.Add (jObj ["cardId"].ToString (), new Dictionary<string, int>(){
					{"analogCount", int.Parse (jObj ["analogCount"].ToString ())},
					{"digitalCount", int.Parse (jObj ["digitalCount"].ToString ())},
					{"totalCount",cnt}
				});
			}
		}
		return cards;
	}
}
