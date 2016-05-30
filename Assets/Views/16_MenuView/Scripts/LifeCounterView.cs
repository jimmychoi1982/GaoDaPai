using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class LifeCounterView : MonoBehaviour {
	public Sprite[] timeSprite = new Sprite[10]; 
	public Sprite[] LifeSprite = new Sprite[10];

	public Image player1LifeNumber10;
	public Image player1LifeNumber01;
	public Image player2LifeNumber10;
	public Image player2LifeNumber01;

	public Image[] TimerNumberImages = new Image[3];
	public Image[] FlashingImage = new Image[6];

	public GameObject turnGraphic;
	public GameObject timerBox;
	public GameObject setupWindow;
	public GameObject setupWindowBackground;
	public CanvasGroup setupWindowParts;
	public GameObject configWindow;
	public GameObject configWindowBackground;
	public CanvasGroup configWindowParts;

	public Image[] SetTimeButton = new Image[4];

	private int player1Life = 30;
	private int player2Life = 30;
	private int startTime = 90;
	private int currentTime;
	private float timeleft;

	private bool timeCount;
	private bool isStart = false;
	private bool isShowView = false;

	private int[] OddAndEven = new int[2];
	private float[] DeathTime = new float[2];

	private bool turnFlg = true;
	
	private Steward steward;

	void Start () {
		steward = GameObject.Find ("/Steward").GetComponent<Steward> ();
	}

	void Update () {
		if (!isShowView) return;

		if (timeCount == true) {
			timeleft -= Time.deltaTime;
			if (timeleft <= 0.0) {
				timeleft = 1.0f;
				
				currentTime--;
			}

			SetTimerImage ();

			if (currentTime < 0) {
				EndTurn ();
			}
		}

		if (player1Life == 0) {
			DeathTime [0] -= Time.deltaTime;
			if (DeathTime [0] <= 0.0) {
				DeathTime [0] = 1.0f;
				OddAndEven [0]++;
			}

			for (int i=0; i<3; i++) {
				if (OddAndEven [0] % 2 == 0) {
					FlashingImage [i].GetComponent<Image> ().DOFade (0f, 1f);
				} else {
					FlashingImage [i].GetComponent<Image> ().DOFade (1f, 1f);
				}
			}
		} else {
			for (int i=0; i<3; i++) {
				FlashingImage [i].GetComponent<Image> ().color = new Color(1f,1f,1f,1f);
			}
		}

		if (player2Life == 0) {
			DeathTime[1] -= Time.deltaTime;
			if (DeathTime[1] <= 0.0) {
				DeathTime[1] = 1.0f;
				OddAndEven[1]++;
			}
			
			for(int i=3; i<6; i++) {
				if(OddAndEven[1] % 2 == 0) {
					FlashingImage[i].GetComponent<Image> ().DOFade (0f, 1f);
				} else {
					FlashingImage[i].GetComponent<Image> ().DOFade (1f, 1f);
				}
			}
		} else {
			for (int i=3; i<6; i++) {
				FlashingImage [i].GetComponent<Image> ().color = new Color(1f,1f,1f,1f);
			}
		}

	}

	public void Initialize () {
		player1Life = 30;
		player2Life = 30;		
		SetPlayer1LifeImage ();
		SetPlayer2LifeImage ();
		turnFlg = true;
		turnGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
		timerBox.transform.localScale = new Vector3(1f, 1f, 1f);

		timeleft = 1.0f;
		startTime = 90;
		currentTime = startTime;
		SetButtonColor (1);
		SetTimerImage ();
		timeCount = false;
		isStart = false;

		isShowView = true;
	}

	public void OpenSetupWindow () {
		steward.PlaySETap ();
		new Task (fadeInAnimation (setupWindow, setupWindowBackground, setupWindowParts));
		timeCount = false;
	}
	
	public void SetStartTime (int startTime) {
		steward.PlaySETap ();
		
		this.startTime = startTime;
		currentTime = startTime;
		SetTimerImage ();
	}
	
	public void SetButtonColor (int index) {
		this.startTime = startTime;
		foreach (var image in SetTimeButton) {
			image.color = new Color ((float)57 / (float)255, (float)255 / (float)255, (float)255 / (float)255);
		}
		SetTimeButton [index].color = new Color ((float)228 / (float)255, (float)152 / (float)255, (float)15 / (float)255);
	}
	
	public void CloseSetupWindow () {
		steward.PlaySECancel ();
		new Task (fadeOutAnimation (setupWindow, setupWindowBackground, setupWindowParts));
		if (isStart) {
			timeCount = true;
		}
	}
	
	public void StartLifeCounter () {
		steward.PlaySETap ();

		player1Life = 30;
		player2Life = 30;
		SetPlayer1LifeImage ();
		SetPlayer2LifeImage ();
		turnFlg = true;
		turnGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
		timerBox.transform.localScale = new Vector3(1f, 1f, 1f);

		timeleft = 1.0f;
		currentTime = startTime;
		timeCount = true;
		isStart = true;
		new Task (fadeOutAnimation (setupWindow, setupWindowBackground, setupWindowParts));
	}

	public void ChangeTurn () {
		steward.PlaySETap ();
		EndTurn ();
	}
	
	private void EndTurn () {
		timeleft = 1.0f;
		currentTime = startTime;
		
		if (turnFlg) {
			turnFlg = false;
			turnGraphic.transform.localScale = new Vector3(1f, -1f, 1f);
			timerBox.transform.localScale = new Vector3(-1f, -1f, -1f);
		} else {
			turnFlg = true;
			turnGraphic.transform.localScale = new Vector3(1f, 1f, 1f);
			timerBox.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}
	
	public void OpenConfigWindow () {
		steward.PlaySETap ();
		new Task (fadeInAnimation (configWindow, configWindowBackground, configWindowParts));
		timeCount = false;
	}

	public void Restart () {
		steward.PlaySETap ();
		new Task (fadeOutAnimation (configWindow, configWindowBackground, configWindowParts));
		new Task (fadeInAnimation (setupWindow, setupWindowBackground, setupWindowParts));
		timeCount = false;
	}

	public void BackToMenuView () {
		isShowView = false;
		timeCount = false;
		isStart = false;
		new Task (fadeOutAnimation (configWindow, configWindowBackground, configWindowParts));
		GameObject.Find ("/Main Canvas").GetComponent<MenuView> ().Back (true);
	}

	public void CloseConfigWindow () {
		steward.PlaySECancel ();
		new Task (fadeOutAnimation (configWindow, configWindowBackground, configWindowParts));
		if (isStart) {
			timeCount = true;
		}
	}
	
	public void AddPlayer1Life () {
		steward.PlaySETap ();
		if (player1Life >= 30) return;
		player1Life++;
		SetPlayer1LifeImage ();
	}
	
	public void SubtractPlayer1Life() {
		steward.PlaySETap ();
		if (player1Life <= 0) return;
		player1Life--;
		SetPlayer1LifeImage ();
	}

	public void AddPlayer2Life () {
		steward.PlaySETap ();
		if (player2Life >= 30) return;
		player2Life++;
		SetPlayer2LifeImage ();
	}
	
	public void SubtractPlayer2Life () {
		steward.PlaySETap ();
		if (player2Life <= 0) return;
		player2Life--;
		SetPlayer2LifeImage ();
	}
	
	private void SetPlayer1LifeImage () {
		player1LifeNumber10.sprite = LifeSprite[(int)(player1Life/10)];
		player1LifeNumber01.sprite = LifeSprite[(int)(player1Life%10)];
	}
	
	private void SetPlayer2LifeImage () {
		player2LifeNumber10.sprite = LifeSprite[(int)(player2Life/10)];
		player2LifeNumber01.sprite = LifeSprite[(int)(player2Life%10)];
	}

	private void SetTimerImage () {
		if (currentTime >= 0) {
			TimerNumberImages [0].sprite = timeSprite [(int)(currentTime / 60)];
			TimerNumberImages [1].sprite = timeSprite [(int)((int)(currentTime % 60) / 10)];
			TimerNumberImages [2].sprite = timeSprite [(int)((int)(currentTime % 60) % 10)];
		}
	}

	private IEnumerator fadeInAnimation (GameObject window, GameObject background, CanvasGroup canvasGroup) {
		window.SetActive (true);
		background.SetActive (true);
		background.GetComponent<Image> ().DOFade (0.7f, 0.4f);
		yield return new WaitForSeconds (0.1f);
		canvasGroup.DOFade (1f, 0.1f);
	}
	
	private IEnumerator fadeOutAnimation (GameObject window, GameObject background, CanvasGroup canvasGroup) {
		background.GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
		background.SetActive (false);
		canvasGroup.DOFade (0f, 0.1f);
		yield return new WaitForSeconds (0.2f);
		window.SetActive (false);
	}
}
