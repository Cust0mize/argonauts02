using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConceptsManager : MonoBehaviour {
    [SerializeField] private UITweener[] tweeners;
     
	public RawImage picture = null;
	public GameObject btnNext = null;
	public GameObject btnPrev = null;

	private int pictureIndex = 0;
	private const int minPictureIndex = 0;
	private const int maxPictureIndex = 9;

    private IEnumerator Start() {
        picture.color = new Color(1, 1, 1, 0);
        ButtonsHide();
        StartCoroutine(IE_ShowPicture());

        FadeManager.I.ForceFadeIn();
        yield return new WaitForSeconds(0.1F);
        yield return FadeManager.I.FadeOut(2F);

        foreach (UITweener tw in tweeners) {
            tw.PlayForward();
        }
    }

	private void ButtonsShow() {
		btnPrev.SetActive(pictureIndex != minPictureIndex);
		btnNext.SetActive(pictureIndex != maxPictureIndex);
	}

	private void ButtonsHide() {
		btnPrev.SetActive(false);
		btnNext.SetActive(false);
	}

	private IEnumerator IE_ShowPicture() {

		Color currColor = picture.color;

		while (currColor.a > 0.0f) {
			currColor.a -= 5 * Time.deltaTime;
			currColor.a = Mathf.Max(currColor.a, 0.0f);
			picture.color = currColor;
			yield return 0;
		}

		string lang = ProjectSettings.I.language.ToLower ();
		if (!string.Equals(lang, "ru")) {
			lang = "en";
		}

		string filePath = "file://" + Application.dataPath + "/../EXTRAS/Concepts/" +
			lang + "_concepts_" + (pictureIndex + 1).ToString("00") + ".jpg";

		if (Application.platform == RuntimePlatform.OSXPlayer) {
			filePath = "file://" + Application.streamingAssetsPath + "/EXTRAS/Concepts/" +
				lang + "_concepts_" + (pictureIndex + 1).ToString("00") + ".jpg";
		}

		WWW www = new WWW (filePath);
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			picture.texture = www.texture;
		}

		while (currColor.a < 1.0f) {
			currColor.a += 5 * Time.deltaTime;
			currColor.a = Mathf.Min(currColor.a, 1.0f);
			picture.color = currColor;
			yield return 0;
		}

		ButtonsShow ();
	}


	public void OnBtn_Next() {
		StopAllCoroutines ();
		pictureIndex = Mathf.Clamp (pictureIndex + 1, minPictureIndex, maxPictureIndex);
		ButtonsHide ();
		StartCoroutine (IE_ShowPicture());
	}

	public void OnBtn_Prev() {
		StopAllCoroutines ();
		pictureIndex = Mathf.Clamp (pictureIndex - 1, minPictureIndex, maxPictureIndex);
		ButtonsHide ();
		StartCoroutine (IE_ShowPicture());
	}

	public void OnBtn_Back() {
        TransitionManager.I.LoadMenu ();
	}
}
