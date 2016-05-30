using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

using Newtonsoft.Json.Linq;

public class BannerControllerVer2 : MonoBehaviour {
	public GameObject bannerPrefab;

	private Dictionary<string, JObject> bannerData = new Dictionary<string, JObject>();
	private GameObject[] Banner;

	private float moveAmount = 300f;
	private float moveDuration = 0.4f;
	private float moveInterval = 3f;
	private float timeleft;
	private bool slideMargin;
	private string bannerUri;
	private Transform bannerParent;

	private enum MoveDirection {
		Left = -1,
		Right = 1,
	}

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Logger logger { get { return mage.logger("HomeView"); } }

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
		
		bannerUri = GameSettings.AssetBundleURI + "/banner";
		bannerParent = transform.Find ("BannerMaskArea");

		timeleft = moveInterval;
		slideMargin = true;

		steward.SetNewTask (user.getEvent ("2", (Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error user.getEvent:");
				return;
			}

			if (steward.ResponseHasErrorCode(result)) return;

			JArray resultArr = result["result"] as JArray;
			foreach (var token in resultArr) {
				var obj = token as JObject;
				bannerData.Add(obj["eventId"].ToString(), obj);
			}
			List<KeyValuePair<string, JObject>> bannerList = new List<KeyValuePair<string, JObject>>(bannerData);
			bannerList.Sort(delegate(KeyValuePair<string, JObject> x, KeyValuePair<string, JObject> y) {
				return int.Parse (x.Value ["displayOrder"].ToString ()).CompareTo(int.Parse (y.Value ["displayOrder"].ToString ()));
			});
			Banner = new GameObject[resultArr.Count];
			int index = 0;
			foreach (var kv in bannerList) {
				var obj = kv.Value as JObject;
				Banner[index] = Instantiate (bannerPrefab);
				Banner[index].transform.SetParent(bannerParent);
				Banner[index].transform.localPosition = new Vector3 (moveAmount * index, 0f, 0f);
				Banner[index].transform.localScale = bannerPrefab.transform.localScale;
				Banner[index].GetComponent<Button>().onClick.RemoveAllListeners();
				if (obj["target"].ToString() != "0") {
					Banner[index].GetComponent<Button> ().onClick.RemoveAllListeners ();
					Banner[index].GetComponent<Button> ().onClick.AddListener(() => {
						OpenBannerURL(obj["eventId"].ToString ());
					});
				}
				new Task (GetBannerImage(index, obj["imageUrl"].ToString()));
				index++;
			}
		}));
	}
	
	// Update is called once per frame
	void Update () {
		timeleft -= Time.deltaTime;

		if (timeleft <= 0f) {
			ClickCursor((int)MoveDirection.Left);
		}
	}

	public void ClickCursor(int direction) {
		if (Banner == default(GameObject[])) return;
		if (slideMargin == true) {
			for (int i=0; i<Banner.Length; i++) {
				AdjustBannerMatrix (i, direction);
				Banner [i].transform.DOLocalMove (new Vector3 (Banner [i].transform.localPosition.x + (moveAmount * direction), 0f, 0f), moveDuration);
			}
			slideMargin = false;
			new Task (WaitMargin ());
		}
	}

	public void OpenBannerURL(string eventId) {
		var data = bannerData [eventId];
		var url = data ["href"].ToString ();
		if (data ["target"].ToString () == "1") {
			steward.OpenBrowser (url);
		} else if (data ["target"].ToString () == "2") {
			steward.OpenWebView (url);
		}
	}
	
	private void AdjustBannerMatrix(int index, int direction) {
		if (Banner [index].transform.localPosition.x * direction > 0 && Mathf.Abs (Banner [index].transform.localPosition.x * direction) >= Mathf.Abs (moveAmount * direction)) {
			Banner [index].transform.localPosition = new Vector3 (Banner [index].transform.localPosition.x + (moveAmount * Banner.Length * direction * -1), 0f, 0f);
		}
	}

	private IEnumerator WaitMargin() {
		yield return new WaitForSeconds (moveDuration);
		timeleft = moveInterval;
		slideMargin = true;
	}

	private IEnumerator GetBannerImage (int index, string fileName) {
		var downloadUri = bannerUri + "/" + fileName;
		using (WWW www = new WWW(downloadUri)) {
			yield return www;
			if (www.error != null) {
				logger.data (www.error).debug ("WWW download had an error uri:" + downloadUri);
				www.Dispose ();
				yield break;
			}
			
			while (!www.isDone) {
				yield return null;
			}

			this.Banner [index].GetComponent<Image> ().sprite = Sprite.Create (www.texture, new Rect (0f, 0f, 641f, 220f), Vector2.zero);
			www.Dispose();
		}
	}
}
