using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelLayersView : BaseViewController {
	#region view variables

	[SerializeField]
	Color colorSelectionLayer;
	[SerializeField]
	Color colorUnselectionLayer;

	[SerializeField]
	ConstantElementLayerView objectLayer;
	[SerializeField]
	ConstantElementLayerView pathLayer;
	[SerializeField]
	GameObject layersParent;

	[SerializeField]
	Button[] dynamicLayersButtons;
	[SerializeField]
	InputField nameInputField, xInputField, yInputField;

	[SerializeField]
	GameObject panelCreate;

	#endregion

	public ElementObjectLayerView ObjectLayer {
		get {
			return objectLayer as ElementObjectLayerView;
		}
	}

	public ElementPathLayerView PathLayer {
		get {
			return pathLayer as ElementPathLayerView;
		}
	}

	BaseElementLayerView selectedLayer;

	public BaseElementLayerView SelectedLayer {
		get {
			return selectedLayer;
		}
		set {
			selectedLayer = value;

			UpdateView ();

			if (OnLayerSelectedEvent != null) {
				OnLayerSelectedEvent.Invoke (selectedLayer);
			}
		}
	}

	public event Action<BaseElementLayerView> OnLayerActiveChangedEvent;
	public event Action<BaseElementLayerView> OnLayerSelectedEvent;

	public GameObject GetLayerPrefab (BaseLayer.LayerTypes layerType) {
		switch (layerType) {
			case BaseLayer.LayerTypes.Decor:
				return ModuleContainer.I.PrefabController.ElementDecorLayer;
			case BaseLayer.LayerTypes.Road:
				return ModuleContainer.I.PrefabController.ElementRoadLayer;
			case BaseLayer.LayerTypes.Tile:
				return ModuleContainer.I.PrefabController.ElementTileLayer;
		}
		return null;
	}

	public override void StartInit () {
		Hide ();

		objectLayer.LayerInOrder = 0;
		pathLayer.LayerInOrder = 1;

		pathLayer.Init (new PathLayer (Integer2.Zero, "Path Layer"));
		objectLayer.Init (new ObjectLayer (Integer2.Zero, "Object Layer"));

		pathLayer.StartInit ();
		objectLayer.StartInit ();
	}

	public override void Init (object model = null) {
		lockEdit = true;

		pathLayer.ClearLayer ();
		objectLayer.ClearLayer ();

		ClearDynamicLayers ();
		ResetDynamicLayersUI ();

		this.model = model;

		if (model == null)
			ModuleContainer.I.ViewsController.LevelController.New ();

		ModuleContainer.I.StartCoroutine (DelayInit (model as LayerContainer));

		lockEdit = false;
	}

	IEnumerator DelayInit (LayerContainer layers) {
		yield return new WaitForEndOfFrame ();

		pathLayer.StartInit ();
		objectLayer.StartInit ();

		if (layers != null) {
			objectLayer.Init (layers.Layers [0]);
			pathLayer.Init (layers.Layers [1]);

			int countLayers = layers.Layers.Length;
			for (int i = 2; i < countLayers; i++) {
				CreateLayer (layers.Layers [i]);
			}
		} else {
			pathLayer.Init ();
			objectLayer.Init ();
		}
	}

	public override object GetModel () {
		int countLayers = layersParent.transform.childCount + 2; // add 2 static layers

		LayerContainer lc = new LayerContainer ();
		lc.Layers = new BaseLayer[countLayers];

		lc.Layers [0] = objectLayer.GetModel () as ObjectLayer;
		lc.Layers [1] = pathLayer.GetModel () as PathLayer;

		for (int i = 2; i < countLayers; i++) {
			lc.Layers [i] = layersParent.transform.GetChild (i - 2).GetComponent<DynamicElementLayerView> ().GetModel () as BaseLayer;
		}

		return lc;
	}

	public void OnLayerSelected (BaseElementLayerView layer) {
		SelectedLayer = layer;
	}

	public void OnLayerSetActiveChanged (BaseElementLayerView layer) {
		layer.UpdateVisibleState ();

		if (OnLayerActiveChangedEvent != null) {
			OnLayerActiveChangedEvent.Invoke (layer);
		}
	}

	public void SetActiveDynamicLayersUI (bool value) {
		int countButtons = dynamicLayersButtons.Length;
		for (int i = 0; i < countButtons; i++) {
			dynamicLayersButtons [i].interactable = value;
		}

		nameInputField.interactable = value;
		xInputField.interactable = value;
		yInputField.interactable = value;
	}

    public void SetActiveOffsetUI(bool value) {
        xInputField.interactable = value;
        yInputField.interactable = value;
    }

	void ResetDynamicLayersUI () {
		nameInputField.text = "Layer name";
		xInputField.text = "0";
		yInputField.text = "0";
	}

	void UpdateView () {
		lockEdit = true;

		if (SelectedLayer == null) {
			SetActiveDynamicLayersUI (false);

			ResetDynamicLayersUI ();
		} else {

			if (SelectedLayer == objectLayer || SelectedLayer == pathLayer) {
				SetActiveDynamicLayersUI (false);
                SetActiveOffsetUI(true);
			} else {
				SetActiveDynamicLayersUI (true);
			}

			nameInputField.text = (SelectedLayer.GetModel () as BaseLayer).Name;
			xInputField.text = (SelectedLayer.GetModel () as BaseLayer).Position.X.ToString ();
			yInputField.text = (SelectedLayer.GetModel () as BaseLayer).Position.Y.ToString ();
		}

		lockEdit = false;
	}

	public Color GetSelectionColor (bool active) {
		if (active) {
			return colorSelectionLayer;
		} else {
			return colorUnselectionLayer;
		}
	}

	public void CreateLayer (BaseLayer bl) {
		BaseElementLayerView elementLayer = InstantiateLayer (bl.LayerType);

		elementLayer.StartInit ();
		elementLayer.Init (bl);
	}

	public void CreateLayer (BaseLayer.LayerTypes type) {
		BaseElementLayerView elementLayer = InstantiateLayer (type);

		BaseLayer layerModel = null;

		string defaultName = string.Format ("Dynamic Layer {0}", layersParent.transform.childCount);

		switch (type) {
			case BaseLayer.LayerTypes.Decor:
				layerModel = new DecorLayer (Integer2.Zero, defaultName);
				break;
			case BaseLayer.LayerTypes.Road:
				layerModel = new RoadLayer (Integer2.Zero, defaultName);
				break;
			case BaseLayer.LayerTypes.Tile:
				layerModel = new TileLayer (Integer2.Zero, defaultName);
				break;
		}

		elementLayer.StartInit ();
		elementLayer.Init (layerModel);

		SetActivePanelCreate ();
	}

	public void CreateLayer (int intType) {
		CreateLayer ((BaseLayer.LayerTypes)intType);
	}

	BaseElementLayerView InstantiateLayer (BaseLayer.LayerTypes layerType) {
		int layerInOrder = layersParent.transform.childCount;

		GameObject layer = Instantiate (GetLayerPrefab (layerType), layersParent.transform, false);

		layer.name = string.Format ("{0} layer {1}", layerType, layersParent.transform.childCount);

		BaseElementLayerView elementLayer = layer.GetComponent<BaseElementLayerView> ();

		UpdateLayerInOrder ();

		return elementLayer;
	}

	public void AddLayer () {
		SetActivePanelCreate ();
	}

	public void UpCurrentLayer () {
		if (SelectedLayer != null) {
			if (SelectedLayer.transform.GetSiblingIndex () - 1 >= 0) {
				SelectedLayer.transform.SetSiblingIndex (SelectedLayer.transform.GetSiblingIndex () - 1);
			}
		}
		UpdateLayerInOrder ();
	}

	public void DownCurrentLayer () {
		if (SelectedLayer != null) {
			if (SelectedLayer.transform.GetSiblingIndex () + 1 < transform.childCount) {
				SelectedLayer.transform.SetSiblingIndex (SelectedLayer.transform.GetSiblingIndex () + 1);
			}
		}
		UpdateLayerInOrder ();
	}

	public void DeleteCurrentLayer () {
		if (SelectedLayer != null) {
			SelectedLayer.OnDestroy ();
			Destroy (SelectedLayer.gameObject);
			SelectedLayer = null;
		}
	}

	void UpdateLayerInOrder () {
		int countChilds = layersParent.transform.childCount;

		objectLayer.LayerInOrder = countChilds;
		pathLayer.LayerInOrder = countChilds + 1;

		for (int i = 0; i < countChilds; i++) {
			layersParent.transform.GetChild (i).GetComponent<BaseElementLayerView> ().LayerInOrder = layersParent.transform.childCount - (1 + i);
		}
	}

	public void SetActivePanelCreate () {
		panelCreate.SetActive (!panelCreate.activeSelf);
	}

	void ClearDynamicLayers () {
		int countLayers = layersParent.transform.childCount;
		for (int i = 0; i < countLayers; i++) {
			layersParent.transform.GetChild (i).GetComponent<BaseElementLayerView> ().OnDestroy ();
			Destroy (layersParent.transform.GetChild (i).gameObject);
		}
	}

	public override void OnUpdateViewController () {
		if (lockEdit)
			return;

		if (SelectedLayer != null && (SelectedLayer as DynamicElementLayerView)) {
			(SelectedLayer.GetModel () as BaseLayer).Name = nameInputField.text;
			(SelectedLayer.GetModel () as BaseLayer).Position.X = GetInt (xInputField.text);
			(SelectedLayer.GetModel () as BaseLayer).Position.Y = GetInt (yInputField.text);

			(SelectedLayer as DynamicElementLayerView).ReInit ();
		}
	}

    public void UpdateOffset() {
        if (SelectedLayer.GetModel() is ObjectLayer) {
            foreach(string k in (SelectedLayer.GetModel() as ObjectLayer).LayerObjectContainer.Objects.Keys){
                (SelectedLayer.GetModel() as ObjectLayer).LayerObjectContainer.Objects[k].Position += new Vector2(GetInt(xInputField.text), GetInt(yInputField.text));
            }
            ObjectLayer.ObjectVisualizer.Draw();
        } else if (SelectedLayer.GetModel() is PathLayer) {
            foreach (PathPoint p in (SelectedLayer.GetModel() as PathLayer).PathGraph.Points) {
                p.Position += new Vector2(GetInt(xInputField.text), GetInt(yInputField.text));
            }
            PathLayer.PathVisualizer.Draw();
            ObjectLayer.ObjectVisualizer.Draw();

        } else if (SelectedLayer.GetModel() is RoadLayer) {
            foreach (string k in (SelectedLayer.GetModel() as RoadLayer).RoadContainer.Roads.Keys) {
                (SelectedLayer.GetModel() as RoadLayer).RoadContainer.Roads[k].Position += new Vector2(GetInt(xInputField.text), GetInt(yInputField.text));
            }
            (SelectedLayer as ElementRoadLayerView).RoadVisualizer.Draw();
        }

        xInputField.text = "0";
        yInputField.text = "0";
    }

	public void Use (InputData inputData) { //The main method of interaction with the layers
		if (SelectedLayer == null) {
			Debug.Log ("Please select any layer");
			return;
		}

		SelectedLayer.Use (inputData);
	}
}
