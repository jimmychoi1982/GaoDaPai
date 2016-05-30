using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class LoadingAnimation : MonoBehaviour {

	public Image bk;
	public Transform halo, haloBox;
	public Transform haloRef;
	public GameObject dot1, dot2, dot3;

	//アニメーションループ用
	public bool AnimeReady;

	// Use this for initialization
	void Start () {

		this.AnimeReady = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (AnimeReady == true) {
			StartCoroutine(nowLodingAnimationStart());
			AnimeReady = false;
		}

	}
	

	IEnumerator nowLodingAnimationStart(){

		StartCoroutine(dotAppear ());
		StartCoroutine (haloMove ());

		yield return new WaitForSeconds (2.5f);
		AnimeReady = true;
	}

	//総尺2.2f
	IEnumerator dotAppear(){
		int a = 0;
		while (a < 5) {
			a++;
			dot1.SetActive (true);
			yield return new WaitForSeconds (0.15f);
			dot2.SetActive (true);
			yield return new WaitForSeconds (0.15f);
			dot3.SetActive (true);
			yield return new WaitForSeconds (0.15f);
			dot3.SetActive (false);
			dot2.SetActive (false);
			dot1.SetActive (false);
			yield return new WaitForSeconds (0.15f);
		}
	}

	//総尺1.7f
	IEnumerator haloMove(){
		yield return new WaitForSeconds (0.5f);
		halo.DOLocalJump (new Vector3 (0, 0, 0), 20, 1, 0.3f, false);
		haloRef.DOLocalJump (new Vector3 (0, 0, 0), 20, 1, 0.3f, false);
		yield return new WaitForSeconds (0.3f);
		halo.DOLocalJump (new Vector3 (0, 0, 0), 20, 1, 0.3f, false);
		haloRef.DOLocalJump (new Vector3 (0, 0, 0), 20, 1, 0.3f, false);
		yield return new WaitForSeconds (0.3f);
		halo.DORotate (new Vector3(0, 0, 80), 0.6f).SetLoops (2, LoopType.Yoyo);
		haloRef.DORotate (new Vector3(0, 0, 80), 0.6f).SetLoops (2, LoopType.Yoyo);
		haloBox.DOLocalMoveX (-50, 0.6f).SetLoops (2, LoopType.Yoyo).SetRelative();
	}


}
