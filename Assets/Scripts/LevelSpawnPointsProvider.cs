using UnityEngine;

public class LevelSpawnPointsProvider : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform[] SpawnPoints => spawnPoints;
}
