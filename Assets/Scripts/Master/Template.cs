using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json.Linq;

public class Templates {
	public Dictionary<string, Template> rows = new Dictionary<string, Template>();

	public Templates(JToken token) {
		foreach (var kv in (token as JObject)) {
			this.rows.Add (kv.Key, new Template (kv.Value as JToken));
		}
	}
}

public class Template : MasterBase<Template> {
	private int t_int;
	private string t_string;
	private List<int> t_intarray;
	private List<string> t_strarray;
	private Dictionary<int, string> t_inthashmap;
	private Dictionary<string, string> t_stringhashmap;

	public int TInt {
		set { t_int = value; }
		get { return t_int; }
	}
	public string TString {
		set { t_string = value; }
		get { return t_string; }
	}
	public List<int> TIntarray {
		set { t_intarray = value; }
		get { return t_intarray; }
	}
	public List<string> TStrarray {
		set { t_strarray = value; }
		get { return t_strarray; }
	}
	public Dictionary<int, string> TInthashmap {
		set { t_inthashmap = value; }
		get { return t_inthashmap; }
	}
	public Dictionary<string, string> TStringhashmap {
		set { t_stringhashmap = value; }
		get { return t_stringhashmap; }
	}

	public Template(JToken token) {
		SetFields (token);
	}
}
