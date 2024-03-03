using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;

public class LocalizationManager {

	private static LocalizationManager instance = new LocalizationManager();
	private string currentLocale = "en";
	public string localizationFilesPath = "Localization_{0}/localization.csv";
	public string localizationRootDirectory = "/Localization_{0}";
	private Dictionary<string, string> localization = new Dictionary<string, string>();
	private Dictionary<string, Sprite> localizationSprites = new Dictionary<string, Sprite>();
	private List<string> needSprites = new List<string>() { "logo.png", "logo.png" };

	public string CurrentLocale {
		get { return currentLocale; }
		set {
			currentLocale = value;
			LoadLocalization(currentLocale);
		}
	}

	public static Action OnLanguageLoaded = delegate { };

	public static int CountSpritesLeft;

	public static LocalizationManager Instance {
		get { return instance; }
	}


	public static string GetLocalizedString(string id) {
		if (Instance.localization.ContainsKey(id))
			return Instance.localization[id].Replace("\\n", "\n");

		return id;
	}

	public static Sprite GetSprite(string id) {
		if (Instance.localizationSprites.ContainsKey(string.Format("{0}:{1}", Instance.CurrentLocale, id))) {
			return Instance.localizationSprites[string.Format("{0}:{1}", Instance.CurrentLocale, id)];
		}

		return null;
	}

	private LocalizationManager() {

		currentLocale = "en";

		switch (Application.systemLanguage) {
			case SystemLanguage.Russian:
				currentLocale = "ru";
				break;
			case SystemLanguage.French:
				currentLocale = "fr";
				break;
			case SystemLanguage.German:
				currentLocale = "de";
				break;
			default:
				currentLocale = "en";
				break;
		}

		LoadLocalization(currentLocale);
	}

	async UniTask<string> GetTextAsync(UnityWebRequest req) {
		var op = await req.SendWebRequest();
		return op.downloadHandler.text;
	}

	async public void LoadLocalization(string locale) {
		localization.Clear();
		localizationSprites.Clear();

		LoadSprites();

		string stringKey = "";
		string stringValue = "";
		int index = 0;

		string path = Path.Combine(Application.streamingAssetsPath, string.Format(localizationFilesPath, locale));
#if !UNITY_ANDROID || UNITY_EDITOR
		path = "file://" + path;
#endif
		string localizationStrings = await GetTextAsync(UnityWebRequest.Get(path));
		while (localizationStrings.Length > 0) {
			index = localizationStrings.IndexOf("\n");

			// get strings pair {key,value}
			if (index != -1) {
				stringValue = localizationStrings.Substring(0, index);
				localizationStrings = localizationStrings.Remove(0, index + 1);
			}
			else {
				stringValue = localizationStrings;
				localizationStrings = localizationStrings.Remove(0);
			}

			// parse strings pair
			index = stringValue.IndexOf(",");

			if ((index != -1) && (index != 0)) {
				stringKey = stringValue.Substring(0, index);
				stringValue = stringValue.Remove(0, index + 1);

				// delete symbol \r
				index = stringValue.IndexOf("\r");

				if (index != -1) {
					stringValue = stringValue.Remove(index, 1);
				}

				// delete symbol \"
				index = stringValue.IndexOf("\"");

				if (index != -1) {
					stringValue = stringValue.Remove(0, index + 1);
					stringValue = stringValue.Remove(stringValue.Length - 1);
				}

				// replace \"\" to \"
				stringValue = stringValue.Replace("\"\"", "\"");

				// replace <br> to \n
				stringValue = stringValue.Replace("<br>", "\n");


				// add gradient
				int gradientCount = 0;
				string stringBuffer = stringValue;

				index = stringValue.IndexOf("<gradient>");

				if (index != -1) {
					gradientCount = 1;
					stringValue = stringValue.Remove(index, 10);

					// calc amount of lines
					stringBuffer = stringValue;
					while ((index = stringBuffer.IndexOf("\n")) != -1) {
						++gradientCount;
						stringBuffer = stringBuffer.Remove(0, index + 1);
					}

					// add ^N
					int gradientIndex = gradientCount;
					if (gradientCount > 2)
						gradientIndex = 4;

					stringBuffer = stringValue;
					stringValue = "";
					while ((index = stringBuffer.IndexOf("\n")) != -1) {
						stringValue += "^" + gradientIndex.ToString();
						stringValue += stringBuffer.Substring(0, index + 1);
						stringBuffer = stringBuffer.Remove(0, index + 1);
						++gradientIndex;
					}

					stringValue += "^" + gradientIndex.ToString() + stringBuffer;
				}


				// replace <colorN> to ^N
				stringValue = stringValue.Replace("<color0>", "^0");
				stringValue = stringValue.Replace("<color1>", "^1");
				stringValue = stringValue.Replace("<color2>", "^2");
				stringValue = stringValue.Replace("<color3>", "^3");
				stringValue = stringValue.Replace("<color4>", "^4");
				stringValue = stringValue.Replace("<color5>", "^5");
				stringValue = stringValue.Replace("<color6>", "^6");
				stringValue = stringValue.Replace("<color7>", "^7");
				stringValue = stringValue.Replace("<color8>", "^8");


				// add {key, value} to dictionary
				if (!localization.ContainsKey(stringKey))
					localization.Add(stringKey, stringValue);
			}
		}
	}

	private void LoadSprites() {
		GameObject loader = new GameObject("SpriteLoader");
		loader.AddComponent<SpriteLoader>();

		foreach (string p in needSprites) {
			if (!Regex.IsMatch(p, @".+\.png$"))
				continue;

			CountSpritesLeft++;

			loader.GetComponent<SpriteLoader>().StartCoroutine(LoadSprite(Application.streamingAssetsPath + string.Format(localizationRootDirectory, CurrentLocale) + "/" + p));
		}

		loader.GetComponent<SpriteLoader>().StartCoroutine(WaitLoadSprites(loader));
	}

	private IEnumerator LoadSprite(string path) {
#if !UNITY_ANDROID || UNITY_EDITOR
		path = "file://" + path;
#endif

		UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
		yield return www.SendWebRequest();

		Texture myTexture = DownloadHandlerTexture.GetContent(www);

		while (!www.isDone) {
			yield return null;
		}

		if (localizationSprites.ContainsKey(string.Format("{0}:{1}", CurrentLocale, Path.GetFileNameWithoutExtension(path)))) {
			localizationSprites[string.Format("{0}:{1}", CurrentLocale, Path.GetFileNameWithoutExtension(path))] = Sprite.Create(myTexture as Texture2D, new Rect(0f, 0f, myTexture.width, myTexture.height), Vector2.zero);
		}
		else {
			localizationSprites.Add(string.Format("{0}:{1}", CurrentLocale, Path.GetFileNameWithoutExtension(path)), Sprite.Create(myTexture as Texture2D, new Rect(0f, 0f, myTexture.width, myTexture.height), Vector2.zero));
		}

		CountSpritesLeft--;
	}

	private IEnumerator WaitLoadSprites(GameObject loader) {
		while (CountSpritesLeft > 0) {
			yield return null;
		}
		loader.GetComponent<SpriteLoader>().Suicide();
	}
}
