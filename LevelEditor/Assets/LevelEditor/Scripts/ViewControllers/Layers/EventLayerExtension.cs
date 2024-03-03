using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventLayerExtension : MonoBehaviour {
    public void OnValueChanged(BaseElementLayerView element) {
        ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSetActiveChanged(element);
    }

    public void OnSelected(BaseElementLayerView element) {
        ModuleContainer.I.ViewsController.PanelLayersView.SelectedLayer = element;
    }
}
