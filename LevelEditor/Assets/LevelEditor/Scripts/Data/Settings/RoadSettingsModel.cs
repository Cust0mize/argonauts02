using System;
using UnityEngine;

[Serializable]
public class RoadSettingsModel {
    public int SizeBrush;

    public RoadSettingsModel() { SizeBrush = 30; }

    public RoadSettingsModel(int sizeBrush) {
        SizeBrush = sizeBrush;
    }
}
