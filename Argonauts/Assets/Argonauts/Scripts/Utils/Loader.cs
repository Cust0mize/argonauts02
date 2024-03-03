using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {
	[SerializeField] Image loadBar;

	private IEnumerator Start() {
		Time.timeScale = 1.0f;
		loadBar.fillAmount = 0f;

		FadeManager.I.ForceFadeIn();
		yield return new WaitForSeconds(0.1f);
		yield return FadeManager.I.FadeOut(2f);
		yield return LoadAsyncScene();
	}

	IEnumerator LoadAsyncScene() {
		yield return null;

		AsyncOperation ao = SceneManager.LoadSceneAsync(TransitionManager.I.TargetScene, LoadSceneMode.Single);
		ao.allowSceneActivation = false;

		while (!ao.isDone) {
			float progress = Mathf.Clamp01(ao.progress / 0.9f);

			loadBar.fillAmount = progress;
			if (Mathf.Approximately(ao.progress, 0.9f)) {
				break;
			}

			yield return null;
		}

		GC.Collect();
		GC.Collect();
		Resources.UnloadUnusedAssets();
		yield return new WaitForSeconds(0.1f);
		yield return FadeManager.I.FadeIn(2f);
		ao.allowSceneActivation = true;
	}
}
