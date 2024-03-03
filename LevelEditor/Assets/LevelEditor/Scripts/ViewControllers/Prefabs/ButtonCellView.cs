using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCellView : BaseViewController {
    public ButtonCellView[] CellBrothers;
    public bool IsMain = false;

    public string Path = string.Empty;
    bool isDoneBrowser = false;

    public override void StartInit() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public override void Init(object model = null) {
        if(model == null) {
            Path = string.Empty;

            UpdateImage();

            if(CellBrothers.Length > 0 && IsMain) {
                foreach(ButtonCellView b in CellBrothers) {
                    b.UpdateImage();
                }
            }
        }
        Path = Convert.ToString(model);
        ModuleContainer.I.StartCoroutine(LoadSprite(Convert.ToString(model)));
    }

    public void OnClick() {
        if(CellBrothers.Length > 0 && !IsMain) {
            foreach(ButtonCellView b in CellBrothers) {
                b.OnClick();
            }
            return;
        }

        StartCoroutine(LoadSprite());
        FileBrowser.OpenFilePanel("Open file Title", System.IO.Directory.GetCurrentDirectory(), null, null, (bool canceled, string filePath) => {
            if(!canceled) {
                Path = filePath;

                if(filePath.Contains(Directory.GetCurrentDirectory())) {
                    string currentDirectory = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar;
                    Path = filePath.Replace(currentDirectory, string.Empty);
                }
            }
            isDoneBrowser = true;
        });
    }

    IEnumerator LoadSprite() {
        isDoneBrowser = false;

        while(!isDoneBrowser) {
            yield return null;
        }

        yield return LoadSprite(Path);
    }

    IEnumerator LoadSprite(string path) {
        if(string.IsNullOrEmpty(path)) yield break;

        WWW www = new WWW("file://" + System.IO.Path.GetFullPath(path));

        while(!www.isDone) {
            yield return null;
        }

        Integer2 pivotPixels = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).PivotOffset;
        Vector2 pivot = Vector2.zero;

        if(!string.IsNullOrEmpty(www.error)) {
            Debug.Log(www.error);
            yield break;
        } else {
            pivot = new Vector2((float)pivotPixels.X / (float)www.texture.width, (float)pivotPixels.Y / (float)www.texture.height);
            Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), pivot, 1);
            UpdateImage(sprite);

            if(CellBrothers.Length > 0 && IsMain) {
                foreach(ButtonCellView b in CellBrothers) {
                    b.UpdateImage(sprite); ;
                }
            }
        }

        yield return 0;
    }

    public void UpdateImage(Sprite sprite = null) {
        if(sprite == null) {
            GetComponent<Image>().sprite = ModuleContainer.I.SpriteController.PrefabCellSprite;
            return;
        }
        GetComponent<Image>().sprite = sprite;
    }
}
