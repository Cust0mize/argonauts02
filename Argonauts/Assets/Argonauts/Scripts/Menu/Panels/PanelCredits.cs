using System.Collections;
using System.Collections.Generic;
using HGL;
using UnityEngine;

public class PanelCredits : MonoBehaviour {
	public void Close() {
		HGL_WindowManager.I.CloseWindow(null, null, "PanelCredits", false, false);
	}
}
