using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private CinemachineTargetGroup cameraTargetGroup;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    private List<PlayerInstance> playerInstances = new List<PlayerInstance>();
    void Start()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput obj)
    {
        PlayerInstance player = obj.GetComponent<PlayerInstance>();
        playerInstances.Add(player);
        player.SetCharacter(SpawnCharacter());
    }

    private void OnPlayerLeft(PlayerInput obj)
    {
        playerInstances.Remove(obj.GetComponent<PlayerInstance>());
    }

    private GameObject SpawnCharacter()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject character = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        cameraTargetGroup.AddMember(character.transform, 1.0f, 3.0f);
        return character;
    }
}
