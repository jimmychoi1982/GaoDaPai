using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class BattlerAttackManager : MonoBehaviour {

	public GameObject Manager;
	public UIElement_BattlerIcon Battler1, Battler2;
	public GameObject costIcon;

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

		//1ターン目
		Manager.GetComponent<Battler_Scale>().battler = Battler1;
		Manager.GetComponent<Battler_Scale>().DragScale_In = true;

		yield return new WaitForSeconds (0.5f);

		Manager.GetComponent<Battler_Attack>().atkBattler = Battler1;
		Manager.GetComponent<Battler_Attack>().damageBattler = Battler2;
		Manager.GetComponent<Battler_Attack>().PlayerTurn = true;
		Manager.GetComponent<Battler_Attack>().Attack_Move = true;

		yield return new WaitForSeconds (1f);
		Manager.GetComponent<PopupNumAppear>().battler = Battler2;
		Manager.GetComponent<PopupNumAppear>().defNum = "-3";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;
		yield return new WaitForSeconds (0.05f);
		Manager.GetComponent<PopupNumAppear>().battler = Battler1;
		Manager.GetComponent<PopupNumAppear>().defNum = "-1";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;

		Manager.GetComponent<Battler_Scale>().DragScale_Out = true;

		yield return new WaitForSeconds (0.4f);

		Manager.GetComponent<Battler_Num>().battler = Battler2;
		Manager.GetComponent<Battler_Num>().changeNumDef = 4;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (0.05f);
		Manager.GetComponent<Battler_Num>().battler = Battler1;
		Manager.GetComponent<Battler_Num>().changeNumDef = 1;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (1f);


//		//2ターン目
		Manager.GetComponent<Battler_Scale>().battler = Battler2;
		Manager.GetComponent<Battler_Scale>().DragScale_In = true;

		yield return new WaitForSeconds (0.2f);

		Manager.GetComponent<Battler_Attack>().atkBattler = Battler2;
		Manager.GetComponent<Battler_Attack>().damageBattler = Battler1;
		Manager.GetComponent<Battler_Attack>().PlayerTurn = false;
		Manager.GetComponent<Battler_Attack>().Attack_Move = true;

		yield return new WaitForSeconds (1f);
		Manager.GetComponent<PopupNumAppear>().battler = Battler1;
		Manager.GetComponent<PopupNumAppear>().defNum = "-1";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;
		yield return new WaitForSeconds (0.05f);
		Manager.GetComponent<PopupNumAppear>().battler = Battler2;
		Manager.GetComponent<PopupNumAppear>().defNum = "-3";
		Manager.GetComponent<PopupNumAppear>().popupNumDefStart = true;

		Manager.GetComponent<Battler_Scale>().DragScale_Out = true;

		yield return new WaitForSeconds (0.4f);

		Manager.GetComponent<Battler_Num>().battler = Battler1;
		Manager.GetComponent<Battler_Num>().changeNumDef = 0;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (0.05f);
		Manager.GetComponent<Battler_Num>().battler = Battler2;
		Manager.GetComponent<Battler_Num>().changeNumDef = 1;
		Manager.GetComponent<Battler_Num>().defNumChangeStart = true;
		yield return new WaitForSeconds (1f);

		Manager.GetComponent<Battler_Death>().battler = Battler1;
		Manager.GetComponent<Battler_Death>().Death = true;

		yield return new WaitForSeconds (0.8f);

		Manager.GetComponent<PilotAreaBack>().battler = Battler1;
		Manager.GetComponent<PilotAreaBack>().PilotAreaBack_Start = true;
		yield return new WaitForSeconds (1f);
		costIcon.SetActive (true);

	}
}
