using System.Collections.Generic;

using UnityEngine;


namespace GameView {
	public class CostCountManager : MonoBehaviour {
		//
		LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; }}

		//
		public GameObject pearlsParent;
		public GameObject pearlsPrefab;

		//
		public GameObject costOver;

		//
		private Dictionary<string, List<GameObject>> pearls;


		//
		public void SetData(TomeObject cardData) {
			// Reset pearls
			pearls = new Dictionary<string, List<GameObject>>();
			for (int i = 0; i < pearlsParent.transform.childCount; i += 1) {
				GameObject.Destroy(pearlsParent.transform.GetChild(i).gameObject);
			}

			// Check if this is a flush only
			if (cardData == null) {
				return;
			}

			// Add pearls for each cost
			TomeObject cardCost = cardData["currentCost"] as TomeObject;
			if (cardCost["play"] != null) {
				cardCost = cardCost["play"] as TomeObject;
			}
			for (int i = 0; i < (int)cardCost["neutral"]; i += 1) {
				AddPearl("neutral");
			}

			foreach (var property in cardCost) {
				string color = property.Key;
				if (color == "neutral") {
					continue;
				}

				int cost = (int)property.Value;
				for (int i = 0; i < cost; i += 1) {
					AddPearl(color);
				}
			}
		}

		//
		public void AddPearl(string color) {
			GameObject newPearl = GameObject.Instantiate(pearlsPrefab);
			newPearl.transform.SetParent(pearlsParent.transform);
			newPearl.transform.localScale = pearlsParent.transform.localScale;

			loadAssetBundle.SetCostIconImage(color, newPearl);

			if (!pearls.ContainsKey(color)) {
				pearls.Add(color, new List<GameObject>());
			}
			pearls[color].Add(newPearl);
		}

		//
		public void UpdateSelected(Dictionary<string, int> selectedCost) {
			// Reset all pearls
			foreach (var pair in pearls) {
				var pearlList = pair.Value;
				for (int i = 0; i < pearlList.Count; i++) {
					pearlList[i].SetActive(true);
				}
			};
			// Copy selectedCost
			Dictionary<string, int> selectedCostTemp = new Dictionary<string, int>();
			foreach (var pair in selectedCost) {
				selectedCostTemp.Add(pair.Key, pair.Value);
			}
			// Update in priority the colored pearls
			List<GameObject> pearlListNeutral;
			pearlListNeutral = null;
			foreach (var pair in pearls) {
				string color = pair.Key;
				if (color == "neutral") {
					pearlListNeutral = pair.Value;
				} else {
					if (selectedCostTemp.ContainsKey(color)) {
						var pearlList = pair.Value;
						for (int i = 0; i < pearlList.Count; i++) {
							if (selectedCostTemp[color] > 0) {
								pearlList[i].SetActive(false);
								selectedCostTemp[color]--;
							}
						}
					}
				}
			}
			// Update then the neutral pearls
			if (pearlListNeutral != null) {
				for (int i = 0; i < pearlListNeutral.Count; i++) {
					foreach (var pair in selectedCostTemp) {
						int selectedCostLeft = pair.Value;
						if (selectedCostLeft > 0) {
							pearlListNeutral[i].SetActive(false);
							selectedCostTemp[pair.Key]--;
							break;
						}
					}
				}
			}
			// Check if it is cost over
			pearlsParent.SetActive(true);
			costOver.SetActive(false);
			foreach (var property in selectedCostTemp) {
				if (property.Value > 0) {
					pearlsParent.SetActive(false);
					costOver.SetActive(true);
					break;
				}
			}
		}
	}
}