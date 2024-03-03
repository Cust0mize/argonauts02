using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DecorLayer : BaseLayer {
    public DecorContainer DecorContainer;

    public DecorLayer(Integer2 position, string name, DecorContainer decorContainer) {
        LayerType = LayerTypes.Decor;
        Position = position;
        Name = name;
        DecorContainer = decorContainer;
    }

    public DecorLayer() {
        LayerType = LayerTypes.Decor;
    }

    public DecorLayer(Integer2 position, string name) {
        LayerType = LayerTypes.Decor;
        Position = position;
        Name = name;
    }
}
