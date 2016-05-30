using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Prime31;

public class TutorialSkipWindow : TutorialWindow {

	[SerializeField] private Button okButton;
	[SerializeField] private Button closeButton;
	[SerializeField] private Button allSkipbutton;

	[SerializeField] private Text messageText;

	public void Init (){
		
		gameObject.SetActive (true);
		okButton.gameObject.SetActive (false);
		closeButton.gameObject.SetActive (false);
		allSkipbutton.gameObject.SetActive (false);
		
		transform.localScale = new Vector3 (1f, 1f, 0f);

		messageText.text = getMessage ();

		m_callback = () => {};
		appear ();
	}

	public void OkButton (){

		if (GameObject.Find ("Steward") != null) {
			
			var steward = GameObject.Find ("Steward").GetComponent <Steward> ();
			steward.PlaySEOK ();
		}
		Time.timeScale = 1;
		Application.LoadLevel (GetNextTutorialScene ());
	}

	public void CancelButton (){

		if (GameObject.Find ("Steward") != null) {
			
			var steward = GameObject.Find ("Steward").GetComponent <Steward> ();
			steward.PlaySECancel ();
		}
		gameObject.SetActive (false);
	}

	public void AllSkip (){

		if (GameObject.Find ("Steward") != null) {
			
			var steward = GameObject.Find ("Steward").GetComponent <Steward> ();
			steward.PlaySEOK ();
		}
		#if UNITY_ANDROID || UNITY_IOS
		Localytics.tagEvent("Tutorial: Skipped all");
		#endif
		Time.timeScale = 1;
		Application.LoadLevel ("NewTutorialEndView");
	}

	/// <summary>
	/// 次のチュートリアルシーン名を取得
	/// </summary>
	/// <returns>The next tutorial scene.</returns>
	public string GetNextTutorialScene (){
		string currentStep = PlayerPrefs.GetString ("Tutorial");
		
		if (currentStep == "Step1") {
			
			return "NewTutorialViewStep2";
			
		} else if (currentStep == "Step2") {
			
			return "NewTutorialViewStep3";
			
		} else if (currentStep == "Step3") {
			
			return "NewTutorialViewStep4";		
			
		} else if (currentStep == "Step4") {
			
			return "NewTutorialViewStep5";	
			
		} else if (currentStep == "Step5") {
			
			return "NewTutorialEndView";
		}
		
		Debug.LogError ("Unknow Scene Name");
		return "";
	}

	private string getMessage (){

		string message = "チュートリアル" + PlayerPrefs.GetString ("Tutorial").ToUpper () + "をスキップします。よろしいですか？";
		return message;
	}

	override protected void onAppearmoveComplete (){
		
		okButton.gameObject.SetActive (true);
		closeButton.gameObject.SetActive (true);
		allSkipbutton.gameObject.SetActive (true);
	}
}
