using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TutorialActionStep {

	public delegate void Action ();
	public class ActionArray
	{
		public Action action;
	}

	public int currentIndex = -1;
	
	private List<ActionArray> actionArray = new List<ActionArray> ();
	
	//
	public int GetCurrentIndex (){
		return currentIndex;
	}

	//
	public void SetCurrentIndex (int index){
		currentIndex = index;
	}

	public void AddAction (Action enemyAction){

		ActionArray array = new ActionArray ();
		array.action = enemyAction;

		actionArray.Add (array);
	}

	public ActionArray GetEnemyActionOfIndex (int index){
		return actionArray [index];
	}

	//
	public void LaunchEnemyActionOfIndex (int index){
		actionArray [index].action ();
	}

	public void LauchNextAction (){

		if (currentIndex < actionArray.Count - 1) {
			int index = currentIndex + 1;
			actionArray [index].action ();
			currentIndex++;
			Debug.Log ("Tutorial Action Current index became " + currentIndex);
		}
	}
}
