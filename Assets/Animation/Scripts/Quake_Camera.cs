using UnityEngine;
using System.Collections;

public class Quake_Camera : MonoBehaviour 
{
	public Transform Quake_Target;

	public bool Quake_Lv1;
	public bool Quake_Lv2;
	public bool Quake_Lv3;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

		//Ｌｖ１の揺れを実行
		if (this.Quake_Lv1 == true) 
		{
			this.Quake_Target.position = new Vector3(Random.Range (-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Quake_Target.position.z);
			StartCoroutine(Quake_Start_Count (0.5f));
		}

		//Ｌｖ２の揺れを実行
		if (this.Quake_Lv2 == true) 
		{
			this.Quake_Target.position = new Vector3(Random.Range (-0.06f, 0.06f), Random.Range(-0.06f, 0.06f), Quake_Target.position.z);
			StartCoroutine(Quake_Start_Count (0.75f));
		}

		//Ｌｖ３の揺れを実行
		if (this.Quake_Lv3 == true) 
		{
			this.Quake_Target.position = new Vector3(Random.Range (-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Quake_Target.position.z);
			StartCoroutine(Quake_Start_Count (2.0f));
		}
	}

	//実行時間を設定する関数
	IEnumerator Quake_Start_Count(float What_Wait_Time)
	{
		yield return new WaitForSeconds (What_Wait_Time);
		this.Quake_Lv1 = false;
		this.Quake_Lv2 = false;
		this.Quake_Lv3 = false;
		this.Quake_Target.position = new Vector3(0.0f, 0.0f, Quake_Target.position.z);
	}
}
