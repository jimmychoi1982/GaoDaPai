using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class BoardIcon : MonoBehaviour {
		//
		Mage mage { get { return Mage.Instance; }}
		Logger logger { get { return mage.logger("BoardIcon"); }}

		GameView gameView { get { return GameView.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

		//
		[Header("Icon")]
		public GameObject icon;
		public Image iconImage;
		public Image iconFrame;
		public Image iconHighlight;
		public CanvasGroup iconGroup;

		//
		[Header("Stats")]
		public StatValue statATK;
		public StatValue statDEF;
		public Image imgATK;
		public Image imgDEF;
		public Image imgPilot;
		public Image imgSize;

		//
		[Header("Voluntary Tapping")]
		public GameObject indicatorParent;

		//
		[Header("Animations")]
		public Animator animator;

		//
		public TomeObject iconData;
		private bool tauntSet = false;
		private bool isMobilised = true;

		//
		private bool actionsRunning = false;
		private List<IEnumerator> actionQueue = new List<IEnumerator>();
		private List<Action> actionCbs = new List<Action>();


		//
		public void SetData(TomeObject iconData, bool autoAnimate = true) {
			this.iconData = iconData;


			// Animate if we're supposed to
			if (!autoAnimate) {
				SetIconImg();
				UpdateFromData(false);
			} else {
				SetIconImg(true);
				UpdateFromData(false);
				QueueAction(Appear());
				bool hasTaunt = GameHelpers.UnitHasTaunt(iconData);
				if (hasTaunt) {
					QueueAction(GiveTaunt());
				}
			}
		}

		//
		public void UpdateFromData(bool animate = true) {
			//
			statATK.SetValue(GameHelpers.UnitGetAtk(iconData, true), GameHelpers.UnitGetAtk(iconData), animate);
			statDEF.SetValue(GameHelpers.UnitGetDef(iconData, true), GameHelpers.UnitGetDef(iconData), animate);
			
			//
			bool hasShelling = GameHelpers.UnitHasBombardment(iconData);
			if (indicatorParent != null) {
				if (hasShelling && isMobilised && GameHelpers.IsUsersTurn((string)iconData["userId"])) {
					indicatorParent.SetActive(true);
				} else {
					indicatorParent.SetActive(false);
				}
			}

			if (animate) {
				//
				bool iconTapped = GameHelpers.IconIsTapped(iconData);
				if (iconTapped) {
					QueueAction(Immobilise());
				} else {
					QueueAction(Mobilise());
				}

				//
				bool hasTaunt = GameHelpers.UnitHasTaunt(iconData);
				if (hasTaunt) {
					QueueAction(GiveTaunt());
				} else {
					QueueAction(RemoveTaunt());
				}
			}
		}

		private bool IsTapped() {
			return iconData["tapped"] != null && (int)iconData["tapped"] > 0;
		}

		//
		public void SetIconImg(bool ignoreTaunt = false) {

			// Reverse the enemy card (rotation of 180°)
			if (gameView.enemyUserId == (string)iconData["userId"]) {
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
			}

			//
			string cardId = (string)iconData["cardId"];
			string size = (string)iconData["size"];
			string color = (string)iconData["color"];
			bool isTapped = IsTapped();
			bool hasTaunt = GameHelpers.UnitHasTaunt(iconData) && !ignoreTaunt;
			
			//
			int displayType = GameHelpers.UnitGetDisplayType(IsTapped(), hasTaunt);
			loadAssetBundle.SetCardImage(cardId, displayType, iconImage.gameObject);
			
			// Set Sprites
			imgATK.sprite = GameHelpers.GetUnitNumberFrame(color, isTapped);
			imgDEF.sprite = GameHelpers.GetUnitNumberFrame(color, isTapped);
			imgSize.sprite = GameHelpers.GetUnitSizeIcon(color, size, isTapped);
			iconFrame.sprite = GameHelpers.GetUnitFrame(color, size, hasTaunt, isTapped);
			iconHighlight.sprite = GameHelpers.GetUnitHighlight(size, hasTaunt);
			
			//
			imgPilot.gameObject.SetActive(iconData["pilot"] != null);
		}

		//
		public void QueueAction(IEnumerator action, Action cb = null) {
			actionQueue.Add(action);
			actionCbs.Add(cb);
			if (!actionsRunning) {
				actionsRunning = true;
				new Task(ProcessQueue());
			}
		}

		//
		private IEnumerator ProcessQueue() {
			while (actionQueue.Count > 0) {
				// Check if this object has been destroyed mid queue
				if (gameObject == null) {
					yield break;
				}

				// Wait for the gameobject to be active
				while (!gameObject.activeInHierarchy) {
					yield return null;
				}

				IEnumerator action = actionQueue[0];
				Action cb = actionCbs[0];

				yield return StartCoroutine(action);
				if (cb != null) {
					cb();
				}

				actionQueue.RemoveAt(0);
				actionCbs.RemoveAt(0);
			}

			actionsRunning = false;
		}

		//
		public IEnumerator Appear() {
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectChangeUnitCardToIcon"));
		}

		//
		public IEnumerator GiveTaunt() {
			if (tauntSet == true) {
				yield break;
			}

			SetIconImg();
			tauntSet = true;
			string animationName = ((string)iconData["size"] == "L") ? "CardEffectTauntLSize" : "CardEffectTaunt";
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, animationName));
		}
		
		//
		public IEnumerator RemoveTaunt() {
			if (tauntSet == false) {
				yield break;
			}

			SetIconImg();
			tauntSet = false;
			string animationName = ((string)iconData["size"] == "L") ? "CardEffectTauntBrokeLsize" : "CardEffectTauntBroke";
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, animationName));
		}
		
		//
		public IEnumerator Destroy(string animation) {
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, animation));
		}

		//
		public IEnumerator Attack(string targetId) {
			RectTransform thisRect = icon.GetComponent<RectTransform>();
			RectTransform targetRect = GameHelpers.GetTargetIconRect(targetId);
			float direction = Mathf.Sign(thisRect.position.y - targetRect.position.y);

			// Pull back a bit
			thisRect.DOLocalMoveY(40 * direction, 0.3f);
			yield return new WaitForSeconds (0.3f);

			// Move to target
			Vector2 dirVector = targetRect.position - thisRect.position;
			Vector2 endPos = (Vector2)thisRect.position + (dirVector.normalized * (dirVector.magnitude - 2));
			thisRect.DOMove(endPos, 0.4f, false).SetEase(Ease.InExpo);
			yield return new WaitForSeconds (0.4f);

			// Update our def value, we don't wait for this
			statDEF.SetValue(GameHelpers.UnitGetDef(iconData, true), GameHelpers.UnitGetDef(iconData));

			// Trigger target attacked animation, we don't wait for this
			BoardIcon targetIcon = GameHelpers.FindBoardIcon(targetId);
			if (targetIcon != null) {
				targetIcon.QueueAction(targetIcon.Attacked());
			}
			Mothership targetMothership = GameHelpers.FindMothership(targetId);
			if (targetMothership != null) {
				targetMothership.Attacked();
			}

			// Return back to origin
			thisRect.DOLocalMove(Vector2.zero, 0.3f).SetEase(Ease.OutExpo);
			yield return new WaitForSeconds (0.3f);
		}

		//
		public IEnumerator Attacked() {
			UpdateFromData();
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectNormalAttack"));
		}

		//
		public IEnumerator EffectDamage() {
			UpdateFromData();
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectDamage"));
		}

		//
		public IEnumerator PositiveStatChange() {
			yield return AnimationCallbackManager.StartTrigger(animator, "CardEffectKeywordUp");
			UpdateFromData();
		}

		//
		public IEnumerator NegativeStatChange() {
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectKeywordDown"));
			UpdateFromData();
		}

		public void SetTap(bool tapped) {
			if (tapped) {
				QueueAction(Immobilise());
			}
			else {
				QueueAction(Mobilise());
			}
		}

		//
		private IEnumerator Mobilise() {
			if (isMobilised) {
				yield break;
			}

			SetIconImg();
			isMobilised = true;
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectNotAction"));
		}

		//
		private IEnumerator Immobilise() {
			if (!isMobilised) {
				yield break;
			}

			SetIconImg();
			isMobilised = false;
			yield return StartCoroutine(AnimationCallbackManager.StartTrigger(animator, "CardEffectActed"));
		}
	}
}