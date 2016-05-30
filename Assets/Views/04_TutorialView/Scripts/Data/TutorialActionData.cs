using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class TutorialActionData {

	public delegate void Action ();
	public class ActionArray
	{
		public Action action;
	}

	public abstract void AddAction (Action action);

	public abstract int GetCurrentIndex ();
	public abstract void SetCurrentIndex (int index);
	
	public abstract void LaunchEnemyActionOfIndex (int index);

	public abstract ActionArray GetEnemyActionOfIndex (int index);
	public abstract void LauchNextAction ();
}
