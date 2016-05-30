using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using GameView;

public class Time_Count : MonoBehaviour {
	GameView.GameView gameView { get { return GameView.GameView.Instance; }}

	public Slider _slider;
	public Image sliderImage;
	public AudioSource SE_Alart;
	public bool timeCountStart;
	public bool SE_ON, SE_OFF;
	public float limitTime;
	private float subValue;
	//private float startTime;

	void Start () {

		//初期値は1(max)
		_slider.value = 1;
		if (limitTime != 0) {
			subValue = Time.fixedDeltaTime / limitTime;
		}
	}

	float _time = 1;
	void FixedUpdate () {
		//ゲージを減らしていく
		if (timeCountStart == true) {
			_time -= subValue;
			sliderImage.color -= new Color(0, subValue, subValue, 0);//ゲージをだんだん赤くしていく
			_slider.value = _time;
			if(_slider.value <= 0){
				Debug.Log ("Time_Count:時間切れ");
				timeCountStart = false;
				//StartCoroutine( gameView.TimeOut() );
			}
		}

		//時間切れ付近のSE再生
		if (SE_ON == true) {
			SE_Alart.volume = 0.2f;
			SE_Alart.Play ();
			SE_ON = false;
		}

		//SEをストップ
		if (SE_OFF == true) {
			SE_Alart.volume -= 0.01f;//フェードアウトして消えるようにする
			if(SE_Alart.volume == 0){
				SE_Alart.Stop ();
				SE_OFF = false;
			}
		}

	}

	//ゲージをmax値までリセット
	public void CountReset(){
		_time = 1;
		sliderImage.color = new Color(1, 1, 1, 1);
		_slider.value = _time;
	}


}
