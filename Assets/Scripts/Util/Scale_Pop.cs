using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

[RequireComponent( typeof( Image ) )]
public class Scale_Pop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private GameObject attachObject;

	[HideInInspector]
	public Vector3 startScale;

	public Image image { get { return GetComponent<Image>(); } }

	public bool isPop = true;
	
	void Awake() {
		this.startScale = this.image.transform.localScale;
	}

	public void OnPointerEnter( PointerEventData eventData ) {
		if (!isPop) return;
		image.transform.DOPunchScale (startScale * 0.1f, 0.3f, 1, 1).SetEase (Ease.Linear);
	}
	
	public void OnPointerExit( PointerEventData eventData ) {
		if (!isPop) return;
		image.transform.DOScale (startScale, 0.2f);
	}
}
