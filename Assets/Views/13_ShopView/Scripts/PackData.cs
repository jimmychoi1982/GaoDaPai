using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Newtonsoft.Json.Linq;

public class PackData : MonoBehaviour {
	public string id;
	public string packName;
	public string description;
	public int eventId;
	public int pointId;
	public int count;
	public int amount;
	public bool newFlg;
	
	public void StoreData(JObject jObj) {
		this.id = jObj ["id"].ToString();
		this.packName = jObj ["name"].ToString();
		this.description = jObj ["description"].ToString();
		this.eventId = int.Parse (jObj ["eventId"].ToString());
		this.pointId = int.Parse (jObj ["pointId"].ToString());
		this.count = int.Parse (jObj ["count"].ToString());
		this.amount = int.Parse (jObj["amount"].ToString());
		this.newFlg = bool.Parse (jObj ["newFlg"].ToString());
		if (this.newFlg) {
			transform.Find ("NewLabel").gameObject.SetActive (true);
		}

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("Main Canvas").GetComponent<ShopView>().SelectPack(transform.gameObject);
		});
	}
}
