using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class PilotAreaBack : MonoBehaviour {

	//救出される対象パイロットが乗っているBattler
	public UIElement_BattlerIcon battler;

	//生成されるエフェクト
	public GameObject EF_pilotBack;

	//エフェクトが行き着く場所
	public Vector3 lastPosition;

	//アニメーション再生トリガー
	public bool PilotAreaBack_Start;

	// Use this for initialization
	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {

		if(PilotAreaBack_Start == true){
			StartCoroutine(PilotAreaBack_Animation());
			PilotAreaBack_Start = false;
		}
	
	}

	//ユニットが破壊されたとき、乗っていたパイロットがコストエリアまで戻るアニメーション
	public IEnumerator PilotAreaBack_Animation () {
		
		//Reset
		Transform battlerTra = battler.battlerTra;
		
		yield return new WaitForSeconds (0.4f);
		
		//エフェクト再生
		GameObject efObj = (GameObject)Instantiate(EF_pilotBack, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,-104,0);//エフェクトを指定Battlerのパイロットアイコンの位置に表示する
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//エフェクトを表示したいコストアイコンの場所まで移動する　※ワールド座標（ローカル座標の方が使い勝手が良い場合はDOLocalMoveに変更してください）
		efTra.DOMove (lastPosition, 0.4f).SetEase (Ease.InOutQuart);
		
		yield return new WaitForSeconds (0.5f);
		//このタイミングでコストエリアに戻されたコストアイコンを表示
		
		//エフェクトDestroy
		yield return new WaitForSeconds (3f);
		GameObject.Destroy(efObj);
	}


}
