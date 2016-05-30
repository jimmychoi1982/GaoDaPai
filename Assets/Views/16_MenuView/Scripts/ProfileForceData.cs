using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class ProfileForceData : MonoBehaviour {
	public string forceId;
	
	public void StoreData(string id) {
		this.forceId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas/Panel/ProfileView").GetComponent<ProfileView>().SelectForce(transform.gameObject);
		});
	}
}
