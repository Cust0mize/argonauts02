using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour {
	public bool NeedCheck = false;
	public CanvasGroup TargetCanvasGroup;

	private void Update() {
		if (NeedCheck && TargetCanvasGroup) {
			GetComponent<CanvasGroup>().alpha = TargetCanvasGroup.alpha;
		}
	}
}
