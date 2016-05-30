using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

public class SortPopUpWindow : PopUpWindow {
	[HideInInspector] public string currentSort;
	[HideInInspector] public string colorRefineKey;
	[HideInInspector] public string prohibitRefineKey;
	[HideInInspector] public string packRefineKey;

	private Dictionary<string, string> colorSortList = new Dictionary<string, string> ();
	private Dictionary<string, string> prohibitSortList = new Dictionary<string, string> ();
	private Dictionary<string, string> packSortList = new Dictionary<string, string> ();
	private DropDownList colorDropDownList;
	private DropDownList prohibitDropDownList;
	private DropDownList packDropDownList;
	private bool isColorDropDown = false;
	private bool isProhibitDropDown = false;
	private bool isPackDropDown = false;
	private string defaultSort = "cardCode.asc";

	private Action execAction;

	private Mage mage { get { return Mage.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("DeckConstructionView"); } }

	private Color colorNegative = new Color ((float)57 / (float)255, (float)255 / (float)255, (float)255 / (float)255);
	private Color colorPositive = new Color ((float)228 / (float)255, (float)152 / (float)255, (float)15 / (float)255);

	private Steward _steward;

	public void Init (Action callback) {
		_steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		base
			.Init (true)
			.SetSingleActionEvent ("閉じる", () => {});

		// Create sort drop down
		colorSortList.Add ("None", "選択");
		foreach (JProperty prop in master.staticData ["cardColors"]) {
			colorSortList.Add (prop.Name, (prop.Value as JObject)["name"].ToString ());
		}
		
		GameObject ColorDropDown = UITools.AddChild (transform.Find ("Window/Parts").gameObject, Resources.Load ("Common/Prefabs/DropDownBox") as GameObject);
		RectTransform colorListParent = transform.Find ("Window/Parts/ColorDropDown").GetComponent<RectTransform> ();
		colorDropDownList = ColorDropDown.GetComponent<DropDownList> ().SetList (colorSortList, colorListParent.sizeDelta.x, 4, "color", (GameObject obj) => {
			SelectSort (obj);
		});
		ColorDropDown.transform.localPosition = new Vector2 ((colorListParent.transform.localPosition.x - (colorListParent.sizeDelta.x * 0.5f)), (colorListParent.transform.localPosition.y - (colorListParent.sizeDelta.y * 0.5f)));
		
		prohibitSortList.Add ("No", "デッキに入れられないカードを含む");
		prohibitSortList.Add ("Yes", "デッキに入れられないカードを含まない");
		
		GameObject ProhibitDropDown = UITools.AddChild (transform.Find ("Window/Parts").gameObject, Resources.Load ("Common/Prefabs/DropDownBox") as GameObject);
		RectTransform prohibitListParent = transform.Find ("Window/Parts/ProhibitDropDown").GetComponent<RectTransform> ();
		prohibitDropDownList = ProhibitDropDown.GetComponent<DropDownList> ().SetList (prohibitSortList, prohibitListParent.sizeDelta.x, 4, "prohibit", (GameObject obj) => {
			SelectSort (obj);
		});
		ProhibitDropDown.transform.localPosition = new Vector2 ((prohibitListParent.transform.localPosition.x - (prohibitListParent.sizeDelta.x * 0.5f)), (prohibitListParent.transform.localPosition.y - (prohibitListParent.sizeDelta.y * 0.5f)));
		
		packSortList.Add ("None", "選択");
		foreach (JProperty prop in master.staticData ["cardsPacks"]) {
			packSortList.Add (prop.Name, (prop.Value as JObject)["name"].ToString ());
		}
		
		GameObject PackDropDown = UITools.AddChild (transform.Find ("Window/Parts").gameObject, Resources.Load ("Common/Prefabs/DropDownBox") as GameObject);
		RectTransform packListParent = transform.Find ("Window/Parts/PackDropDown").GetComponent<RectTransform> ();
		packDropDownList = PackDropDown.GetComponent<DropDownList> ().SetList (packSortList, packListParent.sizeDelta.x, 4, "pack", (GameObject obj) => {
			SelectSort (obj);
		});
		PackDropDown.transform.localPosition = new Vector2 ((packListParent.transform.localPosition.x - (packListParent.sizeDelta.x * 0.5f)), (packListParent.transform.localPosition.y - (packListParent.sizeDelta.y * 0.5f)));

		Reset (); 

		callback ();
	}

