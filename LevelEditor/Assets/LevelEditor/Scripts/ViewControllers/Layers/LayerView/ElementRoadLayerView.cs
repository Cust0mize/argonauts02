using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementRoadLayerView : DynamicElementLayerView {
	public RoadVisualizer RoadVisualizer;

	public RoadContainer RoadContainer {
		get {
			return (model as RoadLayer).RoadContainer;
		}
		set { (model as RoadLayer).RoadContainer = value; }
	}

	Vector2 lastDownPoint;
	RaycastHit hitInfo;
	Ray ray;
	bool downMouse = false;
	Road draggingRoad = null;
	Vector2 offsetDragging = Vector2.zero;
	Transform draggingTransform;

	const float FACTOR_ERROR_ADD_POINT = 1;

	public override void StartInit () {
		base.StartInit ();

		if (model != null && RoadContainer == null) {
			RoadContainer = new RoadContainer ();
		}

		RoadVisualizer = LayerGameObject.AddComponent<RoadVisualizer> ();
		RoadVisualizer.Init (this);
	}

	IEnumerator DelayDraw () {
		if (RoadVisualizer == null)
			yield break;
		yield return RoadVisualizer.LoadResources ();
		RoadVisualizer.Draw ();
	}

	public override object GetModel () {
		return new RoadLayer ((this.model as BaseLayer).Position, (this.model as BaseLayer).Name, RoadContainer);
	}

	public override void Init (object model = null) {
		lockEdit = true;

		base.Init (model);

		if (this.model == null) {
			this.model = new RoadLayer (Integer2.Zero, "Road Layer");
		}
		if (this.model != null && RoadContainer == null) {
			RoadContainer = new RoadContainer ();
		}

		ModuleContainer.I.StartCoroutine (DelayDraw ());

		lockEdit = false;
	}

	public override void UpdateVisibleState () {
		RoadVisualizer.SetActive (LayerToggle.isOn);
	}

	public override void OnLayerInOrderChanged () {
		if (RoadVisualizer != null) {
			RoadVisualizer.Draw ();
		}
	}

	public override void Use (InputData inputData) {
		if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {

			switch (ModuleContainer.I.ViewsController.PanelRoadToolsView.GetCurrentTool ()) {
				case PanelRoadToolsView.RoadTools.AddRoad:
					if (ModuleContainer.I.ViewsController.PanelRoadToolsView.TextureCurrentRoad == null)
						return;

					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						if (!downMouse) {
							lastDownPoint = ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position;
							RoadVisualizer.AddPointToLastRoad (lastDownPoint);

							downMouse = true;
						}

						if (Vector2.Distance (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, lastDownPoint) > ModuleContainer.I.ViewsController.PanelRoadToolsView.SizeBrush / FACTOR_ERROR_ADD_POINT) {
							lastDownPoint = ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position;

							RoadVisualizer.AddPointToLastRoad (lastDownPoint);
						}
					} else if (inputData.CurrentStateMouse == InputData.StateMouse.Up) {
						if (Vector2.Distance (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, lastDownPoint) > 10) {
							lastDownPoint = ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position;

							RoadVisualizer.AddPointToLastRoad (lastDownPoint);
						}
						RoadVisualizer.FinalizeLastRoad ();

						if (RoadVisualizer.LastRoadMesh != null && !RoadVisualizer.LastRoadMesh.Destroyed) {
							if (!RoadContainer.Roads.ContainsKey (RoadVisualizer.GetIDLastRoad ()))
								RoadContainer.Roads.Add (RoadVisualizer.GetIDLastRoad (), RoadVisualizer.GetLastRoad ());
							else
								RoadContainer.Roads [RoadVisualizer.GetIDLastRoad ()] = RoadVisualizer.GetLastRoad ();
						}

						RoadVisualizer.ClearLastRoad ();
						downMouse = false;
					}
					break;
				case PanelRoadToolsView.RoadTools.MoveRoad:

					if (draggingRoad != null && inputData.CurrentStateMouse == InputData.StateMouse.Up && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {
						draggingRoad = null;
						draggingTransform = null;
						offsetDragging = Vector2.zero;
					}

					if (draggingRoad == null && !inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						ray = Camera.main.ScreenPointToRay (Input.mousePosition);

						if (Physics.Raycast (ray, out hitInfo, 1000F, LayerMask.GetMask ("Road"))) {
							if (hitInfo.transform != null) {
								offsetDragging = hitInfo.transform.localPosition - hitInfo.point;
								draggingRoad = RoadContainer.GetRoad (hitInfo.transform.GetComponent<RoadMesh> ().ID);
								draggingTransform = hitInfo.transform;
							}
						}
					} else if (draggingRoad != null && inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingTransform.localPosition = (Vector2)ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + offsetDragging;
						draggingRoad.Position = draggingTransform.localPosition;
					}

					break;
				case PanelRoadToolsView.RoadTools.RemoveRoad:
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hitInfo, 1000F, LayerMask.GetMask ("Road"))) {
						if (hitInfo.transform != null) {
							RoadVisualizer.Roads.Remove (hitInfo.transform.GetComponent<RoadMesh> ().ID);

							if (RoadContainer.Roads.ContainsKey (hitInfo.transform.GetComponent<RoadMesh> ().ID)) {
								RoadContainer.Roads.Remove (hitInfo.transform.GetComponent<RoadMesh> ().ID);
							}

							Destroy (hitInfo.transform.gameObject);
						}
					}

					break;
			}
		} else if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			ModuleContainer.I.ViewsController.PanelRoadToolsView.SetNothingTool ();
		}
	}
}
