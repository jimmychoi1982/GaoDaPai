using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RankPopUpController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	public GameObject header;

	private bool isPointerDown = false;

	public void OnPointerEnter (PointerEventData pointerEventData) {
		header.GetComponent<Header> ().OpenRankPopUp ();
	}
	
	public void OnPointerExit (PointerEventData pointerEventData) {
		if (isPointerDown) return;
		header.GetComponent<Header> ().CloseRankPopUp ();
	}

	public void OnPointerDown (PointerEventData pointerEventData) {
		isPointerDown = true;
		header.GetComponent<Header> ().OpenRankPopUp ();
	}
	
	public void OnPointerUp (PointerEventData pointerEventData) {
		header.GetComponent<Header> ().CloseRankPopUp ();
		isPointerDown = false;
	}
}
