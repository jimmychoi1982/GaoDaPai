using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI
using DG.Tweening;//DOTween

public class CostIconUnitSortieAnimation : MonoBehaviour {

	public UIElement_CostCards costCards;
	private AudioSource SE_IconTap;
	public GameObject costIcon;
	public Transform costIconTra;
	public Image costIconImg;

	public bool costSelect, costTapIn, costTapOut;

	// Use this for initialization
	void Start () {

		SE_IconTap = costCards.SE_IconTap;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (costSelect == true) {
			costSelectAnimation();
			costSelect = false;
		}

		if(costTapIn == true){
			TapScale_In_Animation();
			costTapIn = false;
		}
		
		if(costTapOut == true){
			TapScale_Out_Animation();
			costTapOut = false;
		}

	}

	//コスト選択アニメ
	void costSelectAnimation(){
		SE_IconTap.Play ();
		//拡縮アニメ
		costIconTra.DOPunchScale (new Vector3 (0.1f, 0.1f, 0.1f), 0.2f, 1, 1);
		//画像変更（行動済みに）※再度押すと未行動の画像に変更するようにしてください
		costIconImg.sprite = Resources.Load("Reletion_Card_Atlas/Cost_Icon_In/ui_d_BT01-150_2", typeof(Sprite)) as Sprite;
	}

	//アイコンタップで少し大きくなるアニメ
	void TapScale_In_Animation () {
		SE_IconTap.Play();
		costIconTra.localScale = new Vector3(1,1,1);
		costIconTra.DOScale(0.1f, 0.2f).SetRelative();
	}
	
	//指を離すともとの大きさに戻るアニメ
	void TapScale_Out_Animation () {
		costIconTra.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		costIconTra.DOScale(-0.1f, 0.2f).SetRelative().SetEase(Ease.OutBack);
	}



}
