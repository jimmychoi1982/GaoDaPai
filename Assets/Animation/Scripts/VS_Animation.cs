using UnityEngine;
using System.Collections;

public class VS_Animation : MonoBehaviour 
{

	public GameObject VS_Animation_Plate;
	public bool Event_Active;
	
	public float Count_Time;
	public float Active_Marzin;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.Event_Active == true) 
		{
			this.VS_Animation_Plate.SetActive(true);
			this.Count_Time += Time.deltaTime;
			
			if(this.Count_Time > this.Active_Marzin)
			{
				this.Event_Active = false;
				this.Count_Time = 0.0f;
			}
		}
		if (this.Event_Active == false) 
		{
			this.VS_Animation_Plate.SetActive(false);
		}
	}
}
