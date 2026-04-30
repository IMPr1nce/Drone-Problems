using System.Collections.Generic;
using UnityEngine;

public class Drone_spawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject dronePrefab;
    public Transform player;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Spawn Settings")]
    public int maxDrones = 5;

    private List<GameObject> activeDrones = new List<GameObject>();

    void Start()
    {
        FindPlayerIfNeeded();

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

    void SpawnDrone()
    {
        if (dronePrefab == null || player == null)
        {
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No drone spawn points assigned.");
            return;
        }

        Transform randomSpawnPoint = GetRandomSpawnPoint();

        if (randomSpawnPoint == null)
        {
            Debug.LogWarning("Random spawn point is missing.");
            return;
        }

        GameObject newDrone = Instantiate(
            dronePrefab,
            randomSpawnPoint.position,
            randomSpawnPoint.rotation
        );

        Drone_follow droneFollow = newDrone.GetComponent<Drone_follow>();

        if (droneFollow != null)
        {
            droneFollow.target = player;
        }

        activeDrones.Add(newDrone);
    }

    Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex];
    }
} 