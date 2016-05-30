using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_BattlerIcon : MonoBehaviour {
	
	public GameObject battlerObj;
	public Transform battlerTra;
	public GameObject freamObj;
	public GameObject iconObj;
	public GameObject lightObj;
	public Image atkNum;
	public Transform atkNumTra;
	public Image defNum;
	public Transform defNumTra;
	public GameObject sizeObj;
	public GameObject pilotObj;
	public CanvasGroup battlerCanvasGroup;
	public Image deffenceIcon;
	public Image deffenceFrame;
	public Image deffenceLight;
	public GameObject normalSet;
	public GameObject deffenceSet;
	public float nomalAtkY;
	public float nomalDefY;
	public float nomalSizeY;
	public float onBoardAtkY;
	public float onBoardDefY;
	public float onBoardSizeY;
}