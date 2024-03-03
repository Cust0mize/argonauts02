using System;
using System.Collections;
using System.Collections.Generic;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameGUIManager : LocalSingletonBehaviour<GameGUIManager> {
	[SerializeField] PanelLevelState panelLevelState;
    [SerializeField] PanelScore panelScore;
	[SerializeField] PanelResources panelResources;
	[SerializeField] PanelPathBlocked panelPathBlocked;
	[SerializeField] PanelActionInfo panelActionInfo;
    [SerializeField] PanelNoResources panelNoResources;
	[SerializeField] StarsController starsController;
    [SerializeField] GameObject bonusePanel;
    [SerializeField] GameObject pauseButton;

	[SerializeField] BonusBar resourceBar;
	[SerializeField] BonusBar speedBar;
	[SerializeField] BonusBar workerBar;
	[SerializeField] BonusBar workBar;

    public BonusBar ResourceBar {
        get {
            return resourceBar;
        }
    }

    public BonusBar SpeedBar {
        get {
            return speedBar;
        }
    }

    public BonusBar WorkerBar {
        get {
            return workerBar;
        }
    }

    public BonusBar WorkBar {
        get {
            return workBar;
        }
    }

    [SerializeField] GameObject popupResourcePrefab;

    [SerializeField] GameObject popupScorePrefab;

    [SerializeField] TextMeshProUGUI startTipText;

    [SerializeField] ParticleSystem gameEndParticles;

    IEnumerator delayHideNoResourcesPanel;
    private bool _isEndLevel;
    private readonly Vector2 PANEL_OFFSET_REFERENCES_SCREEN = new Vector2(1600, 900);
        
	public override void DoAwake() {
        HideStartTipText();

		base.DoAwake();
		GameManager.I.OnTimeChanged += OnTimeChanged;
		OverseerController.I.OnNotEnoughtResources += OnNotEnoughtResources;
		GameManager.I.OnResourcesAdded += OnResourcesAdded;
		GameManager.I.OnResourcesTaked += OnResourcesTaked;
        GameManager.I.OnResourcesChanged += OnResourcesChanged;
		GameManager.I.InputUserHandler.OnPathBlockedEvent += OnPathBlocked;
		LevelTaskController.I.OnTaskUpdated += OnTaskUpdated;
		GameManager.I.OnLevelWasEnd += OnLevelWasEnd;
        GameManager.I.OnScoreChanged += OnScoreChanged;
	}

    public void ShowStartTipText () {
        startTipText.gameObject.SetActive(true);
    }

    public void HideStartTipText(){
        startTipText.gameObject.SetActive(false);
    }

	private void OnTimeChanged(float obj) {
		panelLevelState.UpdateTimeBar(obj, GameManager.I.LevelTime);
	}

	public void CalculateStarPositions(Level level) {
		panelLevelState.InitStars(level);
		starsController.InitTime(level);
	}

    private void OnNotEnoughtResources(List<Resource> resources, BaseInObject obj) {
        panelResources.CheckExistResource(resources);

        Vector2 pos = Camera.main.WorldToScreenPoint(obj.transform.position);

        panelNoResources.gameObject.SetActive(true);
        panelNoResources.Init(resources);
        panelNoResources.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);

        float factorX = Screen.width / PANEL_OFFSET_REFERENCES_SCREEN.x;
        float factorY = Screen.height / PANEL_OFFSET_REFERENCES_SCREEN.y;

        Vector2 startPos = pos;
        pos += obj.OffsetPanelsTop * factorY;

        bool topDecreased = false;

        if (pos.x >= Screen.width * 0.8F) {
            topDecreased = true;
            pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsLeft * factorX;
            panelNoResources.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
        } else if (pos.x <= Screen.width * 0.2F) {
            topDecreased = true;
            pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsRight * factorX;
            panelNoResources.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        }

        if (pos.y >= Screen.height * 0.85F) {
            if (!topDecreased)
                pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsBot * factorY;
            panelNoResources.GetComponent<RectTransform>().pivot = new Vector2(panelNoResources.GetComponent<RectTransform>().pivot.x, 1f);
        } else if (pos.y <= Screen.height * 0.1f) {
            if (topDecreased)
                pos += obj.OffsetPanelsTop * factorY;
            panelNoResources.GetComponent<RectTransform>().pivot = new Vector2(panelNoResources.GetComponent<RectTransform>().pivot.x, 0F);
        }

        panelNoResources.GetComponent<RectTransform>().position = pos;

        if (delayHideNoResourcesPanel != null) {
            StopCoroutine(delayHideNoResourcesPanel);
            delayHideNoResourcesPanel = null;
        }

        delayHideNoResourcesPanel = DelayHideNoResourcesPanel();
        StartCoroutine(delayHideNoResourcesPanel);

        HideActionInfo();
	}

    private IEnumerator DelayHideNoResourcesPanel () {
        yield return new WaitForSeconds(1.3F);
        HideNoResourcesPanel();
        delayHideNoResourcesPanel = null;
    }

    public void HideNoResourcesPanel () {
        if (panelNoResources)
            panelNoResources.gameObject.SetActive(false);
    }

	public void UpdateResourceBar(float value, float max, bool isFill) {
        if (isFill) resourceBar.UpdateFillBar(value, max);
        else resourceBar.UpdateSpendBar(value, max);
	}

    public void UpdateSpeedBar(float value, float max, bool isFill) {
        if (isFill) speedBar.UpdateFillBar(value, max);
        else speedBar.UpdateSpendBar(value, max);
	}

    public void UpdateWorkerBar(float value, float max, bool isFill) {
        if (isFill) workerBar.UpdateFillBar(value, max);
        else workerBar.UpdateSpendBar(value, max);
	}

    public void UpdateWorkBar(float value, float max, bool isFill) {
        if (isFill) workBar.UpdateFillBar(value, max);
        else workBar.UpdateSpendBar(value, max);
	}

	public void OnClickResourceBonusBar() {
		GameManager.I.TryActiveResourceBonus();
	}

	public void OnClickSpeedBonusBar() {
		GameManager.I.TryActiveSpeedBonus();
	}

	public void OnClickWorkerBonusBar() {
		GameManager.I.TryActiveWorkerBonus();
	}

	public void OnClickWorkBonusBar() {
		GameManager.I.TryActiveWorkBonus();
	}

	public void DoInit() {
		foreach (Task t in LevelTaskController.I.Tasks) {
			OnTaskUpdated(t);
		}
		if (!GameManager.I.ResourceBonusNeed) {
			resourceBar.gameObject.SetActive(false);
		}
		if (!GameManager.I.SpeedBonusNeed) {
			speedBar.gameObject.SetActive(false);
		}
		if (!GameManager.I.WorkerBonusNeed) {
			workerBar.gameObject.SetActive(false);
		}
		if (!GameManager.I.WorkBonusNeed) {
			workBar.gameObject.SetActive(false);
		}
	}

    void OnResourcesAdded(List<Resource> resources, bool action = false) {
        panelResources.UpdateData(GameResourceManager.I.Resources);

        if (action)
            ShowPopupResources(GameManager.I.CampObject.transform.position, new List<Resource>(resources));
	}

    void OnResourcesTaked(List<Resource> resources, BaseInObject baseObject = null) {
        panelResources.UpdateData(GameResourceManager.I.Resources);

        if(baseObject && !(baseObject is MarketPlaceObject))
            ShowPopupResources(baseObject.transform.position, new List<Resource>(resources));
	}

    void OnResourcesChanged (List<Resource> resources) {
        if(panelActionInfo.gameObject.activeSelf) {
            panelActionInfo.ReInit();
        }
    }

    void OnLevelWasEnd(bool win) {
        StartCoroutine(LevelEnd());
	}

    private IEnumerator LevelEnd() {
        _isEndLevel = true;
        StartCoroutine(IELevelFinishMusic());
        yield return 0;

        GameManager.I.AddTimeScore();
        yield return new WaitForSeconds(3f);

        HideRetractablePanels();
        yield return 0;

        gameEndParticles.Play();
        yield return new WaitForSeconds(2);

        if (ProjectSettings.I.isTrophiesEnabled)
            yield return AwardShower.I.DoShow();

        yield return 0;

        int s = 0;
        if (GameManager.I.PassedTime <= GameManager.I.LevelStar3) s = 3;
        else if (GameManager.I.PassedTime <= GameManager.I.LevelStar2) s = 2;
        else if (GameManager.I.PassedTime <= GameManager.I.LevelStar1) s = 1;

        yield return 0;

        UserData.I.SaveLevelDump(GameManager.I.LevelNumber, GameManager.I.PassedTime, true, s, GameManager.I.Scores);

        yield return 0;

        StartCoroutine(DelayOpenPanelFinish());
    }

    private IEnumerator IELevelFinishMusic() {
        AudioClip levelFinish = ResourceManager.GetMusic("level_finish");

        AudioWrapper.I.PlaySimpleMusic("level_finish", false);
        yield return new WaitForSeconds(0.1F);
        yield return AudioWrapper.I.WaitMusicPlaying(levelFinish);

        AudioWrapper.I.PlayLevelMusic();
    }

    private IEnumerator DelayOpenPanelFinish() {
        yield return new WaitForSeconds(.1f);

        LevelDumpData dump = UserData.I.GetLevelDump(GameManager.I.LevelNumber);

        float bestTime = dump.BestTime;
        int bestScore = dump.BestScore;
        if (dump == null || dump.Passed == false) {
            bestTime = GameManager.I.PassedTime;
            bestScore = GameManager.I.Scores;

            Debug.Log(bestScore);
        }

        int stars = 0;

        if (GameManager.I.PassedTime <= GameManager.I.LevelStar3) stars = 3;
        else if (GameManager.I.PassedTime <= GameManager.I.LevelStar2) stars = 2;
        else if (GameManager.I.PassedTime <= GameManager.I.LevelStar1) stars = 1;

        HGL_WindowManager.I.GetWindow("PanelFinish").GetComponent<PanelFinish>().Init(stars, GameManager.I.PassedTime, bestTime, GameManager.I.Scores, bestScore);
        HGL_WindowManager.I.OpenWindow(null, null, "PanelFinish", false, true);
    }

	void OnTaskUpdated(Task task) {
		panelLevelState.TaskUpdated();
	}

    void OnPathBlocked(BaseInObject obj) {
        HideActionInfo();
        HideNoResourcesPanel();

        Vector2 pos = Camera.main.WorldToScreenPoint(obj.transform.position);

        panelPathBlocked.Open();
        panelPathBlocked.GetComponent<RectTransform>().position = pos;
	}

	public void ShowActionInfo(string actionName, List<Resource> need, List<Resource> income, BaseInObject obj) {
        if (!obj.Available)
            return;
        if (!obj.Clickable)
            return;
        if (!obj.CanBeUsed())
            return;
        if (OverseerController.I.ObjectInOrder(obj)) 
            return;

        Vector2 pos = Camera.main.WorldToScreenPoint(obj.transform.position );

        panelActionInfo.gameObject.SetActive(true);
        panelActionInfo.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);

        float factorX = Screen.width / PANEL_OFFSET_REFERENCES_SCREEN.x;
        float factorY = Screen.height / PANEL_OFFSET_REFERENCES_SCREEN.y;

        pos += obj.OffsetPanelsTop * factorY;

        bool topDecreased = false;

        if (pos.x >= Screen.width * 0.8F) {
            topDecreased = true;
            pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsLeft * factorX;
            panelActionInfo.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
        } else if (pos.x <= Screen.width * 0.2F) {
            topDecreased = true;
            pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsRight * factorX;
            panelActionInfo.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        }

        if (pos.y >= Screen.height * 0.85F) {
            if (!topDecreased)
                pos -= obj.OffsetPanelsTop * factorY;
            pos += obj.OffsetPanelsBot * factorY;
            panelActionInfo.GetComponent<RectTransform>().pivot = new Vector2(panelActionInfo.GetComponent<RectTransform>().pivot.x, 1f);
        } else if (pos.y <= Screen.height * 0.15f) {
            if (topDecreased)
                pos += obj.OffsetPanelsTop * factorY;
            panelActionInfo.GetComponent<RectTransform>().pivot = new Vector2(panelActionInfo.GetComponent<RectTransform>().pivot.x, 0F);
        }

        panelActionInfo.transform.position = pos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelActionInfo.GetComponent<RectTransform>());
        panelActionInfo.Init(actionName, need, income);
	}

	public void OnObjectClickedMobile(string actionName, List<Resource> need, List<Resource> income, BaseInObject obj) {
        if (!_isEndLevel) {
            StopAllCoroutines();
        }
        ShowActionInfo(actionName, need, income, obj);
		StartCoroutine(OnObjectClickedDelay());
	}

	public void HideActionInfo() {
        if (panelActionInfo)
		    panelActionInfo.gameObject.SetActive(false);
	}

    public void ShowRetractablePanels() {
        pauseButton.GetComponent<TweenAnchoredPosition>().PlayForward();
        bonusePanel.GetComponent<TweenAnchoredPosition>().PlayForward();
        panelLevelState.GetComponent<TweenAnchoredPosition>().PlayForward();
        panelResources.GetComponent<TweenAnchoredPosition>().PlayForward();
        panelScore.GetComponent<TweenAnchoredPosition>().PlayForward();
    }

    public void HideRetractablePanels() {
        pauseButton.GetComponent<TweenAnchoredPosition>().PlayReverse();
        bonusePanel.GetComponent<TweenAnchoredPosition>().PlayReverse();
        panelLevelState.GetComponent<TweenAnchoredPosition>().PlayReverse();
        panelResources.GetComponent<TweenAnchoredPosition>().PlayReverse();
        panelScore.GetComponent<TweenAnchoredPosition>().PlayReverse();
    }

    public void ShowPopupResources (Vector3 pos, List<Resource> resourceDelta) {
        StartCoroutine(IEShowPopupResources(pos, resourceDelta));
    }

    public void ShowPopupScores(Vector3 pos, int value) {
        StartCoroutine(IEShowPopupScore(pos, value));
    }

    public void ShowEndPopupScores(Vector3 from, Vector3 to, int value, Action callback) {
        StartCoroutine(IEShowEndPopupScore(from, to, value, callback));
    }

    private void OnScoreChanged(int newScore) {
        panelScore.UpdateScores(newScore);
    }

	private IEnumerator OnObjectClickedDelay() {
		yield return new WaitForSeconds(2.5F);
		HideActionInfo();
	}

    private IEnumerator IEShowPopupResources (Vector3 pos, List<Resource> resourceDelta) {
        foreach (Resource r in resourceDelta) {
            if (r.Type == Resource.Types.Jaison || r.Type == Resource.Types.Medea || r.Type == Resource.Types.Worker) continue;

            GameObject pop = Instantiate(popupResourcePrefab, pos, Quaternion.identity, GameManager.I.Canvas2D.transform);
            pop.transform.position = pos;
            pop.GetComponent<PopupResourceElementGUI>().Init(r);
            pop.GetComponent<PopupResourceElementGUI>().DoPopUp(0.7F, 90f);
            yield return new WaitForSeconds(0.225f);
        }
    }

    private IEnumerator IEShowPopupScore(Vector3 pos, int score) {
        GameObject pop = Instantiate(popupScorePrefab, pos, Quaternion.identity, GameManager.I.Canvas2D.transform);
        pop.transform.position = pos;
        pop.GetComponent<PopupScoreElementGUI>().Init(score);
        pop.GetComponent<PopupScoreElementGUI>().DoPopUp(0.7F, 90f);
        yield return new WaitForSeconds(0.225f);
    }

    private IEnumerator IEShowEndPopupScore(Vector3 from, Vector3 to, int score, Action callback) {
        GameObject pop = Instantiate(popupScorePrefab, from, Quaternion.identity, GameManager.I.CanvasUI.transform);
        pop.transform.position = from;
        pop.GetComponent<PopupScoreElementGUI>().Init(score);
        pop.GetComponent<PopupScoreElementGUI>().DoPopUp(from, to, 1000F, callback);
        yield return new WaitForSeconds(0.225f);
    }


    //--------------------------------------------------------------------------------
    // game pause

    public void OnButtonPauseClick() {
        if (!GameManager.I.IsPause && !GameManager.I.IsFinishPanel) {
            HGL_WindowManager.I.OpenWindow(null, null, "PanelPause", false, true);
        }
    }

    private void Update() {
        #if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space)) {
            OnButtonPauseClick();
        }
        #endif
    }

    private void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            OnButtonPauseClick();
        }
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            OnButtonPauseClick();
        }
    }
}
