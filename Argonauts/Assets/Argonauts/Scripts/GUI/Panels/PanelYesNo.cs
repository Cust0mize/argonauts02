using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;

public class PanelYesNo : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI message;
	public event Action<bool> OnResultClose = delegate { };

	public void Init(string message) {
		this.message.text = message;
	}

	public void Yes() {
		HGL_WindowManager.I.CloseWindow(null, () => { OnResultClose.Invoke(true); }, "PanelYesNo", false);
	}

	public void No() {
		HGL_WindowManager.I.CloseWindow(null, () => { OnResultClose.Invoke(false); }, "PanelYesNo", false);
	}
}
