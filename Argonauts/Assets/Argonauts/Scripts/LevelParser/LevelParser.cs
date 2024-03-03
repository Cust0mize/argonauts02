using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;

public class LevelParser : LocalSingletonBehaviour<LevelParser>
{

	async UniTask<string> GetTextAsync(UnityWebRequest req) {
		var op = await req.SendWebRequest();
		return op.downloadHandler.text;
	}

	async UniTask<string> GetJsonLevel(int numLevel) {
		string path = Path.Combine(Application.streamingAssetsPath, $"Levels/Level{numLevel:00}/Level.txt");
#if !UNITY_ANDROID || UNITY_EDITOR
		path = "file://" + path;
#endif
		return await GetTextAsync(UnityWebRequest.Get(path));
	}

	async public Task<Level> GetLevel(int numLevel) {
		string jsonLevel = await GetJsonLevel(numLevel);
		if (jsonLevel == null) {
			return null;
		}

		return JsonConvert.DeserializeObject<Level>(jsonLevel, GetJsonSettings());
	}

	public static async Task<Sprite> GetLevelBack(int numLevel) {
		string path = string.Format("Levels/Level{0:00}/LevelBack.jpg", numLevel);
		return await StreamingAssetsLoader.GetSprite(path);
	}

	public static JsonSerializerSettings GetJsonSettings() {
		JsonSerializerSettings settings = new JsonSerializerSettings();
		settings.TypeNameHandling = TypeNameHandling.Auto;
		return settings;
	}
}
