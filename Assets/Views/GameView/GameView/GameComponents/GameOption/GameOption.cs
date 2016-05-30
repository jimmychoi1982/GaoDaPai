using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace GameView {
	public class GameOption : MonoBehaviour {
		//
		static Game game { get { return Game.Instance; }}
		static GameView gameView { get { return GameView.Instance; }}

		//
		[Header("Sound ON/OFF Button")]
		public Button soundOnButton;
		public Button soundOffButton;

		//
		[Header("Volume Slider")]
		public Slider bgmSlider;
		public Slider seSlider;

		private Steward steward;
		private bool isSteward = false;

		private bool isSoundOn;

		[Header("Debug Button")]
		public GameObject autoWinButton;

		//
		void Start() {
			// If Steward is in hierarchy, keep steward as property.
			var stewardObj = GameObject.Find ("Steward");
			if (stewardObj != null) {
				isSteward = true;
				steward = stewardObj.GetComponent<Steward> ();
			}

			bgmSlider.value = GameSettings.VolumeBGM;
			seSlider.value = GameSettings.VolumeSE;
			
			isSoundOn = (GameSettings.VolumeMaster == 1f);
			ChangeSoundButtonColor();

			if (Debug.isDebugBuild) {
				autoWinButton.SetActive(true);
			}

		}

		//
		public void Open() {
			bool meStarted = (bool)game.tCurrentGame["playerStarted"][gameView.meUserId];
			bool enemyStarted = (bool)game.tCurrentGame["playerStarted"][gameView.enemyUserId];
			if (!meStarted || !enemyStarted) return;
			gameObject.SetActive(true);
		}

		//
		public void SetSoundOn() {
			GameSettings.VolumeMaster = 1f;
			ToggleSoundButton();
			if (isSteward) {
				steward.SetVolume();
			}
		}

		//
		public void SetSoundOff() {
			GameSettings.VolumeMaster = 0f;
			ToggleSoundButton();
			if (isSteward) {
				steward.SetVolume();
			}
		}

		//
		public void SetBGMVolume() {
			GameSettings.VolumeBGM = bgmSlider.value;
			if (isSteward) {
				steward.SetVolume();
			}
		}

		//
		public void SetSEVolume() {
			GameSettings.VolumeSE = seSlider.value;
			if (isSteward) {
				steward.SetVolume();
			}
		}
		
		//
		public void Surrender() {
			gameView.ForfeitGame();
		}

		public void Autowin() {
			gameView.AutoWinGame ();

		}

		public void Close() {
			gameObject.SetActive(false);
		}
		
		//
		private void ToggleSoundButton() {
			isSoundOn = !isSoundOn;
			ChangeSoundButtonColor();
		}	

		//
		private void ChangeSoundButtonColor() {
			if (isSoundOn) {
				ChangeButtonColor (soundOnButton, false);
				ChangeButtonColor (soundOffButton, true);
			} else {
				ChangeButtonColor (soundOnButton, true);
				ChangeButtonColor (soundOffButton, false);
			}
		}	

		//
		private void ChangeButtonColor(Button btn, bool isOn) {
			btn.interactable = isOn;
		}	
	}
}
