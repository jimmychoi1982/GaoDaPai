using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class ScreenShootDeck : MonoBehaviour {

	public Image ssImage;
	public Sprite[] ssSpriteDeck;

	public void ImageSet(int ssNumber) {
		ssImage.sprite = ssSpriteDeck [ssNumber];
	}
}
