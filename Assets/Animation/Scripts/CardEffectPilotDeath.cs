using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardEffectPilotDeath : MonoBehaviour {

	public UIElement_BattlerIcon battlerIcon;
	private GameObject pilotObj;
	// 生成されるエフェクト
	public GameObject EF_pilotDeath;
	public bool effectStart;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(effectStart == true){
			StartCoroutine(keywordEffectAnimation());
			effectStart = false;
		}
	
	}

	//パイロットアイコンが消える
	IEnumerator keywordEffectAnimation () {
		//Reset
		Transform battlerTra = battlerIcon.battlerTra;
		pilotObj = battlerIcon.pilotObj;
		Transform pilot = pilotObj.GetComponent<Transform> ();

		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(EF_pilotDeath, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,-104,0);//パイロットアイコンの位置にエフェクト生成
		efObj.GetComponent<EF_pilotDeath>().Effect_Start = true;
		
		//ダメージを受けて震える
		pilot.DOShakePosition(0.3f, new Vector3(5, 5, 0), 40, 10, false);
		yield return new WaitForSeconds (0.5f);

		battlerTra.DOShakePosition (0.8f, new Vector3 (0, 40, 0), 40, 10, false);
		yield return new WaitForSeconds (0.7f);
		pilotObj.SetActive (false);

		yield return new WaitForSeconds (1f);
		GameObject.Destroy(efObj);
		
	}

}
