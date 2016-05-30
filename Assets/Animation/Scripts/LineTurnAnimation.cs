using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LineTurnAnimation : MonoBehaviour {

	public GameObject turnBox;
	public ParticleSystem particle;

	public IEnumerator AnimeStart(){
		//reset
		turnBox.GetComponent<CanvasGroup> ().DOKill();
		turnBox.GetComponent<CanvasGroup> ().alpha = 0;

		turnBox.transform.DOKill();
		turnBox.transform.localScale = new Vector3 (1, 0, 1);

		yield return new WaitForSeconds(1.1f);

		//play Animation
		turnBox.GetComponent<CanvasGroup> ().DOFade (1, 0.3f);
		turnBox.transform.DOScale (new Vector3 (1, 1, 1), 0.3f);
		yield return new WaitForSeconds(0.2f);
		particle.Play ();
		yield return new WaitForSeconds(0.1f);

	}
}
