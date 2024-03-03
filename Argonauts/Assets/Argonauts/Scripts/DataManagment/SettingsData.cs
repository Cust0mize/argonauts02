using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SettingsData : PersistentObject {
	public const string SOUND_VOLUME_ID = "SoundVolume";
	public const string MUSIC_VOLUME_ID = "MusicVolume";
	public const string FULLSCREEN_ENABLED_ID = "FullscreenEnabled";
    public const string SYSTEM_CURSOR_ENABLED_ID = "SystemCursorEnabled";

	const string CURRENT_PROFILE_NAME = "currentProfileName";
	const string PROFILES_NAMES = "profilesNames";

	public string CurrentProfileName {
		get {
			return GetValue<string>(CURRENT_PROFILE_NAME);
		}
		set {
			SetValue(CURRENT_PROFILE_NAME, value);
		}
	}

	public string[] ProfilesNames {
		get {
			return GetValue<string[]>(PROFILES_NAMES);
		}
		set {
			SetValue(PROFILES_NAMES, value);
		}
	}

	public SettingsData(SerializationInfo info, StreamingContext context) {
		Values = (Dictionary<string, object>)info.GetValue("values", typeof(Dictionary<string, object>));
	}

	public SettingsData(Dictionary<string, object> values) {
		Values = values;
	}

	public SettingsData() {
		Values = new Dictionary<string, object>() {
			{ SOUND_VOLUME_ID, 0.5F },
			{ MUSIC_VOLUME_ID, 0.3F },
			{ FULLSCREEN_ENABLED_ID, true },
            { SYSTEM_CURSOR_ENABLED_ID, false}
		};
	}
}
