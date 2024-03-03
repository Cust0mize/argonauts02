using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelPathToolsView : BaseViewController {
	ToggleGroup toggleGroup;

	ToggleGroup ToggleGroup {
		get {
			if (toggleGroup == null) {
				toggleGroup = GetComponent<ToggleGroup> ();
			}
			return toggleGroup;
		}
	}

	[SerializeField]
	Toggle nothingToggle;
	[SerializeField]
	Toggle addPointToggle;
	[SerializeField]
	Toggle movePointToggle;
	[SerializeField]
	Toggle removePointToggle;
	[SerializeField]
	Toggle addJointObjectToggle;
	[SerializeField]
	Toggle removeJointObjectToggle;
	[SerializeField]
	Toggle markSpecialPoint;
	[SerializeField]
	Toggle addJointObject2Toggle;
	[SerializeField]
	Toggle removeJointObject2Toggle;

	public enum PathTools {
		Nothing,
		AddPoint,
		MovePoint,
		RemovePoint,
		AddJointObject,
		RemoveJointObject,
		MarkSpecialPoint,
		AddJointObject2,
		RemoveJointObject2
	}

	public override void StartInit () {
		Hide ();

		ModuleContainer.I.ViewsController.PanelLayersView.OnLayerSelectedEvent += OnLayerSelected;

		nothingToggle.isOn = true;
	}

	void OnLayerSelected (BaseElementLayerView layer) {
		if (layer != null && (layer.GetModel () as BaseLayer).LayerType == BaseLayer.LayerTypes.Path) {
			Show ();
		} else {
			Hide ();
		}
	}

	public PathTools GetCurrentTool () {
		List<Toggle> list = ToggleGroup.ActiveToggles ().ToList ();

		if (list.Count == 0)
			return PathTools.Nothing;

		if (list [0] == nothingToggle) {
			return PathTools.Nothing;
		} else if (list [0] == addPointToggle) {
			return PathTools.AddPoint;
		} else if (list [0] == movePointToggle) {
			return PathTools.MovePoint;
		} else if (list [0] == removePointToggle) {
			return PathTools.RemovePoint;
		} else if (list [0] == addJointObjectToggle) {
			return PathTools.AddJointObject;
		} else if (list [0] == removeJointObjectToggle) {
			return PathTools.RemoveJointObject;
		} else if (list [0] == markSpecialPoint) {
			return PathTools.MarkSpecialPoint;
		} else if (list [0] == addJointObject2Toggle) {
			return PathTools.AddJointObject2;
		} else if (list [0] == removeJointObject2Toggle) {
			return PathTools.RemoveJointObject2;
		}

		return PathTools.Nothing;
	}

	public void SetNothingTool () {
		ToggleGroup.SetAllTogglesOff ();
		nothingToggle.isOn = true;
	}
}
