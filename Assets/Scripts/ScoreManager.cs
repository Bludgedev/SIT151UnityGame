using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    // Total Score - persists across levels
    public int TotalScore { get; private set; }
    public int TotalKills { get; private set; }
    public int TotalPickups { get; private set; }

    
    // Level Score - resets each level
    public int LevelScore { get; private set; }
    public int LevelKills { get; private set; }
    public int LevelPickups { get; private set; }

    private float levelStartTime;
    public float LevelTime => Time.time - levelStartTime;

    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // -------------------------
    // LEVEL FLOW
    // -------------------------

    public void StartLevel()
    {
        LevelScore = 0;
        LevelKills = 0;
        LevelPickups = 0;
        levelStartTime = Time.time;

        OnScoreChanged?.Invoke(LevelScore);
    }

    public void EndLevel()
    {
        // Roll level into total
        TotalScore += LevelScore;
        TotalKills += LevelKills;
        TotalPickups += LevelPickups;
    }

    // -------------------------
    // GAME EVENTS
    // -------------------------

    public void AddScore(int amount)
    {
        LevelScore += amount;
        OnScoreChanged?.Invoke(LevelScore);
    }

    public void RegisterKill()
    {
        LevelKills++;
    }

    public void RegisterPickup()
    {
        LevelPickups++;
    }

    public void UpdateScoreBoard()
    {
        OnScoreChanged?.Invoke(LevelScore);
    }

    // -------------------------
    // RESULTS
    // -------------------------

    public LevelResults GetLevelResults()
    {
        return new LevelResults
        {
            score = LevelScore,
            kills = LevelKills,
            pickups = LevelPickups,
            survivalTime = LevelTime
        };
    }

    public TotalResults GetTotalResults()
    {
        return new TotalResults
        {
            totalScore = TotalScore,
            totalKills = TotalKills,
            totalPickups = TotalPickups
        };
    }
}