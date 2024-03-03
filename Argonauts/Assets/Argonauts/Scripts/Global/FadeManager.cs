using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : LocalSingletonBehaviour<FadeManager> {
	[SerializeField]
	Image fadeImage;

	public override void DoAwake () {
		if (fadeImage != null) {
			fadeImage.gameObject.SetActive(false);
		}
	}

	public IEnumerator FadeIn (float speed = 3F) {
		StopAllCoroutines();

		if (fadeImage != null) {
			fadeImage.gameObject.SetActive(true);
			fadeImage.color = Color.clear;
		}

		Color dColor = Color.clear;

		while (fadeImage.color.a < 1F) {
			dColor.a += speed * Time.unscaledDeltaTime;
			if (fadeImage != null) {
				fadeImage.color = dColor;
			} else {
				yield break;
			}
			yield return null;
		}

		if (fadeImage != null) {
			fadeImage.color = Color.black;
		}
	}

	public IEnumerator FadeOut (float speed = 3F) {
		StopAllCoroutines();

		if (fadeImage != null) {
			fadeImage.gameObject.SetActive(true);
			fadeImage.color = Color.black;
		}

		Color dColor = Color.black;

		while (fadeImage.color.a > 0F) {
			dColor.a -= speed * Time.unscaledDeltaTime;
			if (fadeImage != null) {
				fadeImage.color = dColor;
			} else {
				yield break;
			}
			yield return null;
		}

		if (fadeImage != null) {
			fadeImage.color = Color.clear;
			fadeImage.gameObject.SetActive(false);
		}
	}

	public void ForceFadeIn () {
		if (fadeImage != null) {
			fadeImage.gameObject.SetActive(true);
			fadeImage.color = Color.black;
		}
	}

	public void ForceFadeOut () {
		if (fadeImage != null) {
			fadeImage.gameObject.SetActive(true);
			fadeImage.color = Color.clear;
		}
	}
}
