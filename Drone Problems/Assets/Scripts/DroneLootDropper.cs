using UnityEngine;

public class DroneLootDropper : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject coinPrefab;
    public GameObject magazinePrefab;

    [Header("Drop Ratio")]
    [Range(0f, 100f)]
    public float coinDropChance = 80f;

    [Header("Drop Position")]
    public float rayStartHeight = 3f;
    public float maxDropDistance = 100f;
    public float surfaceOffset = 0.35f;
    public float fallbackGroundY = 1f;
    public float randomSideOffset = 0.5f;

    [Header("Drop Surface Mask")]
    public LayerMask dropSurfaceMask = ~0;

    public void DropLoot()
    {
        if (coinPrefab == null || magazinePrefab == null)
        {
            Debug.LogWarning("Coin or Magazine prefab is missing.");
            return;
        }

        GameObject prefabToDrop;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < coinDropChance)
        {
            prefabToDrop = coinPrefab;
        }
        else
        {
            prefabToDrop = magazinePrefab;
        }

        Vector3 dropPosition = GetDropPosition();

        GameObject droppedItem = Instantiate(prefabToDrop, dropPosition, Quaternion.identity);
        droppedItem.SetActive(true);

        Debug.Log("Dropped item: " + droppedItem.name + " at " + dropPosition);
    }

    Vector3 GetDropPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomSideOffset, randomSideOffset),
            0f,
            Random.Range(-randomSideOffset, randomSideOffset)
        );

        Vector3 startPosition = transform.position + randomOffset;
        Vector3 rayStart = startPosition + Vector3.up * rayStartHeight;

        RaycastHit[] hits = Physics.RaycastAll(
            rayStart,
            Vector3.down,
            maxDropDistance,
            dropSurfaceMask,
            QueryTriggerInteraction.Ignore
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (ShouldIgnoreSurface(hit.collider))
            {
                continue;
            }

            return hit.point + Vector3.up * surfaceOffset;
        }

        return new Vector3(startPosition.x, fallbackGroundY, startPosition.z);
    }

    bool ShouldIgnoreSurface(Collider collider)
    {
        if (collider == null)
        {
            return true;
        }

        if (collider.transform == transform || collider.transform.IsChildOf(transform))
        {
            return true;
        }

        if (collider.GetComponentInParent<Drone_follow>() != null)
        {
            return true;
        }

        if (collider.GetComponentInParent<DroneLootDropper>() != null)
        {
            return true;
        }

        if (collider.GetComponentInParent<CoinPickup>() != null)
        {
            return true;
        }

        if (collider.GetComponentInParent<BulletPickup>() != null)
        {
            return true;
        }

        return false;
    }
}