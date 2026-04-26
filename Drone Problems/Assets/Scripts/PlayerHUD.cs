using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player Reference")]
    public player playerScript;

    [Header("UI Text")]
    public TMP_Text healthText;
    public TMP_Text bulletsText;
    public TMP_Text coinsText;

    void Start()
    {
        if (playerScript == null)
        {
            playerScript = Object.FindFirstObjectByType<player>();
        }
    }

    void Update()
    {
        if (playerScript == null)
        {
            playerScript = Object.FindFirstObjectByType<player>();

            if (playerScript == null)
            {
                return;
            }
        }

        if (healthText != null)
        {
            healthText.text = "Health: " + playerScript.health;
        }

        if (bulletsText != null)
        {
            bulletsText.text = "Bullets: " + playerScript.current_bullets;
        }

        if (coinsText != null)
        {
            coinsText.text = "Coins: " + playerScript.coins;
        }
    }
}