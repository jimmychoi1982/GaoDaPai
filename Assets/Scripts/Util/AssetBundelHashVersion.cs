using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AssetBundleHashVersion {
	public string assetBundleName;
	public string assetBundleHash;
	public int version;
	
	public AssetBundleHashVersion(string _assetBundleName, string _assetBundleHash, int _version) {
		this.assetBundleName = _assetBundleName;
		this.assetBundleHash = _assetBundleHash;
		this.version = _version;
	}

	public AssetBundleHashVersion(JToken token) {
		var obj = token as JObject;
		this.assetBundleName = obj["assetBundleName"].ToString ();
		this.assetBundleHash = obj["assetBundleHash"].ToString ();
		this.version = int.Parse (obj["version"].ToString ());
	}
}
