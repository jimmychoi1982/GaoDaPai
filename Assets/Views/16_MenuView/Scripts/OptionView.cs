using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionView : MonoBehaviour {
	public Image[] soundSpriteFrontEnd = new Image[2];
	public Sprite[] soundSpriteOverride = new Sprite[4];

	public GameObject bgmSlider;
	public GameObject seSlider;

	private Steward steward;
	
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		bgmSlider.GetComponent<Slider> ().value = GameSettings.VolumeBGM;
		seSlider.GetComponent<Slider> ().value = GameSettings.VolumeSE;

		if (GameSettings.VolumeMaster == 1f) {
			SoundOnPushButton ();
		} else {
			SoundOffPushButton ();
		}
	}
	
	public void SoundOnPushButton() {
		GameSettings.VolumeMaster = 1f;
		soundSpriteFrontEnd[0].sprite = soundSpriteOverride[0];
		soundSpriteFrontEnd[1].sprite = soundSpriteOverride[1];
		steward.SetVolume ();
	}
	
	public void SoundOffPushButton() {
		GameSettings.VolumeMaster = 0f;
		soundSpriteFrontEnd[0].sprite = soundSpriteOverride[2];
		soundSpriteFrontEnd[1].sprite = soundSpriteOverride[3];
		steward.SetVolume ();
	}

	public void OnValueChangedBGM () {
		GameSettings.VolumeBGM = bgmSlider.GetComponent<Slider> ().value;
		steward.SetVolume ();
	}
	
	public void OnValueChangedSE () {
		GameSettings.VolumeSE = seSlider.GetComponent<Slider> ().value;
		steward.SetVolume ();
	}
}
