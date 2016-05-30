using UnityEngine;
using System;
using System.Collections;

public class TutorialAtkDefButton : MonoBehaviour {

	private Action _action;

	public void Init (Action action){
		gameObject.SetActive (true);
		_action = action;
	}

	public void OnClick (){
		_action ();
		gameObject.SetActive (false);		
	}
}
