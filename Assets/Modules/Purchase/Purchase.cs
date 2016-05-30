using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Purchase : Module<Purchase> {
	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"gashas",
				"events"
			};
		}
	}
	
	//
	protected override string commandPrefix { get { return "purchases"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"doGasha",
				"registerPhysicalCard",
				"registerPhysicalSleeve",
				"releasePhysicalCard",
				"registerSerial",
				"releaseSerial",
			};
		}
	}
	
	
	//
	public IEnumerator doGasha(string gashaId, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("gashaId", new JValue(gashaId));

		UsercommandStatus status = this.command("doGasha", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}
	
	//
	public IEnumerator registerPhysicalCard(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("registerPhysicalCard", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator registerPhysicalSleeve(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("registerPhysicalSleeve", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator releasePhysicalCard(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("releasePhysicalCard", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}

	//
	public IEnumerator registerSerial(string[] serialCodeArray, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add(new JProperty("serialArray", serialCodeArray));

		UsercommandStatus status = this.command("registerSerial", arguments);
		while (!status.done) {
			yield return null;
		}

		cb (status.error, status.result);
	}
	
	//
	public IEnumerator releaseSerial(string serialCode, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		arguments.Add("serial", new JValue(serialCode));
		
		UsercommandStatus status = this.command("releaseSerial", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
}