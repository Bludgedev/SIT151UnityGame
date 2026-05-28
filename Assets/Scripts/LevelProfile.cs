using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Level Profile")]
public class LevelProfile : ScriptableObject
{
    [Header("Waves")]
    public WaveProfile[] waves;

    public PhaseBlock early;
    public PhaseBlock mid;
    public PhaseBlock late;

    [Header("Boss")]
    public GameObject bossPrefab;
    public float bossSpawnDelay = 2f;

    [Header("Audio")]
    public AudioClip music;

    [Header("Meta")]
    public string levelName;
}