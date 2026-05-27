using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreBoard;

    private void Start()
    {
        if (scoreBoard == null)
            scoreBoard = GameObject.Find("ScoreBoard")?.GetComponent<TMP_Text>();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(ScoreManager.Instance.Score);
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