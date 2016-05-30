using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineManager : MonoBehaviour {
	private static CoroutineManager instance {
		get {
			if (__instance == null) {
				GameObject go = new GameObject("_CoroutineManager");
				DontDestroyOnLoad(go);
				__instance = go.AddComponent<CoroutineManager>();
			}
			return __instance;
		}
	}
	private static CoroutineManager __instance;

	public static Coroutine WStartCoroutine(IEnumerator _routine) {
		return instance.StartCoroutine(_routine);
	}
}