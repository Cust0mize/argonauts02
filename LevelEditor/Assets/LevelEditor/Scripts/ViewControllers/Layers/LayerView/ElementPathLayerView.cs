using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPathLayerView : ConstantElementLayerView {
	public PathVisualizer PathVisualizer;

	public PathGraph PathGraph {
		get {
			return (model as PathLayer).PathGraph;
		}
		set { (model as PathLayer).PathGraph = value; }
	}

	//dynamic use variables
	Vector2 lastDownPoint;
	GameObject draggingPointGO;
	public PathPoint draggingPoint = null;
	bool pointAdded = false;

	public override void StartInit () {
		base.StartInit ();

		if (model != null && PathGraph == null) {
			PathGraph = new PathGraph ();
		}

		PathVisualizer = LayerGameObject.AddComponent<PathVisualizer> ();
		PathVisualizer.Init (this);
	}

	public override void Init (object model = null) {
		lockEdit = true;

		base.Init (model);

		if (this.model == null) {
			this.model = new PathLayer (Integer2.Zero, "Path Layer");
		}
		if (this.model != null && PathGraph == null) {
			PathGraph = new PathGraph ();
		}

		if (PathVisualizer != null && this.model != null) {
			PathVisualizer.ClearVisualizer ();
			PathVisualizer.Draw ();
		}

		lockEdit = false;
	}

	public override object GetModel () {
		return model;
	}

	public override void OnSomeLayerSelect (BaseElementLayerView layer) {
		base.OnSomeLayerSelect (layer);

		if (layer == this) {
			PathVisualizer.Draw ();
		}
	}

	public override void UpdateVisibleState () {
		PathVisualizer.SetActive (LayerToggle.isOn);
		ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.SetActiveJoints (LayerToggle.isOn);
	}

	public override void OnLayerInOrderChanged () {
		if (PathVisualizer != null) {
			PathVisualizer.Draw ();
		}
	}

	public override void Use (InputData inputData) {
		Ray ray;
		RaycastHit hitInfo;
		RaycastHit2D hitInfo2D;

		if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {
			switch (ModuleContainer.I.ViewsController.PanelPathToolsView.GetCurrentTool ()) {
				case PanelPathToolsView.PathTools.AddPoint:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						if (!pointAdded) {
							lastDownPoint = ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position;

							ray = Camera.main.ScreenPointToRay (Input.mousePosition);

							if (Physics.Raycast (ray, out hitInfo, 1000F, LayerMask.GetMask ("Joint"))) {
								if (hitInfo.transform != null) {
									PathGraph.AddPointInJoint (hitInfo.transform.parent.GetComponent<ObjectID> ().ID, lastDownPoint);
									PathVisualizer.RemoveJoint (hitInfo.transform.parent.GetComponent<ObjectID> ().ID);
								} else {
									PathGraph.Add (lastDownPoint);
									pointAdded = true;
								}
							} else {
								PathGraph.Add (lastDownPoint);
								pointAdded = true;
							}
						}
					} else if (inputData.CurrentStateMouse == InputData.StateMouse.Up) {
						PathGraph.Join (lastDownPoint, ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);
						pointAdded = false;
					}

					PathVisualizer.Draw ();
					break;

				case PanelPathToolsView.PathTools.MovePoint:
					if (draggingPoint != null && inputData.CurrentStateMouse == InputData.StateMouse.Up && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {
						draggingPoint = null;
					}

					if (draggingPoint == null && !inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingPoint = PathGraph.GetPoint (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);
					} else if (draggingPoint != null && inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingPoint.Position = ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position;

						ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.RedrawJoint (draggingPoint.ID);
					}

					PathVisualizer.Draw ();
					break;
				case PanelPathToolsView.PathTools.RemovePoint:
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);

					if (Physics.Raycast (ray, out hitInfo, 1000F, LayerMask.GetMask ("Joint"))) {
						if (hitInfo.transform != null) {
							PathGraph.RemoveJoint (hitInfo.transform.parent.GetComponent<ObjectID> ().ID);
							PathVisualizer.RemoveJoint (hitInfo.transform.parent.GetComponent<ObjectID> ().ID);

							PathVisualizer.Draw ();
							break;
						}
					}

					PathPoint removedPoint = PathGraph.Remove (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);

					if (removedPoint != null) {
						ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.RemoveJoint (PathVisualizer.GetPoint (removedPoint.ID).GetComponent<PointID> ().JoinLayerObjectID, removedPoint.ID);
						ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.RemoveJoint (removedPoint.ID);

						PathVisualizer.RemovePoint (removedPoint.ID);
						PathVisualizer.RemoveJoints (GetIDsJoints (PathGraph.GetJoints (removedPoint)));
						PathGraph.RemoveJoints (PathGraph.GetJoints (removedPoint));
					}

					PathVisualizer.Draw ();
					break;
				case PanelPathToolsView.PathTools.AddJointObject:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						if (draggingPoint == null) {
							draggingPoint = PathGraph.GetPoint (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);

                            if (draggingPoint != null && ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.PointJoined(draggingPoint.ID)) {
                                draggingPoint = null;
                            }
                        }
					} else if (inputData.CurrentStateMouse == InputData.StateMouse.Up) {
						if (draggingPoint != null) {
							hitInfo2D = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object"));

							if (hitInfo2D.transform != null) {
								ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.AddJoint (hitInfo2D.transform.GetComponent<ObjectID> ().ID, draggingPoint.ID);

								if (ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.PointsObjects.ContainsKey (draggingPoint.ID)) {
									ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.PointsObjects [draggingPoint.ID].GetComponent<PointID> ().JoinLayerObjectID = hitInfo2D.transform.GetComponent<LayerObjectID> ().ID;
									ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.JoinObject (ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.PointsObjects [draggingPoint.ID], hitInfo2D.transform.gameObject, draggingPoint.ID);
								} else {
									ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.Draw ();
								}
							}
						}

						draggingPoint = null;
					}
					break;
				case PanelPathToolsView.PathTools.RemoveJointObject:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingPoint = ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathGraph.GetPoint (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);

						if (draggingPoint != null) {
							draggingPointGO = ModuleContainer.I.ViewsController.PanelLayersView.PathLayer.PathVisualizer.GetPoint (draggingPoint.ID);
							ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.RemoveJoint (draggingPoint.ID);
							ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.RemoveJoint (draggingPointGO.GetComponent<PointID> ().JoinLayerObjectID, draggingPoint.ID);
							draggingPointGO.GetComponent<PointID> ().JoinLayerObjectID = string.Empty;

							draggingPoint = null;
							draggingPointGO = null;
						}
					}
					break;
				case PanelPathToolsView.PathTools.MarkSpecialPoint:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingPoint = PathGraph.GetPoint (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position);

						if (draggingPoint != null) {
							string idObject = ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.GetObjectIDWithPoint (draggingPoint.ID);

							if (!string.IsNullOrEmpty (idObject)) {
								ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.ToggleSpecialPoint (idObject, draggingPoint.ID);

								PathVisualizer.Draw ();
							}
						}

						draggingPoint = null;
					}
					break;
				case PanelPathToolsView.PathTools.AddJointObject2:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						if (draggingPointGO == null) {
							draggingPointGO = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object")).transform.gameObject;
						}
					} else if (inputData.CurrentStateMouse == InputData.StateMouse.Up) {
						if (draggingPointGO != null) {
							hitInfo2D = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object"));

							if (hitInfo2D.transform == draggingPointGO.transform)
								return;

							if (hitInfo2D.transform != null) {
								ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.AddJointObjects (draggingPointGO.transform.GetComponent<ObjectID> ().ID, hitInfo2D.transform.GetComponent<ObjectID> ().ID);
								ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.Draw ();
							}
						}

						draggingPointGO = null;
					}
					break;
				case PanelPathToolsView.PathTools.RemoveJointObject2:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						if (draggingPointGO == null) {
							draggingPointGO = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object")).transform.gameObject;
                            if (draggingPointGO == null) return;

							string idDragGo = draggingPointGO.transform.GetComponent<ObjectID> ().ID;

							for (int i = 0; i < ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.GetObject (idDragGo).JoinObjectsIDs.Count; i++) {
                                ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.RemoveJoint(idDragGo, ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.GetObject(idDragGo).JoinObjectsIDs[i]);
                                ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.RemoveJointObjects (idDragGo, ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.LayerObjectContainer.GetObject (idDragGo).JoinObjectsIDs [i]);
                            }

							ModuleContainer.I.ViewsController.PanelLayersView.ObjectLayer.ObjectVisualizer.Draw ();

                            draggingPointGO = null;
						}
					}
					break;
			}
		} else if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			ModuleContainer.I.ViewsController.PanelPathToolsView.SetNothingTool ();
		}
	}

	string[] GetIDsJoints (PathJoint[] joints) {
		List<string> result = new List<string> ();
		foreach (PathJoint j in joints) {
			result.Add (j.ID);
		}
		return result.ToArray ();
	}

	public override void ClearLayer () {
		if (model != null) {
			PathGraph = new PathGraph ();
		}
		base.ClearLayer ();
	}
}
