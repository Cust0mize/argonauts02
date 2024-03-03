using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class PanelFinish : MonoBehaviour {
    [SerializeField] GameObject timeBlock;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestScoreText;

    [SerializeField] GameObject[] stars;
    [SerializeField] GameObject[] explosions;

    private int countStars;


    public void Init (int countStars, float time, float bestTime, int score, int bestScore) {
        this.countStars = countStars;

        TimeSpan t = TimeSpan.FromSeconds(time);
        TimeSpan bt = TimeSpan.FromSeconds(bestTime);

        timeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        bestTimeText.text = string.Format("{0:D2}:{1:D2}", bt.Minutes, bt.Seconds);
        scoreText.text = score.ToString();
        bestScoreText.text = bestScore.ToString();
	}

    private void OnEnable () {
        timeBlock.SetActive(!UserData.I.GameMode.Equals(2));

        GameManager.I.IsFinishPanel = true;

        StartCoroutine(ShowStars());
        StartCoroutine(ShowExplosion());
    }

    private IEnumerator ShowStars () {
        yield return new WaitForSeconds(0.5F);

        for (int i = 0; i < countStars; i++) {
            this.stars[i].gameObject.SetActive(true);
            this.stars[i].gameObject.GetComponent<TweenScale>().enabled = true;
            this.stars[i].gameObject.GetComponent<TweenAnchoredPosition>().enabled = true;
            this.stars[i].gameObject.GetComponent<TweenAlpha>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ShowExplosion () {
        yield return new WaitForSeconds(0.5F);

        for (int i = 0; i < countStars; i++) {
            this.explosions[i].gameObject.SetActive(true);
            this.explosions[i].GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnButtonContinueClicked () {
        if (ProjectSettings.I.IsBFG_Survey && GameManager.I.LevelNumber.Equals(UserData.SURVEY_MAX_LEVELS)) {
            TransitionManager.I.MustOpenDemoWindow = true;
            TransitionManager.I.LoadMap();
            return;
        }

        GameManager.I.IsFinishPanel = false;

        if (TransitionManager.I.ComicsExits(GameManager.I.LevelNumber) &&
            !UserData.I.ComicsIsShown(TransitionManager.I.GetComicsNumber(GameManager.I.LevelNumber)))
        {
            if (GameManager.I.LevelNumber < 51) {
                TransitionManager.I.ComicsCallback = () => { TransitionManager.I.LoadMap(); };
            }
            else {
                TransitionManager.I.ComicsCallback = () => { TransitionManager.I.LoadMapExtra(); };
            }

            TransitionManager.I.LoadComics(TransitionManager.I.GetComicsNumber(GameManager.I.LevelNumber));
        } else {
            
            if (GameManager.I.LevelNumber < 51)
                TransitionManager.I.LoadMap();
            else
                TransitionManager.I.LoadMapExtra();
        }
    }

	public void OnButtonRestartClicked () {
        AnalyticsEvent.Custom("level_restart", new Dictionary<string, object> {
            { "level_number", GameManager.I.LevelNumber }
        });

		TransitionManager.I.LoadLevel(TransitionManager.I.TargetLevel);

        GameManager.I.IsFinishPanel = false;
	}
}
