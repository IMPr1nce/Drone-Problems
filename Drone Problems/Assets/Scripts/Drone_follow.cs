using UnityEngine;

public class Drone_follow : MonoBehaviour
{
    public Transform target;
    public Transform forwardFocus;

    public float moveSpeed = 4f;
    public float turnSpeed = 720f;
    public float shootingDistance = 8f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform leftFirePoint;
    public Transform rightFirePoint;
    public float bulletSpeed = 25f;
    public float fireCooldown = 1f;
    public float aimAngleTolerance = 5f;

    private float nextFireTime = 0f;
    private Quaternion modelOffset;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        if (forwardFocus != null)
        {
            Vector3 localForward = forwardFocus.localPosition.normalized;

            if (localForward != Vector3.zero)
            {
                modelOffset = Quaternion.Inverse(Quaternion.LookRotation(localForward));
            }
        }
    }

    void Update()
    {
        if (target == null || forwardFocus == null) return;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;

        if (toTarget == Vector3.zero) return;

        Quaternion targetRotation =
            Quaternion.LookRotation(toTarget.normalized) * modelOffset;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        if (distance <= shootingDistance)
        {
            float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleToTarget <= aimAngleTolerance)
            {
                ShootAtTarget(toTarget.normalized);
            }

            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );
    }

    void ShootAtTarget(Vector3 shootDirection)
    {
        if (Time.time < nextFireTime)
        {
            return;
        }

        bool firedLeft = FireBullet(leftFirePoint, shootDirection);
        bool firedRight = FireBullet(rightFirePoint, shootDirection);

        if (firedLeft || firedRight)
        {
            Debug.Log("Drone shot at the player!");
            nextFireTime = Time.time + fireCooldown;
        }
    }

    bool FireBullet(Transform firePoint, Vector3 shootDirection)
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

        GameObject newBullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        Drone_bullet bulletScript = newBullet.GetComponent<Drone_bullet>();

        if (bulletScript == null)
        {
            bulletScript = newBullet.AddComponent<Drone_bullet>();
        }

        bulletScript.SetDirection(shootDirection, bulletSpeed);

        return true;
    }
}