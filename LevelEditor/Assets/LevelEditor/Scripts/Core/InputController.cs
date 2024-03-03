using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IEndDragHandler, IBeginDragHandler {
    [SerializeField] KeyCode[] layerSupportedKeys;

    List<InputData> readyDatas = new List<InputData>();
    List<KeyCode> pressedKeys = new List<KeyCode>();

    void Update() {
        if(readyDatas != null && readyDatas.Count > 0) {

            if(!CheckCameraCustomInput()) {
                LayerInput();
            }

            readyDatas.RemoveAt(0);
            pressedKeys.Clear();
        }
    }

    bool CheckCameraCustomInput() {
        return ModuleContainer.I.CameraController.NeedInput(readyDatas[0]);
    }

    void LayerInput() {
        foreach(KeyCode k in layerSupportedKeys) {
            if(Input.GetKey(k)) {
                pressedKeys.Add(k);
            }
        }
        readyDatas[0].PressedKeys = pressedKeys.ToArray();

        ModuleContainer.I.ViewsController.PanelLayersView.Use(readyDatas[0]);
    }

    public void OnPointerClick(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Click;
        readyDatas.Add(i);
    }

    public void OnPointerDown(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Down;
        readyDatas.Add(i);
    }

    public void OnPointerUp(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Up;
        readyDatas.Add(i);
    }

    public void OnDrag(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Down;
        i.CurrentStateDragging = InputData.StateDragging.Drag;
        readyDatas.Add(i);
    }

    public void OnScroll(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        readyDatas.Add(i);
    }

    public void OnEndDrag(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Up;
        i.CurrentStateDragging = InputData.StateDragging.End;
        readyDatas.Add(i);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        InputData i = new InputData(eventData);
        i.CurrentStateMouse = InputData.StateMouse.Down;
        i.CurrentStateDragging = InputData.StateDragging.Begin;
        readyDatas.Add(i);
    }
}

public class InputData {
    public enum StateMouse { None, Up, Down, Click }
    public enum StateDragging { None, Begin, End, Drag }
    public StateMouse CurrentStateMouse;
    public StateDragging CurrentStateDragging;
    public PointerEventData EventData;
    public KeyCode[] PressedKeys;

    public InputData() {
        CurrentStateMouse = StateMouse.None;
        CurrentStateDragging = StateDragging.None;
    }
    public InputData(PointerEventData eventData) {
        CurrentStateMouse = StateMouse.None;
        CurrentStateDragging = StateDragging.None;

        EventData = eventData;
    }
    public InputData(PointerEventData eventData, KeyCode[] pressedKeys) {
        CurrentStateMouse = StateMouse.None;
        CurrentStateDragging = StateDragging.None;

        EventData = eventData;
        PressedKeys = pressedKeys;
    }
}
