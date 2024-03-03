using System.IO;
using UnityEngine;

public class SurveyBuild : MonoBehaviour {
    string url;

    private void Awake() {
        if(ProjectSettings.I.IsBFG_Survey) {
            DontDestroyOnLoad(gameObject);
            LoadURL();
        }
    }

    void LoadURL() {
        if(File.Exists(Path.GetDirectoryName(Application.dataPath) + "/url.txt"))
            url = File.ReadAllLines(Path.GetDirectoryName(Application.dataPath) + "/url.txt")[0];
    }

    private void OnApplicationQuit() {
        if(ProjectSettings.I.IsBFG_Survey && !string.IsNullOrEmpty(url)) {
            Application.OpenURL(url);
        }
    }
}
