using UnityEngine;
using System.Collections;

public class UITools : MonoBehaviour {
	static public GameObject AddChild (GameObject parent, GameObject prefab) {
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		if (go != null && parent != null) {
			Transform tf = go.transform;
			tf.SetParent(parent.transform);
			tf.localPosition = Vector3.zero;
			tf.localRotation = Quaternion.identity;
			tf.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}
}
