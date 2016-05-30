using UnityEngine;
using System.Collections;

public class Rideing_Event : MonoBehaviour
{
	public GameObject Event_Cut_In;
	public bool Event_Active;

	public float Count_Time;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.Event_Active == true) 
		{
			this.Event_Cut_In.SetActive(true);
			this.Count_Time += Time.deltaTime;

			if(this.Count_Time > 2.0f)
			{
				this.Event_Active = false;
				this.Count_Time = 0.0f;
			}
		}
		if (this.Event_Active == false) 
		{
			this.Event_Cut_In.SetActive(false);
		}
	}
}
