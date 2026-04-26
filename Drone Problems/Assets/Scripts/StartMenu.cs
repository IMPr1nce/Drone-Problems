using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("Difficulty Scenes")]
    public string easySceneName = "EasyMap";
    public string mediumSceneName = "MediumMap";
    public string hardSceneName = "HardMap";

    public void PlayEasy()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Easy;
        SceneManager.LoadScene(easySceneName);
    }

    public void PlayMedium()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Medium;
        SceneManager.LoadScene(mediumSceneName);
    }

    public void PlayHard()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Hard;
        SceneManager.LoadScene(hardSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}