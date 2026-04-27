using UnityEngine;

public class shooting : MonoBehaviour
{
    [Header("Aim Setup")]
    public Camera aimCamera;
    public float shootDistance = 1000f;
    public float hitRadius = 0.4f;
    public LayerMask shootMask = ~0;

    public void Shoot()
    {
        Camera cameraToUse = GetCamera();

        if (cameraToUse == null)
        {
            Debug.LogWarning("No camera found for shooting.");
            return;
        }

        Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, 1f);

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            hitRadius,
            shootDistance,
            shootMask,
            QueryTriggerInteraction.Collide
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                continue;
            }

            if (hit.collider.GetComponentInParent<player>() != null)
            {
                continue;
            }

            Drone_follow drone = hit.collider.GetComponentInParent<Drone_follow>();

            if (drone != null)
            {
                Debug.Log("Player shot and destroyed drone: " + drone.name);
                Destroy(drone.gameObject);
                return;
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Player shot and destroyed enemy: " + hit.collider.name);
                Destroy(hit.collider.transform.root.gameObject);
                return;
            }

            Debug.Log("Shot hit object: " + hit.collider.name);
            return;
        }

        Debug.Log("Shot missed.");
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