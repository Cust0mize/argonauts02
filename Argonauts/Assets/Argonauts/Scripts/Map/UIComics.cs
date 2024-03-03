using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIComics : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	[SerializeField] private int numberComics;
    [SerializeField] private int minLevel;
	[SerializeField] private Image button;
	[SerializeField] private Image pinLight;

	[SerializeField] private Sprite buttonOff;
	[SerializeField] private Sprite buttonOn;

	private void Awake() {
		pinLight.gameObject.SetActive(false);
	}

	public void Init(int lastPassedLevel) {
        gameObject.SetActive(minLevel <= lastPassedLevel && UserData.I.ComicsIsShown(numberComics));
        GetComponent<Button>().onClick.AddListener(Click);
	}

	public void OnPointerDown(PointerEventData eventData) {
		pinLight.gameObject.SetActive(true);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		button.sprite = buttonOn;
	}

	public void OnPointerExit(PointerEventData eventData) {
		pinLight.gameObject.SetActive(false);
		button.sprite = buttonOff;
	}

	public void OnPointerUp(PointerEventData eventData) {
		pinLight.gameObject.SetActive(false);
	}

    private void Click()
    {
        if (MapManager.I.ScrollingEnable) {
            if (MapManager.I.ScrollingPanel.CanClickLevel) {
                TransitionManager.I.ComicsCallback = () => {
                    if (numberComics >= 7) TransitionManager.I.LoadMapExtra();
                    else TransitionManager.I.LoadMap();
                };
                TransitionManager.I.LoadComics(numberComics);
            }
        } else {
            TransitionManager.I.ComicsCallback = () => {
                if (numberComics >= 7) TransitionManager.I.LoadMapExtra();
                else TransitionManager.I.LoadMap();
            };
            TransitionManager.I.LoadComics(numberComics);
        }
    }
}
