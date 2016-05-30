using UnityEngine;
using System.Collections;

public class Resident : MonoBehaviour {
	void Awake () {
		if (GameObject.FindGameObjectsWithTag ("Resident").Length > 1) {
			Destroy (this.gameObject);
		}
		DontDestroyOnLoad (this.gameObject);
	}
}
