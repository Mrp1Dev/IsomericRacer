using System;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private Dictionary<PlayerInstance, int> playerScores = new Dictionary<PlayerInstance, int>();

    public event Action OnScoresUpdated;

    private void Awake()
    {
        playerManager.OnPlayerInstanceCreated += OnPlayerCreated;
    }

    private void OnPlayerCreated(PlayerInstance player)
    {
        playerScores.Add(player, 0);
        player.OnPlayerCharacterSpawned += (car) =>
        {
            car.GetComponent<HealthSystem>().OnDeathByPossiblePlayer += (by) =>
            {
                if (by == null) return;
                if (by == player)
                {
                    playerScores[by]--;
                    if (playerScores[by] < 0) playerScores[by] = 0;
                }
                else
                {
                    playerScores[by]++;
                }
                OnScoresUpdated?.Invoke();
            };
        };
    }

    public int GetScore(PlayerInstance player) => playerScores[player];
}
