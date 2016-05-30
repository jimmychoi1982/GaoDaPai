using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;



public class GameDiffEvent {
	public string eventType;
	public JObject eventData;

	public GameDiffEvent(string _eventType, JObject _eventData) {
		eventType = _eventType;
		eventData = _eventData;
	}
}

public class Game : Module<Game> {
	//
	Mage mage { get { return Mage.Instance; }}

	//
	public JObject tRecords = null;
	public JObject tClasses = null;
	public TomeObject tCurrentGame = null;
	public TomeObject tCurrentGameSecrets = null;

	
	//
	protected override string commandPrefix { get { return "game"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"answerQuestion",
				"attackEnemy",
				"endTurn",
				"forfeit",
				"adminAutoWin",
				"placeCard",
				"revenge",
				"start",
				"takeTurn",
				"takeStart",
				"tapIcon",
				"fixCpuUserRecords"
			};
		}
	}


	//
	public void CleanupCurrentGame() {
		if (tCurrentGame != null) {
			JObject gameIndex = new JObject();
			gameIndex.Add("gameId", new JValue((string)tCurrentGame["gameId"]));
			mage.archivist.DeleteCacheItem("game", gameIndex);
			tCurrentGame = null;
		}

		if (tCurrentGameSecrets != null) {
			JObject gameSecretsIndex = new JObject();
			gameSecretsIndex.Add("userId", new JValue(GameSettings.UserId));
			mage.archivist.DeleteCacheItem("gameSecrets", gameSecretsIndex);
			tCurrentGameSecrets = null;
		}
	}

	//
	public IEnumerator SetupCurrentGame(string gameId, Action<Exception> cb) {
		// Get new current game and set it to tCurrentGame
		Exception error = null;
		tCurrentGame = null;

		JObject queryGame = new JObject();
		queryGame.Add("topic", new JValue("game"));

		JObject indexGame = new JObject();
		indexGame.Add("gameId", new JValue(gameId));
		queryGame.Add("index", indexGame);

		JObject queryGameSecrets = new JObject();
		queryGameSecrets.Add("topic", new JValue("gameSecrets"));

		JObject indexGameSecrets = new JObject();
		indexGameSecrets.Add("userId", new JValue(GameSettings.UserId));
		queryGameSecrets.Add("index", indexGameSecrets);

		JObject queries = new JObject();
		queries.Add("tGame", queryGame);
		queries.Add("tGameSecrets", queryGameSecrets);

		mage.archivist.mget(queries, null, (Exception _error, JToken results) => {
			if (_error != null) {
				error = _error;
				return;
			}

			tCurrentGame = (TomeObject)results["tGame"];
			tCurrentGameSecrets = (TomeObject)results["tGameSecrets"];
		});

		// Wait for response
		while (error == null && tCurrentGame == null) {
			yield return null;
		}

		cb(error);
	}


	//
	public IEnumerator Start(List<string> burnCards, Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		arguments.Add("burnCards", new JArray(burnCards));
		
		UsercommandStatus status = this.command("start", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator PlaceCard(string instanceId, JToken placeOptions, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		arguments.Add("instanceId", new JValue(instanceId));
		arguments.Add("placeOptions", placeOptions);
		
		UsercommandStatus status = this.command("placeCard", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator AttackEnemy(string boardAttackerId, string boardReceiverId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		arguments.Add("boardAttackerId", new JValue(boardAttackerId));
		arguments.Add("boardReceiverId", new JValue(boardReceiverId));
		
		UsercommandStatus status = this.command("attackEnemy", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator TapIcon(string instanceId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		arguments.Add("instanceId", new JValue(instanceId));
		
		UsercommandStatus status = this.command("tapIcon", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator AnswerQuestion(JToken questionAnswer, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		arguments.Add("questionAnswers", questionAnswer);

		UsercommandStatus status = this.command("answerQuestion", arguments);
		while (!status.done) {
			yield return null;
		}

		cb(status.error, status.result);
	}
	
	//
	public IEnumerator EndTurn(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		
		UsercommandStatus status = this.command("endTurn", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator TakeTurn(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		
		UsercommandStatus status = this.command("takeTurn", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}

	//
	public IEnumerator TakeStart(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		
		UsercommandStatus status = this.command("takeStart", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator Forfeit(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		
		UsercommandStatus status = this.command("forfeit", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}

	public IEnumerator AdminAutoWin(Action<Exception> cb) {
		JObject arguments = new JObject ();
		arguments.Add("gameId", tCurrentGame["gameId"]);

		UsercommandStatus status = this.command ("adminAutoWin", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error);
	}
	
	//
	public IEnumerator Revenge(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", tCurrentGame["gameId"]);
		
		UsercommandStatus status = this.command("revenge", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
		
	//
	public IEnumerator GetUserRecords(Action<Exception> cb) {
		// Get new current game and set it to tCurrentGame
		Exception error = null;
		
		JObject index = new JObject();
		index.Add("userId", new JValue(GameSettings.UserId));
		
		JObject recordQuery = new JObject();
		recordQuery.Add("topic", new JValue("userRecords"));
		recordQuery.Add("index", index);

		JObject queries = new JObject();
		queries.Add("tRecords", recordQuery);

		mage.archivist.mget(queries, null, (Exception _error, JToken results) => {
			if (_error != null) {
				error = _error;
				return;
			}
			
			tRecords = (JObject)results["tRecords"];
		});
		
		// Wait for response
		while (error == null && tRecords == null) {
			yield return null;
		}
		
		cb(error);
	}
	
	//
	public IEnumerator GetUserClasses(Action<Exception> cb) {
		// Get new current game and set it to tCurrentGame
		Exception error = null;
		
		JObject index = new JObject();
		index.Add("userId", new JValue(GameSettings.UserId));
		
		JObject classQuery = new JObject();
		classQuery.Add("topic", new JValue("userClasses"));
		classQuery.Add("index", index);
		
		JObject queries = new JObject();
		queries.Add("tClasses", classQuery);
		
		mage.archivist.mget(queries, null, (Exception _error, JToken results) => {
			if (_error != null) {
				error = _error;
				return;
			}
			
			tClasses = (JObject)results["tClasses"];
		});
		
		// Wait for response
		while (error == null && tClasses == null) {
			yield return null;
		}
		
		cb(error);
	}

	public IEnumerator FixCpuUserRecords(Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("userId", new JValue(GameSettings.UserId));
		
		UsercommandStatus status = this.command("fixCpuUserRecords", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb(status.error);
	}
}