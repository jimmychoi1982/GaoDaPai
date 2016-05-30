using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_GasyaAnimation : MonoBehaviour {

	public Transform gasyaCamera;

	//ドア
	public GameObject doorBox;
	public RectTransform doorTop, doorBot;
	public Transform sukima, doorLightWh, doorLightGr;
	public Image sukimaImage, doorLightWhImage, doorLightGrImage;

	//ガシャルーム（ドアの奥の部屋）
	public Transform gasyaRoom, roomLight;
	public GameObject burstBox;

	//エフェクト
	public ParticleSystem holoPar;
	public ParticleSystem kiraPar;
	
	//カードボックス
	public Transform cardBox;

	//得たカード個別
	public Transform getCard1, getCard2, getCard3;

	//ガチャして出てきたカード群表示要素
	public GameObject get;

	//リザルト画面要素
	public GameObject resultBox;
	public ParticleSystem kira2, kira3;

	//カード裏の黒み、全体の白
	public Image bk;
	public GameObject wh;

	//ジングル,SE
	public AudioSource jingle;
	public AudioSource seHolo;
	public AudioSource seDoor;

}
