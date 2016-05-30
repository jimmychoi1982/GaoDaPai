using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace GameView {
	public class GameEndController : MonoBehaviour {
		//
		static GameView gameView { get { return GameView.Instance; }}

		//
		[Header("Overlay")]
		public Image overlay;

		//
		[Header("Win")]
		public GameObject winContainer;
		public Image winUnder, winOver;
		public ParticleSystem winParticle;
		public AudioSource seWin;

		//
		[Header("Lose")]
		public GameObject loseContainer;
		public Image loseText;
		public AudioSource seLose;


		//
		public void Open(string winnerId, Action cb) {
			winContainer.SetActive(false);
			loseContainer.SetActive(false);

			Task endAnim = (winnerId == gameView.meUserId) ? new Task(DoWin(), false) : new Task(DoLose(), false);
			endAnim.Finished += (manual) => { cb(); };
			endAnim.Start();
		}

		//
		public IEnumerator DoWin() {
			// Reset 
			overlay.DOFade(0, 0f);
			winOver.DOFade(0, 0f);
			winContainer.transform.localScale = new Vector3(6, 6, 6);
			yield return new WaitForSeconds(0.1f);

			overlay.gameObject.SetActive(true);
			winContainer.SetActive(true);
			overlay.DOFade(0.5f, 0.5f);
			seWin.Play();
			winContainer.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
			winOver.DOFade(1, 0.2f);
			yield return new WaitForSeconds(0.2f);
			
			//Particleスタート
			winParticle.Play();
			
			//文字の裏の光画像が明滅する
			winUnder.DOFade(1, 0.4f);
			yield return new WaitForSeconds(0.4f);
			winUnder.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
			winUnder.DOFade(1, 0.4f);
			yield return new WaitForSeconds(0.4f);
			winUnder.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
			
			//Particleストップ
			winParticle.Stop();
			
			//fade out
			winOver.DOFade(0, 0.5f);
			yield return new WaitForSeconds(2f);
		}

		//
		public IEnumerator DoLose() {
			// Reset 
			overlay.DOFade(0, 0f);
			loseText.color = new Color(1, 1, 1, 1);
			loseContainer.transform.localPosition = new Vector3(0, 700, 0);
			yield return new WaitForSeconds(0.1f);

			overlay.gameObject.SetActive(true);
			loseContainer.SetActive(true);
			loseText.DOFade(1f, 0.5f);
			seLose.Play ();
			loseContainer.transform.DOLocalMove(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.OutBounce);
			yield return new WaitForSeconds(2.5f);
			
			loseText.DOFade(0, 0.3f);
			yield return new WaitForSeconds(2.5f);
		}
	}
}