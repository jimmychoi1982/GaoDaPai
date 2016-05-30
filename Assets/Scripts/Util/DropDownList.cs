using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DropDownList : MonoBehaviour {
	public GameObject dropDownBox;
	public GameObject scrollView;
	public GameObject scrollBar;
	public GameObject itemBox;
	public GameObject dropDownFlame;
	private Dictionary<string, string> dropDownItems;
	private float boxHeight;
	private float boxWidth;

	public DropDownList SetList(Dictionary<string, string> items, float dropWidth, int dropSize, string refineType, Action<GameObject> callback) {
		this.dropDownItems = items;
		RectTransform dropDownBoxRect = dropDownBox.transform as RectTransform;
		RectTransform scrollViewRect = scrollView.transform as RectTransform;
		RectTransform scrollBarRect = scrollBar.transform as RectTransform;
		RectTransform dropDownFlameRect = dropDownFlame.transform as RectTransform;
		dropSize = dropSize > items.Count ? items.Count : dropSize;
		boxHeight = (25f * dropSize) + 5f;
		boxWidth = dropWidth;
		Vector2 boxSize = new Vector2 (boxWidth, boxHeight);
		dropDownBoxRect.sizeDelta = boxSize;
		scrollViewRect.sizeDelta = boxSize;
		scrollBarRect.sizeDelta = new Vector2 (20f, boxHeight);
		dropDownFlameRect.sizeDelta = boxSize;
		itemBox.GetComponent<LayoutElement> ().minWidth = boxWidth - 25f;

		foreach (var item in dropDownItems) {
			var clone = UITools.AddChild(itemBox.transform.parent.gameObject, itemBox);
			clone.transform.Find ("itemText").GetComponent<Text>().text = item.Value;
			(clone.transform.Find ("itemText"). transform as RectTransform).sizeDelta = new Vector2((dropWidth - 25f), 25f);
			clone.GetComponent<DropDownItemData>().refineType = refineType;
			clone.GetComponent<DropDownItemData>().refineKey = item.Key;
			clone.GetComponent<Button>().onClick.RemoveAllListeners();
			clone.GetComponent<Button>().onClick.AddListener(() => {
				callback(clone.gameObject);
			});
		}

		itemBox.SetActive (false);

		dropDownBox.transform.localScale = new Vector3 (1f, 0f, 1f);
		return transform.GetComponent<DropDownList> ();
	}

	
	public void DropDown() {
		StartCoroutine(DropDownAnimation());
	}
	
	public void FoldUp() {
		StartCoroutine(FoldUpAnimation());
	}
	
	private IEnumerator DropDownAnimation() {
		dropDownBox.transform.DOScaleY (1, 0.3f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds (0.1f);
	}
	
	private IEnumerator FoldUpAnimation() {
		dropDownBox.transform.DOScaleY (0, 0.2f).SetEase(Ease.InQuad);
		yield return new WaitForSeconds (0.2f);
	}
	
}
