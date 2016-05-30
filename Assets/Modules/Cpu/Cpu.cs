using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Cpu : Module<Cpu> {
	Game game { get { return Game.Instance; }}
	private string currentGameId = null;
	private Action<Exception> cbTemp = null;
	
	protected override string commandPrefix { get { return "cpu"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"queue",
				"executeAction",
				"endTurn"
			};
		}
	}
	
	public void Setup(Action<Exception> cb) {
		// Catch archivist set events for new game creation. We use this to know
		// when a new game has been created for this player.
		mage.archivist.on("game:set", (object sender, VaultValue vaultValue) => {
			if ((string)vaultValue.data["type"] == "cpu") {
				currentGameId = (string)vaultValue.index["gameId"];
				logger.debug("Got new game event " + currentGameId);
				TaskManagerMainThread.Queue(onNewGame(currentGameId));
			}
		});
		
		cb(null);
	}
	
	
	public IEnumerator queue(int userDeckId, string cpuId, Action<Exception> cb) {
		logger.debug("userDeckId " + userDeckId);
		logger.debug("cpuId " + cpuId);
		
		
		game.CleanupCurrentGame();
		currentGameId = null;
		cbTemp = cb;
		
		JObject arguments = new JObject();
		arguments.Add("userDeckId", new JValue(userDeckId));
		arguments.Add("cpuId", new JValue(cpuId));
		
		UsercommandStatus status = this.command("queue", arguments);
		while (!status.done) {
			yield return null;
		}
	}
	
	private IEnumerator onNewGame(string gameId) {
		new Task(game.SetupCurrentGame(gameId, (Exception setupError) => {
			cbTemp(null);
		}));
		yield break;
	}

	public IEnumerator executeAction(string gameId, Action<Exception> cb) {
		JObject arguments = new JObject();
		arguments.Add("gameId", new JValue(gameId));

		UsercommandStatus status = this.command("executeAction", arguments);
		while (!status.done)
		{
			yield return null;
		}

		cb(status.error);
	}

	public IEnumerator endTurn(string gameId, Action<Exception> cb)
	{
		JObject arguments = new JObject();
		arguments.Add("gameId", new JValue(gameId));

		UsercommandStatus status = this.command("endTurn", arguments);
		while (!status.done)
		{
			yield return null;
		}

		cb(status.error);
	}
}