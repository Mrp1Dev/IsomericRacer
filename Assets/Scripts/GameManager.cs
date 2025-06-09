using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct CarPrefabs
{
    public GameObject LobbyPrefab;
    public GameObject GamePrefab;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameScoreManager gameScoreManager;

    [SerializeField] private float levelContinueDelay;
    [SerializeField] private bool require2Players;
    

    [SerializeField] private PlayerCarSelector[] playerCarSelectors;
    [SerializeField] private CarPrefabs[] carPrefabs;

    [SerializeField] private InputActionReference startGameButton;
    private List<PlayerInstance> players = new List<PlayerInstance>();

    private bool gameStarted = false;
    private void Awake()
    {
        playerManager.OnPlayerInstanceCreated += OnPlayerCreated;
        startGameButton.action.performed += OnStartGameAttempted;
        gameScoreManager.OnScoresUpdated += OnScoresUpdated;
    }

    private void OnPlayerCreated(PlayerInstance player)
    {
        players.Add(player);
        var selection = carPrefabs[0];
        playerCarSelectors[player.PlayerIndex].SetCarVisual(selection.LobbyPrefab);
        player.CarPrefab = selection.GamePrefab;
    }

    private void OnStartGameAttempted(InputAction.CallbackContext _)
    {
        if (gameStarted) return;
        gameStarted = true;
        if (playerManager.PlayerCount > 1 || require2Players == false)
        {
            StartNewLevel();
        }
    }

    private void StartNewLevel()
    {
        GameObject newLevel = levelManager.LoadNewGameplayLevel();
        var spawnPointsProvider = newLevel.GetComponentInChildren<LevelSpawnPointsProvider>();
        for (int i = 0; i < playerManager.PlayerCount; i++)
        {
            GameObject playerCar = playerManager.GetPlayerInstance(i).SpawnCharacter(spawnPointsProvider.SpawnPoints[i]);
        }
    }

    private void OnScoresUpdated()
    {
        int aliveCharacters = 0;
        PlayerInstance currentWinner = null;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].HasActiveCharacter())
            {
                aliveCharacters += 1;
            }
            if (aliveCharacters > 1) return;
            currentWinner = players[i];
        }
        StartCoroutine(ContinueToNextLevelAfterDelay(levelContinueDelay));
    }

    private IEnumerator ContinueToNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].DestroyCharacter();
        }
        StartNewLevel();
    }
}
