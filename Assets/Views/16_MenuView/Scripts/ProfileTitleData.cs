using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class ProfileTitleData : MonoBehaviour {
	public string titleId;
	
	public void StoreData(string id) {
		this.titleId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas/Panel/ProfileView").GetComponent<ProfileView>().SelectTitle (transform.parent.gameObject);
		});
	}
}
