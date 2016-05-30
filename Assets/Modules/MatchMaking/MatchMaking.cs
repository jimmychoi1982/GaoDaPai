using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json.Linq;

using UnityEngine;

public class MatchMaking : Module<MatchMaking> {
	//
	Game game { get { return Game.Instance; }}

	//
	// TODO: We should have a inQueue boolean / int queueNumber, to know if the player us currently in
	// the queue. This will help make for better edge case checks such as: getting a match when not
	// actually queued; etc.
	private int handshakeCompleteTimeout = 10000;
	private Timer handshakeTimer = null;
	private Action<Exception> handshakeCompletedCb = null;
	private string currentGameId = null;

	
	//
	protected override string commandPrefix { get { return "matchMaking"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"accept",
				"cancel",
				"queue"
			};
		}
	}

	
	//
	public void Setup(Action<Exception> cb) {
		// Catch archivist set events for new game creation. We use this to know
		// when a new game has been created for this player.
		mage.archivist.on("game:set", (object sender, VaultValue vaultValue) => {
			if ((string)vaultValue.data["type"] != "cpu") {
				logger.debug("Got new game event");
				currentGameId = (string)vaultValue.index["gameId"];
				TaskManagerMainThread.Queue(onNewGame(currentGameId));
			}
		});

		cb(null);
	}

	// Called when a game match has been found and the handshake sequence should be
	// initiated.
	private IEnumerator onNewGame(string gameId) {
		// Make sure we're on the queue screen
		if (Application.loadedLevelName != "MultiplayView") {
			logger.error("Got a new game object when we're not on the queue screen");
			yield break;
		}

		// Make sure we are in the queue
		if (handshakeCompletedCb == null) {
			logger.error("Got a new game object but have not entered the queue");
			yield break;
		}

		// Make sure we don't have a game started already
		if (game.tCurrentGame != null) {
			logger.error("Got a new game object when a game has already been initialized");
			yield break;
		}

		// Setup the current game object and accept the invitation
		logger.debug("Setting up new game");
		new Task(game.SetupCurrentGame(gameId, (Exception setupError) => {
			if (setupError != null) {
				finalCallback(setupError);
				return;
			}

			// Setup timeout for final handshake completion. The handshake should
			// take a matter of milliseconds to complete. If it doesn't then the
			// player has gone so we can re-join the queue.
			handshakeTimer = new Timer((object state) => {
				handshakeTimer = null;
				logger.warning("Handshake time out");
				TaskManagerMainThread.Queue(playerReadyFailed());
			}, null, handshakeCompleteTimeout, Timeout.Infinite);

			// Accept the handshake on our end
			logger.debug("Successfully setup new game");
			new Task(accept(gameId, (Exception acceptError) => {
				if (acceptError != null) {
					handshakeTimer.Dispose();
					handshakeTimer = null;
					finalCallback(acceptError);
					return;
				}

				// Determine opponent userId
				string enemyUserId = (string)game.tCurrentGame["players"][0];
				if (enemyUserId == GameSettings.UserId) {
					enemyUserId = (string)game.tCurrentGame["players"][1];
				}

				// Check if opponent has accepted, otherwise wait for it
				Tome.OnChanged checkOpponentReady = (JToken oldValue) => {
					if ((bool)game.tCurrentGame["playerReady"][enemyUserId] && handshakeTimer != null) {
						logger.debug("Both players accepted game");
						handshakeTimer.Dispose();
						handshakeTimer = null;

						TaskManagerMainThread.Queue(onPlayerReady());
					}
				};

				logger.debug("Successfully accepted game match");
				(game.tCurrentGame["playerReady"] as TomeObject).onChanged += checkOpponentReady;
				checkOpponentReady(null);
			}));
		}));
	}

	// Called if the handshake sequence timesout. This means a player joined the queue
	// but went away (no longer listening to message stream events). In these instances
	// we drop the game and re-join the queue.
	private IEnumerator playerReadyFailed() {
		string type = GameSettings.LastQueueType;
		string deckId = (string)game.tCurrentGameSecrets["deckId"];

		Action<Exception> finalCB = handshakeCompletedCb;
		handshakeCompletedCb = null;
		new Task(queue(GameSettings.LastQueueType, 2, finalCB));
		yield break;
	}
	
	// Called when the playerReady property is true for both players. If this is the
	// case the handshake was successful so we can return the final callback.
	private IEnumerator onPlayerReady() {
		finalCallback(null);
		yield break;
	}

	// Helper to call handshake callback and clean it up
	private void finalCallback(Exception error) {
		Action<Exception> finalCB = handshakeCompletedCb;
		handshakeCompletedCb = null;
		finalCB(error);
	}
	
	// Enter game queue and wait for handshake sequence
	public IEnumerator queue(string type, int deckId, Action<Exception> cb) {
		// Make sure we are clean before entering queue again.
		// We want to hard fail here so that bugs can be fixed.
		if (handshakeCompletedCb != null) {
			logger.error("Already in the queue");
			yield break;
		}

		// Any current game setup
		game.CleanupCurrentGame();
		currentGameId = null;

		// Continue with queuing
		handshakeCompletedCb = cb;

		JObject arguments = new JObject();
		arguments.Add("type", new JValue(type));
		arguments.Add("deckId", new JValue(deckId));
		
		UsercommandStatus status = this.command("queue", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error != null) {
			finalCallback(status.error);
		}
	}

	// Leave game queue
	public IEnumerator cancel(Action<Exception> cb) {
		if (handshakeCompletedCb == null) {
			cb(new Exception("Could not cancel, not inside queue"));
			yield break;
		}
		
		if (currentGameId != null) {
			cb(new Exception("Could not cancel, already handshaking"));
			yield break;
		}

		if (game.tCurrentGame != null) {
			cb(new Exception("Could not cancel, already inside a game"));
			yield break;
		}

		JObject arguments = new JObject();
		UsercommandStatus status = this.command("cancel", arguments);
		while (!status.done) {
			yield return null;
		}
		
		if (status.error == null) {
			handshakeCompletedCb = null;
		}

		cb(status.error);
	}
	
	// Accept game handshake
	public IEnumerator accept(string gameId, Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", new JValue(gameId));
		
		UsercommandStatus status = this.command("accept", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
}
