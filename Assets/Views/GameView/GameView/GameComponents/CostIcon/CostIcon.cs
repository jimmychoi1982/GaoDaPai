using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Newtonsoft.Json.Linq;


namespace GameView {
	public class CostIcon : MonoBehaviour {
		//
		Card card { get { return Card.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		//
		[Header("Icon")]
		public CanvasGroup iconGroup;
		public Image iconImage;
		public Image iconFrame;
		public Image overlay;

		[Header("Effects")]
		public Change_CardToIcon changeCardToIcon;
		public Effect_Play_Limit pilotBack;

		//
		public TomeObject iconData;


		//
		void Start() {
			iconGroup.alpha = 0;
		}

		//
		public void SetData(TomeObject iconData) {
			this.iconData = iconData;
			SetIconImage();
		}

		//
		public void SetIconImage() {
			bool isTapped = (int)iconData["tapped"] > 0;
			bool isPilot = card.IsPilotCard((string)iconData["cardId"]);

			// Set icon
			int displayType = GameHelpers.UnitGetDisplayType(isTapped, false);
			loadAssetBundle.SetCardImage((string)iconData["cardId"], displayType, iconImage.gameObject);

			// Set frame
			if (isPilot) {
				iconFrame.sprite = GameHelpers.GetPilotFrame(isTapped, false);
			} else {
				iconFrame.sprite = GameHelpers.GetCrewFrame((string)iconData["color"], isTapped);
			}
		}

		/// <summary>
		/// Public accessor to make cost icon appear
		/// </summary>
		public void Appear() {
			Task appearAnimation = new Task(AppearAnimation());
		}

		public void Return() {
			Task returnAnimation = new Task(ReturnAnimation());
		}

		/// <summary>
		/// Reset the icon group
		/// </summary>
		private void ResetIconGroup() {
			iconGroup.DOKill();
			iconGroup.DOFade(0, 0);
		}

		/// <summary>
		/// Cost icon appear animation
		/// Resets group, plays flash, fades in icon
		/// </summary>
		private IEnumerator AppearAnimation() {
			// Reset
			ResetIconGroup();
			yield return new WaitForSeconds(0.1f);

			// Do flash effect
			Task flashEffect = new Task(changeCardToIcon.Effect_Start(), false);

			flashEffect.Finished += (manual) => {
				// Show icon and callback if required
				iconGroup.DOFade(1, 0);
			};

			flashEffect.Start();
		}

		//
		private IEnumerator ReturnAnimation() {
			// Reset
			ResetIconGroup();
			yield return new WaitForSeconds(0.1f);
			
			// Do flash effect
			pilotBack.Play();

			// Fade in icon
			iconGroup.DOFade(1, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}

		//
		public IEnumerator Destroy() {
			// Reset
			ResetIconGroup();
			yield return new WaitForSeconds(0.1f);

			// Play flash particle effect
			pilotBack.Play();

			// Fade out icon
			iconGroup.DOFade(0, 0.3f);
			yield return new WaitForSeconds(0.3f);
		}
	}
}