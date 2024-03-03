using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewsController : MonoBehaviour {
	public GridSettingsView GridSettingsView;
	public RoadSettingsView RoadSettingsView;
	public PanelSettingsView PanelSettingsView;
	public PathsSettingsView PathsSettingsView;
	public PanelToolView PanelToolView;
	public PanelLayersView PanelLayersView;
	public LevelController LevelController;
	public PanelTilePrefabsView PanelTilePrefabsView;
	public PanelTileToolsView PanelTileToolsView;
	public PanelPrefabFillView PanelPrefabFillView;
	public PanelPathToolsView PanelPathToolsView;
	public PanelDecorToolsView PanelDecorToolsView;
	public PanelObjectToolsView PanelObjectToolsView;
	public PanelRoadToolsView PanelRoadToolsView;

	public void StartInit () {
		GridSettingsView.StartInit();
		PanelSettingsView.StartInit();
		PathsSettingsView.StartInit();
		PanelToolView.StartInit();
		PanelLayersView.StartInit();
		LevelController.StartInit();
		PanelTilePrefabsView.StartInit();
		PanelTileToolsView.StartInit();
		PanelPrefabFillView.StartInit();
		PanelPathToolsView.StartInit();
		PanelDecorToolsView.StartInit();
		PanelObjectToolsView.StartInit();
		PanelRoadToolsView.StartInit();
		RoadSettingsView.StartInit();
	}
}
