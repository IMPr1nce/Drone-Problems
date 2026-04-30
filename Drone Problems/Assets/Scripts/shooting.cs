using UnityEngine;

public class shooting : MonoBehaviour
{
    [Header("Aim Setup")]
    public Camera aimCamera;
    public float shootDistance = 1000f;
    public float hitRadius = 0.8f;
    public LayerMask shootMask = ~0;

    [Header("Debug")]
    public bool showDebugRay = true;

    public void Shoot()
    {
        Camera cameraToUse = GetCamera();

        if (cameraToUse == null)
        {
            Debug.LogWarning("No camera found for shooting.");
            return;
        }

        Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, 1f);
        }

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            hitRadius,
            shootDistance,
            shootMask,
            QueryTriggerInteraction.Collide
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        GameObject bestTarget = null;
        float bestDistanceFromCenter = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (IsPlayer(hit.collider))
            {
                continue;
            }

            GameObject target = GetShootableTarget(hit.collider);

            if (target != null)
            {
                float distanceFromCenter = Vector3.Cross(ray.direction, hit.point - ray.origin).magnitude;

                if (distanceFromCenter < bestDistanceFromCenter)
                {
                    bestDistanceFromCenter = distanceFromCenter;
                    bestTarget = target;
                }
            }
        }

        if (bestTarget != null)
        {
            DropLootIfPossible(bestTarget);

            Debug.Log("Player shot and destroyed: " + bestTarget.name);

            Destroy(bestTarget);
            return;
        }

        Debug.Log("Shot missed.");
    }

    private void DropLootIfPossible(GameObject target)
    {
        DroneLootDropper lootDropper = target.GetComponent<DroneLootDropper>();

        if (lootDropper == null)
        {
            lootDropper = target.GetComponentInChildren<DroneLootDropper>();
        }

        if (lootDropper == null)
        {
            lootDropper = target.GetComponentInParent<DroneLootDropper>();
        }

        if (lootDropper != null)
        {
            Debug.Log("Loot dropper found. Dropping loot.");
            lootDropper.DropLoot();
        }
        else
        {
            Debug.LogWarning("No DroneLootDropper found on: " + target.name);
        }
    }

    private bool IsPlayer(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            return true;
        }

        if (collider.GetComponentInParent<player>() != null)
        {
            return true;
        }

        return false;
    }

    private GameObject GetShootableTarget(Collider collider)
    {
        Drone_follow drone = collider.GetComponentInParent<Drone_follow>();

        if (drone != null)
        {
            return drone.gameObject;
        }

        if (collider.CompareTag("Enemy"))
        {
            return collider.transform.root.gameObject;
        }

        return null;
    }

    Camera GetCamera()
    {
        if (aimCamera != null)
        {
            return aimCamera;
        }

        Camera cameraOnThisObject = GetComponent<Camera>();

        if (cameraOnThisObject != null)
        {
            return cameraOnThisObject;
        }

        return Camera.main;
    }
}