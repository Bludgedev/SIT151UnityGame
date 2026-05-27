using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelDuration = 300f;

    [Header("References")]
    public WaveSpawner waveSpawner;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    private float timer;
    private bool levelRunning;
    private bool bossActive;

    private GameObject currentBoss;

    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        timer = levelDuration;
        levelRunning = true;
        bossActive = false;

        waveSpawner.Begin();
    }

    void Update()
    {
        if (!levelRunning)
            return;

        // 1. Player fail check (hook this properly later via PlayerHealth event)
        if (PlayerIsDead())
        {
            EndLevel(false);
            return;
        }

        // 2. Timer phase (waves)
        if (!bossActive)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StartBossPhase();
            }
        }

        // 3. Boss win condition
        if (bossActive && currentBoss == null)
        {
            EndLevel(true);
        }
    }

    void StartBossPhase()
    {
        bossActive = true;

        waveSpawner.StopSpawning();

        Vector3 spawnPos = bossSpawnPoint != null
            ? bossSpawnPoint.position
            : new Vector3(0f, 6f, 0f);

        currentBoss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
    }

    public void EndLevel(bool success)
    {
        if (!levelRunning)
            return;

        levelRunning = false;

        waveSpawner.StopSpawning();

        Debug.Log(success
            ? "[LEVEL COMPLETE] Player Wins"
            : "[LEVEL FAILED] Player Died");

        // TODO:
        // stats.FinishRun(success);
        // UIEndScreen.Show(...)
    }

    bool PlayerIsDead()
    {
        // Replace this with your real health reference later
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return true;

        var hp = player.GetComponent<PlayerHealth>();
        if (hp == null) return false;

        return hp.IsDead; // assume you expose this bool
    }

    public void OnBossDestroyed()
    {
        if (bossActive)
        {
            currentBoss = null;
        }
    }

    public float GetProgressPercent()
    {
        return Mathf.Clamp01(1f - (timer / levelDuration));
    }
}