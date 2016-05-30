using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace GameView {
	public class GameEndController_Tutorial : MonoBehaviour {
		//
		static GameView gameView { get { return GameView.Instance; }}
		
		//
		[Header("Overlay")]
		public Image overlay;
		
		//
		[Header("Clear")]
		public GameObject clearContainer;
		public Image clearUnder, clearOver;
		public ParticleSystem clearParticle;
		public AudioSource seWin;
		
		
		//
		public void Open(string winnerId, Action cb) {
			clearContainer.SetActive(false);
			
			Task endAnim = new Task (DoClear (), false);
			endAnim.Finished += (manual) => { cb(); };
			endAnim.Start();
		}
		
		//
		public IEnumerator DoClear() {
			// Reset 
			overlay.DOFade(0, 0f);
			clearOver.DOFade(0, 0f);
			clearContainer.transform.localScale = new Vector3(6, 6, 6);
			yield return new WaitForSeconds(0.1f);
			
			overlay.gameObject.SetActive(true);
			clearContainer.SetActive(true);
			overlay.DOFade(0.5f, 0.5f);
			seWin.Play();
			clearContainer.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
			clearOver.DOFade(1, 0.2f);
			yield return new WaitForSeconds(0.2f);
			
			//Particleスタート
			clearParticle.Play();
			
			//文字の裏の光画像が明滅する
			clearUnder.DOFade(1, 0.4f);
			yield return new WaitForSeconds(0.4f);
			clearUnder.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
			clearUnder.DOFade(1, 0.4f);
			yield return new WaitForSeconds(0.4f);
			clearUnder.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
			
			//Particleストップ
			clearParticle.Stop();
			
			//fade out
			clearOver.DOFade(0, 0.5f);
			yield return new WaitForSeconds(2f);
		}
	}
}