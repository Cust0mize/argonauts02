using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HGL;
using UnityEngine;

public class MapManager : LocalSingletonBehaviour<MapManager> {
	[SerializeField] private UITweener[] tweeners;

	[SerializeField] private Transform parentLevels;

	[SerializeField] private GameObject leftArrow;
	[SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject buttonTrophy;

	[SerializeField] private float speed;
	[SerializeField] private float minCameraPosX;
	[SerializeField] private float maxCameraPosX;

	[SerializeField] private Sprite base1Sprite;
	[SerializeField] private Sprite base2Sprite;

	[SerializeField] private Sprite pinRedSprite;
	[SerializeField] private Sprite pinGreenSprite;
	[SerializeField] private Sprite pinBlueSprite;
	[SerializeField] private Sprite pinGraySprite;

	[SerializeField] private Sprite pinRedASprite;
	[SerializeField] private Sprite pinGreenASprite;
	[SerializeField] private Sprite pinBlueASprite;

	[SerializeField] private Sprite pinLightRedSprite;
	[SerializeField] private Sprite pinLightGreenSprite;
	[SerializeField] private Sprite pinLightBlueSprite;

	[SerializeField] private ScrollingPanel scrollingPanel;

    [SerializeField] private UIComics[] comicsButtons;

    [SerializeField] private MapObject[] mapObjects;

    [SerializeField] private bool scrollingEnable = true;
    [SerializeField] public bool isExtra = false;
	[SerializeField] private GameObject unlockLevelsButtonIOS;


	public bool ScrollingEnable {
        get {
            return scrollingEnable;
        }
    }

    public ScrollingPanel ScrollingPanel {
        get {
            return scrollingPanel;
        }
    }

    public Sprite Base1Sprite {
		get {
			return base1Sprite;
		}
	}

	public Sprite Base2Sprite {
		get {
			return base2Sprite;
		}
	}

	public Sprite PinRedSprite {
		get {
			return pinRedSprite;
		}
	}

	public Sprite PinGreenSprite {
		get {
			return pinGreenSprite;
		}
	}

	public Sprite PinBlueSprite {
		get {
			return pinBlueSprite;
		}
	}

	public Sprite PinGraySprite {
		get {
			return pinGraySprite;
		}
	}

	public Sprite PinRedASprite {
		get {
			return pinRedASprite;
		}
	}

	public Sprite PinGreenASprite {
		get {
			return pinGreenASprite;
		}
	}

	public Sprite PinBlueASprite {
		get {
			return pinBlueASprite;
		}
	}

	public Sprite PinLightRedSprite {
		get {
			return pinLightRedSprite;
		}
	}

	public Sprite PinLightGreenSprite {
		get {
			return pinLightGreenSprite;
		}
	}

	public Sprite PinLightBlueSprite {
		get {
			return pinLightBlueSprite;
		}
	}

	private Transform cameraTransform;
	private List<UILevel> cachedLevels;

	public List<UILevel> Levels {
		get {
			if (cachedLevels == null) {
				cachedLevels = new List<UILevel>(60);
				foreach (Transform t in parentLevels) {
					cachedLevels.Add(t.GetComponent<UILevel>());
				}
			}
			return cachedLevels;
		}
	}

	public float MinCameraPosX {
		get {
			return minCameraPosX;
		}
		private set { minCameraPosX = value; }
	}

	public float MaxCameraPosX {
		get {
			return maxCameraPosX;
		}
		private set { maxCameraPosX = value; }
	}

	public Transform CameraTransform {
		get {
			if (cameraTransform == null)
				cameraTransform = Camera.main.transform;
			return cameraTransform;
		}
		private set { cameraTransform = value; }
	}

	public bool IsLeftScrolling { get; private set; }

	public bool IsRightScrolling { get; private set; }

    private static float LAST_CAMERA_POS = float.MinValue;
    private float SAVED_CAMERA_POS = float.MinValue;
    private int LAST_PASSED_LEVEL = 0;

    public override void DoAwake() {
        buttonTrophy.gameObject.SetActive(ProjectSettings.I.isTrophiesEnabled);

		unlockLevelsButtonIOS.SetActive(false);

#if UNITY_IOS
        unlockLevelsButtonIOS.SetActive(!UserData.I.IsLevelUnlocked(UserData.COUNT_LEVELS_IN_GAME));
#endif

		if (scrollingEnable) {
            LAST_CAMERA_POS = UserData.I.GetMapCameraPos();
            LAST_PASSED_LEVEL = UserData.I.GetMapLastPassedLevel();

            if (Mathf.Approximately(LAST_CAMERA_POS, float.MinValue)) {
                CameraTransform.position = new Vector3(MinCameraPosX, 0, -10);
            } else {
                CameraTransform.position = new Vector3(LAST_CAMERA_POS, 0, -10);
            }

            SAVED_CAMERA_POS = CameraTransform.position.x;
        }
    }

    public override void DoDestroy() {
        if (scrollingEnable) {
            UserData.I.SaveMapCameraPos(SAVED_CAMERA_POS);
        }
    }

    private void Update() {
        if (scrollingEnable) {
            if (CameraTransform.position.x <= minCameraPosX) {
                leftArrow.SetActive(false);
                IsLeftScrolling = false;
            } else {
                leftArrow.SetActive(true);
            }
            if (CameraTransform.position.x >= maxCameraPosX) {
                rightArrow.SetActive(false);
                IsRightScrolling = false;
            } else {
                rightArrow.SetActive(true);
            }
            if (IsLeftScrolling) {
                Left();
            }
            if (IsRightScrolling) {
                Right();
            }
        }
	}

	private async void Start() {
		FadeManager.I.ForceFadeIn();

		Level level = null;

        int lastPassedLevel = 0;

		for (int i = 0; i < Levels.Count; i++) {
            LevelDumpData dump = UserData.I.GetLevelDump(Levels[i].NumberLevel);
			level = await LevelParser.I.GetLevel(Levels[i].NumberLevel);

			Levels[i].Init();
			Levels[i].OnClick += MapManager_OnClick;

			if (level == null) {
				Levels[i].SetUnlockType(UILevel.UnlockTypes.Disabled, 0);
			} else {
                if (i + 1 > UserData.SURVEY_MAX_LEVELS && ProjectSettings.I.IsBFG_Survey) {
                    Levels[i].SetUnlockType(UILevel.UnlockTypes.Disabled, 0);
                    continue;
                }

				if (i > 0) {
					if (Levels[i - 1].UnlockType == UILevel.UnlockTypes.Passed) {
                        if (dump != null) {
                            lastPassedLevel = dump.Passed ? i + 1 : lastPassedLevel;
                            Levels[i].SetUnlockType(dump.Passed ? UILevel.UnlockTypes.Passed : UILevel.UnlockTypes.Enabled, dump.EarnedStars);
                        } else {
                            Levels[i].SetUnlockType(UILevel.UnlockTypes.Enabled, 0);
                        }
					} else {
						Levels[i].SetUnlockType(UILevel.UnlockTypes.Disabled, 0);
					}
				} else {
                    if (dump != null) {
                        lastPassedLevel = dump.Passed ? i + 1 : lastPassedLevel;
                        Levels[i].SetUnlockType(dump.Passed ? UILevel.UnlockTypes.Passed : UILevel.UnlockTypes.Enabled, dump.EarnedStars);
                    } else {
                        //if(isExtra) Levels[i].SetUnlockType(UILevel.UnlockTypes.Disabled, 0);
                        //else Levels[i].SetUnlockType(UILevel.UnlockTypes.Enabled, 0);
                        Levels[i].SetUnlockType(UILevel.UnlockTypes.Enabled, 0);
                    }
				}
			}
		}

        if (ProjectSettings.I.IsBFG_Survey && lastPassedLevel >= UserData.SURVEY_MAX_LEVELS) {
            lastPassedLevel = UserData.SURVEY_MAX_LEVELS - 1;
        }

        if(isExtra) {
            lastPassedLevel += 50;
        }

        if (scrollingEnable) {
            if (LAST_PASSED_LEVEL != lastPassedLevel && (lastPassedLevel + 1) < Levels.Count) {
                MoveCamera(lastPassedLevel + 1);
                UserData.I.SaveMapLastPassedLevel(lastPassedLevel);
            }
        }

		if (!UserData.I.IsRateUsPanelShowed() && (lastPassedLevel == 6 || lastPassedLevel == 14)) {
			HGL_WindowManager.I.OpenWindow(null, null, nameof(PanelRateUs), false, true);
		}

		for (int i = 0; i < comicsButtons.Length; i++) {
            comicsButtons[i].Init(lastPassedLevel);
        }

        for(int i = 0; i< mapObjects.Length;i++) {
            mapObjects[i].gameObject.SetActive(mapObjects[i].MinPassedLevel <= lastPassedLevel);
        }

		await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);
		await FadeManager.I.FadeOut(2f);

		GameBonusesProgressManager.I.OnMapLoaded();
        GameBonusesManager.I.OnMapLoad();

        foreach (UITweener tw in tweeners) {
			tw.PlayForward();
		}

        if (TransitionManager.I.MustOpenDemoWindow) {
            OpenDemoWindow();
			return;
        }

        if (!UserData.I.GameModePanelShown) {
            OpenGameModePanel();
			return;
		}
    }

