using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBonusesProgressManager : GlobalSingletonBehaviour<GameBonusesProgressManager> {
    public int CountStars;
    public int LastCountStars;
    public int LastPassedLevel;

    public override void DoAwake()
    {
        Load();
    }

    public void Load()
    {
        if (PlayerProfileManager.I.GetCurrentProfile() == null) return;
        CountStars = UserData.I.GetEarnedStars();
        LastCountStars = CountStars;
        LastPassedLevel = UserData.I.GetBonusLastLevelPassed();
    }

    public void Clear()
    {
        if (PlayerProfileManager.I.GetCurrentProfile() == null) return;
        CountStars = UserData.I.GetEarnedStars();
        LastCountStars = CountStars;
        LastPassedLevel = UserData.I.GetBonusLastLevelPassed();
    }

    public void OnMapLoaded()
    {
        if (CountStars > LastCountStars && LastPassedLevel > 0) {
            StartCoroutine(GameBonusesUIManager.I.AnimateFillStars(LastCountStars, CountStars - LastCountStars, LastPassedLevel, .2F));
            LastCountStars = CountStars;
        }
    }

    public void AddStars(int countStars, int levelNumber)
    {
        CountStars = Mathf.Clamp(CountStars + countStars, 0, 5);
        LastPassedLevel = levelNumber;
        UserData.I.SetEarnedStars(CountStars);
        UserData.I.SetBonusLastLevelPassed(levelNumber);
    }

    public void ClearOnUse()
    {
        CountStars = 0;
        LastCountStars = 0;
        UserData.I.SetEarnedStars(CountStars);
        UserData.I.SetBonusLastLevelPassed(-1);
        GameBonusesUIManager.I.ClearStars();
    }

    public void CheatFill()
    {
        AddStars(5, 1);
        if (CountStars > LastCountStars && LastPassedLevel > 0) {
            StartCoroutine(GameBonusesUIManager.I.AnimateFillStars(LastCountStars, CountStars - LastCountStars, LastPassedLevel, .2F));
            LastCountStars = CountStars;
        }
    }
}
