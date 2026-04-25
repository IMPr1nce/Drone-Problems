using System.Collections.Generic;
using UnityEngine;

public class Drone_spawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject dronePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int maxDrones = 5;
    public float spawnRadius = 20f;
    public float minSpawnDistance = 10f;
    public float spawnHeightMin = 3f;
    public float spawnHeightMax = 8f;

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

        Vector3 spawnPosition = GetSpawnPosition();

        GameObject newDrone = Instantiate(
            dronePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Drone_follow droneFollow = newDrone.GetComponent<Drone_follow>();

        if (droneFollow != null)
        {
            droneFollow.target = player;
        }

        activeDrones.Add(newDrone);
    }

    Vector3 GetSpawnPosition()
    {
        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, spawnRadius);
        float yOffset = Random.Range(spawnHeightMin, spawnHeightMax);

        Vector3 spawnPosition = new Vector3(
            player.position.x + circle.x,
            player.position.y + yOffset,
            player.position.z + circle.y
        );

        return spawnPosition;
    }
}