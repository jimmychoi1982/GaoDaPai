using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MemoryWatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this.transform.parent. gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		uint monoUsed = Profiler.GetMonoUsedSize ();
		uint monoSize = Profiler.GetMonoHeapSize ();
		uint totalUsed = Profiler.GetTotalAllocatedMemory (); // == Profiler.usedHeapSize
		uint totalSize = Profiler.GetTotalReservedMemory ();
		string text = string.Format(
			"mono:{0}/{1} kb({2:f1}%)\n" +
			"total:{3}/{4} kb({5:f1}%)\n", 
			monoUsed/1024, monoSize/1024, 100.0*monoUsed/monoSize, 
			totalUsed/1024, totalSize/1024, 100.0*totalUsed/totalSize);
		transform.Find ("Text").GetComponent<Text>().text = text;
	}
}
