using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPrefabView : BaseViewController {
    [SerializeField] Text prefabName;

    public override void Init(object model = null) {
        lockEdit = true;

        this.model = model;

        Prefab prefab = model as Prefab;

        prefabName.text = prefab.Name;

        lockEdit = false;
    }

    public void ReInit() {
        if(model != null) {
            Init(model);
        }
    }

    public void Select() {
        GetComponent<Image>().color = ModuleContainer.I.ViewsController.PanelTilePrefabsView.GetSelectionColor(true);
    }

    public void Unselect() {
        GetComponent<Image>().color = ModuleContainer.I.ViewsController.PanelTilePrefabsView.GetSelectionColor(false);
    }

    public void Open() {
        ModuleContainer.I.ViewsController.PanelTilePrefabsView.SelectedPrefab = this;

        ModuleContainer.I.ViewsController.PanelPrefabFillView.Init(model);
        ModuleContainer.I.ViewsController.PanelPrefabFillView.Show();
    }
}
