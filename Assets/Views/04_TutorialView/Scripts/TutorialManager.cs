using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialManager : MonoBehaviour {

	public GameObject bk, bkOver;

	public Text maxSize;
	public Text nowSize;

	public Live2DTutorialController Live2DTutorialController;
	public GameObject ssWindow;
	public ScreenShootDeck ssDeck;
	public GameObject LoadingTextBox;

	private int deepHierarchyNum = -1;

	private bool inputMargin;

	private Steward steward;

	private Mage mage { get { return Mage.Instance; }}
	private Logger logger { get { return mage.logger("TutorialManager"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial) {
			LoadingTextBox.SetActive (true);
			steward.PlayBGMHome ();
		}

		new Task (loadAssetBundle.DownloadAllAssetBundles (() => {}));
	}

	void Update() {
		if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial) {
			maxSize.text = loadAssetBundle.assetBundleTotalCount.ToString ();
			nowSize.text = loadAssetBundle.assetBundleDownloadCount.ToString ();
		}
	}

	// //Button Click (Tutorial End)
	public void WindowButtonClick2 (){
		if (inputMargin == false) {
			
			
			deepHierarchyNum++;
			inputMargin = true;
			new Task (InputMarginSetting ());
			
			switch (deepHierarchyNum) {
			case 0:
				Live2DTutorialController.SaylaTalkSwitch2 (0);
				break;
				
			case 1:
				Live2DTutorialController.SaylaTalkSwitch2 (1);
				break;
				
			case 2:
				Live2DTutorialController.SaylaTalkSwitch2 (2);
				break;
				
			case 3:
				Live2DTutorialController.SaylaTalkSwitch2 (3);
				break;
				
			case 4:
				Live2DTutorialController.SaylaTalkSwitch2 (4);
				break;
				
			case 5:
				Live2DTutorialController.SaylaTalkSwitch2 (5);
				break;
				
			case 6:
				Live2DTutorialController.SaylaTalkSwitch2 (6);
				break;

			case 7:
				Live2DTutorialController.SaylaTalkSwitch2 (7);
				break;
					
			case 8:
				Live2DTutorialController.SaylaTalkSwitch2 (8);
				break;
					
			case 9:
				Live2DTutorialController.SaylaTalkSwitch2 (9);
				break;
					
			case 10:
				Live2DTutorialController.SaylaTalkSwitch2 (10);
				break;
					
			case 11:
				Live2DTutorialController.SaylaTalkSwitch2 (11);
				break;
					
			case 12:
				Live2DTutorialController.SaylaTalkSwitch2 (12);
				break;

			case 13:
				new Task (EndFunction2 ());
				break;
			}
		}
	}

	//Button Click (Tutorial Intro)
	public void WindowButtonClick(){

		if (inputMargin == false) {


			deepHierarchyNum++;
			inputMargin = true;
			new Task (InputMarginSetting ());
		
			switch (deepHierarchyNum) {
			case 0:
				Live2DTutorialController.SaylaTalkSwitch (0);
				break;

			case 1:
				Live2DTutorialController.SaylaTalkSwitch (1);
				break;
					
			case 2:
				Live2DTutorialController.SaylaTalkSwitch (2);
				break;
					
			case 3:
//				bk.GetComponent<Image> ().DOFade (0.5f, 0.5f);
				Live2DTutorialController.SaylaTalkSwitch (3);
				break;
					
			case 4:
				Live2DTutorialController.SaylaTalkSwitch (4);
				break;
					
			case 5:
				Live2DTutorialController.SaylaTalkSwitch (5);
				break;
					
			case 6:
				Live2DTutorialController.SaylaTalkSwitch (6);
				break;

			case 7:
				Live2DTutorialController.SaylaTalkSwitch (7);				
				break;

			case 8:
				new Task (EndFunction ());
				break;
				/*	
			case 8:
				ssDeck.ImageSet (0);
				Live2DTutorialController.SaylaTalkSwitch (8);
				break;
					
			case 9:
				ssDeck.ImageSet (1);
				Live2DTutorialController.SaylaTalkSwitch (9);
				break;
					
			case 10:
				Live2DTutorialController.SaylaTalkSwitch (10);
				break;
					
			case 11:
				Live2DTutorialController.SaylaTalkSwitch (11);
				break;
					
			case 12:
				new Task (SaylaCenterFunction (12));
				break;
					
			case 13:
				ssDeck.ImageSet (2);
				new Task (SaylaLeftFunction (13));
				break;
					
			case 14:
				Live2DTutorialController.SaylaTalkSwitch (14);
				break;
					
			case 15:
				ssDeck.ImageSet (3);
				Live2DTutorialController.SaylaTalkSwitch (15);
				break;
					
			case 16:
				ssDeck.ImageSet (4);
				Live2DTutorialController.SaylaTalkSwitch (16);
				break;
					
			case 17:
				Live2DTutorialController.SaylaTalkSwitch (17);
				break;
					
			case 18:
				new Task (SaylaCenterFunction (18));
				break;
					
			case 19:
				Live2DTutorialController.SaylaTalkSwitch (19);
				break;
					
			case 20:
				ssDeck.ImageSet (5);
				new Task (SaylaLeftFunction (20));
				break;
					
			case 21:
				ssDeck.ImageSet (6);
				Live2DTutorialController.SaylaTalkSwitch (21);
				break;
					
			case 22:
				ssDeck.ImageSet (7);
				Live2DTutorialController.SaylaTalkSwitch (22);
				break;
					
			case 23:
				ssDeck.ImageSet (8);
				Live2DTutorialController.SaylaTalkSwitch (23);
				break;
					
			case 24:
				ssDeck.ImageSet (9);
				Live2DTutorialController.SaylaTalkSwitch (24);
				break;
					
			case 25:
				ssDeck.ImageSet (10);
				Live2DTutorialController.SaylaTalkSwitch (25);
				break;
					
			case 26:
				ssDeck.ImageSet (11);
				Live2DTutorialController.SaylaTalkSwitch (26);
				break;
					
			case 27:
				new Task (SaylaCenterFunction (27));
				break;
					
			case 28:
				ssDeck.ImageSet (12);
				new Task (SaylaLeftFunction (28));
				break;
					
			case 29:
				ssDeck.ImageSet (13);
				Live2DTutorialController.SaylaTalkSwitch (29);
				break;
					
			case 30:
				ssDeck.ImageSet (14);
				Live2DTutorialController.SaylaTalkSwitch (30);
				break;
					
			case 31:
				Live2DTutorialController.SaylaTalkSwitch (31);
				break;
					
			case 32:
				Live2DTutorialController.SaylaTalkSwitch (32);
				break;
					
			case 33:
				ssDeck.ImageSet (15);
				Live2DTutorialController.SaylaTalkSwitch (33);
				break;
					
			case 34:
				Live2DTutorialController.SaylaTalkSwitch (34);
				break;
					
			case 35:
				new Task (SaylaCenterFunction (35));
				break;
					
			case 36:
				Live2DTutorialController.SaylaTalkSwitch (36);
				break;
					
			case 37:
				Live2DTutorialController.SaylaTalkSwitch (37);
				break;
					
			case 38:
				ssDeck.ImageSet (16);
				new Task (SaylaLeftFunction (38));
				break;
					
			case 39:
				ssDeck.ImageSet (17);
				Live2DTutorialController.SaylaTalkSwitch (39);
				break;
					
			case 40:
				ssDeck.ImageSet (18);
				Live2DTutorialController.SaylaTalkSwitch (40);
				break;
					
			case 41:
				ssDeck.ImageSet (19);
				Live2DTutorialController.SaylaTalkSwitch (41);
				break;
					
			case 42:
				new Task (SaylaCenterFunction (42));
				break;
					
			case 43:
				Live2DTutorialController.SaylaTalkSwitch (43);
				break;
					
			case 44:
				Live2DTutorialController.SaylaTalkSwitch (44);
				break;
					
			case 45:
				Live2DTutorialController.SaylaTalkSwitch (45);
				break;
					
			case 46:
				Live2DTutorialController.SaylaTalkSwitch (46);
				break;
					
			case 47:
				Live2DTutorialController.SaylaTalkSwitch (47);
				break;
					
			case 48:
				Live2DTutorialController.SaylaTalkSwitch (48);
				break;
					
			case 49:
				Live2DTutorialController.SaylaTalkSwitch (49);
				break;
					
			case 50:
				Live2DTutorialController.SaylaTalkSwitch (50);
				break;
					
			case 51:
				new Task (EndFunction ());
				break;
			}
			*/
			}
		}
	}

	//AnimationFunction-------------------------------------------------------------------------------

	IEnumerator Program7Function () {
		Live2DTutorialController.live2DPositionLeft ();
		Live2DTutorialController.SaylaTalkSwitch (7);
		yield return new WaitForSeconds (0.5f);
		ssWindow.SetActive (true);
		ssWindow.GetComponent<CanvasGroup> ().DOFade(1, 0.5f);
	}

	IEnumerator SaylaCenterFunction (int talk) {
		Live2DTutorialController.live2DPositionCenter ();
		ssWindow.GetComponent<CanvasGroup> ().DOFade (0, 0.3f);
		yield return new WaitForSeconds (0.3f);
		Live2DTutorialController.live2DPositionCenter ();
		Live2DTutorialController.SaylaTalkSwitch (talk);
	}

	IEnumerator SaylaLeftFunction (int talk) {
		Live2DTutorialController.live2DPositionLeft ();
		Live2DTutorialController.SaylaTalkSwitch (talk);
		yield return new WaitForSeconds (0.3f);
		ssWindow.GetComponent<CanvasGroup> ().DOFade (1, 0.5f);
	}

	IEnumerator EndFunction () {
		GameObject.Find ("Main Canvas/Panel/skipbutton").GetComponent <Button> ().enabled = false;

		bkOver.SetActive (true);
		bkOver.GetComponent<Image> ().DOFade (1, 0.6f);
		yield return new WaitForSeconds (2f);
		TutorialIntroEnd ();
	}


	IEnumerator EndFunction2 () {
		GameObject.Find ("Main Canvas/Panel/skipbutton").GetComponent <Button> ().enabled = false;
		
		bkOver.SetActive (true);
		bkOver.GetComponent<Image> ().DOFade (1, 0.6f);
		yield return new WaitForSeconds (2f);
		TutorialEnd ();
	}

	//Setting----------------------------------------------------------------------------------------

	IEnumerator bkFadeOut () {
		bk.GetComponent<Image>().DOFade (0f, 0.5f);
		yield return new WaitForSeconds (0.5f);
		bk.SetActive (false);
	}

	IEnumerator InputMarginSetting () {
		yield return new WaitForSeconds (1f);
		inputMargin = false;
	}

	//Skip----------------------------------------------------------------------------------------

	public void SkipButtonEnter(){
		steward.PlaySETap ();
		steward.OpenDialogWindow ("オープニング", "オープニングをスキップします\nよろしいですか？", "はい", () => {TutorialIntroEnd ();}, "いいえ", () => {});
	}

//	public void SkipButtonEnter2(){
//		steward.PlaySETap ();
//		steward.OpenDialogWindow ("エピローグ", "エピローグをスキップします\nよろしいですか？", "はい", () => {TutorialEnd ();}, "いいえ", () => {});
//	}


	// NewTutorialView使用予定、削除予定
	void TutorialEnd(){
		if (GameSettings.TutorialState == GameSettings.TutorialStates.Tutorial) {
			GameSettings.TutorialState = GameSettings.TutorialStates.SecondInitialRegistration;
			steward.LoadNextScene ("LoadingView");
		} else {
			GameSettings.TutorialState = GameSettings.TutorialStates.Done;
			steward.LoadNextScene ("MenuView");
		}
	}

	void TutorialIntroEnd(){
		
		Application.LoadLevel ("NewTutorialViewStep1");
	}

	//loading Update


}
