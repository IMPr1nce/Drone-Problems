using UnityEngine;

public static class GameDifficultySettings
{
    public static int GetContainerCount()
    {
        switch (GameDifficulty.selectedDifficulty)
        {
            case DifficultyLevel.Easy:
                return 15;

            case DifficultyLevel.Medium:
                return 25;

            case DifficultyLevel.Hard:
                return 35;

            default:
                return 15;
        }
    }

    public static int GetMaxDrones()
    {
        switch (GameDifficulty.selectedDifficulty)
        {
            case DifficultyLevel.Easy:
                return 3;

            case DifficultyLevel.Medium:
                return 5;

            case DifficultyLevel.Hard:
                return 8;

            default:
                return 3;
        }
    }

    public static float GetDroneDetectionRange()
    {
        switch (GameDifficulty.selectedDifficulty)
        {
            case DifficultyLevel.Easy:
                return 20f;

            case DifficultyLevel.Medium:
                return 28f;

            case DifficultyLevel.Hard:
                return 35f;

            default:
                return 20f;
        }
    }
}