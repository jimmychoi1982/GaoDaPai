using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class BNID : Module<BNID> {
	//
/*	protected override List<string> staticTopics {
		get {
			return new List<string>() {
			};
		}
	}*/
	
	protected override string commandPrefix { get { return "bnid"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"isAssociated",
				"startAssociate",
				"endAssociate",
				"startDeviceMigration",
				"endDeviceMigration",
				"startRegisterNewDevice",
				"endRegisterNewDevice",
				"disassociate"	// BNID本登録を解除
			};
		}
	}

	private User user { get { return User.Instance; }}


	//
	public IEnumerator isAssociated(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("isAssociated", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator startAssociate(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("startAssociate", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator endAssociate(string code, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("code", new JValue(code));
		arguments.Add("os", new JValue(GameSettings.ClientOS));

		UsercommandStatus status = this.command("endAssociate", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error == null && bool.Parse (status.result ["result"].ToString ()) == true) {
			GameSettings.UserId = (string)status.result ["userId"];
			GameSettings.TutorialState = GameSettings.TutorialStates.Done;
			new Task(user.login(GameSettings.UserId, (Exception e) => {}));
		}

		cb (status.error, status.result);
	}
	
	//
	public IEnumerator startDeviceMigration(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("startDeviceMigration", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator endDeviceMigration(string code, Action<Exception, JToken> cb) { 
		JObject arguments = new JObject();
		arguments.Add("code", new JValue(code));
		
		UsercommandStatus status = this.command("endDeviceMigration", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error == null) {
			GameSettings.IsMigrated = 1;
			GameSettings.UserId = null;
		}

		cb (status.error, status.result);
	}
	
	//
	public IEnumerator startRegisterNewDevice(Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("startRegisterNewDevice", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
	
	//
	public IEnumerator endRegisterNewDevice(string code, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("code", new JValue(code));
		arguments.Add("os", new JValue(GameSettings.ClientOS));

		UsercommandStatus status = this.command("endRegisterNewDevice", arguments);
		while (!status.done) {
			yield return null;
		}

		if (status.error == null && bool.Parse (status.result["result"].ToString()) == true) {
			GameSettings.UserId = (string)status.result["userId"];
			GameSettings.TutorialState = GameSettings.TutorialStates.Done;
			new Task(user.login(GameSettings.UserId, (Exception e) => {}));
		}
		
		cb (status.error, status.result);
	}

	//
	public IEnumerator disassociate(Action<Exception, JToken> cb) { 
		JObject arguments = new JObject();

		UsercommandStatus status = this.command("disassociate", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
}