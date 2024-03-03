using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Camera cameraCache;
    public Camera CameraCache {
        get {
            if(cameraCache == null) {
                cameraCache = GetComponent<Camera>();
            }
            return cameraCache;
        }
    }

    public float MaxOrthographicSize = 1620;
    [SerializeField] float MinOrthographicSize = 108;
    [SerializeField] float speedZooming = 1f;
    [SerializeField] Vector3 defaultPositonCamera = new Vector3(0, 0, -10);

    public void GlobalReset() {
        ResetZoom();
        ResetPosition();
    }

    public void ResetZoom() {
        CameraCache.orthographicSize = 540 * 0.5f;
    }

    public void ResetPosition() {
        transform.position = defaultPositonCamera;
    }

    public bool NeedInput(InputData inputData) {
        if(Input.GetKey(KeyCode.Space) && (inputData.CurrentStateMouse == InputData.StateMouse.Down || inputData.CurrentStateMouse == InputData.StateMouse.Up || inputData.CurrentStateMouse == InputData.StateMouse.Click || inputData.EventData.dragging)) {
            Move(-inputData.EventData.delta);
            return true;
        }else if(inputData.EventData.IsScrolling()) {
            Zoom(inputData.EventData.scrollDelta);
        }
        return false;
    }

    public void Move(Vector3 offset) { //move camera position
        transform.position += offset;
    }

    public void SetPosition(Vector3 offset) { //set camera positon
        transform.position = offset;
    }

    public void Zoom(Vector2 deltaScroll) {
        CameraCache.orthographicSize += -deltaScroll.y * speedZooming * Time.deltaTime;

        if(CameraCache.orthographicSize > MaxOrthographicSize) {
            CameraCache.orthographicSize = MaxOrthographicSize;
        } else if(CameraCache.orthographicSize < MinOrthographicSize) {
            CameraCache.orthographicSize = MinOrthographicSize;
        }

        ModuleContainer.I.ViewsController.PanelToolView.UpdateZoomView();
    }

    public void SetZoom(float value) {
        CameraCache.orthographicSize = value;

        if(CameraCache.orthographicSize > MaxOrthographicSize) {
            CameraCache.orthographicSize = MaxOrthographicSize;
        } else if(CameraCache.orthographicSize < MinOrthographicSize) {
            CameraCache.orthographicSize = MinOrthographicSize;
        }

        ModuleContainer.I.ViewsController.PanelToolView.UpdateZoomView();
    }

    public float GetPrecentZoom() {
        return CameraCache.orthographicSize / 540 * 100;
    }
}
