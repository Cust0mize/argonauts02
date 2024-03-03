using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class UserData : GlobalSingletonBehaviour<UserData> {
    public static int COUNT_LEVELS_IN_GAME = 60;
    public static int SURVEY_MAX_LEVELS = 16;

    public bool GameModePanelShown {
        get {
            return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>("game_mode_shown", false);
        }
        set {
            PlayerProfileManager.I.GetCurrentProfile().SetValue("game_mode_shown", value);
            PlayerProfileManager.I.SaveCurrentProfile();
        }
    }

    public int GameMode {
        get {
            return PlayerProfileManager.I.GetCurrentProfile().GetValue<int>("game_mode", 0);
        }
        set {
            PlayerProfileManager.I.GetCurrentProfile().SetValue("game_mode", value);
            PlayerProfileManager.I.SaveCurrentProfile();
            OnGameModeChanged.Invoke(value);

            AnalyticsEvent.Custom("difficulty_changed", new Dictionary<string, object> {
                { "difficalty", value }
            });
        }
    }

    public event Action<int> OnGameModeChanged = delegate { };

    //0 - normal mode
    //1 - hard mode
    //2 - unlimited time mode
    //2 - untimed mode

    public override void DoAwake()
    {
        PlayerProfileManager.instance = new PlayerProfileManager();
    }

    public LevelDumpData GetLevelDump (int levelNumber) {
        string raw = PlayerProfileManager.I.GetCurrentProfile().GetValue<string>(string.Format("level_dump_{0:00}", levelNumber));
        if (string.IsNullOrEmpty(raw)) return null;
        return JsonConvert.DeserializeObject<LevelDumpData>(raw);
    }

    public void SaveLevelDump (int levelNumber, float time, bool passed, int earnedStars, int score) {
        LevelDumpData lastDump = GetLevelDump(levelNumber);

        float bestTime = 0f;
        int bestScore = 0;
        if (lastDump != null) {
            bestTime = lastDump.BestTime;
            bestScore = lastDump.BestScore;
        }

        LevelDumpData dump = new LevelDumpData(lastDump == null || !lastDump.Passed ? time : Mathf.Min(bestTime, time), passed, earnedStars, Mathf.Max(bestScore, score));

        PlayerProfileManager.I.GetCurrentProfile().SetValue(string.Format("level_dump_{0:00}", levelNumber), JsonConvert.SerializeObject(dump));
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public Award GetAward(string key) {
		string raw = "";

		if (PlayerProfileManager.I.GetCurrentProfile() != null) {
            raw = PlayerProfileManager.I.GetCurrentProfile().GetValue<string>(key);
		}
			
		if (string.IsNullOrEmpty(raw)) {
			return new Award(key, 0);
		}

        return JsonConvert.DeserializeObject<Award>(raw);
    }

    public void SaveAward(Award award) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue(award.Key, JsonConvert.SerializeObject(award));
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool ComicsIsShown(int number) {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>(string.Format("comics_{0}", number));
    }

    public void SetComicsStatus(int number, bool isShown) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue(string.Format("comics_{0}", number), isShown);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public float GetMapCameraPos() {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<float>("map_camera_pos", float.MinValue);
    }

    public void SaveMapCameraPos(float posX) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("map_camera_pos", posX);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public int GetMapLastPassedLevel() {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<int>("map_last_passed_level", 0);
    }

    public void SaveMapLastPassedLevel(int number) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("map_last_passed_level", number);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool GetComicsReadStatus(int number) {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>(string.Format("comics_read_status_{0}", number), false);
    }

    public void SaveComicsReadStatus(int number,bool isRead) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue(string.Format("comics_read_status_{0}", number), isRead);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool GetLevel3StarsStatus(int number) {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>(string.Format("level_3_stars_{0}", number), false);
    }

    public void SaveLevel3StarsStatus(int number, bool is3Stars) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue(string.Format("level_3_stars_{0}", number), is3Stars);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public int GetGems() {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<int>("count_gems", 0);
    }

    public void SetGems(int countGems) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("count_gems", countGems);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public List<GameBonus> GetGameBonuses()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<List<GameBonus>>("game_bonuses", new List<GameBonus>());
    }

    public void SetGameBonuses(List<GameBonus> bonuses)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("game_bonuses", bonuses);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public int GetEarnedStars()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<int>("earned_stars", 0);
    }

    public void SetEarnedStars(int eanredStars)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("earned_stars", eanredStars);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public int GetBonusLastLevelPassed()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<int>("last_level_passed", 0);
    }

    public void SetBonusLastLevelPassed(int lastLevelPassed)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("last_level_passed", lastLevelPassed);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool TakePrizeHintShowed()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>("take_prize_hint_showed", false);
    }

    public void SetTakePrizeHintStatus(bool showed)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("take_prize_hint_showed", showed);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool BonusAppearHintShowed()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>("bonus_appear_hint_showed", false);
    }

    public void SetBonusAppearHintStatus(bool showed)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("bonus_appear_hint_showed", showed);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public RandomBlock<GameBonus.GameBonusTypes> GetPrizeRandomBlock()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<RandomBlock<GameBonus.GameBonusTypes>>("prize_random_block",
            null);
    }

    public void SetPrizeRandomBlock(RandomBlock<GameBonus.GameBonusTypes> prizeRandomBlock)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("prize_random_block", prizeRandomBlock);
        PlayerProfileManager.I.SaveCurrentProfile();
    }


    public RandomBlock<bool> GetChestRandomBlock()
    {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<RandomBlock<bool>>("chest_random_block",
            null);
    }

    public void SetChestRandomBlock(RandomBlock<bool> chestRandomBlock)
    {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("chest_random_block", chestRandomBlock);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public int GetLastLevelPassed()
    {
        int lastPassedLevel = 0;

        for (int i = 0; i < (ProjectSettings.I.IsBFG_Survey ? SURVEY_MAX_LEVELS : 60); i++) {
            LevelDumpData dump = GetLevelDump(i + 1);

            if (i > 0) {
                LevelDumpData d = GetLevelDump(i);

                if (d != null && d.Passed) {
                    if (dump != null) {
                        lastPassedLevel = dump.Passed ? i + 1 : lastPassedLevel;
                    }
                }
            } else {
                if (dump != null) {
                    lastPassedLevel = dump.Passed ? i + 1 : lastPassedLevel;
                }
            }
        }

        return lastPassedLevel + 1;
    }


    public void UnlockLevel(int level) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue($"is_level_unlocked{level:00}", true);
        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public void UnlockAllLevels() {
        for (int i = 0; i < (ProjectSettings.I.IsBFG_Survey ? SURVEY_MAX_LEVELS : 60); i++) {
            int level = i + 1;
            PlayerProfileManager.I.GetCurrentProfile().SetValue($"is_level_unlocked{level:00}", true);
        }

        PlayerProfileManager.I.SaveCurrentProfile();
    }

    public bool IsLevelUnlocked(int level) {
        if (level < 7) return true;

        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>($"is_level_unlocked{level:00}");
    }

    public bool IsRateUsPanelShowed() {
        return PlayerProfileManager.I.GetCurrentProfile().GetValue<bool>("is_rate_us_panel_showed");
    }

    public void SetIsRateUsPanelShowed(bool value) {
        PlayerProfileManager.I.GetCurrentProfile().SetValue("is_rate_us_panel_showed", value);
        PlayerProfileManager.I.SaveCurrentProfile();
    }
}
