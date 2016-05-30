using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {
	//
	private static Mage mage { get { return Mage.Instance; }}

	//
	public static string Environment = "NONE";

	public enum TutorialStates {
		FirstInitialRegistration = 1,
		Tutorial,
		SecondInitialRegistration,
		Done,
		Encore
	}
	
	//
	public static void SetMageEndpoint() {
		switch (Environment) {
		case "LOCAL":
			mage.setEndpoints("http://172.16.35.2:8080", "gameApi");
			break;
		case "TESTING":
			mage.setEndpoints("http://ghm-mage.test-node.wizcorp.jp", "gameApi", "ghm-test-user", "ghm-tester-2015");
			break;
		case "QA":
			// QA環境へ接続 .
			mage.setEndpoints("http://ghm-mage-qa.test-node.wizcorp.jp", "gameApi", "ghm-test-user", "ghm-tester-2015");
			break;
		case "PRODUCTION":
			// AWS 本環境へ接続 .
			mage.setEndpoints("https://gundam-cr.com", "gameApi", "ghm-test-user", "ghm-tester-2015");
			break;
		}
	}

	//
	public static void SetMageLoggerConfig() {
		Dictionary<string, List<string>> logConfig = null;
		switch (Environment) {
		case "LOCAL":
			logConfig = new Dictionary<string, List<string>>() {
				{ "console", new List<string>() { "debug", "info", "notice", "warning", "error", "critical", "alert", "emergency" } },
				{ "server", new List<string>() { "error", "critical", "alert", "emergency" } }
			};
			break;
		case "TESTING":
			logConfig = new Dictionary<string, List<string>>() {
				{ "console", new List<string>() { "debug", "info", "notice", "warning", "error", "critical", "alert", "emergency" } },
				{ "server", new List<string>() { "error", "critical", "alert", "emergency" } }
			};
			break;
		case "QA":
			logConfig = new Dictionary<string, List<string>>() {
				{ "console", new List<string>() { "debug", "info", "notice", "warning", "error", "critical", "alert", "emergency" } },
				{ "server", new List<string>() { "error", "critical", "alert", "emergency" } }
			};
			break;
		case "PRODUCTION":
			logConfig = new Dictionary<string, List<string>>() {
				{ "console", new List<string>() { "info", "notice", "warning", "error", "critical", "alert", "emergency" } },
				{ "server", new List<string>() { "error", "critical", "alert", "emergency" } }
			};
			break;
		}

		Logger.SetConfig(logConfig);
	}

	// User ID for each environment
	public static string UserId {
		get {
			switch(Environment) {
			case "LOCAL":
				return PlayerPrefs.GetString("userIdDev");
			case "TESTING":
				return PlayerPrefs.GetString("userIdTest");
			case "QA":
				return PlayerPrefs.GetString("userIdQA");
			}

			return PlayerPrefs.GetString("userIdProd");
		}
		set {
			switch(Environment) {
			case "LOCAL":
				PlayerPrefs.SetString("userIdDev", value);
				break;
			case "TESTING":
				PlayerPrefs.SetString("userIdTest", value);
				break;
			case "QA":
				PlayerPrefs.SetString("userIdQA", value);
				break;
			}

			PlayerPrefs.SetString("userIdProd", value);
		}
	}
	
	public static int IsMigrated {
		get {
			switch(Environment) {
			case "LOCAL":
				return PlayerPrefs.GetInt("isMigratedDev", 0);
			case "TESTING":
				return PlayerPrefs.GetInt("isMigratedTest", 0);
			case "QA":
				return PlayerPrefs.GetInt("isMigratedQA", 0);
			}
			
			return PlayerPrefs.GetInt("isMigratedProd", 0);
		}
		set {
			switch(Environment) {
			case "LOCAL":
				PlayerPrefs.SetInt("isMigratedDev", value);
				break;
			case "TESTING":
				PlayerPrefs.SetInt("isMigratedTest", value);
				break;
			case "QA":
				PlayerPrefs.SetInt("isMigratedQA", value);
				break;
			}
			
			PlayerPrefs.SetInt("isMigratedProd", value);
		}
	}
	
	// TutorialState for each environment
	public static TutorialStates TutorialState {
		get {
			switch(Environment) {
			case "LOCAL":
				return (TutorialStates)PlayerPrefs.GetInt("tutorialStateDev");
			case "TESTING":
				return (TutorialStates)PlayerPrefs.GetInt("tutorialStateTest");
			case "QA":
				return (TutorialStates)PlayerPrefs.GetInt("tutorialStateQA");
			}
			
			return (TutorialStates)PlayerPrefs.GetInt("tutorialStateProd");
		}
		set {
			switch(Environment) {
			case "LOCAL":
				PlayerPrefs.SetInt("tutorialStateDev", (int)value);
				break;
			case "TESTING":
				PlayerPrefs.SetInt("tutorialStateTest", (int)value);
				break;
			case "QA":
				PlayerPrefs.SetInt("tutorialStateQA", (int)value);
				break;
			}
			
			PlayerPrefs.SetInt("tutorialStateProd", (int)value);
		}
	}

	// Last used queue type
	public static string LastQueueType {
		get {
			return PlayerPrefs.GetString("lastQueueType");
		}
		set {
			PlayerPrefs.SetString("lastQueueType", value);
		}
	}

	// Selected difficulty
	public static string CPUDifficultyType {
		get {
			return PlayerPrefs.GetString ("CPUDifficultyType");
		}
		set {
			PlayerPrefs.SetString ("CPUDifficultyType", value);
		}
	}

	// Selected force color
	public static string CPUForceColorType {
		get {
			return PlayerPrefs.GetString ("CPUForceColorType");
		}
		set {
			PlayerPrefs.SetString ("CPUForceColorType", value);
		}
	}

	public static string AssetBundleURI {
		get {
			switch (Environment) {
			case "LOCAL":
			case "TESTING":
			case "QA":
				return "https://s3-ap-northeast-1.amazonaws.com/gcwdev/asset_bundle";
			case "PRODUCTION":
			default:
				return "https://s3-ap-northeast-1.amazonaws.com/gcwrel/asset_bundle";
			}
		}
	}

	public static float VolumeMaster {
		get {
			return PlayerPrefs.GetFloat("volumeMaster", 1f);
		}
		set {
			PlayerPrefs.SetFloat("volumeMaster", value);
		}
	}
	
	public static float VolumeBGM {
		get {
			return PlayerPrefs.GetFloat("volumeBGM", 1f);
		}
		set {
			PlayerPrefs.SetFloat("volumeBGM", value);
		}
	}
	
	public static float VolumeSE {
		get {
			return PlayerPrefs.GetFloat("volumeSE", 1f);
		}
		set {
			PlayerPrefs.SetFloat("volumeSE", value);
		}
	}

	public static string StoreURI {
		get {
#if UNITY_EDITOR || UNITY_ANDROID
			return "https://play.google.com/store/apps/details?id=com.bandai.gcw";
#else
			return "https://itunes.apple.com/us/app/gundam-cross-war/id1047463857?l=ja&ls=1&mt=8";
#endif
		}
	}
	
	public static string ClientOS {
		get {
#if UNITY_EDITOR || UNITY_ANDROID
			return "Android";
#else
			return "iOS";
#endif
		}
	}
}
