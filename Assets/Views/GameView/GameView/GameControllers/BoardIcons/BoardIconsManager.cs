using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


namespace GameView {
	public class BoardIconsManager : MonoBehaviour {
		//
		[Header("Icon")]
		public GameObject iconPrefab;
		public GameObject iconLargePrefab;

		//
		// Map of icon objects by their instanceId
		public Dictionary<string, BoardIcon> boardIcons = new Dictionary<string, BoardIcon>();


		//
		public void SetData(TomeArray boardData, bool animate = true) {
			// Reset board icons
			// Note: we use DestroyImmediate to prevent the race conditions that
			// occur as the object will still exist till the end of the frame.
			boardIcons = new Dictionary<string, BoardIcon>();
			for (int i = transform.childCount - 1; i >= 0; i -= 1) {
				GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
			}

			// Check if we should populate icons using given costData data
			if (boardData == null) {
				return;
			}

			for (int i = 0; i < boardData.Count; i += 1) {
				AddIcon(boardData[i] as TomeObject, i, animate);
			}
		}

		//
		public BoardIcon AddIcon(TomeObject iconData, int boardPosition, bool autoAnimate = false) {
			string instanceId = (string)iconData["instanceId"];
			if (boardIcons.ContainsKey(instanceId)) {
				return null;
			}

			//
			string size = (string)iconData["size"];
			GameObject prefab = (size == "L") ? iconLargePrefab : iconPrefab;

			//
			GameObject newIcon = GameObject.Instantiate(prefab);
			newIcon.transform.SetParent(transform);
			newIcon.transform.localScale = prefab.transform.localScale;
			newIcon.transform.SetSiblingIndex(boardPosition);
			
			BoardIcon newBoardIcon = newIcon.GetComponent<BoardIcon>();
			newBoardIcon.SetData(iconData, autoAnimate);
			
			boardIcons.Add(instanceId, newBoardIcon);
			return newBoardIcon;
		}
		
		//
		public GameObject CreatePlaceholder(int boardPosition, float width, float height) {
			GameObject placeholder = new GameObject();
			placeholder.transform.SetParent(transform);
			placeholder.transform.SetSiblingIndex(boardPosition);
			
			LayoutElement layoutElement = placeholder.AddComponent<LayoutElement>();
			layoutElement.preferredWidth = width;
			layoutElement.preferredHeight = height;
			
			return placeholder;
		}

		//
		public void DestroyIcon(string instanceId, string animation = "CardEffectUnitDeath", Action cb = null) {
			cb = (cb != null) ? cb : () => {};

			BoardIcon boardIcon = boardIcons[instanceId];
			if (animation == null) {
				GameObject.Destroy(boardIcon.gameObject);
				boardIcons.Remove(instanceId);
				cb();
				return;
			}

			boardIcon.QueueAction(boardIcon.Destroy(animation), () => {
				GameObject.Destroy(boardIcon.gameObject);
				boardIcons.Remove(instanceId);
				cb();
			});
		}
	}
}