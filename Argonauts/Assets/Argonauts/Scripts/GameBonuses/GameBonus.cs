using System;

[Serializable]
public class GameBonus {
    public enum GameBonusTypes
    {
        CharacterMoveSpeed = 0,
        CharacterWorkSpeed = 1,
        BuildsCost = 2,
        TwoWorkers = 3,
        Market = 4,
        EnemyCost = 5,
        LevelBonusCooldown = 6,
        LevelBonusDuration = 7
    }

    public const int DURATION = 600; //10 minutes

    public GameBonusTypes BonusType;
    public DateTime UsedTime;

    public GameBonus(GameBonusTypes bonusType, DateTime usedTime)
    {
        BonusType = bonusType;
        UsedTime = usedTime;
    }

    public int PassedTime()
    {
        return (int)(DateTime.UtcNow - UsedTime).TotalSeconds;
    }

    public bool BonusOver()
    {
        return PassedTime() >= DURATION;
    }
}
