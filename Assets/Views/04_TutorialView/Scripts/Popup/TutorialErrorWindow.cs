using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialErrorWindow : TutorialWindow {

	[SerializeField] private Button closeButton;

	[SerializeField] private Text message;
	private string messageText;

	public void Init (string mes, CallBack callBack){
		
		gameObject.SetActive (true);
		closeButton.gameObject.SetActive (false);

		messageText = mes;
		m_callback = callBack;

		transform.localScale = new Vector3 (1f, 1f, 0f);
		appear ();
	}


	override protected void onAppearmoveComplete (){

		Time.timeScale = 0;
		message.text = messageText;
		closeButton.gameObject.SetActive (true);
	}
}
