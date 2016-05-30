using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Card_LauncherSet : MonoBehaviour {

	public UIElement_launcher launcher;
	private GameObject launcherObj, cardSetObj, ef_Embarkation, ef_Change;
	private Transform launcherTra, cardSet;

	//手札からドラッグしてきたカード
	public GameObject selectCardObj;
	private Transform selectCardTra;

	//アニメーション再生トリガー
	public bool LauncherSet_Start;


	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		launcherObj = launcher.launcher;
		cardSetObj = launcher.cardSetObj;
		launcherTra = launcher.launcherTra;
		cardSet = launcher.cardSet;

		launcherObj.SetActive(false);
	}
	

	void Update () {

		if(LauncherSet_Start == true)
		{
			StartCoroutine(LauncherSet_Animation());
			LauncherSet_Start = false;
		}
	
	}


	//指を離した位置からLauncherに移動
	public IEnumerator LauncherSet_Animation () {

		selectCardTra = selectCardObj.GetComponent<Transform>();

		//Reset------------------------------------
		selectCardObj.SetActive(true);
		launcherObj.SetActive(false);
		launcherTra.localPosition = new Vector3(0,0,0);
		selectCardTra.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		cardSet.localPosition = new Vector3(0,258,0);
		yield return new WaitForSeconds (0.01f);

		//目視でサイズと位置をLauncherのCardに合わせている　※良い方法あったら変更して下さい…
		selectCardTra.DOLocalMove(new Vector3(0, 329f, 0), 0.4f).SetEase(Ease.OutQuad);
		selectCardTra.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
		selectCardTra.DOScale(0.375f, 0.4f);
		yield return new WaitForSeconds (0.4f);

		//ドラッグしていたカードを非表示・Launcherを表示
		launcherObj.SetActive(true);
		cardSetObj.SetActive(true);
		yield return new WaitForSeconds (0.01f);
		selectCardObj.SetActive(false);

	}


}
