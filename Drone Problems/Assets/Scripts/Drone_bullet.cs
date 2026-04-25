using UnityEngine;

public class Drone_bullet : MonoBehaviour
{
    public float lifeTime = 5f;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isReady = false;

    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isReady = true;

        Collider bulletCollider = GetComponent<Collider>();

        if (bulletCollider != null)
        {
            bulletCollider.isTrigger = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (isReady == false)
        {
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Drone bullet hit the player!");
        }

        Destroy(gameObject);
    }
}