	public void Reset () {
		currentSort = defaultSort;
		SetSortRefineKey ("color", colorSortList.First ().Key);
		SetSortRefineKey ("prohibit", prohibitSortList.First ().Key);
		SetSortRefineKey ("pack", packSortList.First ().Key);
		foreach (Transform n in transform.Find ("Window/Parts/Buttons")) {
			n.GetComponent<Image>().color = colorNegative;
		}
	}

	public void SetExecAction (Action execAction) {
		this.execAction = execAction;
	}

	public void Open () {
		base.FadeIn ();
		base.GetSingleActionButton ().SetActive (true);
	}
	
	public void ToggleDropDown (string dropDownObj) {
		_steward.PlaySETap ();
		switch (dropDownObj) {
		case "color":
			if (isColorDropDown) {
				colorDropDownList.FoldUp ();
				isColorDropDown = false;
			} else {
				prohibitDropDownList.FoldUp ();
				isProhibitDropDown = false;
				packDropDownList.FoldUp ();
				isPackDropDown = false;
				colorDropDownList.DropDown ();
				isColorDropDown = true;
			}
			break;
			
		case "prohibit":
			if (isProhibitDropDown) {
				prohibitDropDownList.FoldUp ();
				isProhibitDropDown = false;
			} else {
				colorDropDownList.FoldUp ();
				isColorDropDown = false;
				packDropDownList.FoldUp ();
				isPackDropDown = false;
				prohibitDropDownList.DropDown ();
				isProhibitDropDown = true;
			}
			break;
			
		case "pack":
			if (isPackDropDown) {
				packDropDownList.FoldUp ();
				isPackDropDown = false;
			} else {
				colorDropDownList.FoldUp ();
				isColorDropDown = false;
				prohibitDropDownList.FoldUp ();
				isProhibitDropDown = false;
				packDropDownList.DropDown ();
				isPackDropDown = true;
			}
			break;
		}
	}
	
	public void SelectSort (GameObject obj) {
		_steward.PlaySEOK ();
		var dropDownItemData = obj.GetComponent<DropDownItemData> ();
		SetSortRefineKey (dropDownItemData.refineType, dropDownItemData.refineKey);
		ToggleDropDown (dropDownItemData.refineType);
	}
	
	public void SetSortRefineKey (string refineType, string refineKey) {
		switch (refineType) {
		case "color":
			colorRefineKey = refineKey;
			base.GetObject ("ColorDropDown/ColorSelection").GetComponent<Text>().text = colorSortList[refineKey];
			break;
			
		case "prohibit":
			prohibitRefineKey = refineKey;
			base.GetObject ("ProhibitDropDown/ProhibitSelection").GetComponent<Text>().text = prohibitSortList[refineKey];
			break;
			
		case "pack":
			packRefineKey = refineKey;
			base.GetObject ("PackDropDown/PackSelection").GetComponent<Text>().text = packSortList[refineKey];
			break;
		}
	}

	public void Exec (string sort) {
		currentSort = sort;
		execAction ();
		Close ();
	}

	public void ButtonChange (GameObject obj) {
		foreach (Transform n in transform.Find ("Window/Parts/Buttons")) {
			n.GetComponent<Image>().color = colorNegative;
			if (n.Equals(obj.transform)) {
				n.GetComponent<Image>().color = colorPositive;
			}
		}
	}

	public void Close () {
		_steward.PlaySEOK ();

		colorDropDownList.FoldUp ();
		isColorDropDown = false;
		prohibitDropDownList.FoldUp ();
		isProhibitDropDown = false;
		packDropDownList.FoldUp ();
		isPackDropDown = false;
		
		base.FadeOut (() => {});
	}
}
