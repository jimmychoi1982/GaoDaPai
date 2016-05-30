using System;
using System.Collections;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class StatValue : MonoBehaviour {
	//
	public Text txtValue;
	public string prefix;

	//
	public void SetValue(int newValue, bool animate = true, Action cb = null) {
		SetValue(null, newValue, animate, cb);
	}

	public void SetValue(int? originalValue, int newValue, bool animate = true, Action cb = null) {
		// Do nothing if the value hasn't changed
		cb = (cb != null) ? cb : () => {};
		if (txtValue.text == newValue.ToString()) {
			cb();
			return;
		}

		// Set color based on originalValue
		if (originalValue == null || newValue == originalValue) {
			txtValue.color = Color.white;
		} else if (newValue > originalValue) {
			txtValue.color = Color.blue;
		} else {
			txtValue.color = Color.red;
		}

		// Set the value
		txtValue.text = ((!String.IsNullOrEmpty(prefix)) ? prefix : "") + newValue.ToString();

		// Set the value directly if we aren't animating
		if (!animate) {
			cb();
			return;
		}

		// Perform punch animation
		txtValue.transform.DOKill();
		txtValue.transform.localScale = Vector3.one;
		txtValue.transform.DOPunchScale(new Vector3(2, 2, 1), 0.2f, 1, 1).OnComplete(() => {
			cb();
		});
	}
}
