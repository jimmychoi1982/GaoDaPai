using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialDataFactory {

	/// <summary>
	/// セイラの文章を取得
	/// </summary>
	/// <returns>The dialogue.</returns>
	/// <param name="data">Data.</param>
	public static string GetNextDialogue (TutorialData data){
		
			
		int index = data.GetCurrentIndex () + 1;
		string dialogue = data.GetDialogueOfIndex (index);
		data.SetCurrentIndex (index);
		
		return dialogue;
	}

	/// <summary>
	/// Gets the unit card.
	/// </summary>
	/// <returns>The unit card.</returns>
	/// <param name="data">Data.</param>
	public static string GetNextCard (TutorialData data){

		int index = data.GetCurrentIndex () + 1;
		data.SetCurrentIndex (index);
		return data.GetUnitCardOfIndex (index);
	}

	/// <summary>
	/// Gets the current unit card.
	/// </summary>
	/// <returns>The current unit card.</returns>
	/// <param name="data">Data.</param>
	public static string GetCurrentUnitCard (TutorialData data){

		return data.GetUnitCardOfIndex (data.GetCurrentIndex ());
	}

	/// <summary>
	/// Gets the crew card.
	/// </summary>
	/// <returns>The crew card.</returns>
	/// <param name="data">Data.</param>
	public static string GetCrewCard (TutorialData data){

		return data.GetCrewCardOfIndex (0);
	}


	public static string GetNextCrewCard (TutorialData data){

		int index = data.GetCurrentCrewIndex () + 1;
		data.SetCurrentCrewIndex (index);
		return data.GetCrewCardOfIndex (index);
	}

	public static string GetCurrentCrewCard (TutorialData data){
		
		return data.GetCrewCardOfIndex (data.GetCurrentCrewIndex ());
	}
}
