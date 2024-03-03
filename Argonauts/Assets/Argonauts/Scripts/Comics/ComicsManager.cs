using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System;
using TMPro;

public class ComicsManager : MonoBehaviour {
    [SerializeField] private int numberComics;

    [SerializeField] private Button buttonNext;

    [SerializeField] private Image comicsImage;
    [SerializeField] private Image comicsImageNext;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI textNext;

    [SerializeField] private ComicsFrame[] frames;

    private int currentFrame = 0;
    private Action callback;


    private IEnumerator Start() {
        AnalyticsEvent.Custom("cutscene_start", new Dictionary<string, object> {
            { "cutscene_number", numberComics }
        });

        FadeManager.I.ForceFadeIn();

        callback = TransitionManager.I.ComicsCallback;
        TransitionManager.I.ComicsCallback = null;

        UserData.I.SetComicsStatus(numberComics, true);

        comicsImage.sprite = frames[currentFrame].Image;
        text.text = LocalizationManager.GetLocalizedString(frames[currentFrame].LocalizationKey);

        yield return new WaitForSeconds(0.1f);
        yield return FadeManager.I.FadeOut();
    }

    private void RenderFrame() {
        buttonNext.interactable = false;

        comicsImageNext.sprite = frames[currentFrame].Image;
        textNext.text = LocalizationManager.GetLocalizedString(frames[currentFrame].LocalizationKey);

        StartCoroutine(IERenderFrame());
    }

    private IEnumerator IERenderFrame() {
        comicsImageNext.GetComponent<TweenAlpha>().ResetToBeginning();
        textNext.GetComponent<TweenAlpha>().ResetToBeginning();
        comicsImage.GetComponent<TweenAlpha>().ResetToBeginning();
        text.GetComponent<TweenAlpha>().ResetToBeginning();

        comicsImageNext.GetComponent<TweenAlpha>().PlayForward();
        textNext.GetComponent<TweenAlpha>().PlayForward();
        comicsImage.GetComponent<TweenAlpha>().PlayForward();
        text.GetComponent<TweenAlpha>().PlayForward();

        yield return new WaitForSeconds(1f);

        comicsImage.sprite = comicsImageNext.sprite;
        text.text = textNext.text;

        comicsImageNext.GetComponent<CanvasGroup>().alpha = 0f;
        textNext.GetComponent<CanvasGroup>().alpha = 0f;

        comicsImage.GetComponent<CanvasGroup>().alpha = 1f;
        text.GetComponent<CanvasGroup>().alpha = 1f;

        buttonNext.interactable = true;
    }

    public void OnButtonExitClicked() {
        if (currentFrame + 1 >= frames.Length) {
            AnalyticsEvent.Custom("cutscene_finished", new Dictionary<string, object> {
                { "cutscene_number", numberComics }
            });

            if (!UserData.I.GetComicsReadStatus(numberComics)) {
                AwardHandler.I.UpdateAward("award_reader", 1);
                UserData.I.SaveComicsReadStatus(numberComics, true);
            }
        }
        else {
            AnalyticsEvent.Custom("cutscene_skip", new Dictionary<string, object> {
                { "cutscene_number", numberComics }
            });
        }

        callback.Invoke();
    }

    public void OnButtonNextClicked() {
        if (currentFrame + 1 >= frames.Length) {
            AnalyticsEvent.Custom("cutscene_finished", new Dictionary<string, object> {
                { "cutscene_number", numberComics }
            });

            if (!UserData.I.GetComicsReadStatus(numberComics)) {
                AwardHandler.I.UpdateAward("award_reader", 1);
                UserData.I.SaveComicsReadStatus(numberComics, true);
            }

            callback.Invoke();
        } else {
            currentFrame++;
            RenderFrame();
        }
    }
}

[Serializable]
public class ComicsFrame {
    public Sprite Image;
    public string LocalizationKey;
}
