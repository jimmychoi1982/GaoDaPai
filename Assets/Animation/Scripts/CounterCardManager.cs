using UnityEngine;
using System.Collections;

public class CounterCardManager : MonoBehaviour {

	public GameObject Manager;
	public bool Demo_Start;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Demo_Start == true){
			StartCoroutine(CounterCard_Anime_Start());
			Demo_Start = false;
		}
	
	}

	//動作サンプル
	IEnumerator CounterCard_Anime_Start () {
			Manager.GetComponent<CounterCard_Animation>().MyTurn = true;
			Manager.GetComponent<CounterCard_Animation>().CounterCard_Set = true;
			yield return new WaitForSeconds (2f);
			Manager.GetComponent<CounterCard_Animation>().CounterCard_Invoke = true;
			yield return new WaitForSeconds (1f);
			Manager.GetComponent<CardTipsWindowAnimation>().windowIn = true;
			yield return new WaitForSeconds (3f);
			//このタイミングで効果発動（カウンター効果のエフェクト出す、どんな減少が起こったか見せる）
			//効果を全て終えたら、TipsWindowはOut、横のカードを破棄する
	}
}