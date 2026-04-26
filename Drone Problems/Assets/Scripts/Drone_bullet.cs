using UnityEngine;

public class Drone_bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 10;
    public float hitRadius = 0.2f;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasHit = false;

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
        if (isReady == false || hasHit)
        {
            return;
        }

        float moveDistance = moveSpeed * Time.deltaTime;

        if (Physics.SphereCast(
            transform.position,
            hitRadius,
            moveDirection,
            out RaycastHit hit,
            moveDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        ))
        {
            HandleHit(hit.collider);
            return;
        }

        transform.position += moveDirection * moveDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    void HandleHit(Collider other)
    {
        if (hasHit)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            return;
        }

        player playerScript = other.GetComponent<player>();

        if (playerScript == null)
        {
            playerScript = other.GetComponentInParent<player>();
        }

        if (playerScript != null)
        {
            hasHit = true;
            playerScript.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        hasHit = true;
        Destroy(gameObject);
    }
}