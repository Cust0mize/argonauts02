using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;

public abstract class BaseTutorial : MonoBehaviour, ITutorial {
    protected List<string> observedEvents = new List<string>();
    protected List<GameObject> usedArrows = new List<GameObject>();
    protected bool isOn;

    public bool IsOn {
        get {
            return isOn;
        }
    }

    public virtual void Launch() {
        isOn = true;
        Debug.LogFormat("{0} has been started!", gameObject.name);
    }

    public virtual void StartObserveEvent(string nameEvent) {
        if (!IsOn)
            return;
        NotificationCenter.DefaultCenter().AddObserver(this, nameEvent);
        observedEvents.Add(nameEvent);
    }

    public virtual void StopObserveEvent(string nameEvent) {
        NotificationCenter.DefaultCenter().RemoveObserver(this, nameEvent);
        observedEvents.Remove(nameEvent);
    }

    public virtual void StopAllObserveEvents() {
        foreach (string obsEvent in observedEvents) {
            StopObserveEvent(obsEvent);
        }
        observedEvents.RemoveRange(0, observedEvents.Count);
    }

    public virtual void StopTutorial() {
        isOn = false;
        StopAllObserveEvents();
        StopAllCoroutines();
        HideArrows();
        HideHint();

        Debug.LogFormat("{0} has been stoped!", gameObject.name);
    }

    public GameObject MoveArrow(Vector3 position, ArrowSide side, float distance = 20F, float moveDistance = 30F) {
        Vector3 targetPosition = GetArrowPosition(position, side, distance);
        Vector3 movePosition = GetArrowPosition(position, side, distance + moveDistance);

        RectTransform arrow = TutorialManager.I.GetFreeArrow();
        usedArrows.Add(arrow.gameObject);

        arrow.GetComponent<TweenPosition>().from = targetPosition;
        arrow.GetComponent<TweenPosition>().to = movePosition;

        arrow.GetComponent<TweenPosition>().ResetToBeginning();

        arrow.gameObject.SetActive(true);
        arrow.transform.eulerAngles = Vector3.forward * GetArrowAngle(side);
        arrow.GetComponent<TweenPosition>().PlayForward();

        return arrow.gameObject;
    }

    public GameObject MoveArrowUI(Vector3 position, Vector3 minAnchor, Vector2 maxAnchor, ArrowSide side, float distance = 20F, float moveDistance = 30F) {
        Vector3 targetPosition = GetArrowPosition(position, side, distance);
        Vector3 movePosition = GetArrowPosition(position, side, distance + moveDistance);

        RectTransform arrow = TutorialManager.I.GetFreeArrowUI();
        usedArrows.Add(arrow.gameObject);

        arrow.anchorMin = minAnchor;
        arrow.anchorMax = maxAnchor;
        arrow.GetComponent<TweenAnchoredPosition>().from = targetPosition;
        arrow.GetComponent<TweenAnchoredPosition>().to = movePosition;

        arrow.GetComponent<TweenAnchoredPosition>().ResetToBeginning();

        arrow.gameObject.SetActive(true);
        arrow.transform.eulerAngles = Vector3.forward * GetArrowAngle(side);
        arrow.GetComponent<TweenAnchoredPosition>().PlayForward();

        return arrow.gameObject;
    }

    public void HideLastArrow() {
        if (usedArrows.Count > 0) {
            HideArrow(usedArrows[usedArrows.Count - 1]);
        }
    }

    public void HideArrow(GameObject arrow) {
        arrow.gameObject.SetActive(false);
        usedArrows.Remove(arrow);
    }

    public void HideArrows() {
        TutorialManager.I.HideAllArrow();
        usedArrows.Clear();
    }

    private Vector3 GetArrowPosition(Vector3 position, ArrowSide side, float distance) {
        switch (side) {
            case ArrowSide.Top: return new Vector3(position.x, position.y + distance);
            case ArrowSide.Right: return new Vector3(position.x + distance, position.y);
            case ArrowSide.Bot: return new Vector3(position.x, position.y - distance);
            case ArrowSide.Left: return new Vector3(position.x - distance, position.y);
            case ArrowSide.RightTop: return new Vector3(position.x + distance, position.y + distance);
            case ArrowSide.RightBot: return new Vector3(position.x + distance, position.y - distance);
            case ArrowSide.LeftTop: return new Vector3(position.x - distance, position.y + distance);
            case ArrowSide.LeftBot: return new Vector3(position.x - distance, position.y - distance);
        }
        return position;
    }

    public float GetArrowAngle(ArrowSide side) {
        switch (side) {
            case ArrowSide.Top: return 180F;
            case ArrowSide.Right: return 90F;
            case ArrowSide.Bot: return 360F;
            case ArrowSide.Left: return 270F;
            case ArrowSide.RightTop: return 135F;
            case ArrowSide.RightBot: return 45F;
            case ArrowSide.LeftTop: return 225F;
            case ArrowSide.LeftBot: return 315F;
        }
        return 0F;
    }

    public void ShowHint(string text, Action onClose = null, bool timeClose = false, float durationShow = 1f, float posY = -90) {
        PanelHint panelHint = HGL_WindowManager.I.GetWindow("PanelHint").GetComponent<PanelHint>();
        panelHint.Init(text);
        panelHint.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posY);
        panelHint.GetComponent<RectTransform>().anchorMin = new Vector2(0.5F, 1f);
        panelHint.GetComponent<RectTransform>().anchorMax = new Vector2(0.5F, 1f);
        HGL_WindowManager.I.OpenWindow(null, null, "PanelHint", true, false);

        if (timeClose)
            StartCoroutine(DelayClose(onClose, durationShow));
    }

    public void ShowHint(string text, Vector2 anchorMin, Vector2 anchorMax, float posY = -90, Action onClose = null, bool timeClose = false, float durationShow = 1f) {
        PanelHint panelHint = HGL_WindowManager.I.GetWindow("PanelHint").GetComponent<PanelHint>();
        panelHint.Init(text);
        panelHint.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posY);
        panelHint.GetComponent<RectTransform>().anchorMin = anchorMin;
        panelHint.GetComponent<RectTransform>().anchorMax = anchorMax;
        HGL_WindowManager.I.OpenWindow(null, null, "PanelHint", true, false);

        if (timeClose)
            StartCoroutine(DelayClose(onClose, durationShow));
    }

    public void ShowHint(string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Action onClose = null, bool timeClose = false, float durationShow = 1f) {
        PanelHint panelHint = HGL_WindowManager.I.GetWindow("PanelHint").GetComponent<PanelHint>();
        panelHint.Init(text);
        panelHint.GetComponent<RectTransform>().anchoredPosition = pos;
        panelHint.GetComponent<RectTransform>().anchorMin = anchorMin;
        panelHint.GetComponent<RectTransform>().anchorMax = anchorMax;
        HGL_WindowManager.I.OpenWindow(null, null, "PanelHint", true, false);

        if (timeClose)
            StartCoroutine(DelayClose(onClose, durationShow));
    }

    public void HideHint() {
        HGL_WindowManager.I.CloseWindow(null, null, "PanelHint", true);
    }

    private IEnumerator DelayClose(Action onClose = null, float durationShow = 1f) {
        yield return new WaitForSeconds(durationShow);
        HGL_WindowManager.I.CloseWindow(null, onClose, "PanelHint", true);
    }
}
