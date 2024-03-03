using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : GlobalSingletonBehaviour<CheatManager> {
	private readonly Dictionary<KeyCode , Dictionary<int , string>> CheatCommands = new Dictionary<KeyCode , Dictionary<int , string>>() {
		{
			KeyCode.Alpha1, new Dictionary<int, string>() {
				{ TransitionManager.ID_SCENE_GAME, "AddResources" }
			}
		},{
			KeyCode.N, new Dictionary<int, string>() {
				{ TransitionManager.ID_SCENE_GAME, "NextLevel" }
			}
		},{
			KeyCode.B, new Dictionary<int, string>() {
				{ TransitionManager.ID_SCENE_GAME, "BackLevel" }
			}
		},{
			KeyCode.O, new Dictionary<int, string>() {
				{ TransitionManager.ID_SCENE_MAP, "OpenLevels" },
                { TransitionManager.ID_SCENE_MAP_EXTRA, "OpenLevels" }
			}
        },{
            KeyCode.Alpha2, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "OpenLevelWithoutStars" }
            }
        },{
            KeyCode.Alpha3, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "OpenLevelOneStart" }
            }
        },{
            KeyCode.Alpha4, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "OpenLevelTwoStart" }
            }
        },{
            KeyCode.Alpha5, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "OpenLevelThreeStart" }
            }
        },{
            KeyCode.Alpha6, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "OpenFirstTask" }
            }
        },{
            KeyCode.Alpha7, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_GAME, "FillBonuses" }
            }
        },{
            KeyCode.M, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_MAP, "ActivateBonuses" },
                { TransitionManager.ID_SCENE_MAP_EXTRA, "ActivateBonuses" }
            }
        },{
            KeyCode.K, new Dictionary<int, string>() {
                { TransitionManager.ID_SCENE_MAP, "FillChest" },
                { TransitionManager.ID_SCENE_MAP_EXTRA, "FillChest" }
            }
        }
    };

	void Update () {

		//if (!ProjectSettings.I.isCheatEnabled) {
		//	return;
		//}

		foreach (KeyCode k in CheatCommands.Keys) {
			if (Input.GetKeyDown(k)) {
				if (CheatCommands[k].ContainsKey(TransitionManager.I.GetIDCurrentScene())) {
					string nameMethod = CheatCommands[k][TransitionManager.I.GetIDCurrentScene()];
					SendMessage(nameMethod);
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.P)) {
			ScreenCapture.CaptureScreenshot("screen.png");
		}
	}

	#region Cheat Methods

	void AddResources () {
		GameManager.I.AddResources(new List<Resource>() {
			new Resource(Resource.Types.Food, 10),
			new Resource (Resource.Types.Stone, 10),
			new Resource (Resource.Types.Wood, 10),
			new Resource (Resource.Types.Gold, 10)
		});
	}

    private async void NextLevel() {
        Level level = await LevelParser.I.GetLevel(TransitionManager.I.TargetLevel);
        if (level != null) {
            UserData.I.SaveLevelDump(TransitionManager.I.TargetLevel, 0, true, 3, 0);
            TransitionManager.I.LoadLevel(TransitionManager.I.TargetLevel + 1);
        }
    }

    private async void BackLevel() {
        Level level = await LevelParser.I.GetLevel(TransitionManager.I.TargetLevel);
        if (level != null) {
            UserData.I.SaveLevelDump(TransitionManager.I.TargetLevel, 0, true, 3, 0);
            TransitionManager.I.LoadLevel(TransitionManager.I.TargetLevel - 1);
        }
    }

    private async void OpenLevels() {
        int countLevels = UserData.COUNT_LEVELS_IN_GAME;

        if (ProjectSettings.I.IsBFG_Survey) {
            countLevels = UserData.SURVEY_MAX_LEVELS;
        }

        for (int i = 1; i <= countLevels; i++) {
            Level level = await LevelParser.I.GetLevel(i);
            if (level != null) {
                UserData.I.SaveLevelDump(i, 0, true, 3, 0);
            }
            else break;
        }
        TransitionManager.I.RestartScene();
    }

    void OpenLevelWithoutStars() {
        GameManager.I.DoPassLevel(0);
    }

    void OpenLevelOneStart() {
        GameManager.I.DoPassLevel(1);
    }

    void OpenLevelTwoStart() {
        GameManager.I.DoPassLevel(2);
    }

    void OpenLevelThreeStart() {
        GameManager.I.DoPassLevel(3);
    }

    void OpenFirstTask() {
        LevelTaskController.I.DoFirstTask();
    }

    void FillBonuses() {
        GameManager.I.FillBonuses();
    }

    void ActivateBonuses()
    {
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.CharacterMoveSpeed);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.CharacterWorkSpeed);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.BuildsCost);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.TwoWorkers);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.Market);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.EnemyCost);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.LevelBonusCooldown);
        GameBonusesManager.I.StartBonus(GameBonus.GameBonusTypes.LevelBonusDuration);
        TransitionManager.I.RestartScene();
    }

    void FillChest()
    {
        GameBonusesProgressManager.I.CheatFill();
    }

    #endregion
}
