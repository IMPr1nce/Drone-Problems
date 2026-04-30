using System.Collections.Generic;
using UnityEngine;

public class ContainerMazeSpawner : MonoBehaviour
{
    [Header("Prefabs To Spawn")]
    public GameObject[] prefabsToSpawn;

    [Header("Maze Area")]
    public float width = 40f;
    public float length = 40f;

    [Header("Spawn Settings")]
    public int numberOfObjects = 20;
    public float extraSpaceBetweenObjects = 4f;
    public float edgePadding = 3f;
    public float spawnY = 0f;

    [Header("Random Rotation")]
    public bool randomRotation = true;

    private List<PlacedObject> placedObjects = new List<PlacedObject>();

    private struct PlacedObject
    {
        public Vector3 position;
        public float sizeX;
        public float sizeZ;

        public PlacedObject(Vector3 position, float sizeX, float sizeZ)
        {
            this.position = position;
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
        }
    }

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogError("No prefabs added to Prefabs To Spawn.");
            return;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = 5000;

        while (spawned < numberOfObjects && attempts < maxAttempts)
        {
            attempts++;

            GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

            if (prefab == null)
            {
                continue;
            }

            Vector3 prefabSize = GetPrefabSize(prefab);

            bool rotateObject = randomRotation && Random.Range(0, 2) == 1;

            float sizeX = rotateObject ? prefabSize.z : prefabSize.x;
            float sizeZ = rotateObject ? prefabSize.x : prefabSize.z;

            float minX = transform.position.x - width / 2f + sizeX / 2f + edgePadding;
            float maxX = transform.position.x + width / 2f - sizeX / 2f - edgePadding;

            float minZ = transform.position.z - length / 2f + sizeZ / 2f + edgePadding;
            float maxZ = transform.position.z + length / 2f - sizeZ / 2f - edgePadding;

            if (minX >= maxX || minZ >= maxZ)
            {
                Debug.LogWarning("Maze area is too small for one of the prefabs.");
                continue;
            }

            Vector3 spawnPosition = new Vector3(
                Random.Range(minX, maxX),
                spawnY,
                Random.Range(minZ, maxZ)
            );

            if (OverlapsExistingObject(spawnPosition, sizeX, sizeZ))
            {
                continue;
            }

            Quaternion rotation = rotateObject
                ? Quaternion.Euler(0f, 90f, 0f)
                : Quaternion.identity;

            Instantiate(prefab, spawnPosition, rotation);

            placedObjects.Add(new PlacedObject(spawnPosition, sizeX, sizeZ));
            spawned++;
        }

        Debug.Log("Spawned maze objects: " + spawned);
    }

    Vector3 GetPrefabSize(GameObject prefab)
    {
        BoxCollider box = prefab.GetComponent<BoxCollider>();

        if (box != null)
        {
            Vector3 scale = prefab.transform.localScale;

            return new Vector3(
                box.size.x * scale.x,
                box.size.y * scale.y,
                box.size.z * scale.z
            );
        }

        Renderer renderer = prefab.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.size;
        }

        return new Vector3(6f, 2f, 2f);
    }

    bool OverlapsExistingObject(Vector3 newPosition, float newSizeX, float newSizeZ)
    {
        foreach (PlacedObject placed in placedObjects)
        {
            bool overlapX = Mathf.Abs(newPosition.x - placed.position.x) <
                            (newSizeX / 2f + placed.sizeX / 2f + extraSpaceBetweenObjects);

            bool overlapZ = Mathf.Abs(newPosition.z - placed.position.z) <
                            (newSizeZ / 2f + placed.sizeZ / 2f + extraSpaceBetweenObjects);

            if (overlapX && overlapZ)
            {
                return true;
            }
        }

        return false;
    }
}