using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PanelToolView : BaseViewController {
	[SerializeField]
	Text openedFile;
	[SerializeField]
	Text nameActiveLayer;
	[SerializeField]
	Toggle toggleLayerPanel;
	[SerializeField]
	InputField zoomText;
	[SerializeField]
	float oneClickZoom = 1f;
	string cacheFile;
	bool isDoneBrowser = false;

	void Start () {
		openedFile.text = string.Empty;

		ModuleContainer.I.ViewsController.PanelLayersView.Init ();

		nameActiveLayer.text = "not selected";
		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;
	}

	public void New () {
		openedFile.text = string.Empty;
		ModuleContainer.I.ViewsController.PanelLayersView.Init ();
	}

	public void Open () {
		string startPath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenPath)) {
			startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenPath;
		}

		StartCoroutine (WaitForOpen ());
		FileBrowser.OpenFilePanel ("Open file Title", startPath, null, null, (bool canceled, string filePath) => {
			if (!canceled) {
				openedFile.text = filePath;
				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentOpenPath = Path.GetDirectoryName (filePath);
				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).CurrentOpenFile = filePath;
				cacheFile = File.ReadAllText (filePath);
			}
			isDoneBrowser = true;
		});
	}

	public void Save () {
		string nameFile = "untitled.txt";

		string jsonLevel = ModuleContainer.I.ViewsController.LevelController.ToJson ();
		string startPath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);

		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).CurrentOpenFile)) {
			startPath = Path.GetDirectoryName ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).CurrentOpenFile);
			nameFile = Path.GetFileName ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).CurrentOpenFile);
		} else {
			if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSavePath)) {
				startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSavePath;
			}
		}

		FileBrowser.SaveFilePanel ("Save level", "Select you save path level", startPath, nameFile, new string[] { "txt" }, null, (bool canceled, string filePath) => {
			if (!canceled) {
				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSavePath = Path.GetDirectoryName (filePath);
				File.WriteAllText (filePath, jsonLevel);
			}
		});

		//ModuleContainer.I.ViewsController.LevelController.FromJson (jsonLevel);
	}

	public void LevelParameters () {
		ModuleContainer.I.ViewsController.LevelController.OpenLevelParameters ();
	}

	public void Export () {
		string startPath = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
		if (!string.IsNullOrEmpty ((ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSaveExportPath)) {
			startPath = (ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSaveExportPath;
		}

		Texture2D txr = ModuleContainer.I.ExportController.CaptureScreen ();
		byte[] txrBytes = txr.EncodeToJPG ();

		FileBrowser.SaveFilePanel ("Export bake texture", "Select you save path back texture", startPath, "exportTexture", new string[] { "jpg" }, null, (bool canceled, string filePath) => {
			if (!canceled) {
				(ModuleContainer.I.ViewsController.PathsSettingsView.GetModel () as PathsSettingsModel).RecentSaveExportPath = Path.GetDirectoryName (filePath);
				File.WriteAllBytes (filePath, txrBytes);
			}
		});
	}

	public void Settings () {
		ModuleContainer.I.ViewsController.PanelSettingsView.Init ();
		ModuleContainer.I.ViewsController.PanelSettingsView.Show ();
	}

	public void ZoomIn () {
		ModuleContainer.I.CameraController.Zoom (-Vector2.down * oneClickZoom);
	}

	public void ZoomOut () {
		ModuleContainer.I.CameraController.Zoom (Vector2.down * oneClickZoom);
	}

	public void UpdateZoomView () {
		lockEdit = true;
		zoomText.text = string.Format ("{0}", ModuleContainer.I.CameraController.GetPrecentZoom ());
		lockEdit = false;
	}

	public override void OnUpdateViewController () {
		if (lockEdit)
			return;

		ModuleContainer.I.CameraController.SetZoom (float.Parse (zoomText.text) / 100 * ModuleContainer.I.CameraController.MaxOrthographicSize);
	}

	public void SetActiveLayerPanel () {
		if (toggleLayerPanel.isOn) {
			ModuleContainer.I.ViewsController.PanelLayersView.Show ();
			return;
		}
		ModuleContainer.I.ViewsController.PanelLayersView.Hide ();
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null) {
			nameActiveLayer.text = (layer.GetModel () as BaseLayer).Name;
			return;
		}
		nameActiveLayer.text = "not selected";
	}

	IEnumerator WaitForOpen () {
		isDoneBrowser = false;
		while (!isDoneBrowser) {
			yield return null;
		}
		if (!string.IsNullOrEmpty (cacheFile)) {
			ModuleContainer.I.ViewsController.LevelController.FromJson (cacheFile);
		}
		cacheFile = string.Empty;
		isDoneBrowser = false;
	}
}
