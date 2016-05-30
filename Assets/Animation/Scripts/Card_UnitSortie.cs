using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class Card_UnitSortie : MonoBehaviour {

	public UIElement_launcher launcher;
	private GameObject launcherObj, mainCardObj, costCardObj, ef_Embarkation, ef_Change;
	private Image mainCardImage, costCardImage;
	private Transform mainCardTra, costCardTra, cardSet;

	//コストカードが乗るときはtrue、乗らないときはfalse
	public bool Cost_on_Flg;
	
	//カードがアイコン化した後のアイコン
	public GameObject BattlerIcon;

	//アニメーション再生トリガー
	public bool Embarkation_Start, UnitSortie_Start, Change_Start;


	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		//UIElement_launcher
		launcherObj = launcher.launcher;
		mainCardObj = launcher.mainCardObj;
		mainCardTra = launcher.mainCardTra;
		mainCardImage = launcher.mainCardImage;
		costCardObj = launcher.costCardObj;
		costCardTra = launcher.costCardTra;
		costCardImage = launcher.costCardImage;
		cardSet = launcher.cardSet;
		ef_Embarkation = launcher.ef_Embarkation;
		ef_Change = launcher.ef_Change;

		launcherObj.SetActive(false);
	}
	

	void Update () {

		if(Embarkation_Start == true)
		{
			Cost_on_Flg = true;
			StartCoroutine(Embarkation_Animation());
			Embarkation_Start = false;
		}

		if(UnitSortie_Start == true)
		{
			StartCoroutine(UnitSortie_Animation());
			UnitSortie_Start = false;
		}

		if (Change_Start == true) 
		{
			StartCoroutine (ChangeAnimation());
			Change_Start = false;
		}
	
	}


	//「搭乗！」アニメーション
	public IEnumerator Embarkation_Animation ()
	{
		// Reset----------------------------------------
		mainCardObj.SetActive(true);
		costCardObj.SetActive(false);
		BattlerIcon.SetActive (false);
		cardSet.localPosition = new Vector3(0,258,0);
		mainCardTra.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		costCardTra.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		mainCardTra.localPosition = new Vector3(0,0,0);
		costCardTra.localPosition = new Vector3(0,0,0);
		mainCardImage.color = new Color (1, 1, 1, 1);
		costCardImage.color = new Color (1, 1, 1, 1);
		yield return new WaitForSeconds (0.01f);

		//エフェクトを出す 約2秒
		ef_Embarkation.GetComponent<EF_Embarkation>().Anime_Start = true;

		yield return new WaitForSeconds (0.3f);

		//コストカードを表示する
		costCardObj.gameObject.SetActive(true);

		//カードの動き
		mainCardTra.DOLocalMoveX(-190, 0.4f).SetEase(Ease.OutElastic);
		costCardTra.DOLocalMoveX(190, 0.4f).SetEase(Ease.OutElastic);

		yield return new WaitForSeconds (1.7f);
	}


	//「出撃」ボタンを押したら母艦にカードが移動
	public IEnumerator UnitSortie_Animation ()
	{
		// Reset----------------------------------------
		mainCardObj.SetActive(true);
		BattlerIcon.SetActive (false);
		cardSet.localPosition = new Vector3(0,258,0);
		mainCardTra.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		costCardTra.localScale = new Vector3(0.75f, 0.75f, 0.75f);
		if (Cost_on_Flg == true){
			costCardObj.gameObject.SetActive(true);
			costCardImage.DOFade (1, 0);
			mainCardTra.localPosition = new Vector3(-190, 0, 0);
			costCardTra.localPosition = new Vector3(190, 0, 0);
		}else{
			costCardObj.gameObject.SetActive(false);
			mainCardTra.localPosition = new Vector3(0, 0, 0);
		}
		mainCardImage.DOFade (1, 0);
		yield return new WaitForSeconds (0.01f);

		//ユニットカードを母艦近くまで動かす
		//もしパイロットが搭乗していたらコストカードも一緒に動かす
		if (Cost_on_Flg == true){
			cardSet.DOLocalMove(new Vector3(0, -300, 0), 1.2f).SetEase(Ease.OutQuart);
			mainCardTra.DOScale(0.3f, 0.9f).SetEase(Ease.OutExpo);
			mainCardTra.DOLocalMove(new Vector3(-6, 12, 0), 1.2f).SetEase(Ease.OutQuart);
			costCardTra.DOScale(0.3f, 0.9f).SetEase(Ease.OutExpo);
			costCardTra.DOLocalMove(new Vector3(6, 0, 0), 1.2f).SetEase(Ease.OutQuart);
		}else{
			cardSet.DOLocalMove(new Vector3(0, -300, 0), 1.2f).SetEase(Ease.OutQuart);
			mainCardTra.DOScale(0.3f, 0.9f).SetEase(Ease.OutExpo);
			mainCardTra.DOLocalMove(new Vector3(0, 0, 0), 1.2f).SetEase(Ease.OutQuart);
		}

		yield return new WaitForSeconds (1.2f);
	}


	//チェンジエフェクト再生
	public IEnumerator ChangeAnimation () {

		//Reset-----------------------------------------
		mainCardObj.SetActive(true);
		BattlerIcon.SetActive (false);
		if(Cost_on_Flg == true){
			costCardImage.DOFade (1, 0);
			costCardObj.SetActive(true);
		}
		mainCardImage.DOFade (1, 0);
		yield return new WaitForSeconds (0.01f);

		//SE再生
		ef_Change.GetComponent<Change_CardToIcon>().Effect_Play_Limit = true;

		yield return new WaitForSeconds (0.02f);

		//カードを消す
		mainCardImage.DOFade (0, 0.2f);
		if(Cost_on_Flg == true){
			costCardImage.DOFade(0, 0.2f);
		}
		
		yield return new WaitForSeconds (0.4f);

		//アイコン化されたオブジェクトを表示する
		BattlerIcon.SetActive (true);

		//カードを非表示にする
		mainCardObj.SetActive(false);
		if(Cost_on_Flg == true){
			costCardObj.SetActive(false);
		}

	}


}
