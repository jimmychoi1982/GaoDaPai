using UnityEngine;
using System.Collections;

public class ScrollViewPosY : MonoBehaviour {

	public GameObject content;
	public float yPos;

	// Use this for initialization
	void Start () {
		StartCoroutine (yPosSetting ());
	}

	IEnumerator yPosSetting(){
		yield return new WaitForSeconds (0.1f);
		content.transform.localPosition = new Vector2(0,yPos);
	}

}
