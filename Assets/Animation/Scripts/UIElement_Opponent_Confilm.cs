using UnityEngine;
using System.Collections;

public class UIElement_Opponent_Confilm : MonoBehaviour {

	//Player情報ボックス、Enemy情報ボックス
	public Transform Player_Box, Enemy_Box;
	public Transform Player_Status, Enemy_Status;

	//color
	public GameObject bk, wh;

	//VSロゴ
	public GameObject VS_Animation;
	public Transform Logo;
	public GameObject Text;
	public SpriteRenderer TextSprite;
	public GameObject TextLight;
	public GameObject Effect;

	//退出ボタン
	public GameObject Drop_Battle;

	//SE
	public AudioSource SE_EnemyIn;
	public AudioSource SE_VS;


}
