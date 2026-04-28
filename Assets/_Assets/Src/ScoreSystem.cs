using UnityEngine;

public static class ScoreSystem
{
    private const string BestKey = "BestScore";

    public static int LastScore { get; private set; }
    public static int BestScore => PlayerPrefs.GetInt(BestKey, 0);
    public static bool IsNewBest { get; private set; }

    // score = tỉ lệ block sống sót × thời gian × 1000
    public static int Calculate(int blocksRemaining, int blocksTotal, float surviveSeconds)
    {
        if (blocksTotal == 0) return 0;
        float ratio = (float)blocksRemaining / blocksTotal;
        return Mathf.RoundToInt(ratio * surviveSeconds * 1000f);
    }

    public static void Submit(int score)
    {
        LastScore = score;
        IsNewBest = score > BestScore;
        if (IsNewBest)
            PlayerPrefs.SetInt(BestKey, score);
    }

    public static void Reset()
    {
        LastScore = 0;
        IsNewBest = false;
    }
}
