using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardEffectUnitDamage : MonoBehaviour {
	
	public UIElement_BattlerIcon battlerIcon;
	// 生成されるエフェクト
	public GameObject ef_CardEffectDamage;

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


	//ヒットエフェクト出る
	public IEnumerator keywordEffectAnimation () {

		Transform battlerTra = battlerIcon.battlerTra;

		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(ef_CardEffectDamage, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;
	
		//ダメージを受けて震える
		battlerTra.DOShakePosition(0.4f, new Vector3(20, 20, 0), 40, 10, false);

		//プレハブ消去
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy(efObj);
		
	}
	
}