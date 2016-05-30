using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cursor_Animation : MonoBehaviour {
	public float Power_Num;
	public bool R_or_L_Flg;

	private Vector3 First_Position;
	private bool Cursor_Turn;
	private float timeleft;
	private float Gravity_Power;

	void Start () {
		this.First_Position = this.transform.localPosition;

		//カーソルが動く向きの設定
		if (this.R_or_L_Flg== true) {
			this.Gravity_Power = - Power_Num;
		} else if (this.R_or_L_Flg == false) {
			this.Gravity_Power = + Power_Num;
		}
	}
	
	void Update () {
		timeleft -= Time.deltaTime;

		//一秒ずつ制限時間が減っていく
		if (timeleft <= 0.0) {
			//カーソルの動き判別
			if (this.Cursor_Turn == true) {
				timeleft = 1.0f;
				this.Cursor_Turn = false;
			} else if (this.Cursor_Turn == false) {
				timeleft = 0.1f;
				this.Cursor_Turn = true;
			}
		}

		//元の座標に戻る
		if (this.Cursor_Turn == true) {
			this.transform.DOLocalMove (new Vector3 (this.First_Position.x, this.transform.localPosition.y, 0f), 0.1f);
		//力の働く方向にアニメーションする
		} else if (this.Cursor_Turn == false) {
			this.transform.DOLocalMove (new Vector3 (this.First_Position.x + this.Gravity_Power, this.transform.localPosition.y, 0f), 0.5f);
		}
	}
}
