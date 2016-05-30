using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json.Linq;

public class ErrorCodes {
	public Dictionary<string, ErrorCode> rows = new Dictionary<string, ErrorCode> ();
	
	public ErrorCodes(JToken token) {
		foreach (var kv in (token as JObject)) {
			this.rows.Add (kv.Key, new ErrorCode (kv.Value as JToken));
		}
	}
}			                 

public class ErrorCode : MasterBase<ErrorCode> {
	private string errorCode;
	private string description;
	private string annotation;
	private string status;

	public string _ErrorCode {
		set { errorCode = value; }
		get { return errorCode; }
	}
	public string Description {
		set { description = value; }
		get { return description; }
	}
	public string Annotation {
		set { annotation = value; }
		get { return annotation; }
	}
	public string Status {
		set { status = value; }
		get { return status; }
	}

	public ErrorCode(JToken token) {
		SetFields (token);
	}
}
