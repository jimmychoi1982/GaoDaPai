using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace GameView {
	public class ChoiceTargetClick : MonoBehaviour,
		IPointerClickHandler
	{
		public delegate void OnClick();
		public OnClick onClick;

		public void OnPointerClick(PointerEventData eventData) {
			if (onClick != null) {
				onClick.Invoke();
			}
		}
	}
}