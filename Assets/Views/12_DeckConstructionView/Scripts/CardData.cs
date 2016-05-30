using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardData : MonoBehaviour {
	private string cardId;
	
	public string CardId {
		get { return this.cardId; }
		set { this.cardId = value; }
	}
		
	public void StoreData(string id) {
		this.cardId = id;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView>().OpenCardDetailPopUp(transform.gameObject);
		});
	}
}
