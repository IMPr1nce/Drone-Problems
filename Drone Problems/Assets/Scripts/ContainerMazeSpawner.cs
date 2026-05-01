using System.Collections.Generic;
using UnityEngine;

public class ContainerMazeSpawner : MonoBehaviour
{
    [Header("Prefabs To Spawn")]
    public GameObject[] prefabsToSpawn;

    [Header("Maze Area")]
    public float width = 25f;
    public float length = 25f;

    [Header("Spawn Settings")]
    public int numberOfObjects = 30;
    public float gridCellSize = 2.5f;
    public float extraSpaceBetweenObjects = 0.5f;
    public float edgePadding = 1.5f;
    public float spawnY = 0f;

    [Header("Random Rotation")]
    public bool randomRotation = true;

    [Header("Random Seed")]
    public bool useRandomSeed = true;
    public int seed = 12345;

    private List<Vector3> gridPoints = new List<Vector3>();
    private List<PlacedObject> placedObjects = new List<PlacedObject>();
    private Dictionary<GameObject, Vector3> prefabSizeCache = new Dictionary<GameObject, Vector3>();

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

        if (!useRandomSeed)
        {
            Random.InitState(seed);
        }

        CachePrefabSizes();
        GenerateGridPoints();
        ShuffleGridPoints();

        int spawned = 0;

        foreach (Vector3 point in gridPoints)
        {
            if (spawned >= numberOfObjects)
            {
                break;
            }

            GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

            if (prefab == null)
            {
                continue;
            }

            Vector3 prefabSize = prefabSizeCache[prefab];

            bool rotateObject = randomRotation && Random.Range(0, 2) == 1;

            float sizeX = rotateObject ? prefabSize.z : prefabSize.x;
            float sizeZ = rotateObject ? prefabSize.x : prefabSize.z;

            Vector3 spawnPosition = new Vector3(point.x, spawnY, point.z);

            if (OverlapsExistingObject(spawnPosition, sizeX, sizeZ))
            {
                continue;
            }

            Quaternion rotation = rotateObject
                ? Quaternion.Euler(0f, 90f, 0f)
                : Quaternion.identity;

            Instantiate(prefab, spawnPosition, rotation, transform);

            placedObjects.Add(new PlacedObject(spawnPosition, sizeX, sizeZ));
            spawned++;
        }

        Debug.Log("Spawned maze objects: " + spawned + " / " + numberOfObjects);
    }

    void GenerateGridPoints()
    {
        gridPoints.Clear();

        float minX = transform.position.x - width / 2f + edgePadding;
        float maxX = transform.position.x + width / 2f - edgePadding;

        float minZ = transform.position.z - length / 2f + edgePadding;
        float maxZ = transform.position.z + length / 2f - edgePadding;

        for (float x = minX; x <= maxX; x += gridCellSize)
        {
            for (float z = minZ; z <= maxZ; z += gridCellSize)
            {
                gridPoints.Add(new Vector3(x, spawnY, z));
            }
        }
    }

    void ShuffleGridPoints()
    {
        for (int i = 0; i < gridPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, gridPoints.Count);

            Vector3 temp = gridPoints[i];
            gridPoints[i] = gridPoints[randomIndex];
            gridPoints[randomIndex] = temp;
        }
    }

    void CachePrefabSizes()
    {
        prefabSizeCache.Clear();

        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (prefab == null)
            {
                continue;
            }

            if (!prefabSizeCache.ContainsKey(prefab))
            {
                prefabSizeCache.Add(prefab, GetPrefabSize(prefab));
            }
        }
    }

    Vector3 GetPrefabSize(GameObject prefab)
    {
        BoxCollider box = prefab.GetComponentInChildren<BoxCollider>();

        if (box != null)
        {
            Vector3 scale = box.transform.lossyScale;

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

        return new Vector3(4f, 2f, 2f);
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