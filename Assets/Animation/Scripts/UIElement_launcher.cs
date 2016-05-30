using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_launcher : MonoBehaviour {

	public GameObject launcher;
	public Transform launcherTra;

	//ユニットカード、コストカード
	public GameObject mainCardObj, costCardObj;
	public Image mainCardImage, costCardImage;
	public Transform mainCardTra, costCardTra;
	public GameObject cardSetObj;
	public Transform cardSet;

	//出撃ボタン
	public GameObject button;

	//MS出撃音
	public AudioSource SE_Button;

	//エフェクト
	public GameObject ef_Embarkation;
	public GameObject ef_Change;

}
