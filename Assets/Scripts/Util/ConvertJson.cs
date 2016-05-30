using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class ConvertJson {
	public static Dictionary<string, object> ConvertJTokenToDictionary(JToken token) {
		Dictionary<string, object> result = new Dictionary<string, object>();
		
		foreach (var node in token) {
			if (node is JProperty) {
				var prop = node as JProperty;
				if (prop.Value.Type == JTokenType.Object) {
					result.Add (prop.Name, ConvertJTokenToDictionary(prop.Value));
				} else if (prop.Value.Type == JTokenType.Array) {
					result.Add (prop.Name, ConvertJTokenToList(prop.Value));
				} else {
						result.Add (prop.Name, prop.Value.ToString());
				}
			}
		}
		
		return result;
	}

	public static List<object> ConvertJTokenToList(JToken token) {
		List<object> result = new List<object>();
		
		if (token is JArray) {
			foreach (var item in token as JArray) {
				if (item.Type == JTokenType.Object) {
					result.Add (ConvertJTokenToDictionary(item));
				} else if (item.Type == JTokenType.Array) {
					result.Add (ConvertJTokenToList(item));
				} else {
					result.Add (item.ToString());
				}
			}
		}

		return result;
	}
}
