using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LayerContainer {
    public BaseLayer[] Layers;

    public LayerContainer() {
        Layers = new BaseLayer[0];
    }

    public LayerContainer(BaseLayer[] layers) {
        Layers = layers;
    }
}
