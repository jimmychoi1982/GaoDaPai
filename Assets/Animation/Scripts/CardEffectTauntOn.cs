using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectTauntOn : MonoBehaviour {
	
	public UIElement_BattlerIcon battlerIcon;
	private GameObject freamObj, iconObj, lightObj;
	// 生成されるエフェクト
	public GameObject ef_Taunt;

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


	//通常アイコンから防衛アイコンにチェンジする
	public IEnumerator keywordEffectAnimation () {

		//フレーム画像のスプライト変更
		freamObj = battlerIcon.freamObj;
		Image freamImg = freamObj.GetComponent<Image> ();
		freamImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_State/ui_a_frame2_1", typeof(Sprite)) as Sprite;

		//アイコン画像のスプライト変更
		iconObj = battlerIcon.iconObj;
		Image iconImg = iconObj.GetComponent<Image> ();
		iconImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_In/ui_a_BT01-004_b1", typeof(Sprite)) as Sprite;

		//後光画像のスプライト変更
		lightObj = battlerIcon.lightObj;
		Image lightImg = lightObj.GetComponent<Image> ();
		lightImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_State/ui_a_frame2_7b", typeof(Sprite)) as Sprite;

		//拡縮アニメ
		Transform battlerTra = battlerIcon.battlerTra;
		battlerTra.DOPunchScale (new Vector3(0.6f, 0.6f, 0.6f), 0.2f, 1, 0f);

		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(ef_Taunt, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//プレハブ消去
		yield return new WaitForSeconds (2f);
		GameObject.Destroy(efObj);
		
	}
	
}