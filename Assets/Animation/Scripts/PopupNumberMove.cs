using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class PopupNumberMove : MonoBehaviour 
{
	public GameObject popupNum;
	public Text numberText;
	public string numberString; 
	public bool numberMoveStart;
	public bool plus;

	void Start () 
	{
		//初期化
		DOTween.Init(true, false, LogBehaviour.Default);

		//透明度設定
		numberText.color = new Color (0, 0, 0, 0);

	}
	
	void Update () 
	{
		//ダメージ数値を取得
		popupNum.GetComponent<Text> ().text = numberString;

		if(numberMoveStart == true){
			StartCoroutine(numberMoveAnimation());
			numberMoveStart = false;
		}
	}

	IEnumerator numberMoveAnimation(){

		//Reset
		//プラス値の場合は青、マイナス値の場合は赤
		if (plus == true) {
			numberText.color = new Color (0, 0, 1, 0);
		} else if (plus == false) {
			numberText.color = new Color (1, 0, 0, 0);
		}
		Transform popupNumTra = popupNum.GetComponent<Transform> ();
		yield return new WaitForSeconds(0.05f);

		//上に縮小しながら移動
		popupNumTra.DOLocalMoveY(40, 0.7f).SetRelative();
		popupNumTra.DOPunchScale(new Vector3(1f, 1f, 1f), 0.7f, 1, 0);

		//透明度変化
		numberText.DOFade (1, 0.2f);
		yield return new WaitForSeconds(0.3f);
		numberText.DOFade (0, 0.2f);

	}

}
