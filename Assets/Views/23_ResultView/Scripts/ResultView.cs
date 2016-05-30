using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

using Newtonsoft.Json.Linq;

public class ResultView : MonoBehaviour {
	public GameObject animationManager;
	public GameObject tapScreen;

	public Sprite ItemIconAvatar;
	public Sprite ItemIconMothership;
	public Sprite ItemIconTitle;
	public Sprite ItemIconSleeve;
	public Sprite ItemIconRecoveryItems;
	public Sprite ItemIconNormalCoin;
	public Sprite ItemIconRareCoin;

	public GameObject rewardPrefab;

	public bool battleResult;
	public bool rankUpDownFlg;
	public bool itemGetFlg;
	public string cpuDifficulty;
	public string cpuColor;
	public string rewardId;
	public string itemId;
	public string itemName;
	public string itemQuantity;

	private bool isMoveScene = false;

	private bool[] oneTouch = new bool[2];

	ResultMainAnimation resultMainAnimation;
	ResultClassAnimation resultClassAnimation;
	ResultRewardAnimation resultRewardAnimation;
	
	//
	const string QUEUE_TYPE_CASUAL = "casual";
	const string QUEUE_TYPE_LOW_RANK = "lowRank";
	const string QUEUE_TYPE_ANY_RANK = "anyRank";
	const string QUEUE_TYPE_HIGH_RANK = "highRank";
	const string QUEUE_TYPE_PRACTICE = "practice";

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Game game { get { return Game.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("MultiplayView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	
	private Steward steward;
	
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();

		resultMainAnimation = animationManager.GetComponent<ResultMainAnimation> ();
		resultClassAnimation = animationManager.GetComponent<ResultClassAnimation> ();
		resultRewardAnimation = animationManager.GetComponent<ResultRewardAnimation> ();

		if (game.tCurrentGame["playerWinner"].ToString () == GameSettings.UserId) {
			resultMainAnimation.playerWinFlg = true;
			resultMainAnimation.battleResultElement.winCount.GetComponent<Text> ().color = new Color(0f, 1f, 1f);
		} else {
			resultMainAnimation.playerWinFlg = false;
			resultMainAnimation.battleResultElement.loseCount.GetComponent<Text> ().color = new Color(0f, 1f, 1f);
		}

		if (GameSettings.LastQueueType == QUEUE_TYPE_PRACTICE) {
			foreach (var token in game.tCurrentGame["playerData"]) {
				if (token is JProperty) {
					if ((token as JProperty).Name != GameSettings.UserId) {
						var obj = (token as JProperty).Value;

						cpuDifficulty = obj["difficulty"].ToString();
						cpuColor = obj["color"].ToString();
						rewardId = obj["rewardId"].ToString();
						break;
					}
				}
			}

			if (game.tCurrentGame["playerWinner"].ToString() == GameSettings.UserId && game.tRecords["cpu"][cpuDifficulty][cpuColor].Value<int>("winCount") == 1) {
				itemId = master.staticData["rewards"][rewardId]["itemId"].ToString();
				itemQuantity = master.staticData["rewards"][rewardId]["quantity"].ToString();
				itemName = master.staticData["items"][itemId]["description"].ToString();
				steward.OpenMessageWindow ("攻略報酬", "初クリア報酬として\n" +
				                           itemName + "×" + itemQuantity + "\n" +
				                           "を獲得しました",
				                           "OK", () => {});
			}

			steward.LoadNextScene ("PracticeView");

		} else if (GameSettings.LastQueueType == QUEUE_TYPE_CASUAL) {
			resultMainAnimation.rankMatchFlg = false;
			resultMainAnimation.battleResultElement.matchType.GetComponent<Text> ().text = "演習";
			var data = game.tRecords ["casual"] as JObject;
			resultMainAnimation.battleResultElement.winCount.GetComponent<Text> ().text = data ["winCount"].ToString ();
			resultMainAnimation.battleResultElement.loseCount.GetComponent<Text> ().text = data ["loseCount"].ToString ();

			var userData = game.tCurrentGame ["playerData"] [GameSettings.UserId];

			loadAssetBundle.SetClassIconImage (userData ["classId"].ToString (), resultMainAnimation.battleResultElement.currentClassIcon.gameObject);
			resultMainAnimation.battleResultElement.currentClassName.GetComponent<Text> ().text = master.staticData ["classes"] [userData ["classId"].ToString ()] ["name"].ToString ();	

			resultMainAnimation.inAnimeStart = true;
		} else {
			resultMainAnimation.rankMatchFlg = true;
			resultMainAnimation.battleResultElement.matchType.GetComponent<Text> ().text = "階級戦 (" + master.staticData ["forces"] [user.tUser ["color"].ToString ()] ["name"].ToString () + ")";
			var data = game.tRecords ["ranked"] [user.tUser ["color"].ToString ()] as JObject;
			resultMainAnimation.battleResultElement.winCount.GetComponent<Text> ().text = data ["winCount"].ToString ();
			resultMainAnimation.battleResultElement.loseCount.GetComponent<Text> ().text = data ["loseCount"].ToString ();

			var userData = game.tCurrentGame ["playerData"] [GameSettings.UserId];

			loadAssetBundle.SetClassIconImage (userData ["rankUpdate"] ["preClassId"].ToString (), resultMainAnimation.battleResultElement.currentClassIcon.gameObject);
			resultMainAnimation.battleResultElement.currentClassName.GetComponent<Text> ().text = master.staticData ["classes"] [userData ["rankUpdate"] ["preClassId"].ToString ()] ["name"].ToString ();	
			
			resultMainAnimation.battleResultElement.getExp.SetActive (true);
			resultMainAnimation.battleResultElement.getExpRow.transform.Find ("Exp").GetComponent<Text> ().text = (int.Parse (userData ["exp"].ToString ()) + int.Parse (userData ["bonusExp"].ToString ()) >= 0 ? "+" : "") + (int.Parse (userData ["exp"].ToString ()) + int.Parse (userData ["bonusExp"].ToString ())).ToString ();
			if (int.Parse (userData ["exp"].ToString ()) + int.Parse (userData ["bonusExp"].ToString ()) < 0) {
				resultMainAnimation.battleResultElement.getExpRow.transform.Find ("Exp").GetComponent<Text> ().color = new Color (1f, (float)(53f / 255f), (float)(53f / 255f));
			}
			resultMainAnimation.battleResultElement.totalExpRow.transform.Find ("TotalExp").GetComponent<Text> ().text = game.tClasses [user.tUser ["color"].ToString ()] ["exp"].ToString ();
			if (int.Parse (userData ["bonusExp"].ToString ()) > 0) {
				resultMainAnimation.battleResultElement.expAdditionImage.SetActive (true);
			} else if (int.Parse (userData ["bonusExp"].ToString ()) < 0) {
				resultMainAnimation.battleResultElement.expSubtractionImage.SetActive (true);
			}
			resultMainAnimation.battleResultElement.getExp.SetActive (false);

			resultMainAnimation.battleResultElement.getCoin.SetActive (true);
			resultMainAnimation.battleResultElement.getCoinRow.transform.Find ("Coin").GetComponent<Text> ().text = "+" + userData ["resultReward"] ["quantity"].ToString ();
			resultMainAnimation.battleResultElement.bonusCoinRow.transform.Find ("BonusCoin").GetComponent<Text> ().text = ((userData ["bonus"] != null && int.Parse (userData ["bonus"] ["quantity"].ToString ()) > 0) ? "+" + userData ["bonus"] ["quantity"].ToString () : ""); 
			resultMainAnimation.battleResultElement.totalCoinRow.transform.Find ("TotalCoin").GetComponent<Text> ().text = (int.Parse (userData ["resultReward"] ["quantity"].ToString ()) + ((userData ["bonus"] != null && int.Parse (userData ["bonus"] ["quantity"].ToString ()) > 0) ? int.Parse (userData ["bonus"] ["quantity"].ToString ()) : 0)).ToString (); 
			resultMainAnimation.battleResultElement.getCoin.SetActive (false);

			if (userData ["rankUpdate"] != null && userData ["rankUpdate"] ["status"] != null) {
				var rankUpdateStatus = int.Parse (userData ["rankUpdate"] ["status"].ToString ());
				if (rankUpdateStatus != 0) {
					resultMainAnimation.rankUpDownFlg = true;
					resultMainAnimation.battleResultElement.newClass.SetActive (true);
					loadAssetBundle.SetClassIconImage (userData ["rankUpdate"] ["classId"].ToString (), resultMainAnimation.battleResultElement.newClassIcon.gameObject);
					resultMainAnimation.battleResultElement.newClassName.GetComponent<Text> ().text = master.staticData ["classes"] [userData ["rankUpdate"] ["classId"].ToString ()] ["name"].ToString ();	
					if (rankUpdateStatus > 0) {
						resultMainAnimation.battleResultElement.arrowImage.GetComponent<Image>().color = new Color (0f, 1f, 1f);
						resultMainAnimation.battleResultElement.rankUpImage.SetActive (true);
					} else {
						resultMainAnimation.battleResultElement.arrowImage.GetComponent<Image>().color = new Color (1f, (float)(53f / 255f), (float)(53f / 255f));
						resultMainAnimation.battleResultElement.rankDownImage.SetActive (true);
					}
					resultMainAnimation.battleResultElement.newClass.SetActive (false);

					rankUpDownFlg = true;
					resultClassAnimation.classUpDownElement.oldRank.SetActive (true);
					resultClassAnimation.classUpDownElement.newRank.SetActive (true);
					resultClassAnimation.classUp_Flg = (rankUpdateStatus > 0);
					loadAssetBundle.SetClassIconImage(userData ["rankUpdate"] ["preClassId"].ToString (), resultClassAnimation.classUpDownElement.oldRank.transform.Find ("OldRankImage").gameObject);
					loadAssetBundle.SetClassIconImage(userData ["rankUpdate"] ["classId"].ToString (), resultClassAnimation.classUpDownElement.newRank.transform.Find ("NewRankImage").gameObject);
					loadAssetBundle.SetClassTextImage(int.Parse (master.staticData ["classes"] [userData ["rankUpdate"] ["preClassId"].ToString ()] ["rank"].ToString ()), resultClassAnimation.classUpDownElement.oldRank.transform.Find ("OldRankLogo").gameObject);
					loadAssetBundle.SetClassTextImage(int.Parse (master.staticData ["classes"] [userData ["rankUpdate"] ["classId"].ToString ()] ["rank"].ToString ()), resultClassAnimation.classUpDownElement.newRank.transform.Find ("NewRankLogo").gameObject);
					resultClassAnimation.classUpDownElement.oldRank.SetActive (false);
					resultClassAnimation.classUpDownElement.newRank.SetActive (false);

					int rewardCount = 0;
					foreach (JProperty prop in userData ["rankUpdate"] ["reward"]) {
						var obj = prop.Value as JObject;
						var rewardRow = Instantiate (rewardPrefab);
						rewardRow.transform.SetParent (resultClassAnimation.classUpDownElement.rewardWindow.transform.Find ("ScrollView/Content").transform);
						rewardRow.transform.localPosition = rewardPrefab.transform.localPosition;
						rewardRow.transform.localScale = rewardPrefab.transform.localScale;
						Sprite iconSprite = null;
						switch (obj["itemId"].ToString ()) {
						case "11":
							iconSprite = ItemIconAvatar;
							break;
						case "12":
							iconSprite = ItemIconMothership;
							break;
						case "13":
							iconSprite = ItemIconTitle;
							break;
						case "14":
							iconSprite = ItemIconSleeve;
							break;
						case "101":
							iconSprite = ItemIconRecoveryItems;
							break;
						case "102":
							iconSprite = ItemIconNormalCoin;
							break;
						case "103":
							iconSprite = ItemIconRareCoin;
							break;
						default:
							break;
						}
						rewardRow.transform.Find ("rewardImage").GetComponent<Image> ().sprite = iconSprite;
						rewardRow.transform.Find ("title").GetComponent<Text> ().text = obj["description"].ToString ();
						rewardRow.transform.Find ("getnum").GetComponent<Text> ().text = obj["quantity"].ToString ();
						rewardCount++;
					}
					if (rewardCount == 0) {
						resultClassAnimation.rewardGet_Flg = false;
					}
					resultClassAnimation.animeStart = true;
				} else {
					resultMainAnimation.inAnimeStart = true;
				}
			}
		}

		tapScreen.GetComponent<Image>().DOFade (1, 0.4f).SetEase (Ease.OutCubic).SetLoops (-1, LoopType.Yoyo);
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {
			if (resultMainAnimation.ResultEndAnimation == true) {
				MoveScene ();
			} else if (resultClassAnimation.rankUpEnd == true) {
				resultClassAnimation.Reset ();
				resultMainAnimation.inAnimeStart = true;
			}

		}
	}

	public void RewardEnterButton() {
		resultMainAnimation.inAnimeStart = true;
	}
	
	public void MoveScene() {
		if (isMoveScene) return;
		isMoveScene = true;
		steward.PlaySETap ();
		steward.LoadNextScene ("HomeView");
	}
}
