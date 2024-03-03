using Newtonsoft.Json;
using System;

[Serializable]
public class PathsSettingsModel {
	[NonSerialized] public string EditorSettingsPath;
	[NonSerialized] public string CurrentOpenFile;
	public string RecentOpenPath;
	public string RecentSavePath;
	public string RecentOpenObjectPath;
	public string RecentOpenRoadPath;
	public string RecentOpenDecorPath;
	public string RecentSaveExportPath;

	public PathsSettingsModel () {
		EditorSettingsPath = string.Empty;
		RecentOpenPath = string.Empty;
		RecentSavePath = string.Empty;
		RecentOpenObjectPath = string.Empty;
		RecentOpenRoadPath = string.Empty;
		RecentOpenDecorPath = string.Empty;
		RecentSaveExportPath = string.Empty;
	}

	public PathsSettingsModel (string esp, string rop, string rsp, string recentOpenObjectPath, string recentOpenRoadPath, string recentOpenDecorPath, string recentSaveExportPath) {
		EditorSettingsPath = esp;
		RecentOpenPath = rop;
		RecentSavePath = rsp;
		RecentOpenObjectPath = recentOpenObjectPath;
		RecentOpenRoadPath = recentOpenRoadPath;
		RecentOpenDecorPath = recentOpenDecorPath;
		RecentSaveExportPath = recentSaveExportPath;
	}
}
