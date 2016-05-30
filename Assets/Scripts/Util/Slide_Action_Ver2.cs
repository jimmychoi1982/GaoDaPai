using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Slide_Action_Ver2 : MonoBehaviour {

	public GameObject[] Slide_Cursor = new GameObject[2];
	public GameObject Move_Object;
	public GameObject List_From_Create_Object;
	public Text NowPage_Text;
	public Text MaxPage_Text;

	public float Move_Power;
	public float X_Correction_Value;
	public float Y_Correction_Value;
	
	public int One_Window_MAX_Object;
	
	private Vector3 Set_Pos;
	private int Look_Now_Page_Int;
	private int Slide_Page_MAX_Int;

	private Steward steward;

	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		this.Look_Now_Page_Int = 1;

		this.Slide_Page_MAX_Int = (int)Mathf.Ceil ((float)List_From_Create_Object.GetComponent<List_Instance> ().Create_Number / (float)this.One_Window_MAX_Object);
		this.MaxPage_Text.text = this.Slide_Page_MAX_Int.ToString();
		ChangePageDisplay ();
	}

	void OnEnable() {
		ChangePageDisplay ();
	}

	public void RightSlide() {
		steward.PlaySETap ();

		this.Set_Pos.x -= Move_Power;
		
		this.Move_Object.transform.DOLocalMove (Set_Pos, 0.5f).SetEase(Ease.OutCubic);

		this.Look_Now_Page_Int++;

		ChangePageDisplay ();
	}

	public void LeftSlide() {
		steward.PlaySETap ();

		this.Set_Pos.x += Move_Power;
		
		this.Move_Object.transform.DOLocalMove (Set_Pos, 0.5f).SetEase(Ease.OutCubic);
		
		this.Look_Now_Page_Int--;

		ChangePageDisplay ();
	}
	
	public void Rewind() {
		this.Set_Pos.x = 0;
		
		this.Move_Object.transform.DOLocalMove (Set_Pos, 0.5f).SetEase(Ease.OutCubic);
		
		this.Look_Now_Page_Int = 1;
		
		ChangePageDisplay ();
	}
	
	private void ChangePageDisplay () {
		this.Slide_Page_MAX_Int = (int)Mathf.Ceil ((float)List_From_Create_Object.GetComponent<List_Instance> ().Create_Number / (float)this.One_Window_MAX_Object);
		if (Slide_Page_MAX_Int == 0) {
			Look_Now_Page_Int = 0;
		}
		if (this.Slide_Page_MAX_Int <= 1) {
			for (int i=0; i<2; i++) {
				Slide_Cursor [i].SetActive (false);
			}
		} else {
			if (Look_Now_Page_Int == 1) {
				Slide_Cursor [0].SetActive (true);
				Slide_Cursor [1].SetActive (false);
			} else if (Look_Now_Page_Int == Slide_Page_MAX_Int) {
				Slide_Cursor [0].SetActive (false);
				Slide_Cursor [1].SetActive (true);
			} else {
				for (int i=0; i<2; i++) {
					Slide_Cursor [i].SetActive (true);
				}
			}
		}
		
		this.MaxPage_Text.text = this.Slide_Page_MAX_Int.ToString();
		this.NowPage_Text.text = this.Look_Now_Page_Int.ToString();
	}
}
