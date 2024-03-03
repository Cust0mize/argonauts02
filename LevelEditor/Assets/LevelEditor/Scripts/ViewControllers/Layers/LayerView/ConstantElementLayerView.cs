using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConstantElementLayerView : BaseElementLayerView {
    [SerializeField] BaseLayer.LayerTypes typeLayer;
    [SerializeField] string nameLayer;
    [SerializeField] int xPosition;
    [SerializeField] int yPosition;

    public override object GetModel() {
        switch(typeLayer) {
            case BaseLayer.LayerTypes.Object:
                return new ObjectLayer(new Integer2(xPosition, yPosition), nameLayer);
            case BaseLayer.LayerTypes.Path:
                return new PathLayer(new Integer2(xPosition, yPosition), nameLayer);
            default:
                return null;
        }
    }

    public override void UpdateVisibleState() {
        Debug.Log(gameObject.name + " update visible state");
        //TODO: update visible state 
    }
}
