using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigManager {
	#region Singleton implementation

	private static ConfigManager instance = new ConfigManager ();

	private ConfigManager () {
		LoadConfigs ();
	}

	public static ConfigManager Instance {
		get {
			if (instance == null) {
				instance = new ConfigManager ();
			}
			return instance;
		}
	}

	#endregion

	private const string ACTIONS_CONFIG_PATH = "Actions.csv";
	private Dictionary<string , PlayerAction> Actions = new Dictionary<string , PlayerAction> ();

	public float GetDurationAction (string key) {
		float dur = 0;
		if (Actions.ContainsKey (key)) {
			dur = Actions [key].Duration;
		}
		return dur;
	}

	private void LoadConfigs () {
		LoadActions ();
	}

	async UniTask<string> GetTextAsync(UnityWebRequest req) {
		var op = await req.SendWebRequest();
		return op.downloadHandler.text;
	}

	async private void LoadActions() {
		Actions.Clear();

		string path = Path.Combine(Application.streamingAssetsPath, $"Configs/{ACTIONS_CONFIG_PATH}");
#if !UNITY_ANDROID || UNITY_EDITOR
		path = "file://" + path;
#endif
		string file = await GetTextAsync(UnityWebRequest.Get(path));

		string[,] gridActions = CsvParser.SplitCsvGrid (file);

		Dictionary<string , int> need_res_indexes = new Dictionary<string , int> ();
		int get_res_index_name = -1;
		int get_res_index_amount = -1;
		int duration_act_index = -1;

		for (int i = 0; i < gridActions.GetLength (0); i++) {
			if (!string.IsNullOrEmpty (gridActions [i, 0])) {
				if (Enum.IsDefined (typeof(Resource.Types), gridActions [i, 0])) {
					need_res_indexes.Add (gridActions [i, 0], i);
				} else {
					switch (gridActions [i, 0]) {
						case "Give_Item":
							get_res_index_name = i;
							break;
						case "Give_Amount":
							get_res_index_amount = i;
							break;
						case "Duration":
							duration_act_index = i;
							break;
					}
				}
			}
		}

		for (int i = 1; i < gridActions.GetLength (1); i++) {
			if (!string.IsNullOrEmpty (gridActions [0, i])) {
				List<Resource> needResources = new List<Resource> ();
				Resource getResource = null;

				foreach (string key in need_res_indexes.Keys) {
					if (ParseToInt (gridActions [need_res_indexes [key], i]) > 0) {
						needResources.Add (new Resource ((Resource.Types)Enum.Parse (typeof(Resource.Types), gridActions [need_res_indexes [key], 0]), ParseToInt (gridActions [need_res_indexes [key], i])));
					}
				}

				if (!get_res_index_name.Equals (-1) && !get_res_index_amount.Equals (-1)) {
					if (!string.IsNullOrEmpty (gridActions [get_res_index_name, i]) && !string.IsNullOrEmpty (gridActions [get_res_index_amount, i]))
						getResource = new Resource ((Resource.Types)Enum.Parse (typeof(Resource.Types), gridActions [get_res_index_name, i]), ParseToInt (gridActions [get_res_index_amount, i]));
				}

				if (getResource != null)
					Actions.Add (gridActions [0, i], new PlayerAction (needResources, getResource, ParseToFloat (gridActions [duration_act_index, i])));
				else
					Actions.Add (gridActions [0, i], new PlayerAction (needResources, ParseToFloat (gridActions [duration_act_index, i])));
			}
		}
	}

	private void DebugActions () {
		Debug.Log ("CountActions: " + Actions.Count);

		foreach (string k in Actions.Keys) {
			string d = k;
			if (Actions [k].NeedResources.Count > 0)
				d += "\n";
			foreach (Resource r in Actions[k].NeedResources) {
				d += r.Type + "(" + r.Count + ")" + ", ";
			}
			if (Actions [k].GetResources != null)
				d += "\nGetResouces: " + Actions [k].GetResources.Type.ToString () + " - " + Actions [k].GetResources.Count;
			Debug.Log (d);
		}
	}

	private int ParseToInt (string text) {
		int result = 0;
		int.TryParse (text, out result);
		return result;
	}

	private float ParseToFloat (string text) {
		float result = 0;
		float.TryParse (text, out result);
		return result;
	}

	public PlayerAction GetPlayerAction (string key) {
		PlayerAction playerAction;
		Actions.TryGetValue (key, out playerAction);
        return ObjectCopier.Clone(playerAction);
	}
}
