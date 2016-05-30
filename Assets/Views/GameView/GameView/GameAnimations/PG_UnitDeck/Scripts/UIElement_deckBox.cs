using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_deckBox : MonoBehaviour {

	public Transform deck_card;
	public Transform deckCircle;
	public ParticleSystem particle;
	public Transform drawCard;

	//カード量に応じて表示物を変える
	public GameObject deckAmount1, deckAmount2, deckAmount3, deckLight1, deckLight2, deckLight3;

}
