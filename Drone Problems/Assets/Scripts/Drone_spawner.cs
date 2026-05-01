using System.Collections.Generic;
using UnityEngine;

public class Drone_spawner : MonoBehaviour
{
    [System.Serializable]
    public class DifficultySpawnSettings
    {
        public int maxDrones = 5;
        public int initialDrones = 3;
        public float spawnDelay = 0.5f;
    }

    [Header("Spawner Setup")]
    public GameObject dronePrefab;
    public Transform player;

    [Header("Drone Spawn Points Parent")]
    public Transform droneSpawnPointsParent;

    [Header("Common Spawn Settings")]
    public float minDistanceFromPlayer = 8f;

    [Header("Easy Mode")]
    public DifficultySpawnSettings easySettings = new DifficultySpawnSettings
    {
        maxDrones = 3,
        initialDrones = 2,
        spawnDelay = 1f
    };

    [Header("Medium Mode")]
    public DifficultySpawnSettings mediumSettings = new DifficultySpawnSettings
    {
        maxDrones = 5,
        initialDrones = 3,
        spawnDelay = 0.75f
    };

    [Header("Hard Mode")]
    public DifficultySpawnSettings hardSettings = new DifficultySpawnSettings
    {
        maxDrones = 8,
        initialDrones = 5,
        spawnDelay = 0.5f
    };

    private int currentMaxDrones;
    private int currentInitialDrones;
    private float currentSpawnDelay;

    private List<GameObject> activeDrones = new List<GameObject>();
    private List<Transform> droneSpawnPoints = new List<Transform>();

    private float nextSpawnTime = 0f;

    void Start()
    {
        ApplySelectedDifficultySettings();

        FindPlayerIfNeeded();
        GetDroneSpawnPoints();

        SpawnInitialDrones();

        nextSpawnTime = Time.time + currentSpawnDelay;
    }

    void Update()
    {
        FindPlayerIfNeeded();

        activeDrones.RemoveAll(drone => drone == null);

        if (activeDrones.Count < currentMaxDrones && Time.time >= nextSpawnTime)
        {
            SpawnDrone();
            nextSpawnTime = Time.time + currentSpawnDelay;
        }
    }

    void ApplySelectedDifficultySettings()
    {
        DifficultySpawnSettings selectedSettings = easySettings;

        if (GameDifficulty.selectedDifficulty == DifficultyLevel.Easy)
        {
            selectedSettings = easySettings;
        }
        else if (GameDifficulty.selectedDifficulty == DifficultyLevel.Medium)
        {
            selectedSettings = mediumSettings;
        }
        else if (GameDifficulty.selectedDifficulty == DifficultyLevel.Hard)
        {
            selectedSettings = hardSettings;
        }

        currentMaxDrones = selectedSettings.maxDrones;
        currentInitialDrones = selectedSettings.initialDrones;
        currentSpawnDelay = selectedSettings.spawnDelay;

        if (currentInitialDrones > currentMaxDrones)
        {
            currentInitialDrones = currentMaxDrones;
        }

        Debug.Log("Difficulty: " + GameDifficulty.selectedDifficulty);
        Debug.Log("Max Drones: " + currentMaxDrones);
        Debug.Log("Initial Drones: " + currentInitialDrones);
        Debug.Log("Spawn Delay: " + currentSpawnDelay);
    }

    void SpawnInitialDrones()
    {
        for (int i = 0; i < currentInitialDrones; i++)
        {
            SpawnDrone();
        }
    }

    void FindPlayerIfNeeded()
    {
        if (player != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void GetDroneSpawnPoints()
    {
        droneSpawnPoints.Clear();

        if (droneSpawnPointsParent == null)
        {
            Debug.LogError("Drone Spawn Points Parent is missing.");
            return;
        }

        foreach (Transform child in droneSpawnPointsParent)
        {
            droneSpawnPoints.Add(child);
        }

        Debug.Log("Drone spawn points found: " + droneSpawnPoints.Count);
    }

    void SpawnDrone()
    {
        if (dronePrefab == null)
        {
            Debug.LogError("Drone Prefab is missing.");
            return;
        }

        if (droneSpawnPoints.Count == 0)
        {
            Debug.LogError("No drone spawn points found.");
            return;
        }

        Transform selectedPoint = GetRandomValidSpawnPoint();

        if (selectedPoint == null)
        {
            Debug.LogWarning("No valid drone spawn point found.");
            return;
        }

        GameObject newDrone = Instantiate(
            dronePrefab,
            selectedPoint.position,
            selectedPoint.rotation
        );

        Drone_follow droneFollow = newDrone.GetComponent<Drone_follow>();

        if (droneFollow != null && player != null)
        {
            droneFollow.target = player;
        }

        activeDrones.Add(newDrone);
    }

    Transform GetRandomValidSpawnPoint()
    {
        int attempts = 50;

        for (int i = 0; i < attempts; i++)
        {
            Transform randomPoint = droneSpawnPoints[Random.Range(0, droneSpawnPoints.Count)];

            if (player == null)
            {
                return randomPoint;
            }

            float distanceFromPlayer = Vector3.Distance(randomPoint.position, player.position);

            if (distanceFromPlayer >= minDistanceFromPlayer)
            {
                return randomPoint;
            }
        }

        return droneSpawnPoints[Random.Range(0, droneSpawnPoints.Count)];
    }
}