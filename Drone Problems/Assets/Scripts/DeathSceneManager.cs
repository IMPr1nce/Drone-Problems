using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI bulletsText;
    public TextMeshProUGUI dronesKilledText;
    public TextMeshProUGUI timeSurvivedText;

    [Header("Scene Settings")]
    public string startMenuSceneName = "StartMenu";
    public float waitTimeBeforeMenu = 3f;

    void Start()
    {
        coinsText.text = "Coins: " + GameStats.finalCoins;
        bulletsText.text = "Bullets Left: " + GameStats.finalBullets;
        dronesKilledText.text = "Drones Killed: " + GameStats.dronesKilled;
        timeSurvivedText.text = "Time Survived: " + Mathf.RoundToInt(GameStats.timeSurvived) + "s";

        Invoke(nameof(ReturnToStartMenu), waitTimeBeforeMenu);
    }

    void ReturnToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}