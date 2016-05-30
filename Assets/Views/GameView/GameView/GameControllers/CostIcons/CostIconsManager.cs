using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class CostIconsManager : MonoBehaviour {
		//
		[Header("Rows")]
		public GameObject row1;
		public GameObject row2;

		//
		[Header("Icon")]
		public GameObject iconPrefab;

		// Map of icon objects by their instanceId
		public Dictionary<string, CostIcon> costIcons = new Dictionary<string, CostIcon>();


		// Reset icons using provided data
		public void SetData(TomeObject costData, bool animate = true) {
			// Reset row icons
			// Note: we use DestroyImmediate to prevent the race conditions that
			// occur as the object will still exist till the end of the frame.
			costIcons = new Dictionary<string, CostIcon>();
			for (int i = row1.transform.childCount - 1; i >= 0; i -= 1) {
				GameObject.DestroyImmediate(row1.transform.GetChild(i).gameObject);
			}
			for (int i = row2.transform.childCount - 1; i >= 0; i -= 1) {
				GameObject.DestroyImmediate(row2.transform.GetChild(i).gameObject);
			}

			// Check if we should populate icons using given costData data
			if (costData == null) {
				return;
			}

			foreach (var property in costData) {
				TomeObject costIcon = property.Value as TomeObject;
				AddIcon(costIcon, animate);
			}
		}
		
		// Add icon to list with given iconData
		public CostIcon AddIcon(TomeObject iconData, bool autoAnimate = false) {
			string instanceId = (string)iconData["instanceId"];
			if (costIcons.ContainsKey(instanceId)) {
				return null;
			}

			//
			GameObject container = (row1.transform.childCount >= 5) ? row2 : row1;

			//
			GameObject newIcon = GameObject.Instantiate(iconPrefab);
			newIcon.transform.SetParent(container.transform);
			newIcon.transform.SetAsLastSibling();
			newIcon.transform.localScale = iconPrefab.transform.localScale;

			CostIcon newCostIcon = newIcon.GetComponent<CostIcon>();
			newCostIcon.SetData(iconData);

			costIcons.Add(instanceId, newCostIcon);

			// Check if we should automatically animate
			if (autoAnimate) {
				newCostIcon.Appear();
			}

			return newCostIcon;
		}

		// Destroy an icon. Will also move all icons down to fill in space
		public void DestroyIcon(string instanceId) {
			CostIcon costIcon = costIcons[instanceId];

			Task costIconDestroy = new Task(costIcon.Destroy(), false);
			costIconDestroy.Finished += (manual) => {
				// Destroy icon
				// NOTE: We use DestroyImmediate so that any references to the gameObject
				// (e.g. transform.childCount) are taken into account properly rather
				// than from the next frame.
				GameObject.DestroyImmediate(costIcon.gameObject);
				costIcons.Remove(instanceId);

				// If we destroyed something on the first row, if
				// possible take an item from the second row.
				if (row1.transform.childCount < 5 && row2.transform.childCount > 0) {
					Transform moveIcon = row2.transform.GetChild(0);
					moveIcon.SetParent(row1.transform);
					moveIcon.SetAsLastSibling();
				}
			};
			costIconDestroy.Start();

		}

		// Update all icon states that this manager controls
		public void UpdateIcons() {
			foreach (CostIcon costIcon in costIcons.Values) {
				costIcon.SetIconImage();
			}
		}
	}
}