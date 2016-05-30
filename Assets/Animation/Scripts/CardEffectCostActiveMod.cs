using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class CardEffectCostActiveMod : MonoBehaviour {

	public UIElement_Icon costIcon;
	private Image freamImage;
	private GameObject efObj;
	public UIElement_CostCards costCards;

	//生成するエフェクト
	public GameObject EF_costIconActive;


	public void EffectStart () {
		CardEffectCostActive_Animation();
	}

	public void EffectStop () {

		Destroy(efObj);
		//フレーム画像変更
		freamImage.sprite = Resources.Load("Reletion_Card_Atlas/Cost_Icon_Frame/ui_d1_frame_1", typeof(Sprite)) as Sprite;
	}

	//行動済みのコストアイコンを使用可能にするエフェクト
	//※アイコン状態はグレースのままこのエフェクトを再生する
	void CardEffectCostActive_Animation(){
		freamImage = costIcon.freamImage;

		Transform IconTra = costIcon.IconTra;
		
		//エフェクトプレハブ取得、指定オブジェクトの中にインスタンス生成、トリガーでエフェクト再生
		efObj = (GameObject)Instantiate(EF_costIconActive, transform.position, Quaternion.identity);
		efObj.transform.SetParent (IconTra, true);
		Transform efTra = efObj.GetComponent<Transform>();
		efTra.localScale = new Vector3(1,1,1);
		efTra.localPosition = new Vector3(-2.5f, 9.5f, 0);//コストアイコンの中央にエフェクトを配置
		efObj.GetComponent<Effect_Play_Limit>().Effect_Start = true;

		//フレーム画像変更
		freamImage.sprite = Resources.Load("Reletion_Card_Atlas/Cost_Icon_Frame/ui_d1_frame_3", typeof(Sprite)) as Sprite;

	}
}
