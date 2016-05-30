using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadAssetBundle : Singleton<LoadAssetBundle> {
	private string assetBundleURI;

	private string bundleURL;
	private string assetName;
	private string hash;

	private string platformName = GameSettings.ClientOS;

	private bool isCaching = false;
	private bool isStacking = false;

	private Dictionary<string, string> cardsIdToCode = new Dictionary<string, string> ();
	private Dictionary<string, int> assetBundleVersions;

	private Mage mage { get { return Mage.Instance; }}
	private Deck deck { get { return Deck.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("LoadAssetBundle"); } }
	private CardMasterManager cardMasterManager = new CardMasterManager();

	private List<string> loadWaitObjects = new List<string>();
	private Dictionary<string, Sprite> loadedImage = new Dictionary<string, Sprite> ();

	public enum DisplayType {
		Card,
		Deck,
		Icon,
		Iconb,
		Icong,
		Icongb,
	}

	public enum ForcePlateType {
		LeftTop,
		RightBottom,
		Name,
	}
	
	public int assetBundleTotalCount;
	public int assetBundleDownloadCount;

	private bool isWaitLoading = false;
	public bool IsWaitLoading { get { return isWaitLoading; } }
	private Action LoadWaitCallBack = default(Action);
	private string loadingConditionName = "loadAssetBundle";

	private Steward steward;

	private string FindCardCode (string cardId) {
		if (cardsIdToCode.Count == 0) {
			cardsIdToCode = cardMasterManager.GetCardIdCodePairs();
		}
		return cardsIdToCode.ContainsKey(cardId) ? cardsIdToCode[cardId] : null;
	}

	private string FindDisplayTypeString (int displayType) {
		return Enum.GetName (typeof(DisplayType), displayType).ToLower();
	}
	
	private string FindForcePlateTypeString (int forcePlateType) {
		return Enum.GetName (typeof(ForcePlateType), forcePlateType).ToLower();
	}
	
	private void GetAssetBundleVersions () {
		assetBundleVersions = new Dictionary<string, int> ();
		foreach (JProperty prop in master.staticData ["assetBundleVersions"]) {
			assetBundleVersions.Add (prop.Name, int.Parse ((prop.Value as JObject) ["version"].ToString ()));
		}
	}

	private IEnumerator DownloadAssetBundle (string bundleURL, int version, bool downloadOnly, Action<UnityEngine.Object[]> cb) {
		isCaching = true;
		
		while (!Caching.ready) {
			yield return null;
		}

		UnityEngine.Object[] asset = null;
		using (WWW www = WWW.LoadFromCacheOrDownload (bundleURL, version)) {
			yield return www;
			if (www.error != null) {
//				throw new Exception("WWW download had an error:" + www.error);
				Debug.Log ("WWW download had an error:" + www.error);
				isCaching = false;
				www.Dispose();
				yield break;
			}

			while (!www.isDone) {
				yield return null;
			}

			AssetBundle bundle = www.assetBundle;
			if (!downloadOnly) {
				asset = bundle.LoadAllAssets();
			}
			bundle.Unload(false);
			www.Dispose();

			isCaching = false;
		}
		
		cb (asset);
		yield break;
	}

	private void StackLoadObject (string objectName) {
		isStacking = true;
		loadWaitObjects.Add (objectName);
	}

	private void ClearLoadObject (string objectName) {
		string stackedObjectName = loadWaitObjects.FirstOrDefault<string> (n => n == objectName);
		if (!String.IsNullOrEmpty (stackedObjectName)) {
			loadWaitObjects.Remove (stackedObjectName);
		}

		if (isStacking && loadWaitObjects.Count == 0) {
			isStacking = false;
			if (isWaitLoading) {
				steward.ClearLoadingScreenCondition (loadingConditionName);
				isWaitLoading = false;
			}
			if (LoadWaitCallBack != default(Action)) {
				LoadWaitCallBack ();
				LoadWaitCallBack = default(Action);
			}
		}
	}
	
	private void SetImage(string assetName, string fileName, GameObject gameObj) {
		var loadKey = assetName + "." + fileName;
		StackLoadObject (loadKey);
		if (loadedImage.ContainsKey (loadKey)) {
			gameObj.GetComponent<Image> ().sprite = loadedImage [loadKey];
			ClearLoadObject (loadKey);
			return;
		}
		CoroutineManager.WStartCoroutine (GetObject (assetName, fileName, (UnityEngine.Object obj) => {
			try {
				if (!loadedImage.ContainsKey (loadKey)) {
					loadedImage.Add (loadKey, obj as Sprite);
				}
				if (loadedImage.Count () > 100) {
					while (loadedImage.Count () > 100) {
						loadedImage.Remove (loadedImage.First ().Key);
					}
				}
				gameObj.GetComponent<Image>().sprite = obj as Sprite;
			} catch (Exception ex) {
				Debug.LogError(ex.Message);
			}
			ClearLoadObject (loadKey);

		}));
	}
	
	private Sprite GetSprite(string assetName, string fileName) {
		Sprite spriteObj = null;
		CoroutineManager.WStartCoroutine (GetObject (assetName, fileName, (UnityEngine.Object obj) => {
			spriteObj = obj as Sprite;
		}));
		return spriteObj;
	}
	
	public IEnumerator GetObject (string assetName, string fileName, Action<UnityEngine.Object> cb) {
		var bundleURL = assetBundleURI + "/" + platformName + "/" + assetName;
		var version = assetBundleVersions.ContainsKey (assetName) ? assetBundleVersions [assetName] : 1;

		while (isCaching) {
			yield return null;
		}
		
		UnityEngine.Object result = null;
		CoroutineManager.WStartCoroutine (DownloadAssetBundle (bundleURL, version, false, (UnityEngine.Object[] obj) => {
			isCaching = false;
			foreach (var o in obj) {
				if (o.name == fileName) {
					result = o;
				} else {
					Resources.UnloadAsset(o);
				}
			}
			
			cb (result);
		}));
		yield break;
	}
	
	public IEnumerator GetBGM (string assetName, Action<UnityEngine.Object> cb) {
		var bundleURL = assetBundleURI + "/" + platformName + "/" + assetName;
		var version = assetBundleVersions.ContainsKey (assetName) ? assetBundleVersions [assetName] : 1;

		while (isCaching) {
			yield return null;
		}
		
		UnityEngine.Object result = null;
		CoroutineManager.WStartCoroutine (DownloadAssetBundle (bundleURL, version, false, (UnityEngine.Object[] obj) => {
			isCaching = false;
			foreach (var o in obj) {
				// BGM assetBundle has only one object
				result = o;
			}

			cb (result);
		}));
	}
	
	public IEnumerator GetLive2DOperator (string assetName, Action<GameObject> cb) {
		var bundleURL = assetBundleURI + "/" + platformName + "/" + assetName;
		var version = assetBundleVersions.ContainsKey (assetName) ? assetBundleVersions [assetName] : 1;

		while (isCaching) {
			yield return null;
		}
		
		GameObject live2DElementsPrefab = null;
		CoroutineManager.WStartCoroutine (DownloadAssetBundle (bundleURL,  version, false, (UnityEngine.Object[] obj) => {
			isCaching = false;
			foreach (var o in obj) {
				// Live2DOperator assetBundle has only one object
				live2DElementsPrefab = UnityEngine.Object.Instantiate (o) as GameObject;
			}
		}));
		while (live2DElementsPrefab == null) {
			yield return new WaitForEndOfFrame ();
		}
		cb (live2DElementsPrefab);
	}
	
	public void SetAssetBundleURI(string assetBundleURI) {
		this.assetBundleURI = assetBundleURI;
	}

	public void GetVersionFromMaster(Action cb) {
		GetAssetBundleVersions ();
		cb ();
	}

	public IEnumerator DownloadAllAssetBundles(Action cb) {
		Caching.expirationDelay = 150 * 24 * 60 * 60;
		assetBundleTotalCount = assetBundleVersions.Count;
		assetBundleDownloadCount = 0;
		foreach (var kv in assetBundleVersions) {
			var bundleURL = assetBundleURI + "/" + platformName + "/" + kv.Key;
			var assetName = kv.Key;
			var version = kv.Value;

			if (Caching.IsVersionCached(bundleURL, version)) {
				assetBundleDownloadCount++;
				continue;
			}
			
			while (isCaching) {
				yield return null;
			}

			CoroutineManager.WStartCoroutine (DownloadAssetBundle (bundleURL, version, true, (UnityEngine.Object[] obj) => {
				assetBundleDownloadCount++;
			}));
		}
		cb ();
	}

	public void SetLoadWait (Action endCallBack = default(Action)) {
		if (isWaitLoading) return;
		if (steward == null) {
			steward = GameObject.Find ("/Steward").GetComponent<Steward> ();
		}
		steward.StackLoadingScreenCondition (loadingConditionName);
		loadWaitObjects = new List<string> ();
		isWaitLoading = true;
		if (endCallBack != default(Action)) {
			LoadWaitCallBack = endCallBack;
		}
	}

	public void SetCardImage(string cardId, int displayType, GameObject gameObj) {
		var cardCode = FindCardCode (cardId);
		if (!String.IsNullOrEmpty (cardCode)) {
			var assetName = cardCode.ToLower();
			var fileName = cardCode + "-" + FindDisplayTypeString(displayType);
			SetImage(assetName, fileName, gameObj);
		}
	}
	
	public void SetCardImageFromCardCode(string cardCode, int displayType, GameObject gameObj) {
		if (!String.IsNullOrEmpty (cardCode)) {
			var assetName = cardCode.ToLower();
			var fileName = cardCode + "-" + FindDisplayTypeString(displayType);
			SetImage(assetName, fileName, gameObj);
		}
	}
	
	public void SetClassIconImage(string classId, GameObject gameObj) {
		var assetName = classId.Substring(0, classId.Length - 3).ToLower();
		var fileName = classId;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetClassTextImage(int rank, GameObject gameObj) {
		var assetName = "classtext";
		var fileName = "classText" + rank.ToString("D3");
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetEmblemImage(string EmblemId, GameObject gameObj) {
		var assetName = "emblem";
		var fileName = EmblemId;
		SetImage(assetName, fileName, gameObj);
	}

	public void SetAvatarImage(string avatarId, GameObject gameObj) {
		var assetName = "avatar";
		var fileName = avatarId;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetMothershipImage(string mothershipId, GameObject gameObj) {
		var assetName = "mothership";
		var fileName = mothershipId;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetMothershipIngameImage(string mothershipId, bool isLight, GameObject gameObj) {
		var assetName = "mothership-ingame";
		var fileName = mothershipId + "-ingame";
		if (isLight) fileName = fileName + "-light";
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetSleeveImage(string sleeveId, int displayType, GameObject gameObj) {
		var assetName = "sleeve";
		var fileName = sleeveId + "-" + FindDisplayTypeString(displayType);
		SetImage(assetName, fileName, gameObj);
	}

	public void SetGashaImage(string gashaId, GameObject gameObj) {
		var assetName = "gasha";
		var fileName = gashaId;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetCostIconImage(string color, GameObject gameObj) {
		var assetName = "costicon";
		var fileName = assetName + "_" + color;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetActivateIconImage(string color, GameObject gameObj) {
		var assetName = "activateicon";
		var fileName = assetName + "_" + color;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetSizeIconImage(string size, GameObject gameObj) {
		var assetName = "sizeicon";
		var fileName = assetName + "_" + size.ToLower();
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetForceIconImage(string forceId, GameObject gameObj) {
		var assetName = "forceicon";
		var fileName = assetName + "_" + forceId;
		SetImage(assetName, fileName, gameObj);
	}

	public void SetForcePlateImage(string forceId, int forcePlateType, GameObject gameObj) {
		var assetName = "forceplate";
		var fileName = assetName + "_" + FindForcePlateTypeString(forcePlateType) + "_" +forceId;
		SetImage(assetName, fileName, gameObj);
	}
	
	public void SetRankedMatchFrameImage(string forceId, GameObject gameObj) {
		var assetName = "rankedmatchframe";
		var fileName = assetName + "_" +forceId;
		SetImage(assetName, fileName, gameObj);
	}

	public void SetBundleImage(string bundleCode, GameObject gameObj) {
		var assetName = "bundleimage";
		var fileName = assetName + "_" + bundleCode;
		SetImage (assetName, fileName, gameObj);
	}
	
	public Sprite GetCardSprite(string cardId, int displayType) {
		var cardCode = FindCardCode (cardId);
		if (!String.IsNullOrEmpty (cardCode)) {
			var assetName = cardCode.ToLower();
			var fileName = cardCode + "-" + FindDisplayTypeString(displayType);
			return GetSprite(assetName, fileName);
		}
		return null;
	}
	
	public Sprite GetCardSpriteFromCardCode(string cardCode, int displayType) {
		if (!String.IsNullOrEmpty (cardCode)) {
			var assetName = cardCode.ToLower();
			var fileName = cardCode + "-" + FindDisplayTypeString(displayType);
			return GetSprite(assetName, fileName);
		}
		return null;
	}

	public IEnumerator DownloadBGM (string assetName, Action<AudioClip> cb) {
		while (isCaching) {
			yield return null;
		}
		
		CoroutineManager.WStartCoroutine (GetBGM (assetName, (UnityEngine.Object obj) => {
			cb (obj as AudioClip);
		}));
	}
	
	public IEnumerator DownloadLive2DOperator (string assetName, Transform parent, Action<Live2DElements> cb) {
		while (isCaching) {
			yield return null;
		}
		CoroutineManager.WStartCoroutine (GetLive2DOperator (assetName, (GameObject obj) => {
			obj.transform.SetParent (parent);
			Live2DElements elements = obj.GetComponent<Live2DElements> ();
			cb (elements);
		}));
	}
}