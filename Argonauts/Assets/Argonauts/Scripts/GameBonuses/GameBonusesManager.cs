using System;
using System.Collections.Generic;
using System.Linq;

public class GameBonusesManager : GlobalSingletonBehaviour<GameBonusesManager>
{
    private List<GameBonus> bonuses = new List<GameBonus>();
    private RandomBlock<GameBonus.GameBonusTypes> prizeRandomBlock;
    private RandomBlock<bool> chestRandomBlock;

    public RandomBlock<GameBonus.GameBonusTypes> PrizeRandomBlock {
        get {
            return prizeRandomBlock;
        }
    }

    public RandomBlock<bool> ChestRandomBlock {
        get {
            return chestRandomBlock;
        }
    }

    public override void DoAwake()
    {
        if (PlayerProfileManager.I.GetCurrentProfile() == null) return;
        LoadRandomBlocks();
        LoadBonuses();
    }

    public void Clear()
    {
        if (PlayerProfileManager.I.GetCurrentProfile() == null) return;
        LoadRandomBlocks();
        LoadBonuses();
    }

    public override void DoDestroy()
    {
        if (PlayerProfileManager.I.GetCurrentProfile() == null) return;
        SaveRandomBlocks();
        SaveBonuses();
    }

    private void LoadRandomBlocks()
    {
        prizeRandomBlock = UserData.I.GetPrizeRandomBlock();
        chestRandomBlock = UserData.I.GetChestRandomBlock();

        if (prizeRandomBlock == null) {
            prizeRandomBlock = new RandomBlock<GameBonus.GameBonusTypes>();
            prizeRandomBlock.LevelNumber = UserData.I.GetLastLevelPassed();
            FillPrizeRandomBlock(prizeRandomBlock, prizeRandomBlock.LevelNumber);
        }

        if (chestRandomBlock == null) {
            chestRandomBlock = new RandomBlock<bool>();
            chestRandomBlock.LevelNumber = UserData.I.GetLastLevelPassed();
            FillChestRandomBlock(chestRandomBlock, chestRandomBlock.LevelNumber);
        }
    }

    private void LoadBonuses()
    {
        bonuses = UserData.I.GetGameBonuses();
        RemoveUnactiveBonuses(bonuses);
    }

    private void SaveRandomBlocks()
    {
        UserData.I.SetPrizeRandomBlock(prizeRandomBlock);
        UserData.I.SetChestRandomBlock(chestRandomBlock);
    }

    private void SaveBonuses()
    {
        RemoveUnactiveBonuses(bonuses);
        UserData.I.SetGameBonuses(bonuses);
    }

    public void OnMapLoad()
    {
        if (prizeRandomBlock == null) {
            prizeRandomBlock = new RandomBlock<GameBonus.GameBonusTypes>();
        }

        if (chestRandomBlock == null) {
            chestRandomBlock = new RandomBlock<bool>();
        }

        prizeRandomBlock.LevelNumber = UserData.I.GetLastLevelPassed();
        chestRandomBlock.LevelNumber = UserData.I.GetLastLevelPassed();

        FillPrizeRandomBlock(prizeRandomBlock, prizeRandomBlock.LevelNumber);
        FillChestRandomBlock(chestRandomBlock, chestRandomBlock.LevelNumber);
    }

    public void StartBonus(GameBonus.GameBonusTypes type)
    {
        GameBonus existBonus = bonuses.Where(x => x.BonusType == type).FirstOrDefault();
        if (existBonus != null) {
            existBonus.UsedTime = DateTime.UtcNow;
            return;
        }

        GameBonus bonus = new GameBonus(type, DateTime.UtcNow);
        bonuses.Add(bonus);
    }

    public bool BonuseIsActive(GameBonus.GameBonusTypes type)
    {
        RemoveUnactiveBonuses(bonuses);
        return bonuses.Exists(x => x.BonusType == type);
    }

    public void RemoveUnactiveBonuses(List<GameBonus> bonuses)
    {
        bonuses.RemoveAll(x => x.BonusOver());
    }

    public List<GameBonus> GetActiveBonuses()
    {
        RemoveUnactiveBonuses(bonuses);
        return bonuses;
    }

    public void FillChestRandomBlock(RandomBlock<bool> chestRandomBlock, int lastLevelPassed)
    {
        if (lastLevelPassed <= 3) {
            if(chestRandomBlock.TemplateItems.Where(x => x == true).Count() < 1) {
                chestRandomBlock.TemplateItems.Clear();
                chestRandomBlock.Items.Clear();
                chestRandomBlock.AddTemplateItem(true, 1);
            }
        } else {
            if (chestRandomBlock.TemplateItems.Where(x => x == true).Count() < 3) {
                chestRandomBlock.TemplateItems.Clear();
                chestRandomBlock.Items.Clear();
                chestRandomBlock.AddTemplateItem(false, 2);
                chestRandomBlock.AddTemplateItem(true, 3);
            }
        }
    }

    public void FillPrizeRandomBlock(RandomBlock<GameBonus.GameBonusTypes> prizeRandomBlock, int lastLevelPassed)
    {
        if (lastLevelPassed >= 1) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.CharacterMoveSpeed)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.CharacterMoveSpeed, 8);
            }

            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.CharacterWorkSpeed)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.CharacterWorkSpeed, 8);
            }
        }

        if (lastLevelPassed >= 3) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.BuildsCost)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.BuildsCost, 5);
            }
        }

        if (lastLevelPassed >= 4) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.TwoWorkers)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.TwoWorkers, 3);
            }
        }

        if (lastLevelPassed >= 7) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.Market)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.Market, 7);
            }
        }

        if (lastLevelPassed >= 9) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.EnemyCost)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.EnemyCost, 5);
            }
        }

        if (lastLevelPassed >= 13) {
            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.LevelBonusCooldown)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.LevelBonusCooldown, 4);
            }

            if (!prizeRandomBlock.TemplateItems.Contains(GameBonus.GameBonusTypes.LevelBonusDuration)) {
                prizeRandomBlock.AddTemplateItem(GameBonus.GameBonusTypes.LevelBonusDuration, 4);
            }
        }
    }
}
