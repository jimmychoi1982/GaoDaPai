using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialEnemyCardStep5 :TutorialData {
	
	public int currentIndex = -1;
	
	private List<string> unitCard = new List<string>() {
		"cw32001113",
		"cw32001120",
	};

	private List<string> crewCard = new List<string>() {
		"cw21002010",
		"cw21002010",
		"cw21002010",
		"cw21002010",
		"cw21002010",
	}; 
	
	public override int GetCurrentIndex (){
		return currentIndex;
	}
	
	public override void SetCurrentIndex (int index){
		currentIndex = index;
	}
	
	public override string GetUnitCardOfIndex (int index){
		return unitCard [index];
	}

	//
	public override int GetCurrentCrewIndex (){
		return currentIndex;
	}
	
	public override void SetCurrentCrewIndex (int index){
		currentIndex = index;
	}

	public override string GetCrewCardOfIndex (int index){
		return crewCard [index];
	}
}
