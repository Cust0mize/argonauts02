using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRepeater : MonoBehaviour {
    Camera currentCamera;
    Camera CurrentCamera {
        get {
            if(currentCamera == null) {
                currentCamera = GetComponent<Camera>();
            }
            return currentCamera;
        }
    }

    [SerializeField] Camera targetCamera;

    void Update() {
        CurrentCamera.transform.position = targetCamera.transform.position;
        CurrentCamera.orthographicSize = targetCamera.orthographicSize;
        CurrentCamera.orthographic = targetCamera.orthographic;
        CurrentCamera.transform.rotation = targetCamera.transform.rotation;
    }
}
