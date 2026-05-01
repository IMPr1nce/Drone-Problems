using UnityEngine;

public static class GameStats
{
    public static int finalCoins;
    public static int finalBullets;
    public static int dronesKilled;
    public static float timeSurvived;

    private const string BestCoinsKey = "BestCoinsSingleGame";

    public static void ResetStats()
    {
        finalCoins = 0;
        finalBullets = 0;
        dronesKilled = 0;
        timeSurvived = 0f;
    }

    public static int GetBestCoins()
    {
        return PlayerPrefs.GetInt(BestCoinsKey, 0);
    }

    public static void SaveBestCoinsIfNeeded(int coins)
    {
        int currentBest = GetBestCoins();

        if (coins > currentBest)
        {
            PlayerPrefs.SetInt(BestCoinsKey, coins);
            PlayerPrefs.Save();
        }
    }
}