    public void OpenGameModePanel() {
        PanelGameMode panelGameMode = HGL_WindowManager.I.GetWindow("PanelGameMode").GetComponent<PanelGameMode>();
        panelGameMode.Init();
        HGL_WindowManager.I.OpenWindow(null, null, "PanelGameMode", false, true);
    }

    private void OpenDemoWindow() {
        PanelCommonOk panelOk = HGL_WindowManager.I.GetWindow("PanelCommonOk").GetComponent<PanelCommonOk>();
        panelOk.Init(LocalizationManager.GetLocalizedString("gui_lbl_demo"));
        HGL_WindowManager.I.OpenWindow(null, null, "PanelCommonOk", false, true);
        TransitionManager.I.MustOpenDemoWindow = false;
    }

    private void MoveCamera(int levelButton) {
        CameraTransform.position = new Vector3(Mathf.Clamp(Levels[levelButton - 1].transform.position.x, MinCameraPosX, MaxCameraPosX), 0, -10);
        SaveCameraPos();
    }

	private void MapManager_OnClick(UILevel level) {
        if(scrollingEnable) {
			if (scrollingPanel.CanClickLevel) {
				CheckLevelIsUnlocked(level);
			}
		} else {
			CheckLevelIsUnlocked(level);
		}
	}

    public void OnButtonBackClicked() {
        if(isExtra) {
            StartCoroutine(IEWaitButtonsHide(() => { TransitionManager.I.LoadScene(TransitionManager.I.SceneToMapExtraID); }));
        } else {
            StartCoroutine(IEWaitButtonsHide(TransitionManager.I.LoadMenu));
        }
	}

