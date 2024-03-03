using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelDecorToolsView : BaseViewController {
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
	Toggle addDecorToggle;
	[SerializeField]
	Toggle moveDecorToggle;
	[SerializeField]
	Toggle removeDecorToggle;

	public enum DecorTools {
		Nothing,
		AddDecor,
		MoveDecor,
		RemoveDecor

	}

	Sprite spriteCurrentDecor;
	public string PathCurrentDecor;

	public Sprite SpriteCurrentDecor {
		get {
			return spriteCurrentDecor;
		}
		set {
			spriteCurrentDecor = value;

			if (value != null) {
				IconCurrentDecor.sprite = value;
				IconCurrentDecor.color = Color.white;
			} else {
				IconCurrentDecor.color = Color.clear;
			}
		}
	}

	[SerializeField]
	Image IconCurrentDecor;

	string path;
	bool isDoneBrowser = false;

	public override void StartInit () {
		Hide ();

		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;

		nothingToggle.isOn = true;
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null && (layer.GetModel () as BaseLayer).LayerType == BaseLayer.LayerTypes.Decor) {
			Show ();
		} else {
			Hide ();
		}
	}

	public void OpenDecor () {
		string startPath = System.IO.Directory.GetCurrentDirectory ();
		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenDecorPath)) {
			startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenDecorPath;
		}

		if (startPath.Contains (Directory.GetCurrentDirectory ())) {
			string currentDirectory = Directory.GetCurrentDirectory () + Path.DirectorySeparatorChar;
			path = startPath.Replace (currentDirectory, string.Empty);
		}

		StartCoroutine (LoadSprite ());
		FileBrowser.OpenFilePanel ("Open decor image", startPath, null, null, (bool canceled, string filePath) => {
			if (!canceled) {
				path = filePath;

				if (filePath.Contains (Directory.GetCurrentDirectory ())) {
					string currentDirectory = Directory.GetCurrentDirectory () + Path.DirectorySeparatorChar;
					path = filePath.Replace (currentDirectory, string.Empty);
				}

				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenDecorPath = Path.GetDirectoryName (path);
			}
			isDoneBrowser = true;
		});
	}

	IEnumerator LoadSprite () {
		isDoneBrowser = false;

		while (!isDoneBrowser) {
			yield return null;
		}

		PathCurrentDecor = path;
		yield return ModuleContainer.I.SpriteController.LoadSprite (path, OnSpriteLoaded, new Vector2 (0.5F, 0));

		SetAddTool ();
	}

	void OnSpriteLoaded (Sprite spr) {
		SpriteCurrentDecor = spr;

		ModuleContainer.I.SpriteController.AddSprite (PathCurrentDecor, SpriteCurrentDecor);
	}

	public DecorTools GetCurrentTool () {
		List<Toggle> list = ToggleGroup.ActiveToggles ().ToList ();

		if (list.Count == 0)
			return DecorTools.Nothing;

		if (list [0] == nothingToggle) {
			return DecorTools.Nothing;
		} else if (list [0] == addDecorToggle) {
			return DecorTools.AddDecor;
		} else if (list [0] == moveDecorToggle) {
			return DecorTools.MoveDecor;
		} else if (list [0] == removeDecorToggle) {
			return DecorTools.RemoveDecor;
		}

		return DecorTools.Nothing;
	}

	public void SetNothingTool () {
		ToggleGroup.SetAllTogglesOff ();
		nothingToggle.isOn = true;
	}

	public void SetAddTool () {
		ToggleGroup.SetAllTogglesOff ();
		addDecorToggle.isOn = true;
	}
}

