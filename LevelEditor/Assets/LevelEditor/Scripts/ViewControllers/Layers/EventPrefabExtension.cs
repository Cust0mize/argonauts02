using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPrefabExtension : MonoBehaviour {
    public void OnSelected(ElementPrefabView element) {
        ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab = element;
    }
}
