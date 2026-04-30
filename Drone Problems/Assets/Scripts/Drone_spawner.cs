using System.Collections.Generic;
using UnityEngine;

public class Drone_spawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject dronePrefab;
    public Transform player;

    [Header("Drone Spawn Points Parent")]
    public Transform droneSpawnPointsParent;

    [Header("Spawn Settings")]
    public int maxDrones = 5;
    public float minDistanceFromPlayer = 8f;

    private List<GameObject> activeDrones = new List<GameObject>();
    private List<Transform> droneSpawnPoints = new List<Transform>();

    void Start()
    {
        FindPlayerIfNeeded();
        GetDroneSpawnPoints();

        for (int i = 0; i < maxDrones; i++)
        {
            SpawnDrone();
        }
    }

    void Update()
    {
        FindPlayerIfNeeded();

        activeDrones.RemoveAll(drone => drone == null);

        while (activeDrones.Count < maxDrones)
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