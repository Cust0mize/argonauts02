using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelRoadToolsView : BaseViewController {
	ToggleGroup toggleGroup;

	ToggleGroup ToggleGroup {
		get {
			if (toggleGroup == null) {
				toggleGroup = GetComponent<ToggleGroup> ();
			}
			return toggleGroup;
		}
	}

	[SerializeField] Toggle nothingToggle;
	[SerializeField] Toggle addRoadToggle;
	[SerializeField] Toggle moveRoadToggle;
	[SerializeField] Toggle removRoadToggle;

	[SerializeField] InputField sizeBrushInputField;

	public enum RoadTools {
		Nothing,
		AddRoad,
		MoveRoad,
		RemoveRoad

	}

	Sprite spriteCurrentRoad;
	[HideInInspector] public string PathCurrentRoad;

	public Sprite SpriteCurrentRoad {
		get {
			return spriteCurrentRoad;
		}
		set {
			spriteCurrentRoad = value;

			if (value != null) {
				IconCurrentRoad.sprite = value;
				IconCurrentRoad.color = Color.white;
			} else {
				IconCurrentRoad.color = Color.clear;
			}
		}
	}

	public Texture TextureCurrentRoad {
		get {
			if (spriteCurrentRoad == null)
				return null;
			return spriteCurrentRoad.texture;
		}
	}

	public float SizeBrush {
		get {
			if (string.IsNullOrEmpty (sizeBrushInputField.text))
				return 0;
			return int.Parse (sizeBrushInputField.text);
		}
		set { sizeBrushInputField.text = value.ToString (); }
	}

	[SerializeField] Image IconCurrentRoad;

	string path;
	bool isDoneBrowser = false;

	public override void StartInit () {
		Hide ();

		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;

		SizeBrush = (ModuleContainer.I.ViewsController.RoadSettingsView.GetModel () as RoadSettingsModel).SizeBrush;

		nothingToggle.isOn = true;
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null && (layer.GetModel () as BaseLayer).LayerType == BaseLayer.LayerTypes.Road) {
			Show ();
		} else {
			Hide ();
		}
	}

	public void OpenRoad () {
		string startPath = System.IO.Directory.GetCurrentDirectory ();
		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenRoadPath)) {
			startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenRoadPath;
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
					path = filePath.Replace (currentDirectory, string.Empty);
				}

				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenRoadPath = Path.GetDirectoryName (path);
			}
			isDoneBrowser = true;
		});
	}

	IEnumerator LoadSprite () {
		isDoneBrowser = false;

		while (!isDoneBrowser) {
			yield return null;
		}

		PathCurrentRoad = path;
		yield return ModuleContainer.I.SpriteController.LoadSprite (path, OnSpriteLoaded);

		SetAddTool ();
	}

	void OnSpriteLoaded (Sprite spr) {
		SpriteCurrentRoad = spr;

		ModuleContainer.I.SpriteController.AddSprite (PathCurrentRoad, SpriteCurrentRoad);
	}

	public RoadTools GetCurrentTool () {
		List<Toggle> list = ToggleGroup.ActiveToggles ().ToList ();

		if (list.Count == 0)
			return RoadTools.Nothing;

		if (list [0] == nothingToggle) {
			return RoadTools.Nothing;
		} else if (list [0] == addRoadToggle) {
			return RoadTools.AddRoad;
		} else if (list [0] == moveRoadToggle) {
			return RoadTools.MoveRoad;
		} else if (list [0] == removRoadToggle) {
			return RoadTools.RemoveRoad;
		}
		return RoadTools.Nothing;
	}

	public void AddSizeBrush () {
		SizeBrush += 1;
	}

	public void SubtractSizeBrush () {
		SizeBrush -= 1;
	}

	public void SetNothingTool () {
		ToggleGroup.SetAllTogglesOff ();
		nothingToggle.isOn = true;
	}

	public void SetAddTool () {
		ToggleGroup.SetAllTogglesOff ();
		addRoadToggle.isOn = true;
	}
}
