using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PopupNumAppear : MonoBehaviour {

	public UIElement_BattlerIcon battler;
	private Transform battlerTra;

	//ポップアップ数値
	public GameObject popupNumberPrefab;

	//それぞれ何の数値を入れるか
	public string atkNum, defNum;

	public bool popupNumAtkStart, popupNumDefStart;

	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	void Update () {

		if(popupNumAtkStart == true){
			StartCoroutine(popupAtkAnimation());
			popupNumAtkStart = false;
		}
		if(popupNumDefStart == true){
			StartCoroutine(popupDefAnimation());
			popupNumDefStart = false;
		}

	}

	//attck数値表示
	public IEnumerator popupAtkAnimation (){
		battlerTra = battler.battlerTra;

		//数値表示　 プレハブ取得、ダメージを受けたオブジェクトの中にインスタンス生成、トリガーで再生
		GameObject atkNumObj = (GameObject)Instantiate(popupNumberPrefab, transform.position, Quaternion.identity);
		atkNumObj.transform.SetParent(battlerTra, true);
		Transform atkNumTra = atkNumObj.GetComponent<Transform>();
		atkNumTra.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		atkNumTra.localPosition = new Vector3(-100,-25,0);

		popupPublicFanction (atkNum, atkNumObj);
		
		yield return new WaitForSeconds (0.9f);
		GameObject.Destroy(atkNumObj);
	}

	//def数値表示
	public IEnumerator popupDefAnimation (){
		battlerTra = battler.battlerTra;

		//数値表示　 プレハブ取得、ダメージを受けたオブジェクトの中にインスタンス生成、トリガーで再生
		GameObject defNumObj = (GameObject)Instantiate(popupNumberPrefab, transform.position, Quaternion.identity);
		defNumObj.transform.SetParent(battlerTra, true);
		Transform defNumTra = defNumObj.GetComponent<Transform>();
		defNumTra.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		defNumTra.localPosition = new Vector3(100,-25,0);

		popupPublicFanction (defNum, defNumObj);
		
		yield return new WaitForSeconds (0.9f);
		GameObject.Destroy(defNumObj);
	}

	//共通部分
	void popupPublicFanction (string num, GameObject numObj){
		//数値取得
		numObj.GetComponent<PopupNumberMove> ().numberString = num;
		//カラー設定 trueにするとポップアップ文字が青になる
		numObj.GetComponent<PopupNumberMove> ().plus = false;
		//アニメーション再生
		numObj.GetComponent<PopupNumberMove>().numberMoveStart = true;
	}

}
