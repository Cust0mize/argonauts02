using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelObjectToolsView : BaseViewController {
	ToggleGroup toggleGroup;

	ToggleGroup ToggleGroup {
		get {
			if (toggleGroup == null) {
				toggleGroup = GetComponent<ToggleGroup> ();
			}
			return toggleGroup;
		}
	}

	[SerializeField]
	Toggle nothingToggle;
	[SerializeField]
	Toggle addObjectToggle;
	[SerializeField]
	Toggle moveObjectToggle;
	[SerializeField]
	Toggle removeObjectToggle;
	[SerializeField]
	Toggle flipObjectToggle;

	public enum ObjectTools {
		Nothing,
		AddObject,
		MoveObject,
		RemoveObject,
		FlipObject

	}

	Sprite spriteCurrentObject;
	public string PathCurrentObject;

	public Sprite SpriteCurrentObject {
		get {
			return spriteCurrentObject;
		}
		set {
			spriteCurrentObject = value;

			if (value != null) {
				IconCurrentObject.sprite = value;
				IconCurrentObject.color = Color.white;
			} else {
				IconCurrentObject.color = Color.clear;
			}
		}
	}

	[SerializeField]
	Image IconCurrentObject;

	string path;
	bool isDoneBrowser = false;

	public override void StartInit () {
		Hide ();

		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;

		nothingToggle.isOn = true;
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null && (layer.GetModel () as BaseLayer).LayerType == BaseLayer.LayerTypes.Object) {
			Show ();
		} else {
			Hide ();
		}
	}

	public void OpenObject () {
		string startPath = System.IO.Directory.GetCurrentDirectory ();
		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenObjectPath)) {
			startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenObjectPath;
		}

		if (startPath.Contains (Directory.GetCurrentDirectory ())) {
			string currentDirectory = Directory.GetCurrentDirectory () + Path.DirectorySeparatorChar;
			path = startPath.Replace (currentDirectory, string.Empty);
		}

		StartCoroutine (LoadSprite ());
		FileBrowser.OpenFilePanel ("Open object image", startPath, null, null, (bool canceled, string filePath) => {
			if (!canceled) {
				path = filePath;

				if (filePath.Contains (Directory.GetCurrentDirectory ())) {
					string currentDirectory = Directory.GetCurrentDirectory () + Path.DirectorySeparatorChar;
                    Debug.Log(currentDirectory);
                    Debug.Log(filePath);
					path = filePath.Replace (currentDirectory, string.Empty);
				}

                Debug.Log(path);

				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenObjectPath = Path.GetDirectoryName (path);
			}
			isDoneBrowser = true;
		});
	}

	IEnumerator LoadSprite () {
		isDoneBrowser = false;

		while (!isDoneBrowser) {
			yield return null;
		}

		PathCurrentObject = path;
		yield return ModuleContainer.I.SpriteController.LoadSprite (path, OnSpriteLoaded);

		SetAddTool ();
	}

	void OnSpriteLoaded (Sprite spr) {
		SpriteCurrentObject = spr;

		ModuleContainer.I.SpriteController.AddSprite (PathCurrentObject, SpriteCurrentObject);
	}

	public ObjectTools GetCurrentTool () {
		List<Toggle> list = ToggleGroup.ActiveToggles ().ToList ();

		if (list.Count == 0)
			return ObjectTools.Nothing;

		if (list [0] == nothingToggle) {
			return ObjectTools.Nothing;
		} else if (list [0] == addObjectToggle) {
			return ObjectTools.AddObject;
		} else if (list [0] == moveObjectToggle) {
			return ObjectTools.MoveObject;
		} else if (list [0] == removeObjectToggle) {
			return ObjectTools.RemoveObject;
		} else if (list [0] == flipObjectToggle) {
			return ObjectTools.FlipObject;
		}

		return ObjectTools.Nothing;
	}

	public void SetNothingTool () {
		ToggleGroup.SetAllTogglesOff ();
		nothingToggle.isOn = true;
	}

	public void SetAddTool () {
		ToggleGroup.SetAllTogglesOff ();
		addObjectToggle.isOn = true;
	}
}
