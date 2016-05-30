using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialEnemyCardStep2 :TutorialData {
	
	public int currentIndex = -1;
	
	private List<string> unitCard = new List<string>() {
		"cw32002032",
		"cw32002032",
		"cw32002032",
		"cw32002032",
		"cw32002032",
		"cw32002032",
	};

	private List<string> crewCard = new List<string>() {
		"cw21001010"
	}; 

	// Unit Card
	public override int GetCurrentIndex (){
		return currentIndex;
	}
	
	public override void SetCurrentIndex (int index){
		currentIndex = index;
	}
	
	public override string GetUnitCardOfIndex (int index){
		return unitCard [index];
	}

	// Crew Card
	public override string GetCrewCardOfIndex (int index){
		return crewCard [index];
	}

	public override int GetCurrentCrewIndex (){
		return currentIndex;
	}

	public override void SetCurrentCrewIndex (int index){
		currentIndex = index;
	}
}
