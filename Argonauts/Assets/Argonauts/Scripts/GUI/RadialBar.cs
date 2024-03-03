using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialBar : MonoBehaviour {
	[SerializeField] Image fillImage;
	[SerializeField] Image iconImage;

	public void SetIcon (Sprite icon) {
		iconImage.sprite = icon;
	}

	public void UpdateBar (float value, float maxValue) {
		fillImage.fillAmount = value / maxValue;
	}
}
