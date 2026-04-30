using UnityEngine;

public class DroneLootDropper : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject coinPrefab;
    public GameObject magazinePrefab;

    [Header("Drop Ratio")]
    [Range(0f, 100f)]
    public float coinDropChance = 80f;

    [Header("Drop Surface Detection")]
    public float rayStartHeight = 2f;
    public float maxDropDistance = 100f;
    public float surfaceOffset = 0.5f;
    public LayerMask dropSurfaceMask = ~0;

    public void DropLoot()
    {
        Debug.Log("DropLoot was called from: " + gameObject.name);

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
        Vector3 rayStart = transform.position + Vector3.up * rayStartHeight;

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
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            return hit.point + Vector3.up * surfaceOffset;
        }

        return transform.position + Vector3.up * surfaceOffset;
    }
}