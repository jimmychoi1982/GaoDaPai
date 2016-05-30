using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class BundleVersion {

#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern string GetVersionName_();
	
	public static string Get () {
		string versionName = GetVersionName_();
		return versionName;
	}

#else
	public static string Get () {
		return Application.version;
	}
#endif
}