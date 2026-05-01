using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameStats.ResetStats();
    }

    public void StartEasyMode()
    {
        Time.timeScale = 1f;
        GameStats.ResetStats();
        SceneManager.LoadScene("EasyMap");
    }

    public void StartMediumMode()
    {
        Time.timeScale = 1f;
        GameStats.ResetStats();
        SceneManager.LoadScene("MediumMap");
    }

    public void StartHardMode()
    {
        Time.timeScale = 1f;
        GameStats.ResetStats();
        SceneManager.LoadScene("HardMap");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}