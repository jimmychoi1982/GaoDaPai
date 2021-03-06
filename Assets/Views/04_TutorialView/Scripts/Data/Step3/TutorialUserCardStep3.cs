using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialUserCardStep3 : TutorialData {
	
	public int currentIndex = -1;
	public int currentCrewIndex = -1;
	
	private List<string> unitCard = new List<string>() {
		"cw32001080",
		"cw32001081",
	};

	private List<string> crewCard = new List<string>() {
		"cw21002005",
		"cw12001158"
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
		return currentCrewIndex;
	}
	
	public override void SetCurrentCrewIndex (int index){
		currentCrewIndex = index;
	}
	
	public override string GetCrewCardOfIndex (int index){
		return crewCard [index];
	}
}
