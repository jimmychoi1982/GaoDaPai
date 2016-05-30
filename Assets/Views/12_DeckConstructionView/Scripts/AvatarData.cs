using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class AvatarData : MonoBehaviour {
	public string avatarId;
	
	public void StoreData(string id) {
		this.avatarId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().SelectAvatar (transform.gameObject);
		});
	}
}
