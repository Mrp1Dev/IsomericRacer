using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInstanceData
{
    public CinemachineTargetGroup cinemachineTargetGroup;
    public int playerIndex;
}

public class PlayerInstance : MonoBehaviour
{
    private GameObject playerCharacter;
    private PlayerInstanceData data = new PlayerInstanceData();

    public int PlayerIndex => data.playerIndex;
    public GameObject CarPrefab { get; set; }

    public event Action<GameObject> OnPlayerCharacterSpawned;

    public void Initialize(PlayerInstanceData data)
    {
        this.data = data;
    }

    public GameObject SpawnCharacter(Transform spawnPoint)
    {
        GameObject character = Instantiate(CarPrefab, spawnPoint.position, spawnPoint.rotation);
        data.cinemachineTargetGroup.AddMember(character.transform, 1.0f, 3.0f);
        SetCharacter(character);
        OnPlayerCharacterSpawned?.Invoke(character);
        return character;
    }

    private void SetCharacter(GameObject character)
    {
        playerCharacter = character;
        playerCharacter.GetComponent<CarInput>().SetPlayerInputRef(GetComponent<PlayerInput>());
        var ownedComponents = playerCharacter.GetComponentsInChildren<IOwnedByPlayer>();
        foreach (var owned in ownedComponents)
        {
            owned.OwnerPlayer = this;
        }
    }

    public void DestroyCharacter()
    {
        if (HasActiveCharacter())
        {
            GameObject.Destroy(playerCharacter);
            playerCharacter = null;
        }
    }

    public bool HasActiveCharacter() => playerCharacter != null;
}
