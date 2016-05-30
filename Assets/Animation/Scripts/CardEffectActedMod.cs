using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class CardEffectActedMod : MonoBehaviour {

	public GameObject battler;
	public Image iconImage;
	// 生成されるエフェクト
	public GameObject EF_Acted;

	public void EffectStart (){
		StartCoroutine(ActedAnimation());
	}

	//行動済みにする
	public IEnumerator ActedAnimation () {
		StartCoroutine (IconColorChange ());
		StartCoroutine (EffectBirth ());

		battler.transform.DOPunchPosition (new Vector3 (0, 10, 0), 0.3f, 10, 1, false);

		yield return new WaitForSeconds (0.1f);
		
		//アイコン画像のスプライト変更
		iconImage.sprite = Resources.Load("Reletion_Card_Atlas/Unit_Icon_In/ui_a_BT01-004_a2", typeof(Sprite)) as Sprite;
		
	}

	public IEnumerator EffectBirth(){
		GameObject efObj = (GameObject)Instantiate(EF_Acted, transform.position, Quaternion.identity);
		efObj.transform.SetParent (battler.transform, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(0,0,0);
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		yield return new WaitForSeconds (1f);
		GameObject.Destroy(efObj);
	}

	public IEnumerator IconColorChange(){
		iconImage.DOColor (new Color (0.5f, 0.5f, 0.5f, 1), 0.3f);
		yield return new WaitForSeconds (0.3f);
		iconImage.DOColor (new Color (1, 1, 1, 1), 0.2f);
	}
	
}