using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
public class UIElement_CardDetailWindow : MonoBehaviour {

	public Transform cardDetailWindow;
	public Image backColor;
	public CanvasGroup windowCanvas;
	
	//CardBox
	public GameObject card1, card2;
	public Transform card1Tra, card2Tra;
	public GameObject changeArrowObj;
	public Transform changeArrow;

	//LabelBox
	public Transform cardLabelBox;
	public GameObject cardEffectBox2Obj;
	public Text cardNameLabel, cardTypeLabel, cardKindLabel, cardEffect1Label, cardEffect2Label;

	//SE
	public AudioSource SE_open,SE_change;

	public CanvasGroup canvasGroup;
}
