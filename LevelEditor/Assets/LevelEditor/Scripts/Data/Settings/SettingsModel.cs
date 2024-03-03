using System;

[Serializable]
public class SettingsModel {
    public GridSettingsModel GridSettingsModel;
    public PathsSettingsModel PathsSettingsModel;
    public RoadSettingsModel RoadSettingsModel;

    public SettingsModel() {
        GridSettingsModel = new GridSettingsModel();
        PathsSettingsModel = new PathsSettingsModel();
        RoadSettingsModel = new RoadSettingsModel();
    }

    public SettingsModel(GridSettingsModel gridSettingsModel, PathsSettingsModel pathsSettingsModel, RoadSettingsModel roadSettingsModel) {
        GridSettingsModel = gridSettingsModel;
        PathsSettingsModel = pathsSettingsModel;
        RoadSettingsModel = roadSettingsModel;
    }
}
