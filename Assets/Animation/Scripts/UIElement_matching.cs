using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_matching : MonoBehaviour {

	//Matching_Search
	public GameObject SearchAnimeBox;
	public Transform parts_01, parts_02, parts_03, parts_04, parts_05;
	public Image parts_06, parts_07;

	//Matching_CutIn
	public GameObject parts10Box, textBox, parts12Box;
	public Transform parts12Top, parts12Bot;
	public GameObject parts8, parts9;
	public GameObject warning1, warning2;

	//Matching_BoxMove
	public Image bk;
	public GameObject effectVS, Return_Button, SearchInfo;
	public Transform playerBox, enemyInfo, enemyBox;
	public AudioSource SE_VS;
}
