using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using UnityEngine;

public class User : Module<User> {
	//
	public JObject tUser = null;
	public JObject tAvatars = null;
	public JObject tCards = null;
	public JObject tMotherships = null;
	public JObject tSleeves = null;
	public JObject tTitles = null;
	public JObject tOperators = null;

	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"titles",
			};
		}
	}

	//
	protected override string commandPrefix { get { return "user"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"login",
				"registerGuest",
				"registerKantanId",
				"registerNBID",
				"setInitialSetting",
				"changeDeckMain",
				"changeDeckAvatar",
				"changeDeckMothership",
				"changeDeckSleeve",
				"changeDeckCard",
				"changeDeckName",
				"changeDeckFavoriteCard",
				"setTitle",
				"setOperator",
				"setName",
				"setColor",
				"setFreeword",
				"dailySupply",
				"validateMatch",
				"useRecoveryItem",
				"getSerialLog",
				"getPresentLog",
				"getMatchingLog",
				"getEvent",
				"getMatch",
				"checkVersion"
			};
		}
	}


	//
	public void Setup(Action<Exception> cb) {
		mage.eventManager.on ("session.set", (object sender, JToken session) => {
			JObject index = new JObject();
			index.Add("userId", (session as JObject)["actorId"]);

			mage.archivist.get ("user", index, null, (Exception error, JToken _tUser) => {
				if (error != null) {
					logger.data(error).error("Could not setup user:");
					return;
				}

				tUser = (JObject)_tUser;
				logger.debug("Successfully setup user");
			});
		});

		cb(null);
	}


	//
	public IEnumerator loginOrRegister(Action<Exception> cb) {
		Action<Exception> loginCb = (Exception error) => {
			new Task(login(GameSettings.UserId, cb));
		};

		if (string.IsNullOrEmpty (GameSettings.UserId)) {
			GameSettings.TutorialState = GameSettings.TutorialStates.FirstInitialRegistration;
			new Task(registerKantanId(loginCb));
		} else {
			loginCb (null);
		}

		yield break;
	}

	//
	public IEnumerator login(string userId, Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("userId", new JValue(userId));

		UsercommandStatus status = this.command("login", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error);
	}

	//
	public IEnumerator registerGuest(Action<Exception> cb) {
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("registerGuest", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error == null) {
			GameSettings.UserId = (string)status.result["userId"];
		}

		cb (status.error);
	}

	//
	public IEnumerator registerKantanId(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("os", new JValue(GameSettings.ClientOS));

		UsercommandStatus status = this.command("registerKantanId", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error == null) {
			GameSettings.UserId = (string)status.result["userId"];
		}

		cb (status.error);
	}

	//
	public IEnumerator registerNBID(Action<Exception> cb) {
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("registerNBID", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error);
	}

	//
	public IEnumerator setInitialSetting(string color, string emblemId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("color", new JValue(color));
		arguments.Add("emblemId", new JValue(emblemId));

		UsercommandStatus status = this.command("setInitialSetting", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}
	
	//
	public IEnumerator changeDeckMain(int deckId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));

		UsercommandStatus status = this.command("changeDeckMain", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckAvatar(int deckId, string avatarId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("avatarId", new JValue(avatarId));

		UsercommandStatus status = this.command("changeDeckAvatar", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckMothership(int deckId, string mothershipId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("mothershipId", new JValue(mothershipId));

		UsercommandStatus status = this.command("changeDeckMothership", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckSleeve(int deckId, string sleeveId, string deckType, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("sleeveId", new JValue(sleeveId));
		arguments.Add("deckType", new JValue(deckType));

		UsercommandStatus status = this.command("changeDeckSleeve", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckCard(int deckId, string[] cardIdArray, string deckType, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add(new JProperty("cardIdArray", cardIdArray));
		arguments.Add("deckType", new JValue(deckType));

		UsercommandStatus status = this.command("changeDeckCard", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckName(int deckId, string deckName, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("deckName", new JValue(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(deckName))));

		UsercommandStatus status = this.command("changeDeckName", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator changeDeckFavoriteCard(int deckId, string cardId, string deckType, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("cardId", new JValue(cardId));
		arguments.Add("deckType", new JValue(deckType));

		UsercommandStatus status = this.command("changeDeckFavoriteCard", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator setTitle(string titleId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("titleId", new JValue(titleId));

		UsercommandStatus status = this.command("setTitle", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator setOperator(string operatorId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("operatorId", new JValue(operatorId));
		
		UsercommandStatus status = this.command("setOperator", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator setName(string userName, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("userName", new JValue(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(userName))));

		UsercommandStatus status = this.command("setName", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator setColor(string color, string emblemId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("color", new JValue(color));
		arguments.Add("emblemId", new JValue(emblemId));

		UsercommandStatus status = this.command("setColor", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator setFreeword(string freeword, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("freeword", new JValue(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(freeword))));

		UsercommandStatus status = this.command("setFreeword", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}

	//
	public IEnumerator dailySupply(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("dailySupply", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator validateMatch(int deckId, string matchId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("deckId", new JValue(deckId));
		arguments.Add("matchId", new JValue(matchId));

		UsercommandStatus status = this.command("validateMatch", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator useRecoveryItem(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("useRecoveryItem", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator getSerialLog(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("getSerialLog", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator getPresentLog(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("getPresentLog", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator getMatchingLog(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("getMatchingLog", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator getEvent(string eventType, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add ("eventType", new JValue (eventType));

		UsercommandStatus status = this.command("getEvent", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator getMatch(string color, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add ("color", new JValue (color));
		
		UsercommandStatus status = this.command("getMatch", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator checkVersion(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		var os = Application.platform == RuntimePlatform.Android ? "Android" : "iOS";
		arguments.Add ("os", new JValue (os));
		arguments.Add ("version", new JValue (BundleVersion.Get ()));

		UsercommandStatus status = this.command("checkVersion", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator GetUserData(Action<Exception> cb) {
		// Get new current game and set it to tCurrentGame
		Exception error = null;

		if (GameSettings.TutorialState != GameSettings.TutorialStates.Done && GameSettings.TutorialState != GameSettings.TutorialStates.Encore) {
			cb (error);
			yield break;
		}

		JObject index = new JObject();
		index.Add("userId", new JValue(GameSettings.UserId));

		JObject avatarQuery = new JObject();
		avatarQuery.Add("topic", new JValue("userAvatars"));
		avatarQuery.Add("index", index);

		JObject cardQuery = new JObject();
		cardQuery.Add("topic", new JValue("userCards"));
		cardQuery.Add("index", index);

		JObject mothershipQuery = new JObject();
		mothershipQuery.Add("topic", new JValue("userMotherships"));
		mothershipQuery.Add("index", index);

		JObject sleeveQuery = new JObject();
		sleeveQuery.Add("topic", new JValue("userSleeves"));
		sleeveQuery.Add("index", index);
		
		JObject titleQuery = new JObject();
		titleQuery.Add("topic", new JValue("userTitles"));
		titleQuery.Add("index", index);
		
		JObject operatorQuery = new JObject();
		operatorQuery.Add("topic", new JValue("userOperators"));
		operatorQuery.Add("index", index);

		JObject queries = new JObject();
		queries.Add("tAvatars", avatarQuery);
		queries.Add("tCards", cardQuery);
		queries.Add("tMotherships", mothershipQuery);
		queries.Add("tSleeves", sleeveQuery);
		queries.Add("tTitles", titleQuery);
		queries.Add("tOperators", operatorQuery);

		mage.archivist.mget(queries, null, (Exception _error, JToken results) => {
			if (_error != null) {
				error = _error;
				return;
			}

			tAvatars = (JObject)results["tAvatars"];
			tCards = (JObject)results["tCards"];
			tMotherships = (JObject)results["tMotherships"];
			tSleeves = (JObject)results["tSleeves"];
			tTitles = (JObject)results["tTitles"];
			tOperators = (JObject)results["tOperators"];
		});

		// Wait for response
		while (error == null && tCards == null) {
			yield return null;
		}
		
		cb(error);
	}
}
