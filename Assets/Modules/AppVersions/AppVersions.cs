using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class AppVersions : Module<AppVersions> {

	//
	protected override string commandPrefix { get { return "appVersions"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"isCommandEnable",
			};
		}
	}
	
	//
	public IEnumerator isCommandEnable(string endPoint, Action<Exception, JToken> cb) {
		JObject arguments = new JObject();
		var os = Application.platform == RuntimePlatform.Android ? "Android" : "iOS";
		arguments.Add ("os", new JValue (os));
		arguments.Add ("appVersion", new JValue (BundleVersion.Get ()));
		arguments.Add ("endpoint", new JValue (endPoint));

		UsercommandStatus status = this.command("isCommandEnable", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error, status.result);
	}
}
