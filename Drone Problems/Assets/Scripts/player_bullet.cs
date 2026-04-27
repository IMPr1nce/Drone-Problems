using UnityEngine;

public class player_bullet : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 100;

    [Header("Hit Detection")]
    public float hitRadius = 0.15f;

    [Header("Bullet Direction Points")]
    public Transform frontPoint;
    public Transform backPoint;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasHit = false;

    public void SetDirection(Vector3 direction, float speed, float lifeTime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isReady = true;

        AlignBulletWithDirection(moveDirection);

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.isKinematic = true;

        Collider bulletCollider = GetComponent<Collider>();

        if (bulletCollider != null)
        {
            bulletCollider.isTrigger = true;
        }

        Destroy(gameObject, lifeTime);
    }

    void AlignBulletWithDirection(Vector3 desiredDirection)
    {
        if (frontPoint == null || backPoint == null)
        {
            transform.rotation = Quaternion.LookRotation(desiredDirection);
            return;
        }

        Vector3 currentBulletDirection = frontPoint.position - backPoint.position;

        if (currentBulletDirection == Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(desiredDirection);
            return;
        }

        Quaternion rotationFix = Quaternion.FromToRotation(
            currentBulletDirection.normalized,
            desiredDirection.normalized
        );

        transform.rotation = rotationFix * transform.rotation;
    }

    void Update()
    {
        if (isReady == false || hasHit)
        {
            return;
        }

        float moveDistance = moveSpeed * Time.deltaTime;

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            hitRadius,
            moveDirection,
            moveDistance,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (RaycastHit hit in hits)
        {
            if (TryHandleHit(hit.collider))
            {
                return;
            }
        }

        transform.position += moveDirection * moveDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        TryHandleHit(other);
    }

    bool TryHandleHit(Collider other)
    {
        if (hasHit)
        {
            return true;
        }

        if (other == null)
        {
            return false;
        }

        if (other.transform == transform || other.transform.IsChildOf(transform))
        {
            return false;
        }

        if (other.CompareTag("Player"))
        {
            return false;
        }

        Drone_follow drone = other.GetComponentInParent<Drone_follow>();

        if (drone != null)
        {
            hasHit = true;
            Destroy(drone.gameObject);
            Destroy(gameObject);
            return true;
        }

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            Destroy(other.transform.root.gameObject);
            Destroy(gameObject);
            return true;
        }

        hasHit = true;
        Destroy(gameObject);
        return true;
    }
}