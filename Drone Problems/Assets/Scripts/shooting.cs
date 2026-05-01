using UnityEngine;

public class shooting : MonoBehaviour
{
    [Header("Aim Setup")]
    public Camera aimCamera;
    public float shootDistance = 1000f;
    public LayerMask shootMask = ~0;

    [Header("Built In Crosshair")]
    public bool drawBuiltInCrosshair = true;
    public int crosshairSize = 20;
    public Color normalColor = Color.green;
    public Color enemyAimColor = Color.red;

    [Header("Debug")]
    public bool showDebugRay = true;

    private GameObject currentTarget;

    void Update()
    {
        currentTarget = FindShootableTarget();
    }

    void OnGUI()
    {
        if (!drawBuiltInCrosshair)
        {
            return;
        }

        Color oldColor = GUI.color;
        GUI.color = currentTarget != null ? enemyAimColor : normalColor;

        float x = (Screen.width - crosshairSize) / 2f;
        float y = (Screen.height - crosshairSize) / 2f;

        GUI.DrawTexture(
            new Rect(x, y, crosshairSize, crosshairSize),
            Texture2D.whiteTexture
        );

        GUI.color = oldColor;
    }

    public void Shoot()
    {
        GameObject target = FindShootableTarget();
        
        if (target != null)
        {
            DropLootIfPossible(target);
            GameStats.dronesKilled++;
            Debug.Log("Player shot and destroyed: " + target.name);

            Destroy(target);
            return;
        }

        Debug.Log("Shot missed.");
    }

    private GameObject FindShootableTarget()
{
    Camera cameraToUse = GetCamera();

    if (cameraToUse == null)
    {
        return null;
    }

    Ray ray = cameraToUse.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

    if (showDebugRay)
    {
        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, 0.05f);
    }

    RaycastHit[] hits = Physics.RaycastAll(
        ray,
        shootDistance,
        shootMask,
        QueryTriggerInteraction.Collide
    );

    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

    foreach (RaycastHit hit in hits)
    {
        if (IsPlayer(hit.collider))
        {
            continue;
        }

        if (hit.collider.GetComponentInParent<player_bullet>() != null)
        {
            continue;
        }

        if (hit.collider.GetComponentInParent<CoinPickup>() != null)
        {
            continue;
        }

        if (hit.collider.GetComponentInParent<BulletPickup>() != null)
        {
            continue;
        }

        GameObject target = GetShootableTarget(hit.collider);

        if (target != null)
        {
            return target;
        }

        Debug.Log("Shot blocked by: " + hit.collider.gameObject.name);
        return null;
    }

    return null;
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
            lootDropper.DropLoot();
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

    private Camera GetCamera()
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