using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Values")]
    public int easyCoinValue = 1;
    public int mediumCoinValue = 2;
    public int hardCoinValue = 3;

    private bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp)
        {
            return;
        }

        player playerScript = other.GetComponentInParent<player>();

        if (playerScript == null)
        {
            return;
        }

        pickedUp = true;

        int coinValue = GetCoinValue();
        playerScript.coins += coinValue;

        if (playerScript.audioSource != null && playerScript.coinPickupSound != null)
        {
            playerScript.audioSource.PlayOneShot(playerScript.coinPickupSound);
        }
        else
        {
            Debug.LogWarning("Player AudioSource or Coin Pickup Sound is missing on the Player script.");
        }

        Destroy(gameObject);
    }

    int GetCoinValue()
    {
        if (GameDifficulty.selectedDifficulty == DifficultyLevel.Easy)
        {
            return easyCoinValue;
        }

        if (GameDifficulty.selectedDifficulty == DifficultyLevel.Medium)
        {
            return mediumCoinValue;
        }

        if (GameDifficulty.selectedDifficulty == DifficultyLevel.Hard)
        {
            return hardCoinValue;
        }

        return easyCoinValue;
    }
}