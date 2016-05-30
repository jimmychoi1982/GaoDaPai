using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectNotAction : MonoBehaviour {
	
	public GameObject battler;
	public Image iconImage;
	// 生成されるエフェクト
	public GameObject EF_NotAction;

	public bool effectStart;
	
	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		if(effectStart == true){
			StartCoroutine(NotActionAnimation());
			effectStart = false;
		}
	}


	//アイコン画像を未行動にする
	public IEnumerator NotActionAnimation () {

		StartCoroutine (IconColorChange ());
		StartCoroutine (EffectBirth ());

		battler.transform.DOPunchPosition (new Vector3 (0, 10, 0), 0.3f, 10, 1, false);

		yield return new WaitForSeconds (0.1f);
		
		//アイコン画像のスプライト変更
		iconImage.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_In/ui_a_BT01-004_a1", typeof(Sprite)) as Sprite;

		
	}

	public IEnumerator EffectBirth(){
		GameObject efObj = (GameObject)Instantiate(EF_NotAction, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battler.transform, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;
		
		yield return new WaitForSeconds (1f);
		GameObject.Destroy(efObj);
	}
	
	public IEnumerator IconColorChange(){
		iconImage.DOColor (new Color (1f, 1f, 1f, 0.5f), 0.3f);
		yield return new WaitForSeconds (0.3f);
		iconImage.DOColor (new Color (1, 1, 1, 1), 0.2f);
	}
	
}