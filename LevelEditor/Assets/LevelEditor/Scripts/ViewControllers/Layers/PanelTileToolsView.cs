using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelTileToolsView : BaseViewController {
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
	[SerializeField] Toggle addPrefabToggle;
	[SerializeField] Toggle deletePrefabToggle;
	[SerializeField] Toggle addTileToggle;
	[SerializeField] Toggle deleteTileToggle;

	[SerializeField] Image iconCurrentTile;

	public Tile CurrentTile = new Tile ();

	string path;
	bool isDoneBrowser = false;

	public enum TileTools {
		Nothing,
		AddPrefab,
		DeletePrefab,
		AddTile,
		DeleteTile

	}

	public override void StartInit () {
		Hide ();

		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;

		nothingToggle.isOn = true;
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null && (layer.GetModel () as BaseLayer).LayerType == BaseLayer.LayerTypes.Tile) {
			Show ();
		} else {
			Hide ();
		}
	}

	public void OpenPrefabsPanel () {
		ModuleContainer.I.ViewsController.PanelTilePrefabsView.Show ();
	}

	public void OpenTile () {
		StartCoroutine (LoadSprite ());
		FileBrowser.OpenFilePanel ("Open file Title", System.IO.Directory.GetCurrentDirectory (), null, null, (bool canceled, string filePath) => {
			if (!canceled) {
				path = filePath;

				if (filePath.Contains (Directory.GetCurrentDirectory ())) {
					string currentDirectory = Directory.GetCurrentDirectory () + Path.DirectorySeparatorChar;
					path = filePath.Replace (currentDirectory, string.Empty);
				}
			}
			isDoneBrowser = true;
		});
	}

	IEnumerator LoadSprite () {
		isDoneBrowser = false;

		while (!isDoneBrowser) {
			yield return null;
		}

		CurrentTile.Path = path;

		yield return ModuleContainer.I.SpriteController.LoadSprite (path, OnSpriteLoaded, true);
	}

	void OnSpriteLoaded (Sprite spr) {
		CurrentTile.Sprite = spr;

		ModuleContainer.I.SpriteController.AddSprite (new Tile (CurrentTile.Path, CurrentTile.Sprite));

		iconCurrentTile.sprite = CurrentTile.Sprite;
		iconCurrentTile.color = Color.white;
	}

	public TileTools GetCurrentTool () {
		List<Toggle> list = ToggleGroup.ActiveToggles ().ToList ();

		if (list.Count == 0)
			return TileTools.Nothing;

		if (list [0] == nothingToggle) {
			return TileTools.Nothing;
		} else if (list [0] == addPrefabToggle) {
			return TileTools.AddPrefab;
		} else if (list [0] == deletePrefabToggle) {
			return TileTools.DeletePrefab;
		} else if (list [0] == addTileToggle) {
			return TileTools.AddTile;
		} else if (list [0] == deleteTileToggle) {
			return TileTools.DeleteTile;
		}

		return TileTools.Nothing;
	}

	public void SetNothingTool () {
		ToggleGroup.SetAllTogglesOff ();
		nothingToggle.isOn = true;
	}
}
