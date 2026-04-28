using UnityEngine;

public class Drone_follow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Transform forwardFocus;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float turnSpeed = 720f;

    [Header("Movement Collision")]
    public LayerMask obstacleMask;
    public float collisionRadius = 0.75f;
    public float collisionPadding = 0.2f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform leftFirePoint;
    public Transform rightFirePoint;
    public float shootingDistance = 10f;
    public float bulletSpeed = 30f;
    public float fireCooldown = 0.5f;
    public float aimHeight = 1f;
    public float aimAngleTolerance = 20f;
    public float attackBoundaryDistance = 5f;

    private float nextFireTime = 0f;
    private Quaternion modelOffset = Quaternion.identity;

    void Awake()
    {
        CalculateModelOffset();
    }

    void Start()
    {
        FindPlayerIfNeeded();
        CalculateModelOffset();
    }

    public void FindPlayerIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    void CalculateModelOffset()
    {
        if (forwardFocus == null)
        {
            modelOffset = Quaternion.identity;
            return;
        }

        Vector3 localForward = forwardFocus.localPosition.normalized;

        if (localForward != Vector3.zero)
        {
            modelOffset = Quaternion.Inverse(Quaternion.LookRotation(localForward));
        }
    }

    public void RotateTowardPointXZ(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized) * modelOffset;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    public bool MoveTowardPointXZ(Vector3 point)
    {
        Vector3 fixedPoint = new Vector3(
            point.x,
            transform.position.y,
            point.z
        );

        return MoveSafelyToward(fixedPoint);
    }

    public void RotateTowardPointFree(Vector3 point)
    {
        Vector3 direction = point - transform.position;

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized) * modelOffset;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    public bool MoveTowardPointFree(Vector3 point)
    {
        return MoveSafelyToward(point);
    }

    bool MoveSafelyToward(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        float distance = direction.magnitude;

        if (distance <= 0.01f)
        {
            return true;
        }

        float moveStep = moveSpeed * Time.deltaTime;
        float checkDistance = Mathf.Min(moveStep + collisionPadding, distance);

        if (IsMovementBlocked(direction.normalized, checkDistance))
        {
            return false;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            point,
            moveStep
        );

        return true;
    }

    bool IsMovementBlocked(Vector3 direction, float checkDistance)
    {
        if (obstacleMask.value == 0)
        {
            return false;
        }

        return Physics.SphereCast(
            transform.position,
            collisionRadius,
            direction,
            out RaycastHit hit,
            checkDistance,
            obstacleMask
        );
    }

    public bool MovementPathIsClear(Vector3 startPoint, Vector3 endPoint)
    {
        if (obstacleMask.value == 0)
        {
            return true;
        }

        Vector3 direction = endPoint - startPoint;
        float distance = direction.magnitude;

        if (distance <= 0.01f)
        {
            return true;
        }

        return Physics.SphereCast(
            startPoint,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            obstacleMask
        ) == false;
    }

    public bool IsPointBlocked(Vector3 point)
    {
        if (obstacleMask.value == 0)
        {
            return false;
        }

        return Physics.CheckSphere(
            point,
            collisionRadius,
            obstacleMask
        );
    }

    public void ShootAtTarget()
    {
        if (target == null)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        if (IsWithinShootingAngle() == false)
        {
            return;
        }

        Vector3 targetPosition = target.position + Vector3.up * aimHeight;

        bool firedLeft = FireBullet(leftFirePoint, targetPosition);
        bool firedRight = FireBullet(rightFirePoint, targetPosition);

        if (firedLeft || firedRight)
        {
            Debug.Log("Drone fired at player.");
            nextFireTime = Time.time + fireCooldown;
        }
    }

    bool IsWithinShootingAngle()
    {
        Vector3 targetPosition = target.position + Vector3.up * aimHeight;

        Vector3 directionToPlayer = targetPosition - transform.position;
        Vector3 droneForward = GetDroneForwardDirection();

        if (directionToPlayer == Vector3.zero || droneForward == Vector3.zero)
        {
            return false;
        }

        float currentAngle = Vector3.Angle(
            droneForward.normalized,
            directionToPlayer.normalized
        );

        return currentAngle <= aimAngleTolerance;
    }

    bool FireBullet(Transform firePoint, Vector3 targetPosition)
{
    if (firePoint == null)
    {
        return false;
    }

    if (bulletPrefab == null)
    {
        Debug.LogWarning("Bullet Prefab is missing on the drone.");
        return false;
    }

    Vector3 shootDirection = (targetPosition - firePoint.position).normalized;

    GameObject newBullet = Instantiate(
        bulletPrefab,
        firePoint.position,
        Quaternion.identity
    );

    Drone_bullet bulletScript = newBullet.GetComponent<Drone_bullet>();

    if (bulletScript == null)
    {
        bulletScript = newBullet.AddComponent<Drone_bullet>();
    }

    bulletScript.SetOwner(transform);
    bulletScript.SetDirection(shootDirection, bulletSpeed);

    return true;
}

    public Vector3 GetDroneForwardDirection()
    {
        if (forwardFocus != null)
        {
            Vector3 direction = forwardFocus.position - transform.position;

            if (direction != Vector3.zero)
            {
                return direction.normalized;
            }
        }

        return transform.forward;
    }
}