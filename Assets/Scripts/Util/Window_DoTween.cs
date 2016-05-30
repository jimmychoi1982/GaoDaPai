using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class Window_DoTween : MonoBehaviour {
	public GameObject Set_DoTween_Animation_Object;
	public Vector3 First_Scale;
	public float Set_Duration;
	public Ease Select_Eases;

	//追加
	public GameObject bk;
	public bool bkFade;

	void Awake() {
		if (bkFade == true) {
			this.bk.GetComponent<Image>().color = new Color (1, 1, 1, 0);
			bk.SetActive (false);
		}
		this.First_Scale = this.Set_DoTween_Animation_Object.transform.localScale;
		this.Set_DoTween_Animation_Object.transform.localScale = new Vector3 (0f, 0f, 0f);
	}

	public void Reset_Position() {
		if (bkFade == true) {
			this.bk.GetComponent<Image>().color = new Color (1, 1, 1, 0);
			bk.SetActive (false);
		}
		this.Set_DoTween_Animation_Object.transform.localScale = new Vector3 (0f, 0f, 0f);
	}

	public void Out_Position() {
		this.Set_DoTween_Animation_Object.transform.DOScale (new Vector3 (0f, 0f, 0f), this.Set_Duration).SetEase (this.Select_Eases);
	}

	public void Go_Positon() {
		if (bkFade == true) {
			bk.SetActive (true);
			bk.GetComponent<Image>().DOFade (0.7f, Set_Duration);
		}
		this.Set_DoTween_Animation_Object.transform.DOScale(this.First_Scale, this.Set_Duration).SetEase(this.Select_Eases);
	}
}
