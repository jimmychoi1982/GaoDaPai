using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Battler_Attack : MonoBehaviour {

	public UIElement_BattlerIcon atkBattler, damageBattler;

	private Transform atkBattlerTra, damageBattlerTra;
	private Vector3 addPos;

	// 生成されるエフェクト
	public GameObject ef_NormalAttack;

	public bool PlayerTurn;//自分のターンならtrue
	public bool Attack_Move;

	// Use this for initialization
	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Attack_Move == true){
			StartCoroutine(Attack_Move_Animation());
			Attack_Move = false;
		}
	}

	//攻撃動作
	public IEnumerator Attack_Move_Animation () {

		atkBattlerTra = atkBattler.battlerTra;
		damageBattlerTra = damageBattler.battlerTra;

		Vector3 _atkPos = atkBattlerTra.localPosition;
		Vector3 _damagePos = damageBattlerTra.localPosition;

		// 少し後退する(自分の場合と敵の場合で動く方向を変える)
		if(PlayerTurn == true){
			atkBattlerTra.DOLocalMoveY(-40, 0.3f).SetRelative();
			addPos = new Vector3(0,-100,0);
		}else if(PlayerTurn == false){
			atkBattlerTra.DOLocalMoveY(40, 0.3f).SetRelative();
			addPos = new Vector3(0,100,0);
		}
		yield return new WaitForSeconds (0.3f);

		//相手に突進する
		atkBattlerTra.DOLocalMove(_damagePos + addPos, 0.4f).SetEase(Ease.InExpo);
		yield return new WaitForSeconds (0.4f);

		StartCoroutine(Attack_Damage_Animation());

		//元の位置に戻る
		atkBattlerTra.DOLocalMove(_atkPos, 0.3f).SetEase(Ease.OutExpo);

	}


	//攻撃を受ける動作
	public IEnumerator Attack_Damage_Animation (){

		damageBattlerTra = damageBattler.battlerTra;

		//エフェクト再生 プレハブ取得、ダメージを受けたオブジェクトの中にインスタンス生成、トリガーで再生
		GameObject efObj = (GameObject)Instantiate(ef_NormalAttack, transform.position, Quaternion.identity);
		efObj.transform.SetParent(damageBattlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//敵がふるえる
		damageBattlerTra.DOShakePosition(0.4f, new Vector3(20, 20, 0), 40, 10, false);
		
		//消去
		yield return new WaitForSeconds (2f);
		GameObject.Destroy(efObj);

	}


}
