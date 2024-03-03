using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudObject : MonoBehaviour {
	[SerializeField] private float speedFadeOut;
	[SerializeField] private float speedFadeIn;
	[SerializeField] private float minTimeIdle;
	[SerializeField] private float maxTimeIdle;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxAlpha = 0.8F;

	private Image image;
	private Image Image {
		get {
			if (image == null) image = GetComponent<Image>();
			return image;
		}
	}

	public event Action<CloudObject> OnFadeOut = delegate { };

	public void StartMove() {
		StopAllCoroutines();

		StartCoroutine(IEStartMove());
	}

	public void FadeIn() {
		StopAllCoroutines();

		StartCoroutine(IEFadeIn(speedFadeIn));
	}

	public void FadeOut() {
		StopAllCoroutines();

		StartCoroutine(IEFadeOut(speedFadeOut));
	}

	private IEnumerator IEStartMove() {
		Image.color = Color.clear;
		StartCoroutine(IEMove());
		yield return IEFadeIn(speedFadeOut);
		yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeIdle, maxTimeIdle));
		yield return IEFadeOut(speedFadeIn);
	}

	private IEnumerator IEMove() {
		while (true) {
			transform.localPosition += Vector3.right * moveSpeed * Time.fixedUnscaledDeltaTime;
			yield return null;
		}
	}

	private IEnumerator IEFadeIn(float speed = 3F) {
		Image.color = new Color(1f, 1f, 1f, 0f);

		Color dColor = new Color(1f, 1f, 1f, 0f);

		while (Image.color.a < maxAlpha) {
			dColor.a += speed * Time.fixedUnscaledDeltaTime;
			Image.color = dColor;
			yield return null;
		}

		Image.color = new Color(1f, 1f, 1f, maxAlpha);
	}

	private IEnumerator IEFadeOut(float speed = 3F) {
		Image.gameObject.SetActive(true);
		Image.color = new Color(1f, 1f, 1f, maxAlpha);

		Color dColor = new Color(1f, 1f, 1f, maxAlpha);

		while (Image.color.a > 0F) {
			dColor.a -= speed * Time.fixedUnscaledDeltaTime;
			Image.color = dColor;
			yield return null;
		}

		Image.color = new Color(1f, 1f, 1f, 0f);

		OnFadeOut.Invoke(this);
	}
}
