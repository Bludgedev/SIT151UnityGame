using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Timing")]
    public float levelDuration = 300f;

    private float timer;
    private bool running;

    [Header("References")]
    public WaveSpawner waveSpawner;
    [SerializeField] private LevelProfile level;



    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        timer = levelDuration;
        running = true;

        
    }

    void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        float t = timer / levelDuration;

       
    }

    

    void EnterBossPhase()
    {
        
    }

    public void BossDefeated()
    {
        running = false;
        Debug.Log("[LEVEL COMPLETE]");
        // trigger victory
    }
}