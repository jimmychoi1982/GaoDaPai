using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionData : MonoBehaviour {
	public string cardId;

	public void StoreData(string cardId) {
		this.cardId = cardId;

		transform.gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.gameObject.GetComponent<Button> ().onClick.AddListener (() => {
			GameObject.Find ("/Main Canvas/Panel/CollectionView").GetComponent<CollectionView>().OpenCardDetailPopUp(transform.gameObject);
		});
	}
}
