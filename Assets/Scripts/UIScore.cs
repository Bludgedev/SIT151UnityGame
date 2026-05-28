using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreBoard;

    private void Start()
    {
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(ScoreManager.Instance.LevelScore);
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreBoard != null)
            scoreBoard.text = $"Score: {score}";
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateScore;
    }
}