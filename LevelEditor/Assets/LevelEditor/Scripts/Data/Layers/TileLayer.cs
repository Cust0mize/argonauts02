using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileLayer : BaseLayer {
    public Grid Grid;

    public TileLayer(Integer2 position, string name, Grid grid) {
        LayerType = LayerTypes.Tile;
        Position = position;
        Name = name;
        Grid = grid;
    }

    public TileLayer() {
        LayerType = LayerTypes.Tile;
    }

    public TileLayer(Integer2 position, string name) {
        LayerType = LayerTypes.Tile;
        Position = position;
        Name = name;
    }
}
