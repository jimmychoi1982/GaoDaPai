using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json.Linq;

public class MasterBase <T> {
	private void SetProperty (string key, object data ) {	
		PropertyInfo propertyInfo = this.GetType().GetProperty (key, BindingFlags.Public | BindingFlags.Instance);
		
		if (propertyInfo.PropertyType == typeof(int)) {
			propertyInfo.SetValue(this, int.Parse(data.ToString()), null);
		} else if (propertyInfo.PropertyType == typeof(double)) {
			propertyInfo.SetValue(this, double.Parse(data.ToString()), null);
		} else if (propertyInfo.PropertyType == typeof(string)) {
			propertyInfo.SetValue(this, data.ToString(), null);
		} else if (propertyInfo.PropertyType == typeof(List<int>)) {
			var objList = (List<object>)data;
			List<int> intList = objList.ConvertAll(x => int.Parse(x.ToString()));
			propertyInfo.SetValue(this, intList, null);
		} else if (propertyInfo.PropertyType == typeof(List<double>)) {
			var objList = (List<object>)data;
			List<double> doubleList = objList.ConvertAll(x => double.Parse(x.ToString()));
			propertyInfo.SetValue(this, doubleList, null);
		} else if (propertyInfo.PropertyType == typeof(List<string>)) {
			var objList = (List<object>)data;
			List<string> strList = objList.ConvertAll(x => x.ToString());
			propertyInfo.SetValue(this, strList, null);
		} else if (propertyInfo.PropertyType == typeof(Dictionary<int, string>)) {
			var intDic = new Dictionary<int, string>();
			foreach (KeyValuePair<string, object> kv in (Dictionary<string, object>)data) {
				intDic[int.Parse(kv.Key)] = kv.Value.ToString();
			}
			propertyInfo.SetValue(this, intDic, null);
		} else if (propertyInfo.PropertyType == typeof(Dictionary<string, string>)) {
			var strDic = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> kv in (Dictionary<string, object>)data) {
				strDic[kv.Key] = kv.Value.ToString();
			}
			propertyInfo.SetValue(this, strDic, null);
		}
	}

	public void SetProperties (Dictionary<string, object> data) {
		foreach (string key in data.Keys) {
			SetProperty (key, data[key]);
		}
	}

	private void SetField (string key, object data ) {	
		if (key == "type" || key == "function" || key == "params") key = "_" + key;
		FieldInfo fieldInfo = this.GetType().GetField (key, BindingFlags.NonPublic | BindingFlags.Instance);
		
		if (fieldInfo.FieldType == typeof(int)) {
			fieldInfo.SetValue(this, int.Parse(data.ToString()));
		} else if (fieldInfo.FieldType == typeof(double)) {
			fieldInfo.SetValue(this, double.Parse(data.ToString()));
		} else if (fieldInfo.FieldType == typeof(string)) {
			fieldInfo.SetValue(this, data.ToString());
		} else if (fieldInfo.FieldType == typeof(List<int>)) {
			var objList = (List<object>)data;
			List<int> intList = objList.ConvertAll(x => int.Parse(x.ToString()));
			fieldInfo.SetValue(this, intList);
		} else if (fieldInfo.FieldType == typeof(List<double>)) {
			var objList = (List<object>)data;
			List<double> doubleList = objList.ConvertAll(x => double.Parse(x.ToString()));
			fieldInfo.SetValue(this, doubleList);
		} else if (fieldInfo.FieldType == typeof(List<string>)) {
			var strList = new List<string>();
			var obj = (data as JArray);
			foreach (var tok in obj) {
				strList.Add(tok.ToString());
			}
			fieldInfo.SetValue(this, strList);
		} else if (fieldInfo.FieldType == typeof(Dictionary<int, string>)) {
			var intDic = new Dictionary<int, string>();
			foreach (KeyValuePair<string, object> kv in (Dictionary<string, object>)data) {
				intDic[int.Parse(kv.Key)] = kv.Value.ToString();
			}
			fieldInfo.SetValue(this, intDic);
		} else if (fieldInfo.FieldType == typeof(Dictionary<string, string>)) {
			var strDic = new Dictionary<string, string>();
			var obj = (data as JObject);
			foreach (var kv in obj) {
				strDic[kv.Key] = kv.Value.ToString();
			}
			fieldInfo.SetValue(this, strDic);
		}
	}
	
	public void SetFields (JToken token) {
		JObject data = (token as JObject);
		foreach (var kv in data) {
			SetField (kv.Key, kv.Value);
		}
	}
}
