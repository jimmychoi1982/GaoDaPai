using UnityEngine;
using System.Collections;
using DG.Tweening;//DOTween

public class Emote_ChatLog : MonoBehaviour {

	public GameObject emoteLine;

	private Vector3 emotePos;

	void Start () {
		emotePos = emoteLine.transform.localPosition;
	}
	
	public void Reset(){
		emoteLine.transform.localPosition = emotePos;
		emoteLine.GetComponent<CanvasGroup> ().alpha = 0;
	}

	//animation-----------------------------------------------------------
	
	public void EmoteIn(){
		emoteLine.GetComponent<CanvasGroup>().DOFade (1, 0.5f);
		emoteLine.transform.DOLocalMoveX(0, 0.5f).SetEase (Ease.OutCubic);
	}
	
	public void EmoteUp(){
		emoteLine.transform.DOLocalMoveY (100, 0.4f).SetRelative();
	}
	
	public void EmoteFadeOut(){
		emoteLine.GetComponent<CanvasGroup> ().DOFade (0, 0.5f);
	}

}
