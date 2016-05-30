using UnityEngine;
using System.Collections;

public class TapEffectManager : MonoBehaviour {

	// 再生したいEffectのプレハブを指定する
	public GameObject Effect01 = null;

	public GameObject canvas;

	private bool disabled = false;

	void Start () {
		canvas = GameObject.Find ("/Main Canvas");
	}

	void OnLevelWasLoaded (int level) {
		if (Application.loadedLevelName == "GameView") {
			disabled = true;
		} else {
			disabled = false;
			canvas = GameObject.Find ("/Main Canvas");
		}
	}

	void Update () {
		if (!disabled && Input.GetMouseButtonDown (0)) {
			// 指定したエフェクトを作成
			GameObject tapeff = (GameObject)Instantiate (Effect01, transform.position, Quaternion.identity);
			tapeff.transform.SetParent(canvas.transform, true);
			tapeff.transform.localScale = new Vector3(1,1,1);

			Vector3 worldPos = Vector3.zero;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), Camera.main, out worldPos);
			tapeff.transform.position = worldPos;

			Destroy (tapeff, 0.7f);
		}
	}
}

