using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class Matching_Manager : MonoBehaviour {

	public GameObject Manager;
	public GameObject bkOver;
	public bool Seaching, Matching;

	private Steward steward;

	// Use this for initialization
	void Start () 
	{
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		bkOver.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Seaching == true) {
			Seaching_Animation ();
			Seaching = false;
		}
		if(Matching == true){
			StartCoroutine(Matching_Animation());
			Matching = false;
		}
	}


	//待機中
	void Seaching_Animation () {		
		Manager.GetComponent<Matching_Animation>().searchStart = true;
	}

	//マッチングされた
	IEnumerator Matching_Animation () {
		Manager.GetComponent<Matching_Animation>().searchStop = true;
		Manager.GetComponent<Matching_Animation>().searchCutIn = true;
		yield return new WaitForSeconds (1.2f);

		Manager.GetComponent<Matching_Animation>().enemyIn = true;
		yield return new WaitForSeconds (1.2f);
		Manager.GetComponent<Matching_Animation>().readyCutIn = true;

		yield return new WaitForSeconds (3.4f);

		//全体が黒くなり、ローディングへ
		bkOver.SetActive (true);
		Manager.GetComponent<Matching_Animation>().Stopwarning = true;
		bkOver.GetComponent<Image>().DOFade (1, 0.5f);

		yield return new WaitForSeconds (0.5f);
//		steward.ClearHideMenuCondition ("multiPlayMatching");
		steward.LoadNextScene("GameView");
	}

}
