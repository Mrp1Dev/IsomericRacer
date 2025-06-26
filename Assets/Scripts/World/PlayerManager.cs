using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private CinemachineTargetGroup cameraTargetGroup;
    private List<PlayerInstance> playerInstances = new List<PlayerInstance>();
    public int PlayerCount { get; private set; }

    public event Action<PlayerInstance> OnPlayerInstanceCreated;
    void Start()
    {
        playerInputManager.onPlayerJoined += OnPlayerJoined;
        //playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerJoined(PlayerInput obj)
    {
        PlayerCount++;
        PlayerInstance player = obj.GetComponent<PlayerInstance>();
        playerInstances.Add(player);
        player.Initialize(new PlayerInstanceData { cinemachineTargetGroup = cameraTargetGroup, playerIndex = PlayerCount });
        OnPlayerInstanceCreated?.Invoke(player);
    }

    public PlayerInstance GetPlayerInstance(int index) => playerInstances[index];
    /*private void OnPlayerLeft(PlayerInput obj)
    {
        playerInstances.Remove(obj.GetComponent<PlayerInstance>());
    }*///handle player leaving later.
}
