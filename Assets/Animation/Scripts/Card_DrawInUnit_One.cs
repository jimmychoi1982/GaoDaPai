using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Card_DrawInUnit_One : MonoBehaviour {

	public GameObject Card;
	private Vector3[] path;
	private Vector3 nowPos;

	//last position parametor
	public Vector3 LastPos;//position
	public float Angle;//angle
	public float lastScale; 

	public bool cardSet;
	public bool Anime_Start;

	// Update is called once per frame
	void Update ()
	{
		if(Anime_Start == true)
			StartCoroutine(DrawInSet());
			Anime_Start = false;
	}

	public IEnumerator DrawInSet(){ 
		//first position set (reset)
		Card.SetActive(true);
		Card.GetComponent<CanvasGroup> ().alpha = 1;
		Card.transform.localPosition = new Vector3 (0f, 800f, 0f);
		Card.transform.eulerAngles = new Vector3(0, 0, 0);
		Card.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

		//card throw in
		Card.transform.DOLocalMoveY(150, 0.6f).SetEase(Ease.OutQuad);
		yield return new WaitForSeconds(0.6f);


		//card set or destruction
		if(cardSet == true){
			StartCoroutine(CardSetFunction());
		}else if(cardSet == false){
			StartCoroutine(CardDestructionFunction());
		}
		
	}


	//card set
	public IEnumerator CardSetFunction(){

		Card.transform.DOLocalMove(new Vector3(0, -800,0),0.4f).SetEase (Ease.OutSine);
		yield return new WaitForSeconds(0.4f);

		Card.transform.localPosition = LastPos + new Vector3 (0, -200, 0);
		Card.transform.localScale = new Vector3(0.3f, 0.3f, 1);
		Card.transform.eulerAngles = new Vector3(0, 0, Angle);
		yield return new WaitForSeconds(0.1f);

		Card.transform.DOLocalMove(LastPos, 0.3f);


	}

	//card Destruction
	public IEnumerator CardDestructionFunction(){
		path = new[] {
			new Vector3(0, 150, 0),
			new Vector3(-416, 263, 0),
			new Vector3(-695, 79, 0)
		};
		Card.transform.DOLocalPath(path, 0.4f, PathType.CatmullRom).SetEase(Ease.OutQuart);
		Card.transform.DOScale (0.3f, 0.3f);
		Card.GetComponent<CanvasGroup> ().DOFade (0, 0.35f);
		yield return new WaitForSeconds(0.4f);
	}
	

}
