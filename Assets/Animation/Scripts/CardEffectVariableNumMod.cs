using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class CardEffectVariableNumMod : MonoBehaviour {

	public UIElement_BattlerIcon battlerIcon;
	private Transform battlerTra;
	private Transform atkNumTra, defNumTra;
	// 生成されるエフェクト
	public GameObject EF_VariableNum;

	// Use this for initialization
	public void EffectStart () {
		StartCoroutine(CardEffectVariable_Animation());
	}
	
	//ATK数値とDEF数値が入れ替わる
	IEnumerator CardEffectVariable_Animation(){
		//Reset
		battlerTra = battlerIcon.battlerTra;
		atkNumTra = battlerIcon.atkNumTra;
		defNumTra = battlerIcon.defNumTra;

		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		GameObject efObj = (GameObject)Instantiate(EF_VariableNum, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,-76,0);//パイロットアイコンのATK,DEF数値の位置にエフェクトを表示
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		yield return new WaitForSeconds (0.3f);
		atkNumTra.DOScale (new Vector3 (0, 0, 0), 0.2f);
		defNumTra.DOScale (new Vector3 (0, 0, 0), 0.2f);
		yield return new WaitForSeconds (0.3f);

		//このタイミングでATK,DEF数字画像を取得して、交換して画像変更
		//…のロジックをここに書いてください。

		atkNumTra.DOScale (new Vector3 (1, 1, 1), 0.2f).SetEase (Ease.OutBack);
		defNumTra.DOScale (new Vector3 (1, 1, 1), 0.2f).SetEase (Ease.OutBack);

		yield return new WaitForSeconds (2f);
		GameObject.Destroy(efObj);

	}
}
