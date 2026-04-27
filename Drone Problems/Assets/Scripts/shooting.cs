using UnityEngine;

public class shooting : MonoBehaviour
{
    [Header("Shooting Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Aim Setup")]
    public Camera aimCamera;
    public float maxAimDistance = 1000f;
    public LayerMask aimMask = ~0;

    [Header("Bullet Settings")]
    public float bulletSpeed = 80f;
    public float bulletLifeTime = 5f;

    public void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab is missing.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is missing.");
            return;
        }

        Vector3 shootDirection = GetShootDirection();

        GameObject newBullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        player_bullet bulletScript = newBullet.GetComponent<player_bullet>();

        if (bulletScript == null)
        {
            bulletScript = newBullet.AddComponent<player_bullet>();
        }

        bulletScript.SetDirection(shootDirection, bulletSpeed, bulletLifeTime);
    }

    Vector3 GetShootDirection()
    {
        Camera cameraToUse = aimCamera;

        if (cameraToUse == null)
        {
            cameraToUse = Camera.main;
        }

        if (cameraToUse != null)
        {
            Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Vector3 aimPoint = ray.GetPoint(maxAimDistance);

            if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
            {
                aimPoint = hit.point;
            }

            Vector3 directionFromFirePoint = aimPoint - firePoint.position;

            if (directionFromFirePoint != Vector3.zero)
            {
                return directionFromFirePoint.normalized;
            }
        }

        return firePoint.forward.normalized;
    }
}