    public void OnButtonTrophiesClicked() {
        AwardScreenManager.IsOpenedFromMap = true;
        StartCoroutine(IEWaitButtonsHide(TransitionManager.I.LoadTrophies));
    }

	public void Left() {
		if (CameraTransform.position.x - speed * Time.deltaTime < minCameraPosX) {
			CameraTransform.position = new Vector3(minCameraPosX, 0, -10);
			return;
		}
		CameraTransform.position += Vector3.left * speed * Time.deltaTime;

        SaveCameraPos();
	}

	public void Right() {
		if (CameraTransform.position.x + speed * Time.deltaTime > maxCameraPosX) {
			CameraTransform.position = new Vector3(maxCameraPosX, 0, -10);
			return;
		}
		CameraTransform.position += Vector3.right * speed * Time.deltaTime;

        SaveCameraPos();
	}

    public void SaveCameraPos() {
        SAVED_CAMERA_POS = CameraTransform.position.x;
    }

	private IEnumerator IEWaitButtonsHide(Action callback) {
		float maxTime = 0f;
		foreach (UITweener tw in tweeners) {
			maxTime = Mathf.Max(maxTime, tw.duration);
		}
		foreach (UITweener tw in tweeners) {
			tw.PlayReverse();
		}
		yield return maxTime;
		callback.Invoke();
	}


	public void OurGames() {
#if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/dev?id=7220292605194330413");
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/developer/8floor/id500140554");
#endif
	}

	private void CheckLevelIsUnlocked(UILevel level) {
		if (UserData.I.IsLevelUnlocked(level.NumberLevel)) {
			StartCoroutine(IEWaitButtonsHide(() => { TransitionManager.I.LoadLevel(level.NumberLevel); }));
		}
		else {
			PanelUnlockLevel panelUnlockLevel = HGL_WindowManager.I.GetWindow(nameof(PanelUnlockLevel)).GetComponent<PanelUnlockLevel>();
			panelUnlockLevel.Init(level.NumberLevel);
			HGL_WindowManager.I.OpenWindow(null, null, nameof(PanelUnlockLevel), false, true);
		}
	}

	public void OnButtonUnlcokLevelIOSClicked() {
		HGL_WindowManager.I.OpenWindow(null, null, nameof(PanelUnlockLevelIOS), false, true);
	}
}
