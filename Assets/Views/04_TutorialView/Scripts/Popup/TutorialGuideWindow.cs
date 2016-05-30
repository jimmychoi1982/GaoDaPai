using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialGuideWindow : TutorialWindow {

	[SerializeField] private Text messageText;
	[SerializeField] private Button closeButton;
	[SerializeField] private Button nextButton;
	
	private string[] messageList;
	private int currentMessageIndex = 0;

	private bool init = false;

	public void Init (string message, CallBack callBack){
		
		currentMessageIndex = 0;
		m_callback = callBack;
		messageList = getMessageList (message);
		init = true;
	}

	public void NextButton (){
		showMessage ();
	}

	private bool isLast (){
		return currentMessageIndex > messageList.Length - 1;
	}

	private void showMessage (){
		messageText.text = messageList[currentMessageIndex];
		currentMessageIndex++;

		// 次のメッセージがなかったら、Close Button を Active
		if (isLast ()) {
			nextButton.gameObject.SetActive (false);
			closeButton.gameObject.SetActive (true);
			currentMessageIndex = 0;
		} 
	}

	private string[] getMessageList (string message){

		var list = message.Split ('+');
		return list;
	}

	void Update (){

		if (init) {
			init = false;
			closeButton.gameObject.SetActive (false);
			nextButton.gameObject.SetActive (false);
			
			showMessage ();

			// 連続表示メッセージがある場合Next機能On
			if (messageList.Length > 1) {
				nextButton.gameObject.SetActive (true);
			}

			appear ();
		}
	}

	override protected void onAppearmoveComplete (){

		// 連続表示メッセージがある場合Next機能On
		if (messageList.Length > 1) {
			nextButton.gameObject.SetActive (true);
		} else {
			closeButton.gameObject.SetActive (true);
		}
	}
}
