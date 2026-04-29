using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    public float rotationSpeed = 120f;
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        player playerScript = other.GetComponent<player>();

        if (playerScript == null)
        {
            playerScript = other.GetComponentInParent<player>();
        }

        if (playerScript != null)
        {
            playerScript.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}