using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerProfileManager {
    public static PlayerProfileManager instance;
	public static PlayerProfileManager Instance {
        get {
            return instance;
        }
    }
    public static PlayerProfileManager I {
        get {
            return Instance;
        }
    }

	const string settingsFileName = "save.settings";
	const string profileExtension = ".profile";

	string currentProfileName;
	string savePath;
	List<ProfileData> profiles;
	SettingsData settings;
	List<string> availableProfileNames;

	public string CurrentProfileName {
		get {
			return currentProfileName;
		}
	}
	public SettingsData Settings {
		get {
			return settings;
		}
	}
	public List<string> AvailableProfileNames {
		get {
			return availableProfileNames;
		}
	}

	public PlayerProfileManager() {
		profiles = new List<ProfileData>();
		availableProfileNames = new List<string>();
		settings = new SettingsData();

		LoadAll();
	}

	public void SetSavePath(string path) {
		savePath = path;
	}

	public void CreateProfile(string name) {
		var l_exists = PlayerExist(name);
		if (l_exists) {
			Debug.LogWarning(string.Format("Профиль с именем '{0}' уже существует.", name));
			return;
		}

		ProfileData l_newProfile = new ProfileData(name);
		availableProfileNames.Add(name);
		profiles.Add(l_newProfile);
	}

	public bool PlayerExist(string name) {
		return AvailableProfileNames.Contains(name);
	}

	public void DeleteProfile(string name) {
		if (!availableProfileNames.Contains(name)) {
			Debug.LogWarning(string.Format("Профиль с именем '{0}' не найден.", name));
			return;
		}

		availableProfileNames.Remove(name);

		string l_path = GetProfileSavePath(name);
		if (File.Exists(l_path))
			File.Delete(l_path);

		if (name == currentProfileName)
			currentProfileName = string.Empty;

		UnloadProfile(name);
	}

	public ProfileData GetProfile(string name) {
		if (string.IsNullOrEmpty(name)) {
			Debug.LogWarning("Имя профиля не задано.");
			return null;
		}
		var l_profile = profiles.Find(t => t.Name == name);
		return l_profile;
	}

	public List<ProfileData> GetProfiles() {
		return profiles;
	}

	public void SetCurrentProfile(string name, bool doLoad = false) {
        currentProfileName = name;
		if (doLoad)
			LoadProfile(name);

        AwardHandler.I.ClearCache();
        if (GameBonusesProgressManager.Inited)
            GameBonusesProgressManager.I.Clear();
        if (GameBonusesManager.Inited)
            GameBonusesManager.I.Clear();
    }

	public ProfileData GetCurrentProfile(bool doLoad = false) {
		if (doLoad)
			LoadProfile(currentProfileName);

		return GetProfile(currentProfileName);
	}

	public void RenameProfile(string name, string newName) {
		if (!availableProfileNames.Contains(name)) {
			Debug.LogWarning(string.Format("Профиль с именем {0} не найден.", name));
			return;
		}

		ProfileData l_profile = GetProfile(name);
		if (l_profile == null) {
			Debug.LogWarning(string.Format("Профиль с именем {0} не загружен.", name));
			return;
		}

		availableProfileNames.Remove(name);
		availableProfileNames.Add(newName);

		string l_path = GetProfileSavePath(name);
		if (File.Exists(l_path))
			File.Delete(l_path);

		l_profile.Name = newName;
		SaveProfile(l_profile);
		if (currentProfileName == name)
			SetCurrentProfile(newName);
	}

	public void SaveSettings() {
		Settings.CurrentProfileName = currentProfileName;
		Settings.ProfilesNames = AvailableProfileNames.ToArray();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Create(GetSettingsSavePath());
		bf.Serialize(fs, Settings);
		fs.Close();
	}

	public void SaveCurrentProfile() {
		if (string.IsNullOrEmpty(currentProfileName))
			return;
		SaveProfile(currentProfileName);
	}

	public void SaveProfile(ProfileData profile) {
		string path = GetProfileSavePath(profile.Name);

		var directoryPath = GetProfileFolderPath();
		if (!Directory.Exists(directoryPath))
			Directory.CreateDirectory(directoryPath);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Create(path);
		bf.Serialize(fs, profile);
		fs.Close();
	}

	public void SaveProfile(string name) {
		var l_profile = GetProfile(name);
		if (l_profile == null) {
			Debug.LogWarning(string.Format("Не найден профиль с именем '{0}'" + l_profile.Name));
			return;
		}
		SaveProfile(l_profile);
	}

	public void SaveAllProfiles() {
		foreach (var l_profile in profiles) {
			SaveProfile(l_profile);
		}
	}

	public void SaveAll() {
		SaveAllProfiles();
		SaveSettings();
	}

	public void LoadSettings() {
		string path = GetSettingsSavePath();

		if (!File.Exists(path)) {
			settings = new SettingsData();
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Open(path, FileMode.Open);
		settings = (SettingsData)bf.Deserialize(fs);
		fs.Close();

		currentProfileName = settings.CurrentProfileName;

		if (settings.ProfilesNames != null)
			availableProfileNames = new List<string>(settings.ProfilesNames);
	}

	public void LoadProfile(string name) {
		string path = GetProfileSavePath(name);

		if (!File.Exists(path)) {
			if (ProfileIsExistInLoaded(name))
				UnloadProfile(name);

			profiles.Add(new ProfileData(name));
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Open(path, FileMode.Open);

		if (ProfileIsExistInLoaded(name))
			UnloadProfile(name);

		profiles.Add((ProfileData)bf.Deserialize(fs));

		fs.Close();
	}

	public void UnloadProfile(string name) {
		var l_profile = GetProfile(name);
		if (l_profile != null)
			profiles.Remove(l_profile);
	}

	public void UnloadAllProfiles() {
		for (int i = 0; i < profiles.Count; i++) {
			profiles.Remove(profiles[i]);
		}
	}

	public void LoadAllProfiles() {
		var l_fileNames = availableProfileNames;
		foreach (string name in l_fileNames) {
			LoadProfile(name);
		}
	}

	public void LoadAll() {
		LoadSettings();
		LoadAllProfiles();
		SetCurrentProfile(currentProfileName);
	}

	private bool ProfileIsExistInLoaded(string nameProfile) {
		foreach (ProfileData p in profiles) {
			if (p.Name == nameProfile) {
				return true;
			}
		}
		return false;
	}

	public string GetProfileSavePath(string name) {
		return Path.Combine(GetProfileFolderPath(), name + profileExtension);
	}

	public string GetSettingsSavePath() {
		return Path.Combine(GetSavePath(), settingsFileName);
	}

	private string GetProfileFolderPath() {
		return Path.Combine(GetSavePath(), "Profiles");
	}

	private string GetSavePath() {
		if (!string.IsNullOrEmpty(savePath))
			return savePath;

		string res = Application.persistentDataPath + "/";

		return res;
	}
}
