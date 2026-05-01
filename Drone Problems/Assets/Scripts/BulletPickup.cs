using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    [Header("Bullet Amount Range")]
    public int minBullets = 10;
    public int maxBullets = 30;

    [Header("Visual")]
    public float rotationSpeed = 90f;

    private int bulletAmount;

    void Start()
    {
        bulletAmount = Random.Range(minBullets, maxBullets + 1);
    }

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
            playerScript.AddBullets(bulletAmount);
            //Debug.Log("Picked up " + bulletAmount + " bullets.");
            Destroy(gameObject);
        }
    }
}