using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectTauntOffMod : MonoBehaviour {
	
	public UIElement_BattlerIcon battlerIcon;
	private GameObject freamObj, iconObj, lightObj;
	// 生成されるエフェクト
	public GameObject ef_TauntBroke;


	public void EffectStart (){
		StartCoroutine(keywordEffectAnimation());
	}


	//防衛アイコンを通常アイコンに戻す
	public IEnumerator keywordEffectAnimation () {

		//フレーム画像のスプライト変更
		freamObj = battlerIcon.freamObj;
		Image freamImg = freamObj.GetComponent<Image> ();
		freamImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_State/ui_a_frame1_1", typeof(Sprite)) as Sprite;
		
		//アイコン画像のスプライト変更
		iconObj = battlerIcon.iconObj;
		Image iconImg = iconObj.GetComponent<Image> ();
		iconImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_In/ui_a_BT01-004_a1", typeof(Sprite)) as Sprite;
		
		//後光画像のスプライト変更
		lightObj = battlerIcon.lightObj;
		Image lightImg = lightObj.GetComponent<Image> ();
		lightImg.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_State/ui_a_frame1_7b", typeof(Sprite)) as Sprite;

		//拡縮アニメ
		Transform battlerTra = battlerIcon.battlerTra;
		battlerTra.DOPunchScale (new Vector3(0.6f, 0.6f, 0.6f), 0.2f, 1, 0f);

		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(ef_TauntBroke, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//プレハブ消去
		yield return new WaitForSeconds (1.5f);
		GameObject.Destroy(efObj);
		
	}
	
}