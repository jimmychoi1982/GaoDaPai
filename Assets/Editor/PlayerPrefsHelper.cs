using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class PlayerPrefsHelper {

	[MenuItem ("File/PlayerPrefs/DeleteAll")]
	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}
}
