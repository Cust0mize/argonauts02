using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementObjectLayerView : ConstantElementLayerView {
	public ObjectVisualizer ObjectVisualizer;

	public LayerObjectContainer LayerObjectContainer {
		get {
			return (model as ObjectLayer).LayerObjectContainer;
		}
		set { (model as ObjectLayer).LayerObjectContainer = value; }
	}

	//dynamic use variables
	LayerObject draggingObject = null;
	Vector2 offsetDragging = Vector2.zero;
	Transform draggingTransform;
	float cacheZ;
	RaycastHit2D hitInfo;
	LayerObject removedObject;
	LayerObjectInfo addedObject;
	PathPoint lastDownPoint;
	GameObject lastDownPointGO;

	public override void StartInit () {
		base.StartInit ();

		if (model != null && LayerObjectContainer == null) {
			LayerObjectContainer = new LayerObjectContainer ();
		}

		ObjectVisualizer = LayerGameObject.AddComponent<ObjectVisualizer> ();
		ObjectVisualizer.Init (this);
	}

	IEnumerator DelayDraw () {
		if (ObjectVisualizer == null)
			yield break;
		yield return ObjectVisualizer.LoadAllSprites ();
		ObjectVisualizer.Draw ();
	}

	public override object GetModel () {
		return new ObjectLayer ((this.model as BaseLayer).Position, (this.model as BaseLayer).Name, LayerObjectContainer);
	}

	public override void Init (object model = null) {
		lockEdit = true;

		base.Init (model);

		if (this.model == null) {
			this.model = new ObjectLayer (Integer2.Zero, "Object Layer");
		}
		if (this.model != null && LayerObjectContainer == null) {
			LayerObjectContainer = new LayerObjectContainer ();
		}

		ModuleContainer.I.StartCoroutine (DelayDraw ());

		lockEdit = false;
	}

	public override void Use (InputData inputData) {

		if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {

			switch (ModuleContainer.I.ViewsController.PanelObjectToolsView.GetCurrentTool ()) {
				case PanelObjectToolsView.ObjectTools.AddObject:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down && !string.IsNullOrEmpty (ModuleContainer.I.ViewsController.PanelObjectToolsView.PathCurrentObject)) {
						addedObject = LayerObjectContainer.AddObject (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position, ModuleContainer.I.ViewsController.PanelObjectToolsView.PathCurrentObject);
						ObjectVisualizer.AddObject (addedObject.LayerObject, addedObject.ID);
					}
					break;
				case PanelObjectToolsView.ObjectTools.MoveObject:

					if (draggingObject != null && inputData.CurrentStateMouse == InputData.StateMouse.Up && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {
						draggingObject = null;
						draggingTransform = null;
					}

					if (draggingObject == null && !inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						hitInfo = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object"));

						if (hitInfo.transform != null) {
							offsetDragging = (Vector2)hitInfo.transform.localPosition - hitInfo.point;
							draggingObject = LayerObjectContainer.GetObject (hitInfo.transform.GetComponent<ObjectID> ().ID);
							draggingTransform = hitInfo.transform;
						}
					} else if (draggingObject != null && inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingObject.Position = (Vector2)ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -(Vector2)LayerGameObject.transform.position + offsetDragging;

						cacheZ = draggingTransform.localPosition.z;
						draggingTransform.localPosition = (Vector2)ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -(Vector2)LayerGameObject.transform.position + offsetDragging;
						draggingTransform.localPosition = new Vector3 (draggingTransform.localPosition.x, draggingTransform.localPosition.y, cacheZ);
						ObjectVisualizer.RedrawJoints (draggingTransform.GetComponent<LayerObjectID> ().JointIDs);

						foreach (string id in LayerObjectContainer.GetObject(draggingTransform.GetComponent<LayerObjectID> ().ID).JoinObjectsIDs) {
							ObjectVisualizer.RedrawJoints (ObjectVisualizer.Objects [id].GetComponent<LayerObjectID> ().JointIDs);
						}

						ObjectVisualizer.SortSprite (draggingTransform.gameObject);
					}

					break;
				case PanelObjectToolsView.ObjectTools.RemoveObject:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						hitInfo = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object"));

						if (hitInfo.transform != null) {
							removedObject = LayerObjectContainer.RemoveObject (hitInfo.transform.GetComponent<ObjectID> ().ID);
							if (removedObject != null) {
								ObjectVisualizer.RemoveJoints (removedObject.PointIDs);
								ObjectVisualizer.RemoveJoints (ObjectVisualizer.Objects [hitInfo.transform.GetComponent<ObjectID> ().ID].GetComponent<LayerObjectID> ().JointIDs);

								for (int i = 0; i < removedObject.JoinObjectsIDs.Count; i++) {
									Debug.Log (removedObject.JoinObjectsIDs [i]);

									ObjectVisualizer.RemoveJoints (ObjectVisualizer.Objects [removedObject.JoinObjectsIDs [i]].GetComponent<LayerObjectID> ().JointIDs);
									LayerObjectContainer.RemoveJointObjects (hitInfo.transform.GetComponent<ObjectID> ().ID, removedObject.JoinObjectsIDs [i]);
								}
									
								Destroy (hitInfo.transform.gameObject);
								ObjectVisualizer.RemoveObject (hitInfo.transform.GetComponent<ObjectID> ().ID);
							}
						}
					}
					break;
				case PanelObjectToolsView.ObjectTools.FlipObject:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						hitInfo = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition) + -LayerGameObject.transform.position, Vector2.zero, 1000F, LayerMask.GetMask ("Object"));

						if (hitInfo.transform != null) {
							LayerObjectContainer.FlipObject (hitInfo.transform.GetComponent<ObjectID> ().ID);
							ObjectVisualizer.FlipObject (hitInfo.transform.GetComponent<ObjectID> ().ID);
						}
					}
					break;
			}
		} else if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			ModuleContainer.I.ViewsController.PanelObjectToolsView.SetNothingTool ();
		}
	}

	public override void UpdateVisibleState () {
		ObjectVisualizer.SetActive (LayerToggle.isOn);
	}

	public override void OnLayerInOrderChanged () {
		ModuleContainer.I.StartCoroutine (DelayDraw ());
	}
}

public class LayerObjectInfo {
	public string ID;
	public LayerObject LayerObject;

	public LayerObjectInfo (string id, LayerObject layerObject) {
		ID = id;
		LayerObject = layerObject;
	}
}
