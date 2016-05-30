using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CostOnMiss : MonoBehaviour {

	//乗せられなかったコストアイコン
	public Transform costCard;

	public UIElement_CostCards costCards;
	private AudioSource SE_CostOnMiss;

	////該当コストカードのコストエリアでの位置
	private Vector3 previousPos;

	public bool CostOnMiss_Start;


	void Start () {

		//DOTween宣言
		DOTween.Init(true, false, LogBehaviour.Default);

		//戻る位置：実際ではコストエリア内で位置が可変するので、ロジックを組み替えてください。
		previousPos = costCard.localPosition;

		SE_CostOnMiss = costCards.SE_CostOnMiss;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(CostOnMiss_Start == true){
			StartCoroutine(CostOnMiss_Animation());
			CostOnMiss_Start = false;
		}
	
	}

	//コストカードが乗せられない条件のとき、ドラッグを離した位置から持ち場に戻るアニメ
	public IEnumerator CostOnMiss_Animation () {

		//現在のPositionから、指定した位置にまで戻る
		costCard.DOLocalMove(previousPos, 0.7f).SetEase(Ease.OutExpo);
		yield return new WaitForSeconds (0.2f);

		SE_CostOnMiss.Play ();

		//ゆれ動作
		costCard.DOShakeRotation(0.4f, new Vector3(0,0,90), 70, 40);

	}

}
