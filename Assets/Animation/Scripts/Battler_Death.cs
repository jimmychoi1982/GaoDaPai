using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class Battler_Death : MonoBehaviour {

	//死ぬ対象Battler
	public UIElement_BattlerIcon battler;
	private CanvasGroup battlerCanvasGroup;
	private Transform battlerTra;
	private GameObject battlerObj;

	//生成されるエフェクト
	public GameObject EF_CardDeath;

	public bool Death;

	// Use this for initialization
	void Start () {

		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Death == true){
			StartCoroutine(Death_Animation());
			Death = false;
		}
	}

	//ユニット破壊アニメーション
	public IEnumerator Death_Animation () {

		//Reset
		battlerTra = battler.battlerTra;
		battlerObj = battler.battlerObj;
		battlerObj.SetActive(true);
		battlerCanvasGroup = battler.battlerCanvasGroup;
		battlerCanvasGroup.alpha = 1;

		//対象ユニットがふるえる
		battlerTra.DOShakePosition(0.7f, new Vector3(20, 20, 0), 40, 10, false);

		yield return new WaitForSeconds (0.4f);

		//エフェクト再生
		GameObject efObj = (GameObject)Instantiate(EF_CardDeath, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battlerTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<EF_UnitDeath>().Effect_Play_Limit = true;

		//対象ユニットをフェードアウト
		yield return new WaitForSeconds (0.2f);
		battlerCanvasGroup.DOFade (0, 0.5f);

		//Destroy
		yield return new WaitForSeconds (3f);
		GameObject.Destroy(battlerObj);
		GameObject.Destroy(efObj);
	}


}
