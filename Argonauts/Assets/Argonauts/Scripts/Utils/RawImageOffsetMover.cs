using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageOffsetMover : MonoBehaviour {
	[SerializeField] private float speedX;
	[SerializeField] private float speedY;
	[SerializeField] private float power;
	[SerializeField] private bool unscaled;

	private RawImage rawImage;
	private RawImage RawImage {
		get {
			if (rawImage == null) rawImage = GetComponent<RawImage>();
			return rawImage;
		}
	}

	private void FixedUpdate() {
		if (RawImage != null) {
			RawImage.uvRect = new Rect(
				RawImage.uvRect.x + speedX * power * (unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime),
				RawImage.uvRect.y + speedY * power * (unscaled ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime),
				RawImage.uvRect.width,
				RawImage.uvRect.height);
		}
	}
}
