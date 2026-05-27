using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelResults
{
    public int score;
    public int kills;
    public int pickups;
    public float survivalTime;
}

[System.Serializable]
public struct TotalResults
{
    public int totalScore;
    public int totalKills;
    public int totalPickups;
}