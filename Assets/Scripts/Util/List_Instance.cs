using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class List_Instance : MonoBehaviour {	
	public GameObject Create_Object;
	public GameObject Parent_Object;

	[HideInInspector]public int Create_Number;

	public float Width_Magnification;
	public float Height_Magnification;
	public int Width_Max_Num;
	public int Height_Max_Num;

	public enum Panel_Type {
		Longitudinal_Only,
		Beside_Only,
		Longitudinal_And_Beside_Deck_TYPE,
		Longitudinal_And_Beside_Avater_TYPE
	}

	public Panel_Type panel_type;

	public List<GameObject> Clone_Object = new List<GameObject>();
	private int Width_Limit;
	private int Height_Limit;

	private bool isCreate = false;

	void Awake() {
		if (panel_type == Panel_Type.Longitudinal_And_Beside_Deck_TYPE) {
			this.Width_Limit = -1;
		}
		else if (panel_type == Panel_Type.Longitudinal_And_Beside_Avater_TYPE) {
			this.Height_Limit = -1;
		}
	}

	public void CreateInstance () {
		if (isCreate) return;

		for (int i = 0; i < Create_Number; i++) {
			this.Clone_Object.Add ((GameObject)UITools.AddChild (this.Parent_Object, this.Create_Object));
	
			if(panel_type == Panel_Type.Longitudinal_Only || panel_type == Panel_Type.Beside_Only) {
				this.Clone_Object[i].transform.localPosition = new Vector3 (this.Create_Object.transform.localPosition.x + i * Width_Magnification, this.Create_Object.transform.localPosition.y - i * Height_Magnification, 0f);
				this.Clone_Object[i].transform.localScale = this.Create_Object.transform.localScale;
			} else if(panel_type == Panel_Type.Longitudinal_And_Beside_Deck_TYPE) {
				this.Width_Limit++;

				if (this.Width_Limit == this.Width_Max_Num) {
					this.Width_Limit = 0;
					this.Height_Limit++;
				}

				this.Clone_Object[i].transform.localPosition = new Vector3 (this.Create_Object.transform.localPosition.x + this.Width_Limit * Width_Magnification, this.Create_Object.transform.localPosition.y - this.Height_Limit * Height_Magnification, 0f);
				this.Clone_Object[i].transform.localScale = this.Create_Object.transform.localScale;
			} else if(panel_type == Panel_Type.Longitudinal_And_Beside_Avater_TYPE) {
				this.Height_Limit++;
				
				if(this.Height_Limit == this.Height_Max_Num) {
					this.Height_Limit = 0;
					this.Width_Limit++;
				}
				
				this.Clone_Object[i].transform.localPosition = new Vector3 (this.Create_Object.transform.localPosition.x + this.Width_Limit * Width_Magnification, this.Create_Object.transform.localPosition.y - this.Height_Limit * Height_Magnification, 0f);
				this.Clone_Object[i].transform.localScale = this.Create_Object.transform.localScale;
			}
		}
		isCreate = true;
	}

	public GameObject[] GetCreateObjects () {
		return this.Clone_Object.ToArray ();
	}

	public void Reset () {
		foreach (Transform n in this.Parent_Object.transform) {
			Destroy(n.gameObject);
		}
		this.Clone_Object = new List<GameObject> ();

		isCreate = false;
	}
}
