using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;

public class UnitSortieManager : MonoBehaviour {

	public GameObject Manager;
	public UIElement_launcher launcher;
	private GameObject Button;
	private AudioSource SE_Button;
	public Image bk;
	public Transform moveBatller;
	public bool Demo_Start;

	// Use this for initialization
	void Start () {

		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);

		Button = launcher.button;
		SE_Button = launcher.SE_Button;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Demo_Start == true){
			StartCoroutine(UnitSortie_Anime_Start());
			Demo_Start = false;
		}
	
	}

	//動作サンプル
	IEnumerator UnitSortie_Anime_Start () {

		yield return new WaitForSeconds (0.1f);

		Manager.GetComponent<PlayerHand_Scale>().MainCards_Scale_In = true;
		//MainScale_Inとともに裏に黒みをフェードイン
		bk.DOFade (0.6f, 0.5f);

		yield return new WaitForSeconds (1f);

		Manager.GetComponent<Card_LauncherSet>().LauncherSet_Start = true;

		yield return new WaitForSeconds (0.5f);

		Manager.GetComponent<PlayerHand_Scale>().MainCards_Move_Out = true;

		yield return new WaitForSeconds (2f);

		Manager.GetComponent<CostCards_Scale>().CostCards_Scale_In = true;
		yield return new WaitForSeconds (0.6f);
		Button.SetActive(true);

		yield return new WaitForSeconds (1f);

		Manager.GetComponent<CostCards_Scale>().CostCards_Scale_Out = true;
		Manager.GetComponent<Card_UnitSortie>().Embarkation_Start = true;

		yield return new WaitForSeconds (2f);

		Manager.GetComponent<Card_UnitSortie>().UnitSortie_Start = true;
		SE_Button.Play ();
		Button.SetActive(false);
		//設置ボタンが押されたら裏の黒みをフェードアウト
		bk.DOFade (0, 0.5f);

		yield return new WaitForSeconds (1.5f);
		moveBatller.DOLocalMoveY (-135, 0.5f);
		yield return new WaitForSeconds (0.5f);
		Manager.GetComponent<Card_UnitSortie>().Change_Start = true;

	}
}
