using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ScorecardText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameScoreManager gameScoreManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private String formatPerPlayer;
        private StringBuilder finalText = new StringBuilder();
    private void Update()
    {
        finalText.Clear();
        finalText.AppendLine("SCORES:");
        int playerCount = playerManager.PlayerCount;
        for (int i = 0; i < playerCount; i++)
        {
            var player = playerManager.GetPlayerInstance(i);
            int score = gameScoreManager.GetScore(player);
            finalText.AppendLine(string.Format(formatPerPlayer, i + 1, score));
        }
        text.text = finalText.ToString();
    }
}
