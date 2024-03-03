using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementDecorLayerView : DynamicElementLayerView {
	DecorVisualizer decorVisualizer;

	public DecorContainer DecorContainer {
		get {
			return (model as DecorLayer).DecorContainer;
		}
		set { (model as DecorLayer).DecorContainer = value; }
	}

	//dynamic use variables
	Decor draggingDecor = null;
	Vector2 offsetDragging = Vector2.zero;
	Transform draggingTransform;

	RaycastHit2D hitInfo;
	Decor removedDecor;
	DecorInfo decorInfo;

	public override void StartInit () {
		base.StartInit ();

		if (model != null && DecorContainer == null) {
			DecorContainer = new DecorContainer ();
		}

		decorVisualizer = LayerGameObject.AddComponent<DecorVisualizer> ();
		decorVisualizer.Init (this);
	}

	IEnumerator DelayDraw () {
		if (decorVisualizer == null)
			yield break;
		yield return decorVisualizer.LoadAllSprites ();
		decorVisualizer.Draw ();
	}

	public override object GetModel () {
		return new DecorLayer ((this.model as BaseLayer).Position, (this.model as BaseLayer).Name, DecorContainer);
	}

	public override void Init (object model = null) {
		lockEdit = true;

		base.Init (model);

		if (model != null) {
			ModuleContainer.I.StartCoroutine (DelayDraw ());

			if (DecorContainer == null) {
				DecorContainer = new DecorContainer ();
			}
		}

		lockEdit = false;
	}

	public override void Use (InputData inputData) {

		if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {

			switch (ModuleContainer.I.ViewsController.PanelDecorToolsView.GetCurrentTool ()) {
				case PanelDecorToolsView.DecorTools.AddDecor:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down && !string.IsNullOrEmpty (ModuleContainer.I.ViewsController.PanelDecorToolsView.PathCurrentDecor)) {
						decorInfo = DecorContainer.AddDecor (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + -LayerGameObject.transform.position, ModuleContainer.I.ViewsController.PanelDecorToolsView.PathCurrentDecor);
						decorVisualizer.AddDecor (decorInfo.Decor, decorInfo.ID);
					}
					break;
				case PanelDecorToolsView.DecorTools.MoveDecor:

					if (draggingDecor != null && inputData.CurrentStateMouse == InputData.StateMouse.Up && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left) {
						draggingDecor = null;
						draggingTransform = null;
					}

					if (draggingDecor == null && !inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						hitInfo = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, 1000F, LayerMask.GetMask ("Decor"));

						if (hitInfo.transform != null) {
							offsetDragging = (Vector2)hitInfo.transform.localPosition - hitInfo.point;
							draggingDecor = DecorContainer.GetDecor (hitInfo.transform.GetComponent<ObjectID> ().ID);
							draggingTransform = hitInfo.transform;
						}
					} else if (draggingDecor != null && inputData.EventData.dragging && inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						draggingTransform.localPosition = (Vector2)ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (inputData.EventData.position) + offsetDragging;
						draggingDecor.Position = (Vector2)draggingTransform.localPosition;
						draggingTransform.localPosition = new Vector3 (draggingTransform.localPosition.x, draggingTransform.localPosition.y, 0F);
						decorVisualizer.SortSprite (draggingTransform.gameObject);
					}

					break;
				case PanelDecorToolsView.DecorTools.RemoveDecor:
					if (inputData.CurrentStateMouse == InputData.StateMouse.Down) {
						hitInfo = Physics2D.Raycast (ModuleContainer.I.CameraController.CameraCache.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, 1000F, LayerMask.GetMask ("Decor"));

						if (hitInfo.transform != null) {
							removedDecor = DecorContainer.RemoveDecor (hitInfo.transform.GetComponent<ObjectID> ().ID);
							if (removedDecor != null) {
								Destroy (hitInfo.transform.gameObject);
								decorVisualizer.RemoveDecor (hitInfo.transform.GetComponent<ObjectID> ().ID);
							}
						}
					}
					break;
			}
		} else if (inputData.EventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right && (inputData.CurrentStateMouse == InputData.StateMouse.Down)) {
			ModuleContainer.I.ViewsController.PanelDecorToolsView.SetNothingTool ();
		}
	}

	public override void OnSomeLayerSelect (BaseElementLayerView layer) {
		base.OnSomeLayerSelect (layer);

		if (layer == this) {
			decorVisualizer.Draw ();
		}
	}

	public override void UpdateVisibleState () {
		decorVisualizer.SetActive (LayerToggle.isOn);
	}

	public override void OnLayerInOrderChanged () {
		ModuleContainer.I.StartCoroutine (DelayDraw ());
	}
}
