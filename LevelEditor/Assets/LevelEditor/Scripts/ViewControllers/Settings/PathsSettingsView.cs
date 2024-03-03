using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PathsSettingsView : BaseViewController {
    [SerializeField] Text editorSettingsPath;
    [SerializeField] Text recentOpenPath;
    [SerializeField] Text recentSavePath;

    Text lastText;
    string cacheFolderPath;
    bool isOpened;

    public override void Init(object model) {
        lockEdit = true;

        if((model as PathsSettingsModel) == null) {
            throw new Exception("Set wrong type model");
        }

        this.model = model;

        PathsSettingsModel psm = model as PathsSettingsModel;

        editorSettingsPath.text = GetPathText(psm.EditorSettingsPath);
        recentOpenPath.text = GetPathText(psm.RecentOpenPath);
        recentSavePath.text = GetPathText(psm.RecentSavePath);

        lockEdit = false;
    }

    public override void OnUpdateViewController() {
        if(lockEdit) return;

        if(model == null) {
            model = new PathsSettingsModel();
        }

        PathsSettingsModel psm = model as PathsSettingsModel;

        psm.EditorSettingsPath = editorSettingsPath.text;
        psm.RecentOpenPath = recentOpenPath.text;
        psm.RecentSavePath = recentSavePath.text;
    }

    public void OpenDirBrowser(Text text) {
        isOpened = false;

        lastText = text;

        StartCoroutine(WaitForOpenFolder());

        FileBrowser.OpenFolderPanel("Open folder Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, (bool canceled, string folderPath) => {
            cacheFolderPath = string.Empty;
            if(!canceled) {
                cacheFolderPath = folderPath;
            }
            isOpened = true;
        });
    }

    IEnumerator WaitForOpenFolder() {
        while(!isOpened) {
            yield return null;
        }
        if(lastText != null) {
            if(!string.IsNullOrEmpty(cacheFolderPath)) {
                lastText.text = cacheFolderPath;
            }
        }
        isOpened = false;

        OnUpdateViewController();
    }

    string GetPathText(string input) {
        string output = input;
        if(string.IsNullOrEmpty(output)) {
            output = "<empty>";
        }
        return output;
    }
}
