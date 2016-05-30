using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class MothershipData : MonoBehaviour {
	public string mothershipId;
	
	public void StoreData(string id) {
		this.mothershipId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().SelectMothership (transform.gameObject);
		});
	}
}
