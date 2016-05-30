using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class ProfileEmblemData : MonoBehaviour {
	public string emblemId;
	
	public void StoreData(string id) {
		this.emblemId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas/Panel/ProfileView").GetComponent<ProfileView>().SelectEmblem(transform.gameObject);
		});
	}
}
