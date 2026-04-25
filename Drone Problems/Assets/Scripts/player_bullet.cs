using UnityEngine;

public class player_bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the bullet collides with an object tagged as "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Destroy the enemy object
            Destroy(other.transform.root.gameObject);
            // Destroy the bullet itself
            Destroy(gameObject);
        }
    }
}
