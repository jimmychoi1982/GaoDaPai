using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardEffectProtect : MonoBehaviour {
	
	public UIElement_BattlerIcon atkBattler, targetBattler;
	
	private Transform atkBattlerTra, targetBattlerTra;
	private Vector3 addPos;
	public bool PlayerTurn;//自分のターンならtrue
	
	public GameObject ef_protect;// 生成されるエフェクト

	public bool kwProtectStart, attackMove;
	
	// Use this for initialization
	void Start () {
		
		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(attackMove == true){
			StartCoroutine(attackMoveAnimation());
			attackMove = false;
		}
	}


	
	//攻撃動作
	public IEnumerator attackMoveAnimation () {
		
		atkBattlerTra = atkBattler.battlerTra;
		targetBattlerTra = targetBattler.battlerTra;
		
		Vector3 _atkPos = atkBattlerTra.localPosition;
		Vector3 _damagePos = targetBattlerTra.localPosition;
		
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
		
		//元の位置に戻り、自分が震える
		atkBattlerTra.DOLocalMove(_atkPos, 0.3f).SetEase(Ease.OutExpo);
		yield return new WaitForSeconds (0.2f);
		atkBattlerTra.DOShakePosition(0.4f, new Vector3(20, 20, 0), 40, 10, false);
		
	}
	
	//攻撃をはね返すエフェクト再生
	public IEnumerator Attack_Damage_Animation (){
		
		targetBattlerTra = targetBattler.battlerTra;
		
		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(ef_protect, transform.position, Quaternion.identity);
		efObj.transform.parent = targetBattlerTra;
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		if(PlayerTurn == true){
			efTra.localPosition = new Vector3(0,-100,0);
		}else if(PlayerTurn == false){
			efTra.localPosition = new Vector3(0,100,0);
		}
		
		//消去
		yield return new WaitForSeconds (1f);
		GameObject.Destroy(efObj);
		
	}
	
	
}