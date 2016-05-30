using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class Battler_Num : MonoBehaviour {

	public UIElement_BattlerIcon battler;
	private Image atkNum, defNum;
	private Transform atkNumTra, defNumTra;
	public int changeNumAtk, changeNumDef;
	public bool atkNumChangeStart, defNumChangeStart;

	//カード効果によって文字色を変えるフラグ　※暫定
	public bool cardEffect;
	public bool effectUp;

	// Use this for initialization
	void Start () {

		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);
	
	}
	
	// Update is called once per frame
	void Update () {

		if(atkNumChangeStart == true){
			BattlerReset();
		}

		if(defNumChangeStart == true){
			BattlerReset();
		}
	
	}

	void BattlerReset(){
		atkNumTra = battler.atkNumTra;
		defNumTra = battler.defNumTra;
		atkNum = battler.atkNum;
		defNum = battler.defNum;
		if (atkNumChangeStart == true) {
			numChange_Animation (atkNumTra, atkNum, changeNumAtk);
			atkNumChangeStart = false;
		}
		if(defNumChangeStart == true){
			numChange_Animation(defNumTra, defNum, changeNumDef);
			defNumChangeStart = false;
		}
	}

	//ユニットアイコンの数値が変わったときのアニメーション
	public void numChange_Animation (Transform numTra, Image numImg, int num) {

		numTra.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f, 1, 1);

		//数字画像を動的に変更するロジックを書く
		numImg.sprite = Resources.Load("ATK_DEF_Number/u-atk-number-"+num+"a", typeof(Sprite)) as Sprite;

		//この書き方はマズい気がするので、適宜変更してください・・・。
		if (cardEffect == true) {
			if (effectUp == true) {
				numImg.color = new Color (0, 1, 1, 1);//上昇したらポジティブ色に（水色）
			} else if (effectUp == false) {
				numImg.color = new Color (1, 0, 0, 1);//下降したらネガティブ色に（赤）
			}
		} else {
			numImg.color = new Color (1, 1, 1, 1);//カード効果が切れたら白に戻す
		}
	}
	

}
