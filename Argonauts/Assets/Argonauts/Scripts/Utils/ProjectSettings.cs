using UnityEngine;
using System.Collections;
using System;
using HGL;
using UnityEngine.Networking;
using System.IO;

public class ProjectSettings : GlobalSingletonBehaviour<ProjectSettings> {

    public static string AppVersion = "1.6.0";

    public Texture2D cursor;

	public int typeOfLaunch = 1;
	public bool isCheatEnabled = false;
	public bool isTrophiesEnabled = true;
    public bool isAllLevelsEnabled = false;
	public bool isPlayUrlPresent = false;
	public string url = "";
	public string copyright = "";
	public string language = "";
	public bool isMPC_build = false;
    public bool isFGP_build = false;
    public bool isMRG_build = false;
    public bool isGT_build = false;
    public bool isFGD_build = false;
    public bool IsBFG_Survey = false;
    public bool isBFG_Collector = false;
    public bool SpashEnabled = true;

    public bool isInit = false;

	public override void DoAwake () {
		base.DoAwake();
        ConfigManager.Instance.GetType();
        StartCoroutine(LoadSettings());
	}

    private IEnumerator LoadSettings() {
        string path = Path.Combine(Application.streamingAssetsPath, "settings.txt");
#if !UNITY_ANDROID || UNITY_EDITOR
        path = "file://" + path;
#endif
        string resStrings = "";

        var req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();
        resStrings = req.downloadHandler.text;

        if (!string.IsNullOrEmpty(resStrings)) {
            string[] rows = resStrings.Split(new char[] { '\n' });
            string valueString = "";

            for (int i = 0; i < rows.Length; ++i) {
                valueString = rows[i].Substring(rows[i].IndexOf("=") + 1);
                valueString = valueString.Trim();

				if (rows[i].Contains("launch_type")) {
					typeOfLaunch = Convert.ToInt32(valueString);
				} else if (rows[i].Contains("cheats")) {
					isCheatEnabled = Convert.ToInt32(valueString) != 0;
				} else if (rows[i].Contains("trophies")) {
					isTrophiesEnabled = Convert.ToInt32(valueString) != 0;
				} else if (rows[i].Contains("all_levels")) {
					isAllLevelsEnabled = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("copyright")) {
                    copyright = valueString;
                } else if (rows[i].Contains("language")) {
                    language = valueString;
                } else if (rows[i].Contains("MPC_build")) {
                    isMPC_build = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("FGP_build")) {
                    isFGP_build = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("MRG_build")) {
                    isMRG_build = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("GT_build")) {
                    isGT_build = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("FGD_build")) {
                    isFGD_build = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("bfg_survey")) {
                    IsBFG_Survey = Convert.ToInt32(valueString) != 0;
                } else if (rows[i].Contains("bfg_collector")) {
                    isBFG_Collector = Convert.ToInt32(valueString) != 0;
                }
            }
        }

#if _NO_CHEATS
        isCheatEnabled = false;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        yield return StartCoroutine(LoadUrl());
#endif


#if UNITY_STANDALONE
        LocalizationManager.Instance.LoadLocalization(language);

		if (isBFG_Collector) {
			WindowsSetTitle.SetWindowTitle(LocalizationManager.GetLocalizedString("appname_premium_ce"));
		}
		else {
			WindowsSetTitle.SetWindowTitle(LocalizationManager.GetLocalizedString("appname_premium"));
		}
#else
        switch (Application.systemLanguage) {
            case SystemLanguage.Russian:
                language = "ru";
                break;
            case SystemLanguage.French:
                language = "fr";
                break;
            case SystemLanguage.German:
                language = "de";
                break;
            default:
                language = "en";
                break;
        }
        LocalizationManager.Instance.CurrentLocale = language;
#endif

		if (typeOfLaunch == 1) {
			isInit = true;
		} else if (typeOfLaunch == 2) {
			isInit = true;
		} else if (typeOfLaunch == 3 && !string.IsNullOrEmpty(url)) {
			isInit = true;
		} else {
            HGL_WindowManager.I.OpenWindow(null, null, "PanelError", false, true);
		}

        PlayerProfileManager.Instance.LoadSettings();

        #if UNITY_STANDALONE
        if ((bool)PlayerProfileManager.I.Settings.Values[SettingsData.FULLSCREEN_ENABLED_ID]) {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
        } else {
            Screen.SetResolution(1024, 768, false);
        }

        if ((bool)PlayerProfileManager.I.Settings.Values[SettingsData.SYSTEM_CURSOR_ENABLED_ID]) {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        } else {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }
        #endif

        AudioManager.Instance.VolumeSounds = (float)PlayerProfileManager.I.Settings.Values[SettingsData.SOUND_VOLUME_ID];
        AudioManager.Instance.VolumeMusic = (float)PlayerProfileManager.I.Settings.Values[SettingsData.MUSIC_VOLUME_ID];
	}

	private IEnumerator LoadUrl() {
        string filePath = "file://" + Application.dataPath + "/../" + "play.url";

        if (Application.platform == RuntimePlatform.OSXPlayer) {
            filePath = "file://" + Application.dataPath + "/../../" + "play.url";
        }

        string resStrings = "";
        WWW www = new WWW(filePath);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            resStrings = www.text;

        if (!string.IsNullOrEmpty(resStrings)) {
            string[] rows = resStrings.Split(new char[] { '\n' });
            string valueString = "";

            for (int i = 0; i < rows.Length; ++i) {
                valueString = rows[i].Substring(rows[i].IndexOf("=") + 1);
                valueString = valueString.Trim();

                if (rows[i].Contains("URL") || rows[i].Contains("url")) {
                    url = valueString;
                    UnityEngine.Debug.Log("url: " + valueString);
                    break;
                }
            }
        }
    }

	public void OnExit_Click () {
        HGL_WindowManager.I.CloseWindow(null, null, "PanelError", false);
		Application.Quit();
	}

	public void OnApplicationQuit() {
        if (typeOfLaunch != 1 && !string.IsNullOrEmpty(url)) {
            Application.OpenURL(url);
        }

        //#if UNITY_STANDALONE_WIN
        //
        //      if (isMPC_build) {
        //          string fileName = Application.dataPath + "/../fscommand/" + "wgame.exe";
        //
        //          if (File.Exists (fileName)) {
        //              Process.Start (fileName);
        //          }
        //      }
        //
        //#endif
    }

}
