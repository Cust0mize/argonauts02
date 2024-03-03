using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRaycastUtils : LocalSingletonBehaviour<UIRaycastUtils> {
	public bool Raycast(Vector2 pos, LayerMask layerMask, List<Canvas> canvases, bool overray = false) {
		List<GraphicRaycaster> graphicsRaycasters = new List<GraphicRaycaster>();

		foreach (Canvas c in canvases) {
			if (c.GetComponent<GraphicRaycaster>())
				graphicsRaycasters.Add(c.GetComponent<GraphicRaycaster>());
		}

		PointerEventData ped = new PointerEventData(null);
		ped.position = pos;
		List<RaycastResult> results = new List<RaycastResult>();

		foreach (GraphicRaycaster gr in graphicsRaycasters) {
			gr.Raycast(ped, results);
		}

		bool result = false;

		foreach (RaycastResult rr in results) {
			if (overray) {
				if (layerMask.IsInLayerMask(rr.gameObject.layer))
					return true;
			} else {
				if (layerMask.IsInLayerMask(rr.gameObject.layer))
					result = true;
				else return false;
			}
		}

		return result;
	}
}
