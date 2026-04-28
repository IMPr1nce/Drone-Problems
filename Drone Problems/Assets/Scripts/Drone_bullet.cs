using UnityEngine;

public class Drone_bullet : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Header("Movement")]
    public float lifeTime = 5f;
    public float hitRadius = 0.2f;

    [Header("Bullet Direction Points")]
    public Transform frontPoint;
    public Transform backPoint;

    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isReady = false;
    private bool hasHit = false;
    private Transform ownerRoot;

    public void SetOwner(Transform owner)
    {
        ownerRoot = owner;
    }

    public void SetDirection(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isReady = true;

        FindDirectionPointsIfNeeded();
        AlignBulletWithDirection(moveDirection);

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

        Destroy(gameObject, lifeTime);
    }

    void FindDirectionPointsIfNeeded()
    {
        if (frontPoint == null)
        {
            Transform foundFront = transform.Find("FrontPoint");

            if (foundFront != null)
            {
                frontPoint = foundFront;
            }
        }

        if (backPoint == null)
        {
            Transform foundBack = transform.Find("BackPoint");

            if (foundBack != null)
            {
                backPoint = foundBack;
            }
        }
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

        if (ownerRoot != null && other.transform.IsChildOf(ownerRoot))
        {
            return false;
        }

        if (other.CompareTag("Enemy"))
        {
            return false;
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
            return true;
        }

        hasHit = true;
        Destroy(gameObject);
        return true;
    }
}