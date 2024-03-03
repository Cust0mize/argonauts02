public class LevelDumpData {
    public float BestTime;
    public bool Passed;
    public int EarnedStars;
    public int BestScore;

    public LevelDumpData (float bestTime, bool passed, int earnedStars, int bestScore) {
        BestTime = bestTime;
        Passed = passed;
        EarnedStars = earnedStars;
        BestScore = bestScore;
    }
}
