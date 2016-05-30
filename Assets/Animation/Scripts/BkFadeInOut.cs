using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class BkFadeInOut : MonoBehaviour {

	public Image bk;
	public bool bkIn, bkOut;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (bkIn == true) {
			bkInAnimation ();
			bkIn = false;
		}

		if (bkOut == true) {
			bkOutAnimation();
			bkOut = false;
		}
	
	}

	void bkInAnimation(){
		bk.DOFade (0.5f, 0.3f);
	}

	void bkOutAnimation(){
		bk.DOFade (0f, 0.4f);
	}
}
