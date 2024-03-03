using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PanelSettingsView : BaseViewController {

    public override void StartInit() {
        if(!string.IsNullOrEmpty(ModuleContainer.I.ConfigController.JsonSettings)) {
            model = JsonConvert.DeserializeObject<SettingsModel>(ModuleContainer.I.ConfigController.JsonSettings, ModuleContainer.I.ConfigController.GetJsonSettings());

            ModuleContainer.I.ViewsController.GridSettingsView.Init((model as SettingsModel).GridSettingsModel);
            ModuleContainer.I.ViewsController.PathsSettingsView.Init((model as SettingsModel).PathsSettingsModel);
            ModuleContainer.I.ViewsController.RoadSettingsView.Init((model as SettingsModel).RoadSettingsModel);
            return;
        }

        model = new SettingsModel();
        ModuleContainer.I.ViewsController.GridSettingsView.Init((model as SettingsModel).GridSettingsModel);
        ModuleContainer.I.ViewsController.PathsSettingsView.Init((model as SettingsModel).PathsSettingsModel);
        ModuleContainer.I.ViewsController.RoadSettingsView.Init((model as SettingsModel).RoadSettingsModel);
    }

    public void Save() {
        model = new SettingsModel(ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel, ModuleContainer.I.ViewsController.PathsSettingsView.GetModel() as PathsSettingsModel, ModuleContainer.I.ViewsController.RoadSettingsView.GetModel() as RoadSettingsModel);

        ModuleContainer.I.ConfigController.SaveSettings();

        if(ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer != null && (ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer.GetModel() as BaseLayer).LayerType == BaseLayer.LayerTypes.Tile) {
            Integer2 sizeGrid = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).GridSize;
            (ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer as ElementTileLayerView).Grid.ReInit(sizeGrid.X, sizeGrid.Y, false);
        }

        if(ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer != null && (ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer.GetModel() as BaseLayer).LayerType== BaseLayer.LayerTypes.Tile) {
            (ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer as ElementTileLayerView).GridCellVisualizer.ReDraw();
        }
    }

    public override void Init(object model = null) {
        SettingsModel sm = this.model as SettingsModel;

        sm.PathsSettingsModel.EditorSettingsPath = ModuleContainer.I.ConfigController.GetDirectoryConfigPath();

        ModuleContainer.I.ViewsController.GridSettingsView.Init(sm.GridSettingsModel);
        ModuleContainer.I.ViewsController.PathsSettingsView.Init(sm.PathsSettingsModel);
    }
}
