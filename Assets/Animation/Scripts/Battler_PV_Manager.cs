using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class Battler_PV_Manager : MonoBehaviour {

	public GameObject Manager;
	public UIElement_BattlerIcon Battler1, Battler2;
	public GameObject EF_UnitIventTaunt, EF_keyword_up;

	public bool Demo_Start;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Demo_Start == true){
			StartCoroutine(Battler_Attack_Anime_Start());
			Demo_Start = false;
		}
	
	}

	//動作サンプル
	IEnumerator Battler_Attack_Anime_Start () {
		yield return new WaitForSeconds (1f);
		Manager.GetComponent<Battler_Scale>().battler = Battler1;
		Manager.GetComponent<Battler_Scale>().DragScale_In= true;
		Manager.GetComponent<Arch_Ray> ().attackActive = true;
		yield return new WaitForSeconds (3f);
		Manager.GetComponent<Arch_Ray> ().targetLockOn = true;
		yield return new WaitForSeconds (0.5f);
		Manager.GetComponent<Arch_Ray> ().attackActive = false;

		//キーワード効果発動
		EF_UnitIventTaunt.GetComponent<Effect_Play_Limit> ().Effect_Start = true;
		yield return new WaitForSeconds (0.5f);
		EF_keyword_up.GetComponent<Effect_Play_Limit> ().Effect_Start = true;
		yield return new WaitForSeconds (0.5f);

		//ATK上昇
		Manager.GetComponent<Battler_Num>().battler = Battler1;
		Manager.GetComponent<Battler_Num>().cardEffect = true;
		Manager.GetComponent<Battler_Num>().effectUp = true;
		Manager.GetComponent<Battler_Num>().changeNumAtk= 5;
		Manager.GetComponent<Battler_Num>().atkNumChangeStart = true;
		yield return new WaitForSeconds (0.5f);

		//攻撃
		Manager.GetComponent<Battler_Attack>().atkBattler = Battler1;
		Manager.GetComponent<Battler_Attack>().damageBattler = Battler2;
		Manager.GetComponent<Battler_Attack>().PlayerTurn = true;
		Manager.GetComponent<Battler_Attack>().Attack_Move = true;

		yield return new WaitForSeconds (1f);

		Manager.GetComponent<PopupNumAppear>().battler = Battler2;
		Manager.GetComponent<PopupNumAppear>().defNum = "-5";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;
		yield return new WaitForSeconds (0.01f);
		Manager.GetComponent<PopupNumAppear>().battler = Battler1;
		Manager.GetComponent<PopupNumAppear>().defNum = "-2";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;

		Manager.GetComponent<Battler_Scale>().DragScale_Out = true;

		yield return new WaitForSeconds (0.4f);

		Manager.GetComponent<Battler_Num>().battler = Battler2;
		Manager.GetComponent<Battler_Num>().cardEffect = false;
		Manager.GetComponent<Battler_Num>().changeNumDef = 0;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (0.01f);
		Manager.GetComponent<Battler_Num>().battler = Battler1;
		Manager.GetComponent<Battler_Num>().cardEffect = false;
		Manager.GetComponent<Battler_Num>().changeNumDef = 2;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (1f);

		Manager.GetComponent<Battler_Death>().battler = Battler2;
		Manager.GetComponent<Battler_Death>().Death = true;
		Debug.Log ("終了");

	}
}
