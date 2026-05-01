using UnityEngine;

public static class GameStats
{
    public static int finalCoins;
    public static int finalBullets;
    public static int dronesKilled;
    public static float timeSurvived;

    public static void ResetStats()
    {
        finalCoins = 0;
        finalBullets = 0;
        dronesKilled = 0;
        timeSurvived = 0f;
    }
}