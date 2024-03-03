using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	public enum UnlockTypes {
		Disabled,
		Enabled,
		Passed,
	}

	public UnlockTypes UnlockType;
	public int CountStars;

	public int NumberLevel;
	[SerializeField] private Transform[] bases;
	[SerializeField] private Image[] stars;
	[SerializeField] private Image pin;
	[SerializeField] private Image pinLight;
	[SerializeField] private Transform wing;
	[SerializeField] private Transform flag;

	public event Action<UILevel> OnClick = delegate { };

    public Image[] Stars {
        get {
            return stars;
        }
    }

	private void Awake() {
		pinLight.gameObject.SetActive(false);
	}

	public void Init() {
		GetComponent<Button>().onClick.AddListener(Click);
	}

	public void SetUnlockType(UnlockTypes type, int countStars) {
		UnlockType = type;
		CountStars = countStars;

		flag.gameObject.SetActive(type != UnlockTypes.Disabled);
		pinLight.gameObject.SetActive(type != UnlockTypes.Disabled);
		wing.gameObject.SetActive(type == UnlockTypes.Passed && countStars == 3);

		int baseNeed = countStars == 3 ? 1 : 0;

		for (int i = 0; i < bases.Length; i++) {
			bases[i].gameObject.SetActive(i == baseNeed);
		}

		for (int i = 0; i < stars.Length; i++) {
			pinLight.gameObject.SetActive(false);
			stars[i].gameObject.SetActive(i + 1 <= countStars);
		}

		foreach (Transform t in transform) {
			if (t != flag) {
				if (t.GetComponent<Image>()) t.GetComponent<Image>().raycastTarget = true;
			}
		}

		switch (type) {
			case UnlockTypes.Disabled:
				foreach (Transform t in transform) {
					if (t != flag) {
						if (t.GetComponent<Image>()) t.GetComponent<Image>().raycastTarget = false;
					}
				}

				GetComponent<Button>().interactable = false;

				pin.sprite = MapManager.I.PinGraySprite;
				break;
			case UnlockTypes.Enabled:
				GetComponent<Button>().interactable = true;

				pin.sprite = MapManager.I.PinBlueSprite;
				pinLight.sprite = MapManager.I.PinLightBlueSprite;
				break;
			case UnlockTypes.Passed:
				GetComponent<Button>().interactable = true;

				if (countStars == 3) {
//					pin.sprite = MapManager.I.PinRedSprite;
//					pinLight.sprite = MapManager.I.PinLightRedSprite;

					pin.sprite = MapManager.I.PinGreenSprite;
					pinLight.sprite = MapManager.I.PinLightGreenSprite;

				} else {
					pin.sprite = MapManager.I.PinGreenSprite;
					pinLight.sprite = MapManager.I.PinLightGreenSprite;
				}
				break;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		switch (UnlockType) {
			case UnlockTypes.Enabled:
				pin.sprite = MapManager.I.PinBlueASprite;
				break;
			case UnlockTypes.Passed:
				if (CountStars == 3) {
//					pin.sprite = MapManager.I.PinRedASprite;

					pin.sprite = MapManager.I.PinGreenASprite;
				} else {
					pin.sprite = MapManager.I.PinGreenASprite;
				}
				break;
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		switch (UnlockType) {
			case UnlockTypes.Enabled:
				pin.sprite = MapManager.I.PinBlueSprite;
				break;
			case UnlockTypes.Passed:
				if (CountStars == 3) {
//					pin.sprite = MapManager.I.PinRedSprite;

					pin.sprite = MapManager.I.PinGreenSprite;
				} else {
					pin.sprite = MapManager.I.PinGreenSprite;
				}
				break;
		}
		pinLight.gameObject.SetActive(false);
	}

	private void Click() {
		OnClick.Invoke(this);
	}

	public void OnPointerDown(PointerEventData eventData) {
		pinLight.gameObject.SetActive(true);
	}

	public void OnPointerUp(PointerEventData eventData) {
		pinLight.gameObject.SetActive(false);
	}
}
