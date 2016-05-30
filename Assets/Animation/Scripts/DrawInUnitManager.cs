using UnityEngine;
using System.Collections;

public class DrawInUnitManager : MonoBehaviour {

	public GameObject Manager;
	public UIElement_deckBox DeckBoxPlayer, DeckBoxOpponent;
	public bool demoStart;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(demoStart == true){
			StartCoroutine(DrawInUnit_Anime_Start());
			demoStart = false;
		}
	
	}

	//動作サンプル
	IEnumerator DrawInUnit_Anime_Start () {

		//自分のターン
		Debug.Log ("自分のターン");
			Manager.GetComponent<Deck_Animation_uGUI>().deckBox = DeckBoxPlayer;
		Manager.GetComponent<Deck_Animation_uGUI>().drawCard_Start = true;
			yield return new WaitForSeconds (2f);

			Manager.GetComponent<Card_DrawInUnit>().MyTurn = true;
			Manager.GetComponent<Card_DrawInUnit>().LastPos = new Vector3(680, -350, 0);
			Manager.GetComponent<Card_DrawInUnit>().Anime_Start = true;
			yield return new WaitForSeconds (5f);

		//敵のターン
		Debug.Log ("敵のターン");
		Manager.GetComponent<Deck_Animation_uGUI>().deckBox = DeckBoxOpponent;
		Manager.GetComponent<Deck_Animation_uGUI>().drawCard_Start = true;
			yield return new WaitForSeconds (2f);

			Manager.GetComponent<Card_DrawInUnit>().MyTurn = false;
			Manager.GetComponent<Card_DrawInUnit>().LastPos = new Vector3(-680, 350, 0);
			Manager.GetComponent<Card_DrawInUnit>().Anime_Start = true;

	}

}
