using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CardDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public int cardType;
	[HideInInspector] public bool isDraggable = true;
	[HideInInspector] public string prohibitStatus = null;
	private bool isDraggingSelf = false;

	private Vector3 firstPosition;
	private int firstSiblingIndex;
	private Transform firstParent;

	private bool applicationDisabled = false;

	public GameObject obj { get { return GetComponent<GameObject>(); } }

	public void OnBeginDrag(PointerEventData ped) {
		if (GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging) return;
		if (!isDraggable) {
			if (prohibitStatus == "failure") {
				// If prohibitStatus == "failure", this card has some errors, which needs to be corrected.
				GameObject.Find ("/Steward").GetComponent<Steward> ().OpenMessageWindow ("確認", "このカードは、デッキに入れられません\n\n" +
				                                                                         "※現在、不具合が確認されているため\n" +
				                                                                         "対戦モードで使用することができません\n" +
				                                                                         "不具合が解消され次第、使用可能になります", "閉じる", () => {});
			} else if (prohibitStatus == "unsupported") {
				// If prohibitStatus == "unsupported", this card is not yet supported.
				GameObject.Find ("/Steward").GetComponent<Steward> ().OpenMessageWindow ("確認", "このカードは、デッキに入れられません\n\n" +
				                                                                         "※このカードは、対戦モードでの使用が\n" +
				                                                                         "解禁されていません\n" +
				                                                                         "解禁の時期については公式サイトをご確認ください", "閉じる", () => {});
			} else if (prohibitStatus == "token") {
				// If prohibitStatus == "token", this is default prohibit card.
				GameObject.Find ("/Steward").GetComponent<Steward> ().OpenMessageWindow ("確認", "このカードは、デッキに入れられません", "閉じる", () => {});
			} else {
				GameObject.Find ("/Steward").GetComponent<Steward> ().OpenMessageWindow ("確認", "これ以上、デッキに入れられません", "閉じる", () => {});
			}
			return;
		}
		GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging = true;
		isDraggingSelf = true;
		firstPosition = gameObject.transform.localPosition;
		firstSiblingIndex = gameObject.transform.GetSiblingIndex ();
		firstParent = gameObject.transform.parent;
		if (gameObject.GetComponent<Button> () != null) {
			gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
		gameObject.transform.SetParent(GameObject.Find ("/Main Canvas/Panel").transform);
		gameObject.transform.SetSiblingIndex (int.MaxValue);
	}

	public void OnDrag(PointerEventData ped) {
		if (GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging && !isDraggingSelf) return;
		if (!isDraggable) return;
		Vector3 worldPos = Vector3.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), ped.position, Camera.main, out worldPos);
		gameObject.transform.position = worldPos;
	}

	public void OnEndDrag(PointerEventData ped) {
		if (GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging && !isDraggingSelf) return;
		if (!isDraggable) return;
		GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().ReceiveDragDrop (gameObject, firstPosition, firstParent, firstSiblingIndex);
		if (cardType == 1) {
			gameObject.GetComponent<CardData> ().StoreData (gameObject.GetComponent<CardData> ().CardId);
		}
		isDraggingSelf = false;
		GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging = false;
	}

	void OnApplicationPause (bool pause) {
		if (pause && isDraggingSelf) {
			transform.SetParent(firstParent);
			transform.SetSiblingIndex (firstSiblingIndex);
			transform.localPosition = firstPosition;
			if (cardType == 1) {
				gameObject.GetComponent<CardData> ().StoreData (gameObject.GetComponent<CardData> ().CardId);
			}
			isDraggingSelf = false;
			GameObject.Find ("/Main Canvas").GetComponent<DeckConstructionView> ().IsDragging = false;
			applicationDisabled = false;
		}
	}
}
