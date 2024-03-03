using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementTileLayerView : DynamicElementLayerView {
	public GridCellsVisualizer GridCellVisualizer;
	GridVisualizer gridVisualizer;

	public Grid Grid {
		get {
			return (model as TileLayer).Grid;
		}set { (model as TileLayer).Grid = value; }
	}

	public override void StartInit () {
		base.StartInit ();

		if (model != null && Grid == null) {
			GridSettingsModel gridSettings = ModuleContainer.I.ViewsController.GridSettingsView.GetModel () as GridSettingsModel;
			Grid = new Grid (gridSettings.GridSize.X, gridSettings.GridSize.Y);
		}

		gridVisualizer = LayerGameObject.AddComponent<GridVisualizer> ();
		gridVisualizer.Init (this);

		GridCellVisualizer = Instantiate (ModuleContainer.I.PrefabController.GridCellVisualizer, LayerGameObject.transform).GetComponent<GridCellsVisualizer> ();
		GridCellVisualizer.Init (this);
	}

	IEnumerator DelayDraw () {
		if (gridVisualizer == null)
			yield break;
		yield return gridVisualizer.LoadAllSprites ();
		gridVisualizer.Draw ();
	}

	public override object GetModel () {
		return new TileLayer ((this.model as BaseLayer).Position, (this.model as BaseLayer).Name, Grid);
	}

	public override void Init (object model = null) {
		lockEdit = true;

		base.Init (model);

		if (model != null) {
			if (Grid == null) {
				GridSettingsModel gridSettings = ModuleContainer.I.ViewsController.GridSettingsView.GetModel () as GridSettingsModel;
				Grid = new Grid (gridSettings.GridSize.X, gridSettings.GridSize.Y);
			}
			Grid.FindNeighbors ();

			ModuleContainer.I.StartCoroutine (DelayDraw ());
		}

		lockEdit = false;
	}

	public override void Use (InputData inputData) {
		if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			switch (ModuleContainer.I.ViewsController.PanelTileToolsView.GetCurrentTool ()) {
				case PanelTileToolsView.TileTools.AddPrefab:
					if (ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab != null) {
						Grid.PrefabDraw (Integer2.GetInteger2 (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position), "1");
					}

					gridVisualizer.Draw ();
					break;
				case PanelTileToolsView.TileTools.DeletePrefab:
					Grid.PrefabDraw (Integer2.GetInteger2 (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position));

					gridVisualizer.Draw ();
					break;
				case PanelTileToolsView.TileTools.AddTile:
					Grid.TileDraw (Integer2.GetInteger2 (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position), ModuleContainer.I.ViewsController.PanelTileToolsView.CurrentTile.Path);

					gridVisualizer.Draw ();
					break;
				case PanelTileToolsView.TileTools.DeleteTile:
					Grid.TileDraw (Integer2.GetInteger2 (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position));

					gridVisualizer.Draw ();
					break;
			}
		} else if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			ModuleContainer.I.ViewsController.PanelTileToolsView.SetNothingTool ();
		}
	}

	public override void OnSomeLayerSelect (BaseElementLayerView layer) {
		base.OnSomeLayerSelect (layer);

		if (layer == this) {
			GridCellVisualizer.Draw ();
			gridVisualizer.Draw ();
		} else {
			GridCellVisualizer.SetActiveLines (false);
		}
	}

	public override void UpdateVisibleState () {
		gridVisualizer.SetActive (LayerToggle.isOn);
	}

	public override void OnLayerInOrderChanged () {
		ModuleContainer.I.StartCoroutine (DelayDraw ());
	}

	public override void OnDestroy () {
		base.OnDestroy ();
	}
}
