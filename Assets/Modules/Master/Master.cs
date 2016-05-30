using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Master : Module<Master> {
	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"avatars",
				"classes",
				"emblem",
				"forces",
				"items",
				"motherships",
				"operators",
				"operatorTalks",
				//"presents",
				"rewards",
				"sleeve",
				"titles",
				"characteristics",
				"cardsPacks",
				"matches",
				"cardColors",
				"prohibitCards",
				"errorCodes",
				"assetBundleVersions",
				"defaultDecks",
				"bundle"
			};
		}
	}
}