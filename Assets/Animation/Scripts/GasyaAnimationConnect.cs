 using UnityEngine;
using System.Collections;

public class GasyaAnimationConnect : MonoBehaviour {

	public GameObject Manager;
	public UIElement_GetCard getCard1, getCard2, getCard3;
	public UIElement_GasyaGetCards getCards;
	private GameObject resultCard1, resultCard2, resultCard3, resultNewLabel1, resultNewLabel2, resultNewLabel3;

	public int getCardNum;//1 or 3

	public bool Card1_Rare, Card2_Rare, Card3_Rare;
	public bool Card1_New, Card2_New, Card3_New;

	public bool cardsGetAnimationStart;

	void Update () {
		if (cardsGetAnimationStart == true) {
			StartCoroutine (cardsGetAnimation());
			cardsGetAnimationStart = false;
		}
		
	}
	
	IEnumerator cardsGetAnimation(){

		resultCard1 = getCards.card1;
		resultCard2 = getCards.card2;
		resultCard3 = getCards.card3;
		resultNewLabel1 = getCards.newLabel1;
		resultNewLabel2 = getCards.newLabel2;
		resultNewLabel3 = getCards.newLabel3;

		//1枚目表示
		Manager.GetComponent<GasyaAnimationScript> ().getCard = getCard1;
		Rare_New_Set (Card1_Rare, Card1_New, resultNewLabel1);
		Manager.GetComponent<GasyaAnimationScript>().Gasya_Start = true;

		//時間経過で次のカードに切り替え
		yield return new WaitForSeconds (6f);

		//3枚ガシャの場合、以下のアニメを再生
		if (getCardNum == 3) {
			//2枚目表示
			Manager.GetComponent<GasyaAnimationScript> ().getCard = getCard2;
			Rare_New_Set (Card2_Rare, Card2_New, resultNewLabel2);
			Manager.GetComponent<GasyaAnimationScript> ().Next_Start = true;
			yield return new WaitForSeconds (0.4f);
			Manager.GetComponent<GasyaAnimationScript> ().Appeal_Start = true;

			//時間経過で次のカードに切り替え
			yield return new WaitForSeconds (2f);

			//3枚目表示
			Manager.GetComponent<GasyaAnimationScript> ().getCard = getCard3;
			Rare_New_Set (Card3_Rare, Card3_New, resultNewLabel3);
			Manager.GetComponent<GasyaAnimationScript> ().Next_Start = true;
			yield return new WaitForSeconds (0.4f);
			Manager.GetComponent<GasyaAnimationScript> ().Appeal_Start = true;

			//2秒経過で結果画面に切り替え
			yield return new WaitForSeconds (2f);
		}

		//結果表示
		ResultView ();

	}

	//NEWカード、レアカード判定
	void Rare_New_Set(bool rareFlg, bool newFlg, GameObject cardLabel){
		if (rareFlg == true) {
			Manager.GetComponent<GasyaAnimationScript>().RareCardFlg = true;
		}else if(rareFlg == false){
			Manager.GetComponent<GasyaAnimationScript>().RareCardFlg = false;
		}
		if (newFlg == true) {
			Manager.GetComponent<GasyaAnimationScript> ().NewCardFlg = true;
			cardLabel.SetActive (true);
		} else if (newFlg == false) {
			Manager.GetComponent<GasyaAnimationScript> ().NewCardFlg = false;
			cardLabel.SetActive (false);
		}
	}

	//結果表示
	void ResultView(){

		if (getCardNum == 1) {
			resultCard1.transform.localPosition = new Vector3(0,-50,0);//カード1を真ん中に表示
			resultCard2.SetActive (false);
			resultCard3.SetActive (false);
		} else if (getCardNum == 3) {
			resultCard1.transform.localPosition = new Vector3(-240,-50,0);//カード1左に表示
			resultCard2.SetActive (true);
			resultCard3.SetActive (true);
		}

		Manager.GetComponent<GasyaAnimationScript> ().Result_Start = true;
	}


}
