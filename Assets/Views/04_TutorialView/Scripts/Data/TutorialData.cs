using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// チュートリアルデータの親クラス
/// </summary>
public class TutorialData {

	public virtual int GetCurrentIndex (){return 0;}
	public virtual void SetCurrentIndex (int index){}
	public virtual string GetUnitCardOfIndex (int index){return "";}

	public virtual string GetDialogueOfIndex (int index){return "";}

	public virtual string GetCrewCardOfIndex (int index){return "";}
	public virtual int GetCurrentCrewIndex (){return 0;}
	public virtual void SetCurrentCrewIndex (int index){}

	public virtual void LaunchUserActionOfIndex (int index){}
	public virtual void LaunchEnemyActionOfIndex (int index){}
}
