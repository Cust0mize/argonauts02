using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour {
	[SerializeField] private float powerParallax;
	[SerializeField] private bool force;

	private const float GLOBAL_POWER_FACTOR = 0.35F;

	private Transform cameraTransform;
	private float lastCameraPositionX;
	private float startDelta;

    private bool inited = false;

    private void Start() {
		cameraTransform = Camera.main.transform;
		if (!force) {
			lastCameraPositionX = cameraTransform.transform.position.x;
		} else {
			startDelta = cameraTransform.transform.position.x - transform.position.x;
		}

        inited = true;
	}

	private void FixedUpdate() {
        if (!inited) return;
		if (force) {
			transform.position = new Vector3(cameraTransform.transform.position.x - startDelta, transform.position.y, transform.position.z);
		} else {
			float delta = cameraTransform.transform.position.x - lastCameraPositionX;
            transform.localPosition += Vector3.right * delta * (powerParallax * GLOBAL_POWER_FACTOR) * Time.fixedUnscaledDeltaTime;
			lastCameraPositionX = cameraTransform.transform.position.x;
		}
	}
}
