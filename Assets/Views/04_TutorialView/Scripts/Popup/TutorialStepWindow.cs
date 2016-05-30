using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialStepWindow : TutorialWindow {

	[SerializeField] private GameObject[] stepObj;
	[SerializeField] private Button closeButton;
	[SerializeField] private Button skipbutton;

	[SerializeField] private TutorialSkipWindow tutorialSkipWindow;

	public void Init (string step, CallBack callback){

		foreach (GameObject obj in stepObj) {
			if (step == obj.name){
				obj.GetComponent<TutorialStepButton> ().SetActive (true);
			}else{
				obj.GetComponent<TutorialStepButton> ().SetActive (false);
			}
		}

		m_callback = callback;
		gameObject.SetActive (true);
		closeButton.gameObject.SetActive (false);
		skipbutton.gameObject.SetActive (false);

		transform.localScale = new Vector3 (0.5f, 0.5f, 0f);
		appear ();
	}

	override protected void onAppearmoveComplete (){

		closeButton.gameObject.SetActive (true);
		skipbutton.gameObject.SetActive (true);
	}

	public void SkipButtonEnter(){

		if (GameObject.Find ("Steward") != null) {
		
			var steward = GameObject.Find ("Steward").GetComponent <Steward> ();
			steward.PlaySEOK ();
		}
		tutorialSkipWindow.Init ();
	}

	public void PopupOnTap (){

		m_callback ();
	}
}
