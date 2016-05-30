using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class CoreSystem : Module<CoreSystem> {
	private int majorVersion = 1;
	private int minorVersion = 0;


	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"systemVersions"
			};
		}
	}
	
	//
	protected override string commandPrefix { get { return "coreSystem"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
			};
		}
	}


	// Check version
	public void CheckClientVersion() {
		/*if () {
		} else if () {
		} else {
		}*/
	}
}