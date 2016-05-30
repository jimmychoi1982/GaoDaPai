using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace GameView {
	public class CountersManager : MonoBehaviour {
		Card card { get { return Card.Instance; }}
		GameView gameView { get { return GameView.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

		Master master { get { return Master.Instance; }}

		//
		[Header("Icon")]
		public Image currentIcon;
		public Sprite placeholder;
		public Effect_Play_Limit effectSet;
		public Effect_Play_Limit effectActivate;

		//
		[Header("Quantity")]
		public StatValue statQuantity;

		//
		[Header("Cover")]
		public Animator coverAnimator;


		//
		private TomeArray data;
		private bool doorsOpen = false;


		//
		void Start() {
			statQuantity.GetComponent<CanvasGroup>().alpha = 0;
		}

		// Reset icons using provided data
		public void SetData(TomeArray data, bool animate = true) {
			statQuantity.SetValue(0, false);
			statQuantity.GetComponent<CanvasGroup>().alpha = 1;
			currentIcon.color = Color.clear;

			this.data = data;
			if (data == null || data.Count == 0) {
				new Task(CloseCovers());
				return;
			}
			
			foreach (TomeObject counter in data) {
				AddCounter(counter, animate);
			}
		}

		public void AddCounter(TomeObject counter, bool animate = true) {
			Task openDoors = new Task(OpenDoors(), false);
			openDoors.Finished += (manual) => {
				// Update quantity, we don't wait for this
				statQuantity.SetValue(data.IndexOf(counter) + 1);

				// Update counter icon
				currentIcon.color = Color.white;
				if (counter["cardId"] == null) {
					currentIcon.sprite = placeholder;
				}
				else {
					string cardId = (string)counter["cardId"];
					loadAssetBundle.SetCardImage(cardId, (int)LoadAssetBundle.DisplayType.Icon, currentIcon.gameObject);
				}
			};

			openDoors.Start();
		}

		private IEnumerator OpenDoors() {
			// Open doors if we need to
			if (!doorsOpen) {
				new Task(OpenCovers());
				yield return new WaitForSeconds(1);
			}

			// Play highlight effects
			effectSet.Play();
			yield return new WaitForSeconds(0.2f);
		}

		//
		public IEnumerator TriggerCounter(string cardId, bool wasEffective) {
			// Update quantity, we don't wait for this
			statQuantity.SetValue(data.Count);

			// Play highlight effects
			effectActivate.Play();
			yield return new WaitForSeconds(0.2f);

			// Show counter tips window
			gameView.cardTipsManager.Show(card.GetCard(cardId));

			// Show error message if counter was not effective
			if (wasEffective == false) {
				if (master.staticData["errorCodes"] != null && master.staticData ["errorCodes"] ["G0006"] != null) {
					gameView.ShowError (master.staticData ["errorCodes"] ["G0006"] ["annotation"].ToString ());
				} else {
					gameView.ShowError ("Counter was not activated");
				}
			}

			// Update counter icon if there is one
			if (data.Count > 0) {
				TomeObject topCounter = data[data.Count - 1] as TomeObject;
				if (topCounter["cardId"] == null) {
					currentIcon.sprite = placeholder;
				} else {
					string topCardId = (string)topCounter["cardId"];
					loadAssetBundle.SetCardImage(topCardId, (int)LoadAssetBundle.DisplayType.Icon, currentIcon.gameObject);
				}
			} else {
				currentIcon.color = Color.clear;
			}

			// Close the doors if there are no more counters
			if (data.Count == 0 && doorsOpen) {
				new Task(CloseCovers());
			}
		}
		
		//
		public IEnumerator OpenCovers() {
			doorsOpen = true;
			new Task(AnimationCallbackManager.StartTrigger(coverAnimator, "OpenDoors"));
			yield break;
		}

		//
		public IEnumerator CloseCovers() {
			doorsOpen = false;
			new Task(AnimationCallbackManager.StartTrigger(coverAnimator, "CloseDoors"));
			yield break;
		}
	}
}