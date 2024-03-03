using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelTilePrefabsView : BaseViewController {
    [SerializeField] GameObject prefabsParent;

    [SerializeField] Color colorSelectionLayer;
    [SerializeField] Color colorUnselectionLayer;

    [SerializeField] InputField namePrefabInput;
    [SerializeField] Button buttonDeletePrefab;

    ElementPrefabView selectedPrefab;
    public ElementPrefabView SelectedPrefab {
        get {
            return selectedPrefab;
        }set {
            RemoveAllLayerSelection();
            selectedPrefab = value;
            if(selectedPrefab != null) {
                selectedPrefab.Select();
            }
            UpdateView();
        }
    }

    public override void Init(object model = null) {
        ClearPrefabs();

        SelectedPrefab = null;

        if(model != null) {
            PrefabContainer pc = model as PrefabContainer;

            if(pc.Prefabs == null) return;

            for(int i = 0; i < pc.Prefabs.Length; i++) {
                if(pc.Prefabs[i] == null) continue;

                ElementPrefabView prefab = InstantiatePrefab();
                prefab.Init(pc.Prefabs[i]);

                if(pc.LastSelectedPrefab.Equals(i)) {
                    SelectedPrefab = prefab;
                }

                ModuleContainer.I.SpriteController.AddSprites(pc.Prefabs[i].PrefabOptions.Values.ToArray());
            }
        }
    }

    public override void StartInit() {
        object model = null;

        if(!string.IsNullOrEmpty(ModuleContainer.I.ConfigController.JsonPrefabs)) {
            model = JsonConvert.DeserializeObject<PrefabContainer>(ModuleContainer.I.ConfigController.JsonPrefabs, ModuleContainer.I.ConfigController.GetJsonSettings());
        }

        Init(model);

        Hide();

        ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;
    }

    public override void OnUpdateViewController() {
        if(lockEdit) return;

        if(SelectedPrefab != null) {
            (SelectedPrefab.GetModel() as Prefab).Name = namePrefabInput.text;

            (SelectedPrefab as ElementPrefabView).ReInit();
        }
    }

    public override object GetModel() {
        int indexSelectedPrefab = GetIndexSelectedPrefab();
        int countPrefabs = prefabsParent.transform.childCount; 

        PrefabContainer pc = new PrefabContainer();
        pc.Prefabs = new Prefab[countPrefabs];
        pc.LastSelectedPrefab = indexSelectedPrefab;

        for(int i = 0; i < countPrefabs; i++) {
            pc.Prefabs[i] = prefabsParent.transform.GetChild(i).GetComponent<ElementPrefabView>().GetModel() as Prefab;
        }

        return pc;
    }

    void OnLayerSelected(BaseElementLayerView layer) {
        if(layer != null && (layer.GetModel() as BaseLayer).LayerType != BaseLayer.LayerTypes.Tile) {
            Hide();
        }
    }

    public void Add() {
        ElementPrefabView prefab = InstantiatePrefab();
        prefab.Init(new Prefab(string.Format("New prefab {0}", prefabsParent.transform.childCount), new Dictionary<string, string>(52)));
    }

    public void Delete() {
        if(SelectedPrefab != null) {
            Destroy(SelectedPrefab.gameObject);
            SelectedPrefab = null;
        }
    }

    void RemoveAllLayerSelection() {
        int countLayers = prefabsParent.transform.childCount;
        for(int i = 0; i < countLayers; i++) {
            prefabsParent.transform.GetChild(i).GetComponent<ElementPrefabView>().Unselect();
        }
    }

    void UpdateView() {
        lockEdit = true;

        if(SelectedPrefab == null) {
            namePrefabInput.text = "Prefab name";
            namePrefabInput.interactable = false;
            buttonDeletePrefab.interactable = false;
        }else {
            namePrefabInput.text = (SelectedPrefab.GetModel() as Prefab).Name;
            namePrefabInput.interactable = true;
            buttonDeletePrefab.interactable = true;
        }

        lockEdit = false;
    }

    public Color GetSelectionColor(bool active) {
        if(active) {
            return colorSelectionLayer;
        } else {
            return colorUnselectionLayer;
        }
    }

    ElementPrefabView InstantiatePrefab() {
        GameObject prefab = Instantiate(ModuleContainer.I.PrefabController.ElementPrefab, prefabsParent.transform, false);

        ElementPrefabView elementPrefab = prefab.GetComponent<ElementPrefabView>();

        return elementPrefab;
    }

    void ClearPrefabs() {
        int countLayers = prefabsParent.transform.childCount;
        for(int i = 0; i < countLayers; i++) {
            Destroy(prefabsParent.transform.GetChild(i).gameObject);
        }
    }

    int GetIndexSelectedPrefab() {
        int result = -1;
        int countPrefabs = prefabsParent.transform.childCount;

        if(SelectedPrefab == null) return result;

        for(int i = 0; i < countPrefabs; i++)
            if(SelectedPrefab.transform == prefabsParent.transform.GetChild(i)) return i;

        return result;
    }
}
