using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_Table : MonoBehaviour {

	//サークル回転アニメーション用
	public Transform circle1, circle2;

	//カウンターエリア
	public Image counterIconImage;
	public GameObject counterNumber;
	public Transform counterBox, coverTop, coverBot;
	public GameObject EF_Light_Bomb, EF_CounterCardInvoke;

	public AudioSource SE_CounterCover;

}
