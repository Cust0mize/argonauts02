using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseElementLayerView : BaseViewController {
    public int layerInOrder = 0;
    public int LayerInOrder {
        get {
            return layerInOrder;
        }set {
            layerInOrder = value;
            OnLayerInOrderChanged();
        }
    }

    GameObject layerGameObject;
    protected GameObject LayerGameObject {
        get {
            if(layerGameObject == null) {
                layerGameObject = new GameObject(string.Format("{0} Container", gameObject.name));
            }
            return layerGameObject;
        }
    }

    public Toggle LayerToggle;
    [SerializeField] protected Text layerName;

    PanelLayersView panelLayersView;

    public override void StartInit() {
        panelLayersView = ModuleContainer.I.ViewsController.PanelLayersView;

        ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnSomeLayerSelect;
    }

    public override void Init(object model = null) {
        lockEdit = true;

        this.model = model;

        BaseLayer layer = model as BaseLayer;

        if(model != null) {
            layerName.text = layer.Name;
            LayerGameObject.transform.position = (model as BaseLayer).Position.GetVector2();
        }

        lockEdit = false;
    }

    public void ReInit() {
        if (model != null) {
            Init(model);
        }
    }

    public void ToggleValueChanged() {
        ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelected(this);
    }

    void Select() {
        GetComponent<Image>().color = ModuleContainer.I.ViewsController.PanelLayersView.GetSelectionColor(true);
    }

    void Unselect() {
        GetComponent<Image>().color = ModuleContainer.I.ViewsController.PanelLayersView.GetSelectionColor(false);
    }

    public virtual void OnSomeLayerSelect(BaseElementLayerView layer) {
        if(layer == this) {
            Select();
        } else {
            Unselect();
        }
    }

    public virtual void OnLayerInOrderChanged() { }

    public abstract void UpdateVisibleState(); // realise this method in others child this class
    public abstract void Use(InputData inputData);

    public virtual void OnDestroy() {
        if(panelLayersView.gameObject != null) {
            panelLayersView.OnLayerSelectedEvent -= OnSomeLayerSelect;
        }

        ClearLayer();
    }

    public virtual void ClearLayer() {
        if(layerGameObject != null) {
            DestroyObject(layerGameObject);
        }
    }
}
