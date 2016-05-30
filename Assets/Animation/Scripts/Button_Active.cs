using UnityEngine;
using System.Collections;

public class Button_Active : MonoBehaviour
{
	public GameObject END_TURN;
	public GameObject ENEMY_TURN;

	public bool Both_sides_Flg;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		//END_TURN ENEMY_TURN を入れ替える処理
		if (Both_sides_Flg == true) 
		{
			this.END_TURN.SetActive (false);
			this.ENEMY_TURN.SetActive (true);
		} 
		else if (Both_sides_Flg == false) 
		{
			this.END_TURN.SetActive (true);
			this.ENEMY_TURN.SetActive (false);
		}
	}

	//ボタンが押下されたときに呼ばれる関数
	public void Push_Button()
	{
		//フラグを切り替えます
		if (this.Both_sides_Flg == false) 
		{
			this.Both_sides_Flg = true;
		}

		else if (this.Both_sides_Flg == true) 
		{
			this.Both_sides_Flg = false;
		}
	}
}
