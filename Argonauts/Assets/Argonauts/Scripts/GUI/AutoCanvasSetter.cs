using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCanvasSetter : MonoBehaviour {
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private string layerName;

    private void Awake() {
        Canvas targetCanvas = transform.parent.GetComponent<Canvas>();
        Canvas canvas = GetComponent<Canvas>();

        canvas.overrideSorting = true; 
        canvas.worldCamera = targetCanvas.worldCamera;
        canvas.sortingLayerName = layerName;
        canvas.sortingOrder = sortingOrder;
    }
}
