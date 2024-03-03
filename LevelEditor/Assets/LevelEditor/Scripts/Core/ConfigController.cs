using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class ConfigController : MonoBehaviour {
	public string JsonSettings { get; set; }

	public string JsonPrefabs { get; set; }

	private const string ACTIONS_CONFIG_PATH = "Actions.csv";
    private const string TASKS_CONFIG_PATH = "Tasks.cfg";

	void Awake () {
		string filePathSettings = GetFileSettingsPath ();
		string filePathPrefabs = GetFilePrefabsPath ();

		InitConfigDirectory ();

		if (File.Exists (filePathSettings)) {
			JsonSettings = File.ReadAllText (filePathSettings);
		}

		if (File.Exists (filePathPrefabs)) {
			JsonPrefabs = File.ReadAllText (filePathPrefabs);
		}

		LoadActions ();
        LoadTasks();
	}

	private void LoadActions () {
		List<string> actions = new List<string> ();

		string file = File.ReadAllText (string.Format ("{0}/Configs/{1}", Application.streamingAssetsPath, ACTIONS_CONFIG_PATH));

		string[,] gridActions = CsvParser.SplitCsvGrid (file);

		for (int i = 1; i < gridActions.GetLength (1); i++) {
			if (!string.IsNullOrEmpty (gridActions [0, i]))
				actions.Add (gridActions [0, i]);
		}
	}

    private void LoadTasks () {
        string[] taskLines = File.ReadAllLines(string.Format("{0}/Configs/{1}", Application.streamingAssetsPath, TASKS_CONFIG_PATH));

        ModuleContainer.I.ViewsController.LevelController.LevelParameters.InitDropdowns(taskLines.Where(x => !string.IsNullOrEmpty(x)).ToList());
    }

	public void InitConfigDirectory () {
		string directoryPath = GetDirectoryConfigPath ();

		if (string.IsNullOrEmpty (directoryPath)) {
			throw new Exception ("Directory path settings is not init");
		}

		if (!Directory.Exists (directoryPath)) {
			Directory.CreateDirectory (directoryPath);
		}
	}

	public void Save () {
		SaveSettings ();
		SavePrefabs ();
	}

	public void SaveSettings () {
		if (ModuleContainer.I.ViewsController.PanelSettingsView.GetModel () == null)
			return;
		string json = JsonConvert.SerializeObject (ModuleContainer.I.ViewsController.PanelSettingsView.GetModel (), GetJsonSettings ());

		InitConfigDirectory ();

		string filePath = GetFileSettingsPath ();

		if (File.Exists (filePath)) {
			File.Delete (filePath);
		}

		File.WriteAllText (filePath, json);
	}

	public void SavePrefabs () {
		if (ModuleContainer.I.ViewsController.PanelTilePrefabsView.GetModel () == null)
			return;
		string json = JsonConvert.SerializeObject (ModuleContainer.I.ViewsController.PanelTilePrefabsView.GetModel (), GetJsonSettings ());

		InitConfigDirectory ();

		string filePath = GetFilePrefabsPath ();

		if (File.Exists (filePath)) {
			File.Delete (filePath);
		}

		File.WriteAllText (filePath, json);
	}

	public SettingsModel GetSettings () {
		if (string.IsNullOrEmpty (JsonSettings)) {
			return null;
		}

		return JsonConvert.DeserializeObject<SettingsModel> (JsonSettings, GetJsonSettings ());
	}

	public PrefabContainer GetPrefabContainer () {
		if (string.IsNullOrEmpty (JsonPrefabs)) {
			return null;
		}

		return JsonConvert.DeserializeObject<PrefabContainer> (JsonPrefabs, GetJsonSettings ());
	}

	public string GetDirectoryConfigPath () {
		string directory = string.Empty;

		if (Application.platform == RuntimePlatform.OSXPlayer) {
			directory = Path.Combine ("file://", Application.dataPath);
			directory = Path.Combine (directory, "../../Config/");
		} else {
			directory = Path.Combine (Application.dataPath, "../Config/");
		}

		return directory;
	}

	public string GetFileSettingsPath () {
		return Path.Combine (GetDirectoryConfigPath (), "Settings.txt");
	}

	public string GetFilePrefabsPath () {
		return Path.Combine (GetDirectoryConfigPath (), "Prefabs.txt");
	}

	public JsonSerializerSettings GetJsonSettings () {
		JsonSerializerSettings settings = new JsonSerializerSettings ();
		settings.TypeNameHandling = TypeNameHandling.Auto;
		return settings;
	}

	private void OnApplicationQuit () {
		Save ();
	}
}
