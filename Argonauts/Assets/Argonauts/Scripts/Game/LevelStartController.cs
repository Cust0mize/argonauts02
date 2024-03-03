using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HGL;
using UnityEngine;
using UnityEngine.UI;

public class LevelStartController : LocalSingletonBehaviour<LevelStartController> {
	public Level CurrentLevel;
	[SerializeField] private SpriteRenderer levelBack;

	public override void DoAwake() {
		FadeManager.I.ForceFadeIn();
	}

    async private void Start() {
        CurrentLevel = await LevelParser.I.GetLevel(TransitionManager.I.TargetLevel);
        ConfigureLevelTime(CurrentLevel);

        levelBack.sprite = await LevelParser.GetLevelBack(TransitionManager.I.TargetLevel);


        if (CurrentLevel == null)
            return;

        LevelTaskController.I.DoInit(CurrentLevel);
		UpgradesManager.I.DoInit(CurrentLevel.MaxUpgradeValues);
        GameManager.I.DoInit(CurrentLevel, TransitionManager.I.TargetLevel);
		GameGUIManager.I.DoInit();

        while(!GameManager.I.GameInited) {
            return;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: false);
        LevelTaskPingController.I.DoInit(CurrentLevel);
        await FadeManager.I.FadeOut();

        HGL_WindowManager.I.GetWindow("PanelStart").GetComponent<PanelStart>().Init(TransitionManager.I.TargetLevel,CurrentLevel.Task1, CurrentLevel.Task2, CurrentLevel.Task3);
        HGL_WindowManager.I.OpenWindow(null, null, "PanelStart", false, true);
	}

    private void ConfigureLevelTime(Level level) {
        if (UserData.I.GameMode == 1) {
            level.Time1 -= 15;
            level.Time2 -= 15;
            level.Time3 -= 15;
        }
    }
}
