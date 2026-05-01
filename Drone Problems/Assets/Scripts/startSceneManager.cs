using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("UI")]
    public TextMeshProUGUI bestCoinsText;

    private bool isLoading = false;

    void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameStats.ResetStats();
        UpdateBestCoinsText();
    }

    void UpdateBestCoinsText()
    {
        if (bestCoinsText != null)
        {
            bestCoinsText.text = "Best Coins: " + GameStats.GetBestCoins();
        }
    }

    public void StartEasyMode()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Easy;
        LoadGameScene();
    }

    public void StartMediumMode()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Medium;
        LoadGameScene();
    }

    public void StartHardMode()
    {
        GameDifficulty.selectedDifficulty = DifficultyLevel.Hard;
        LoadGameScene();
    }

    void LoadGameScene()
    {
        if (isLoading)
        {
            return;
        }

        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        isLoading = true;

        Time.timeScale = 1f;
        GameStats.ResetStats();

        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}