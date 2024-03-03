using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollingPanel : MonoBehaviour {
	[SerializeField] private LayerMask mask;
	[SerializeField] private List<Canvas> canvases;
	[SerializeField] private float dragBorder = 10F;

	private Vector2 downPosition;
	private Vector3 downCameraPosition;
    private Vector3 pos;
	private bool isDrag;

	public bool CanClickLevel {
		get {
			return Mathf.Abs((Input.mousePosition - (Vector3)downPosition).x) <= dragBorder;
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown(0) && UIRaycastUtils.I.Raycast(Input.mousePosition, mask, canvases)) {
			isDrag = true;
			downPosition = Input.mousePosition;
			downCameraPosition = MapManager.I.CameraTransform.position;
		}

		if (Input.GetMouseButtonUp(0)) {
			isDrag = false;
		}

		if (isDrag) {
			pos = downCameraPosition - (Input.mousePosition - (Vector3)downPosition) * (transform.parent.GetComponent<CanvasScaler>().referenceResolution.y / Screen.height);
			MapManager.I.CameraTransform.position = new Vector3(
				Mathf.Clamp(pos.x, MapManager.I.MinCameraPosX, MapManager.I.MaxCameraPosX),
				0.0f, -10f
			);
            MapManager.I.SaveCameraPos();
		}
	}
}