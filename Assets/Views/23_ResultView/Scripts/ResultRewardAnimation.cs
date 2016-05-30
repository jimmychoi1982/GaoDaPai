using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;

public class ResultRewardAnimation : MonoBehaviour {

	public GameObject reward, rewardWindow;
	public Image bk;
	public AudioSource SE_Window;
	public bool rewardOpen;

	// Use this for initialization
	void Start () {
	
		Reset ();
	}
	
	// Update is called once per frame
	void Update () {

		if (rewardOpen == true) {
			StartCoroutine(ResultWindowOpen());
			rewardOpen = false;
		}

	}

	void Reset(){
		bk.color = new Color (1, 1, 1, 0);
		reward.SetActive (false);
		rewardWindow.transform.localScale = new Vector3(0, 0, 0);
	}

	IEnumerator ResultWindowOpen(){
		Reset ();
		yield return new WaitForSeconds (0.1f);

		reward.SetActive (true);
		SE_Window.Play ();
		bk.DOFade (0.6f, 0.4f);
		rewardWindow.transform.DOScale (1, 0.5f).SetEase(Ease.OutBack);
	}
}

