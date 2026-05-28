using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhaseBlock
{
    public float startTime;              // when this phase begins
    public WaveProfile[] waves;          // pool of waves for this phase
    public float spawnInterval = 5f;     // how often to trigger waves
}