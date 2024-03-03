using UnityEngine;
using System;
using System.Collections.Generic;

public class TutorialManager : LocalSingletonBehaviour<TutorialManager> {
    private const string TUTORIAL_FORMAT = "Tutorial_{0}";

    private ITutorial currentTutorial;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject arrowPrefabUI;
    [SerializeField] private Transform arrowParent;
    [SerializeField] private Transform arrowParentUI;
    private List<GameObject> arrows = new List<GameObject>();
    private List<GameObject> arrowsUI = new List<GameObject>();

    public void LoadTutorial(int levelNumber) {
        LoadTutorial(string.Format("{0:00}", levelNumber));
    }

    public void LoadTutorial(string postfix) {
        GameObject tutorial = ResourceManager.I.GetTutorial(string.Format(TUTORIAL_FORMAT, postfix));
        if (tutorial == null) return;
        GameObject tutorialInstance = Instantiate(tutorial, transform);
        currentTutorial = tutorialInstance.GetComponent<ITutorial>();
        if (currentTutorial == null) throw new NullReferenceException("currentTutorial");
        currentTutorial.Launch();
    }

    public RectTransform GetFreeArrow() {
        GameObject arrow = null;
        foreach (GameObject a in arrows) {
            if (!a.activeSelf) {
                arrow = a;
                break;
            }
        }

        if (arrow == null) {
            arrow = Instantiate(arrowPrefab, arrowParent).gameObject;
            arrow.gameObject.SetActive(false);
            arrows.Add(arrow);
        }

        return arrow.GetComponent<RectTransform>();
    }

    public RectTransform GetFreeArrowUI() {
        GameObject arrow = null;
        foreach (GameObject a in arrowsUI) {
            if (!a.activeSelf) {
                arrow = a;
                break;
            }
        }

        if (arrow == null) {
            arrow = Instantiate(arrowPrefabUI, arrowParentUI).gameObject;
            arrow.gameObject.SetActive(false);
            arrowsUI.Add(arrow);
        }

        return arrow.GetComponent<RectTransform>();
    }

    public void HideAllArrow() {
        foreach (GameObject arrow in arrows) {
            arrow.gameObject.SetActive(false);
        }
        foreach (GameObject arrow in arrowsUI) {
            arrow.gameObject.SetActive(false);
        }
    }
}

public enum ArrowSide {
    Top,
    Right,
    Bot,
    Left,
    RightTop,
    RightBot,
    LeftTop,
    LeftBot
}
