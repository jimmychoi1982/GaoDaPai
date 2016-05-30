using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingTask {
	public IEnumerator c;
	public bool autoStart;

	public WaitingTask(IEnumerator _c, bool _autoStart) {
		c = _c;
		autoStart = _autoStart;
	}
}

public class TaskManagerMainThread : MonoSingleton<TaskManagerMainThread> {
	private static List<WaitingTask> queuedTasks = new List<WaitingTask>();

	// Create game object instance
	public static void Instantiate() {
		GameObject gameObject = new GameObject();
		gameObject.name = "TaskManagerMainThread";
		gameObject.AddComponent<TaskManagerMainThread>();
	}

	// Add task to queue to be executed on main thread.
	// This is thread safe as it lock the queue list.
	public static void Queue(IEnumerator c, bool autoStart = true) {
		lock ((object)queuedTasks) {
			queuedTasks.Add(new WaitingTask(c, autoStart));
		}
	}

	// Instantiate queued tasks and clear queue list.
	// This is thread safe as it lock the queue list.
	void Update () {
		lock ((object)queuedTasks) {
			foreach (WaitingTask task in queuedTasks) {
				new Task(task.c, task.autoStart);
			}

			queuedTasks.Clear();
		}
	}
}
