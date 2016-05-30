using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class Matching_Animation : MonoBehaviour {

	public UIElement_matching matching;
	//search
	private GameObject SearchAnimeBox;
	private Transform parts_01, parts_02, parts_03, parts_04, parts_05;
	private Image parts_06, parts_07;
	//cutin
	private GameObject parts10Box, textBox, parts12Box;
	private Transform parts12Top, parts12Bot;
	private GameObject parts8, parts9;
	private GameObject warning1, warning2;
	//boxmove
	private Image bk;
	private GameObject effectVS, Return_Button, SearchInfo;
	private Transform playerBox, enemyInfo, enemyBox;
	private AudioSource SE_VS;
	
	public bool searchStart, searchStop;
	public bool searchCutIn, readyCutIn, Stopwarning;
	public bool enemyIn;

	// Use this for initialization
	void Start () {

		//search
		SearchAnimeBox = matching.SearchAnimeBox;
		parts_01 = matching.parts_01;
		parts_02 = matching.parts_02;
		parts_03 = matching.parts_03;
		parts_04 = matching.parts_04;
		parts_05 = matching.parts_05;
		parts_06 = matching.parts_06;
		parts_07 = matching.parts_07;

		//cutin
		parts10Box = matching.parts10Box;
		textBox = matching.textBox;
		parts12Box = matching.parts12Box;
		parts12Top = matching.parts12Top;
		parts12Bot = matching.parts12Bot;
		parts8 = matching.parts8;
		parts9 = matching.parts9;
		warning1 = matching.warning1;
		warning2 = matching.warning2;

		//boxmove
		bk = matching.bk;
		effectVS = matching.effectVS;
		Return_Button = matching.Return_Button;
		SearchInfo = matching.SearchInfo;
		playerBox = matching.playerBox;
		enemyInfo = matching.enemyInfo;
		enemyBox = matching.enemyBox;
		SE_VS = matching.SE_VS;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (searchStart == true) {
			SearchingAnimationStart();
			searchStart = false;
		}
		if (searchStop == true) {
			StartCoroutine(SearchingAnimationStop());
			searchStop = false;
		}
		if (searchCutIn == true) {
			StartCoroutine(searchCutInAnimation());
			searchCutIn = false;
		}
		if (Stopwarning == true) {
			StopWarningAnimation ();
			Stopwarning = false;
		}
		if (readyCutIn == true) {
			StartCoroutine(readyCutInAnimation ());
			readyCutIn = false;
		}
		if (enemyIn == true) {
			StartCoroutine(EnemyInAnimation());
			enemyIn = false;
		}
	
	}

	//Matching_Search========================================================================
	
	void SearchingReset(){
		SearchAnimeBox.SetActive (true);
		SearchAnimeBox.GetComponent<CanvasGroup> ().alpha = 1;
		parts_01.localEulerAngles = new Vector3 (0, 0, 0);
		parts_02.localEulerAngles = new Vector3 (0, 0, 0);
		parts_03.localEulerAngles = new Vector3 (0, 0, 0);
		parts_06.color = new Color(1,1,1,1);
		parts_07.color = new Color(1,1,1,1);
	}

	void SearchingAnimationStart(){

		SearchingReset ();

		//サークル回転
		parts_01.DORotate (new Vector3(0,0,360), 10f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase (Ease.Linear);
		parts_02.DORotate (new Vector3(0,0,-360), 20f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase (Ease.Linear);
		parts_03.DORotate (new Vector3(0,0,360), 5f, RotateMode.FastBeyond360).SetLoops(-1,LoopType.Restart).SetEase (Ease.Linear);

		//飾り明滅
		parts_06.DOFade (0f, 1.5f).SetLoops(-1,LoopType.Yoyo);
		parts_07.DOFade (0f, 1.5f).SetLoops(-1,LoopType.Yoyo);
	}

	IEnumerator SearchingAnimationStop(){

		SearchAnimeBox.GetComponent<CanvasGroup> ().DOFade (0, 0.2f);
		yield return new WaitForSeconds (0.2f);
		SearchAnimeBox.SetActive (false);
		
	}

	//Matching_CuntIn========================================================================

	void CutInReset(){
		parts10Box.SetActive (false);
		parts10Box.GetComponent<CanvasGroup> ().alpha = 0;
		parts10Box.transform.localScale = new Vector3 (1, 0, 1);
		textBox.GetComponent<CanvasGroup> ().alpha = 0;
		parts12Box.GetComponent<CanvasGroup> ().alpha = 0;
		parts12Top.localPosition = new Vector3 (0, 100, 0);
		parts12Bot.localPosition = new Vector3 (0, -100, 0);
	}
	
	//「艦影を確認」カットイン
	IEnumerator searchCutInAnimation(){
		CutInReset ();
		yield return new WaitForSeconds (0.1f);
		
		parts10Box.SetActive (true);
		parts8.SetActive (true);
		parts9.SetActive (false);
		
		TextOpen();
		yield return new WaitForSeconds (0.1f);
		FreamIn (false);
		
		yield return new WaitForSeconds (1f);
		
		TextClose ();
		FreamOut ();
	}
	
	//「第一戦闘配置」カットイン
	IEnumerator readyCutInAnimation(){
		CutInReset ();
		yield return new WaitForSeconds (0.1f);
		
		parts10Box.SetActive (true);
		parts8.SetActive (false);
		parts9.SetActive (true);
		
		TextOpen();
		yield return new WaitForSeconds (0.1f);
		FreamIn (true);
		
		yield return new WaitForSeconds (1.2f);
		
		TextClose ();
		FreamSeparate ();
	}
	
	void TextOpen(){
		parts10Box.transform.DOScale (new Vector3(1, 1, 1), 0.2f);
		parts10Box.GetComponent<CanvasGroup> ().DOFade (1, 0.2f);
		textBox.GetComponent<CanvasGroup>().DOFade (1, 0.3f).SetEase (Ease.InExpo);
	}
	
	void TextClose(){
		parts10Box.transform.DOScale (new Vector3 (1, 0.3f, 1), 0.3f).SetEase (Ease.OutQuart);
		parts10Box.GetComponent<CanvasGroup> ().DOFade (0, 0.3f);
		textBox.GetComponent<CanvasGroup> ().alpha = 0;
		textBox.GetComponent<CanvasGroup>().DOFade (0, 0.3f).SetEase (Ease.InExpo);
	}
	
	void FreamIn(bool warningBool){
		parts12Box.GetComponent<CanvasGroup> ().DOFade (1, 0.4f);
		parts12Top.DOLocalMove (new Vector3(0, 53, 0), 0.4f).SetEase (Ease.OutQuart);
		parts12Bot.DOLocalMove (new Vector3(0, -53, 0), 0.4f).SetEase (Ease.OutQuart);
		warning1.SetActive (warningBool);
		warning2.SetActive (warningBool);
	}
	
	void FreamOut(){
		parts12Box.GetComponent<CanvasGroup> ().DOFade (0, 0.3f);
	}
	
	void FreamSeparate(){
		parts12Top.DOLocalMove (new Vector3(0, 211, 0), 0.4f).SetEase (Ease.OutQuart);
		parts12Bot.DOLocalMove (new Vector3(0, -169, 0), 0.4f).SetEase (Ease.OutQuart);
	}


	//stop
	void StopWarningAnimation(){
		parts12Box.GetComponent<CanvasGroup> ().DOFade (0, 0.5f);
		warning1.SetActive (false);
		warning2.SetActive (false);
	}

	//Matching_BoxMove========================================================================

	void BoxMoveReset(){
		enemyInfo.localPosition = new Vector3 (593, 18, 0);
		Return_Button.SetActive (false);
		SearchInfo.SetActive (true);
	}
	
	//敵のボックスがぶつかるアニメーションシーケンス
	IEnumerator EnemyInAnimation () {
		BoxMoveReset ();
		StartCoroutine (InformationMove ());
		
		bk.DOFade (0.5f, 0.3f);
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (EffectPlay ());
	}
	
	//Infoの動き
	IEnumerator InformationMove(){
		enemyInfo.DOLocalMove (new Vector3(155, 18, 0), 0.6f).SetEase (Ease.InQuint);//ぶつかる位置
		yield return new WaitForSeconds (0.6f);
		enemyInfo.DOLocalMove (new Vector3(205, 18, 0), 0.3f).SetEase (Ease.OutExpo);//FIX位置
		
		//両者ぶるぶる震える
		playerBox.DOShakePosition (1f, 20, 50, 180, true);
		enemyBox.DOShakePosition (1f, 20, 50, 180, true);
		
		//退出ボタン消える
		Return_Button.SetActive (false);
		SearchInfo.SetActive (false);
	}
	
	//エフェクト、SE再生
	IEnumerator EffectPlay(){
		SE_VS.Play ();
		effectVS.GetComponent<MatchingEffect_Manager> ().Effect_Play_Limit = true;
		yield return new WaitForSeconds (0.5f);
		effectVS.GetComponent<MatchingEffect_Manager> ().Effect_Stop_Limit = true;
	}

}
