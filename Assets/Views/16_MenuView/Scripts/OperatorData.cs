using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class OperatorData : MonoBehaviour {
	public string operatorId;
	
	public void StoreData(string id) {
		this.operatorId = id;
		
		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas").GetComponent<MenuView>().SelectOperator (transform.parent.gameObject);
		});
	}
}
