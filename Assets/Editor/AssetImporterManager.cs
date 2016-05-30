using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AssetImporterManager : ScriptableObject
{

	[MenuItem ("Assets/AssetBundles/Manage AssetBundles", false, 2)]
	public static void SetTextureImporters()
	{	
		string[] paths = Directory.GetFiles("Assets/AssetBundleSources", "*.png");
		int i = 0;
		foreach (string path in paths)
		{
			var pathSplit = path.Split('/');
			if (pathSplit[pathSplit.Length-1].Substring (pathSplit[pathSplit.Length-1].LastIndexOf('.') + 1) == "wav") continue;
			var fileName = pathSplit[pathSplit.Length-1].Substring (0, pathSplit[pathSplit.Length-1].LastIndexOf('.'));
			var assetBundleName = GetAssetBundleName(fileName);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.mipmapEnabled = false;
			textureImporter.maxTextureSize = 2048;
			if (assetBundleName == "class" || assetBundleName == "gasha" || assetBundleName == "emblem" || assetBundleName == "costicon" || assetBundleName == "activateicon" || assetBundleName == "sizeicon" || assetBundleName == "forceicon" || assetBundleName == "forceplate" || assetBundleName == "bundleimage") {
				textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
			} else {
				textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			}
			textureImporter.assetBundleName = assetBundleName;
			AssetDatabase.ImportAsset(path);

			EditorUtility.DisplayProgressBar("Change AssetBundles Settings...", fileName, (float)i/(float)paths.Length);
			i++;
		}

		EditorUtility.ClearProgressBar ();
	}

	[MenuItem ("Assets/AssetBundles/Build AssetBundles", false, 3)]
	public static void BuildAllAssetBundles ()
	{
		BuildTarget[] buildTargets = new BuildTarget[]{BuildTarget.Android, BuildTarget.iOS};

		foreach (var buildTarget in buildTargets)
		{
			GetHashFromManifest(BuildAssetBundleByTarget (buildTarget), buildTarget);
		}
	}
	
	[MenuItem ("Assets/AssetBundles/Clear AssetBundle Cache", false, 5)]
	public static void ClearCache()
	{	
		Caching.CleanCache ();
	}

	private static AssetBundleManifest BuildAssetBundleByTarget(BuildTarget buildTarget)
	{
		return BuildPipeline.BuildAssetBundles ("AssetBundles/" + buildTarget.ToString(), BuildAssetBundleOptions.DeterministicAssetBundle, buildTarget);
	}
	
	private static void GetHashFromManifest(AssetBundleManifest manifest, BuildTarget buildTarget)
	{
		Dictionary<string, AssetBundleHashVersion> assetBundleHashVersions = new Dictionary<string, AssetBundleHashVersion> ();
		Dictionary<string, AssetBundleHashVersion> prevAssetBundleHashVersions = new Dictionary<string, AssetBundleHashVersion> ();

		FileStream fs = new FileStream ("AssetBundles/" + buildTarget.ToString() + "/AssetBundleHashVersion.json", FileMode.OpenOrCreate);
		StreamReader sr = new StreamReader (fs);
		string json = sr.ReadToEnd ();
		var tmpAssetBundleHashVersions = JsonConvert.DeserializeObject<Dictionary<string, JToken>> (json);
		if (tmpAssetBundleHashVersions != null) {
			foreach (var kv in tmpAssetBundleHashVersions) {
				prevAssetBundleHashVersions.Add (kv.Key, new AssetBundleHashVersion (kv.Value));
			}
		}
		sr.Close ();

		if (prevAssetBundleHashVersions != null) {
			assetBundleHashVersions = new Dictionary<string, AssetBundleHashVersion> (prevAssetBundleHashVersions);
		}
		var assetBundles = manifest.GetAllAssetBundles ();
		foreach (var bundle in assetBundles)
		{
			var hash = manifest.GetAssetBundleHash(bundle).ToString();
			int version = 1;
			if (prevAssetBundleHashVersions != null && prevAssetBundleHashVersions.ContainsKey(bundle)) {
				version = prevAssetBundleHashVersions[bundle].version;
				if (!hash.Equals(prevAssetBundleHashVersions[bundle].assetBundleHash)) {
					version = prevAssetBundleHashVersions[bundle].version + 1;
				}
			}
			var tmpAssetBundleHashVersion = new AssetBundleHashVersion(bundle, hash, version);
			if (assetBundleHashVersions != null && assetBundleHashVersions.ContainsKey(bundle)) {
				assetBundleHashVersions[bundle] = tmpAssetBundleHashVersion;
			} else {
	           	assetBundleHashVersions.Add(bundle, tmpAssetBundleHashVersion);
			}
		}

		fs = new FileStream ("AssetBundles/" + buildTarget.ToString() + "/AssetBundleHashVersion.json", FileMode.Create);
		StreamWriter sw = new StreamWriter (fs);
		sw.Write (JsonConvert.SerializeObject (assetBundleHashVersions));
		sw.Close ();
		fs.Close ();
	}

	private static string GetAssetBundleName(string fileName)
	{
		if (fileName.Substring (0, 5).ToLower () == "mship") {
			if (fileName.IndexOf ('-') >= 0 && fileName.Substring (fileName.IndexOf ('-') + 1, 6) == "ingame") {
				return "mothership-ingame";
			} else {
				return "mothership";
			}
		} else if (fileName.Length >= 5 && fileName.Substring (0, 5).ToLower () == "class") {
			return fileName.Substring (0, fileName.Length - 3);
		} else if (fileName.Length >= 5 && fileName.Substring (1, 5).ToLower () == "gasha") {
			return "gasha";
		} else if (fileName.Length >= 6 && fileName.Substring (0, 6).ToLower () == "avatar") {
			return "avatar";
		} else if (fileName.Length >= 6 && fileName.Substring (0, 6).ToLower () == "sleeve") {
			return "sleeve";
		} else if (fileName.Length >= 6 && fileName.Substring (0, 6).ToLower () == "emblem") {
			return "emblem";
		} else if (fileName.Length >= 8 && fileName.Substring (0, 8).ToLower () == "costicon") {
			return "costicon";
		} else if (fileName.Length >= 8 && fileName.Substring (0, 8).ToLower () == "sizeicon") {
			return "sizeicon";
		} else if (fileName.Length >= 9 && fileName.Substring (0, 9).ToLower () == "forceicon") {
			return "forceicon";
		} else if (fileName.Length >= 10 && fileName.Substring (0, 10).ToLower () == "forceplate") {
			return "forceplate";
		} else if (fileName.Length >= 12 && fileName.Substring (0, 12).ToLower () == "activateicon") {
			return "activateicon";
		} else if (fileName.Length >= 16 && fileName.Substring (0, 16).ToLower () == "rankedmatchframe") {
			return "rankedmatchframe";
		} else if (fileName.Length >= 11 && fileName.Substring (0, 11).ToLower () == "bundleimage") {
			return "bundleimage";
		} else {
			return fileName.Substring (0, fileName.LastIndexOf ('-'));
		}
	}
}
