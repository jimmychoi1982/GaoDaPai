using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialStepButton : MonoBehaviour {

	[SerializeField] private Image image;
	[SerializeField] private Image arrowImage;
	[SerializeField] private Sprite[] activeSprite; // off = 0; on = 1;
	[SerializeField] private Sprite[] arrowActiveSprite; // off = 0; on = 1;

	public void SetActive (bool active){

		if (active) {
			image.sprite = activeSprite [1];
			arrowImage.sprite = arrowActiveSprite [1];
		} else {
			image.sprite = activeSprite [0];
			arrowImage.sprite = arrowActiveSprite [0];
		}
	}
}
