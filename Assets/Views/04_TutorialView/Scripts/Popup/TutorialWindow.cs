using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public abstract class TutorialWindow : MonoBehaviour {
	
	[SerializeField] private float toAppear;
	[SerializeField] private float toDisappear;
	[SerializeField] private float appearSpeed;
	[SerializeField] private float disappearSpeed;
	[SerializeField] private GameObject mask;

	public delegate void CallBack ();
	public CallBack m_callback;

	protected void appear (){
		//
		mask.SetActive (true);
		iTween.ScaleTo (gameObject, iTween.Hash ("x", toAppear,
		                                         "y", toAppear,
		                                         "speed", appearSpeed,
		                                         "isLocal", true,
		                                         "oncomplete", "onAppearmoveComplete"));
	}
	
	protected void disappear (){

		iTween.ScaleTo (gameObject, iTween.Hash ("x", toDisappear,
		                                         "y", toDisappear,
		                                         "speed", disappearSpeed,
		                                         "isLocal", true,
		                                         "oncomplete", "onDisappearmoveComplete"));
	}

	protected void onDisappearmoveComplete (){

		m_callback ();
		mask.SetActive (false);
	}

	public void CloseButton (){

		Time.timeScale = 1;
		disappear ();
	}
	
	protected abstract void onAppearmoveComplete ();
}
