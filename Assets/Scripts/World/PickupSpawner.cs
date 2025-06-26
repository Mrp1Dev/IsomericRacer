using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> pickupSpawnPoints;
    [SerializeField] private MinMax spawnDelay;
    [SerializeField] private bool blockSpawningUntilCollected;
    [SerializeField] private Pickup pickupToSpawn;
    private float spawnTimer;
    private bool activePickupExists;
    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (activePickupExists && blockSpawningUntilCollected) return;
        if (spawnTimer > Mathf.Epsilon)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            SpawnPickupAtRandomPoint();
            ResetTimer();
        }

    }

    private void SpawnPickupAtRandomPoint()
    {
        SpawnPickup(pickupSpawnPoints[Random.Range(0, pickupSpawnPoints.Count)].position);
    }

    private void SpawnPickup(Vector3 location)
    {
        var pickup = GameObject.Instantiate(pickupToSpawn, location, Quaternion.identity);
        activePickupExists = true;
        pickup.OnCollected += (_) => { activePickupExists = false; };
    }

    private void ResetTimer()
    {
        spawnTimer = Random.Range(spawnDelay.min, spawnDelay.max);
    }

}
