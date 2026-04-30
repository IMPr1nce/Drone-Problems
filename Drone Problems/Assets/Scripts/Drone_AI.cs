using System.Collections;
using UnityEngine;

public class Drone_AI : MonoBehaviour
{
    [Header("AI References")]
    public Drone_follow drone;

    [Header("Roaming")]
    public float roamRadius = 45f;
    public float minRoamDistance = 15f;
    public float roamWaitTime = 0.5f;
    public float roamPointReachDistance = 1.5f;
    public int roamPointAttempts = 30;

    [Header("Height Limits")]
    public bool useHeightLimits = true;
    public float minRoamY = 3f;
    public float maxRoamY = 12f;

    [Header("Detection")]
    public float detectionRange = 25f;
    public float loseRange = 35f;
    public float visionAngle = 120f;
    public bool useVisionAngle = true;
    public LayerMask wallMask;
    public float losePlayerDelay = 1f;

    private float lostPlayerTimer = 0f;

    void Start()
    {
        if (drone == null)
        {
            drone = GetComponent<Drone_follow>();
        }

        StartCoroutine(DroneBrain());
    }

    IEnumerator DroneBrain()
    {
        while (true)
        {
            if (CanSeePlayer())
            {
                yield return StartCoroutine(AttackRoutine());
            }
            else
            {
                yield return StartCoroutine(RoamRoutine());
            }

            yield return null;
        }
    }

    IEnumerator RoamRoutine()
    {
        Vector3 roamPoint = GetRandomRoamPoint();
        float waitTimer = 0f;

        while (CanSeePlayer() == false)
        {
            float distanceToRoamPoint = Vector3.Distance(transform.position, roamPoint);

            if (distanceToRoamPoint <= roamPointReachDistance)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= roamWaitTime)
                {
                    roamPoint = GetRandomRoamPoint();
                    waitTimer = 0f;
                }
            }
            else
            {
                drone.RotateTowardPointFree(roamPoint);

                bool moved = drone.MoveTowardPointFree(roamPoint);

                if (moved == false)
                {
                    roamPoint = GetRandomRoamPoint();
                    waitTimer = 0f;
                }
            }

            ClampCurrentHeight();

            yield return null;
        }
    }

    IEnumerator AttackRoutine()
    {
        lostPlayerTimer = 0f;

        while (drone.target != null)
        {
            if (CanKeepTrackingPlayer())
            {
                lostPlayerTimer = 0f;
            }
            else
            {
                lostPlayerTimer += Time.deltaTime;

                if (lostPlayerTimer >= losePlayerDelay)
                {
                    yield break;
                }
            }

            Vector3 movementTarget = drone.target.position;
            Vector3 aimTarget = drone.target.position + Vector3.up * drone.aimHeight;

            if (useHeightLimits)
            {
                movementTarget.y = Mathf.Clamp(movementTarget.y, minRoamY, maxRoamY);
            }

            float distanceToPlayer = Vector3.Distance(transform.position, drone.target.position);

            drone.RotateTowardPointFree(aimTarget);

            if (distanceToPlayer <= drone.shootingDistance)
            {
                drone.ShootAtTarget();
            }

            if (distanceToPlayer > drone.attackBoundaryDistance)
            {
                drone.MoveTowardPointFree(movementTarget);
            }

            ClampCurrentHeight();

            yield return null;
        }
    }

    Vector3 GetRandomRoamPoint()
    {
        for (int i = 0; i < roamPointAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minRoamDistance, roamRadius);

            float randomY = transform.position.y;

            if (useHeightLimits)
            {
                randomY = Random.Range(minRoamY, maxRoamY);
            }
            else
            {
                randomY = transform.position.y + Random.Range(-5f, 5f);
            }

            Vector3 roamPoint = transform.position + new Vector3(
                randomCircle.x * randomDistance,
                0f,
                randomCircle.y * randomDistance
            );

            roamPoint.y = randomY;

            if (drone.IsPointBlocked(roamPoint))
            {
                continue;
            }

            if (drone.MovementPathIsClear(transform.position, roamPoint))
            {
                return roamPoint;
            }
        }

        return transform.position;
    }

    void ClampCurrentHeight()
    {
        if (!useHeightLimits)
        {
            return;
        }

        Vector3 position = transform.position;
        position.y = Mathf.Clamp(position.y, minRoamY, maxRoamY);
        transform.position = position;
    }

    bool CanSeePlayer()
    {
        if (drone == null)
        {
            return false;
        }

        drone.FindPlayerIfNeeded();

        if (drone.target == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position;
        Vector3 targetPosition = drone.target.position + Vector3.up * drone.aimHeight;

        Vector3 directionToPlayer = targetPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        if (useVisionAngle)
        {
            Vector3 droneForward = drone.GetDroneForwardDirection();

            if (directionToPlayer != Vector3.zero && droneForward != Vector3.zero)
            {
                float angleToPlayer = Vector3.Angle(
                    droneForward.normalized,
                    directionToPlayer.normalized
                );

                if (angleToPlayer > visionAngle / 2f)
                {
                    return false;
                }
            }
        }

        if (Physics.Raycast(eyePosition, directionToPlayer.normalized, distanceToPlayer, wallMask))
        {
            return false;
        }

        return true;
    }

    bool CanKeepTrackingPlayer()
    {
        if (drone == null || drone.target == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position;
        Vector3 targetPosition = drone.target.position + Vector3.up * drone.aimHeight;

        Vector3 directionToPlayer = targetPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > loseRange)
        {
            return false;
        }

        if (Physics.Raycast(eyePosition, directionToPlayer.normalized, distanceToPlayer, wallMask))
        {
            return false;
        }

        return true;
    }
}