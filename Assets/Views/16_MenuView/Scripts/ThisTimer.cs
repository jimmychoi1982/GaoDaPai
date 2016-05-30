using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ThisTimer : MonoBehaviour {
	public Text thisTimeText;
	public bool reftOrLight;

	void Start () {
		this.thisTimeText.text = this.gameObject.name;

		if (this.reftOrLight == true) {
			for (int i=0; i<10; i++) {
				if (int.Parse (this.gameObject.name) == i) {
					this.thisTimeText.text = "0" + this.gameObject.name;
				}
			}
		}
	}
}
