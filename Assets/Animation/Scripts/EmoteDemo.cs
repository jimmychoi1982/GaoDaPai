using UnityEngine;
using System.Collections;

public class EmoteDemo : MonoBehaviour {

	public Emote_ChatLog[] chatLog;
	public Animator emoteSelect;
	public int selectNum;
	public bool demo1, demo2;

	
	// Update is called once per frame
	void Update () {
		if (demo1 == true) {
			StartCoroutine(demo1Animation());
			demo1 = false;
		}

		if (demo2 == true) {
			StartCoroutine(demo2Animation());
			demo2 = false;
		}
	}

	IEnumerator demo1Animation(){
		//reset
		for (int i=0; i<3; i++) {
			chatLog [i].Reset ();
		}

		//first emote in
		chatLog[0].EmoteIn ();
		yield return new WaitForSeconds (2f);

		//second emote in
		chatLog[1].EmoteIn ();
		yield return new WaitForSeconds (2f);

		//third emote in and first emote out
		chatLog[0].EmoteUp ();
		chatLog[0].EmoteFadeOut ();
		chatLog[1].EmoteUp ();
		chatLog[2].EmoteIn ();
		yield return new WaitForSeconds (2f);

		chatLog[1].EmoteUp ();
		chatLog[1].EmoteFadeOut ();
		chatLog[2].EmoteUp ();
		yield return new WaitForSeconds (2f);
		chatLog[2].EmoteUp ();
		chatLog[2].EmoteFadeOut ();
	}
	
	IEnumerator demo2Animation(){
		emoteSelect.SetTrigger ("Open");
		yield return new WaitForSeconds (1f);
		emoteSelect.SetTrigger ("Select_"+selectNum);
		}
}
