using System.Collections;
using System;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class Mothership : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		User user { get { return User.Instance; }}
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		//
		[Header("Mothership Icon")]
		public CanvasGroup iconGroup;
		public Image imgIcon;
		public Image imgHighlight;
		public Image imgBackground;

		//
		[Header("Defence")]
		public StatValue statDEF;

		//
		[Header("Effects")]
		public Effect_Play_Limit effectExplode;
		public GameObject effectDamage;

		//
		public TomeObject data;


		//
		void Start() {
			iconGroup.alpha = 0;
			imgHighlight.color = Color.clear;
			imgBackground.sprite = Resources.Load<Sprite>("Common/blank");
		}

		//
		public void SetData(TomeObject data) {
			this.data = data;
			string userId = "";

			// Animate mothership appearance
			if (data ["userId"] != null) {
				userId = (string)data ["userId"];
			} else {
				userId = (string)data ["cpuId"];
			}

			string motheshipId = (string)game.tCurrentGame["playerData"][userId]["mothershipId"];
			// load background image even if couldn't get that
			if (game.tCurrentGame["playerData"][userId]["color"] != null) {
				imgBackground.sprite = Resources.Load<Sprite>("ShipBGs/ship_bg_" + (string)game.tCurrentGame["playerData"][userId]["color"]);
			} else if (user.tUser["color"] != null) {
				imgBackground.sprite = Resources.Load<Sprite>("ShipBGs/ship_bg_" + (string)user.tUser["color"]);
			} else {
				imgBackground.sprite = Resources.Load<Sprite>("ShipBGs/ship_bg_blue");
			}

			loadAssetBundle.SetMothershipIngameImage(motheshipId, false, imgIcon.gameObject);
			loadAssetBundle.SetMothershipIngameImage(motheshipId, true, imgHighlight.gameObject);
			iconGroup.alpha = 1;

			// Animate mothership health appearance
			statDEF.SetValue((int)data["currentDef"]);
		}

		//
		public void UpdateFromData(Action cb) {
			statDEF.SetValue((int)data["currentDef"]);
		}
		
		/// <summary>
		/// Cost icon added to hand, play mothership animation and then animate icon
		/// </summary>
		/// <param name="icon">cost icon that was drawn</param>
		public void CostIconAdded(CostIcon icon) {
			Task lightUp = new Task(LightUp(new Color(1f, 1f, 1f, 0f)), false);
			lightUp.Finished += (manual) => {
				icon.Appear();
			};

			lightUp.Start();
		}

		//
		private IEnumerator LightUp(Color color) {
			//
			imgHighlight.DOKill();
			imgHighlight.color = color;
			yield return new WaitForSeconds (0.1f);

			//
			imgHighlight.DOFade (1, 0.3f);
			yield return new WaitForSeconds (0.3f);
			imgHighlight.DOFade (0, 0.2f);
			yield return new WaitForSeconds (0.2f);
			imgHighlight.DOFade (1, 0.3f);
			yield return new WaitForSeconds (0.3f);
			imgHighlight.DOFade (0, 0.2f);
		}

		//
		public void Attacked(Action cb = null) {
			statDEF.SetValue((int)data["currentDef"]);

			effectDamage.SetActive(true);
			imgIcon.GetComponent<RectTransform>().DOShakePosition(1f, 20, 30).OnComplete(() => {
				effectDamage.SetActive(false);

				if (cb != null) {
					cb();
				}
			});
		}

		//
		public IEnumerator Explode() {
			effectExplode.Play();
			iconGroup.DOFade(0, 5);
			yield return new WaitForSeconds(5f);
		}
	}
}