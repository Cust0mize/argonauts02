using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathLayer : BaseLayer {
    public PathGraph PathGraph;

    public PathLayer(Integer2 position, string name, PathGraph pathGraph) {
        LayerType = LayerTypes.Path;
        Position = position;
        Name = name;
        PathGraph = pathGraph;
    }

    public PathLayer() {
        LayerType = LayerTypes.Path;
    }

    public PathLayer(Integer2 position, string name) {
        LayerType = LayerTypes.Path;
        Position = position;
        Name = name;
    }
}
