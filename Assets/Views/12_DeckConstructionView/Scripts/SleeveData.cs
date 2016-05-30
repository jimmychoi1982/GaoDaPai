using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class SleeveData : MonoBehaviour {
	public string sleeveId;
	public string sleeveType;

	public void StoreData(string id, string sleeveType) {
		this.sleeveId = id;
		this.sleeveType = sleeveType;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			if (sleeveType == "unit") {
				GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().SelectUnitSleeve (transform.gameObject);
			} else {
				GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().SelectCharacterSleeve (transform.gameObject);
			}
		});
	}
}
