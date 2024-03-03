using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoadLayer : BaseLayer {
    public RoadContainer RoadContainer;

    public RoadLayer(Integer2 position, string name, RoadContainer roadContainer) {
        LayerType = LayerTypes.Road;
        Position = position;
        Name = name;
        RoadContainer = roadContainer;
    }

    public RoadLayer() {
        LayerType = LayerTypes.Road;
    }

    public RoadLayer(Integer2 position, string name) {
        LayerType = LayerTypes.Road;
        Position = position;
        Name = name;
    }
}
