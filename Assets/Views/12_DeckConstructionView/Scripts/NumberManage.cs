using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberManage : MonoBehaviour {
	public GameObject NumberObject10;
	public GameObject NumberObject1;

	//[HideInInspector]
	public int NumString1;

	//[HideInInspector]
	public int NumString10;

	private Sprite[] NumberImage = new Sprite[11];
	private bool isInit = false;

	private void Init() {
		for (int i = 0; i < 10; i++) {
			NumberImage [i] = Resources.Load ("DeckCardNumber/deck_card_number_" + i.ToString (), typeof(Sprite)) as Sprite;
		}
		NumberImage [10] = Resources.Load ("DeckCardNumber/deck_card_number_blank", typeof(Sprite)) as Sprite;
	}

	public void SetNumber (int Num) {
		if (!isInit) {
			Init ();
			isInit = true;
		}
		string NumString = Num.ToString ();
		if (NumString.Length == 0) return;
		NumString1 = int.Parse (NumString.Substring (NumString.Length - 1));
		NumString10 = NumString.Length > 1 ? int.Parse (NumString.Substring (NumString.Length - 2, 1)) : 0;
//		if (NumString10 == 0) NumString10 = 10;

		NumberObject10.GetComponent<Image> ().sprite = NumberImage[NumString10];
		NumberObject1.GetComponent<Image> ().sprite = NumberImage[NumString1];
	}
}
