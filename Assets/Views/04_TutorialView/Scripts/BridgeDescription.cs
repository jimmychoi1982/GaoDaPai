using UnityEngine;
using System.Collections;
using System.Linq;

public class BridgeDescription : MonoBehaviour {

	[Header ("Frames")]
	public GameObject frame1;
	public GameObject frame2;

	public delegate void CallBack ();
	private CallBack _callBack;

	public void Init (CallBack callBack){
		_callBack = callBack;
	}

	public void OnClick (){
		_callBack ();
	}

	public void ActiveFrame1AndAppear (){
		gameObject.SetActive (true);
		frame1.SetActive (true);
	}

	public void UnActiveFrame1ActionFrame2 (){
		frame1.SetActive (false);
		frame2.SetActive (true);
	}

	public void UnActiveFrame2AndDisappear (){
		frame2.SetActive (false);
	}
}
