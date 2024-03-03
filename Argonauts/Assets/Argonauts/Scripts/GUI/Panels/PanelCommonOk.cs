using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;

public class PanelCommonOk : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI messageText;

	public event Action OnOk = delegate { };

	public void Init(string message) {
		messageText.text = message;
	}

	public void Ok() {
		HGL_WindowManager.I.CloseWindow(null, OnOk, "PanelCommonOk", false);
	}
}
