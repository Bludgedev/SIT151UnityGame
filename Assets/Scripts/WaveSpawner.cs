using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Basic,
    Brute,
    Snake
}

[System.Serializable]
public class EnemyPrefabEntry
{
    public EnemyType type;
    public GameObject prefab;
}

[System.Serializable]
public class Wave
{
    [Header("Wave Composition")]
    public int basicCount = 5;
    public int bruteCount = 2;
    public int snakeCount = 1;

    [Header("Timing")]
    public float spawnDelay = 0.3f;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    [Header("Spawn Area")]
    public float spawnXRange = 6f;
    public float spawnY = 6f;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 3f;

    private Dictionary<EnemyType, GameObject> prefabMap;
    private int currentWaveIndex;
    private bool running;

    private void Awake()
    {
        prefabMap = new Dictionary<EnemyType, GameObject>();

        foreach (var entry in enemyPrefabs)
        {
            if (entry.prefab != null)
                prefabMap[entry.type] = entry.prefab;
        }
    }

    public void Begin()
    {
        currentWaveIndex = 0;
        running = true;
        StartCoroutine(SpawnWaves());
    }

    public void StopSpawning()
    {
        running = false;
        StopAllCoroutines();
    }

    

    private IEnumerator SpawnWaves()
    {
        while (running && currentWaveIndex < waves.Count)
        {
            Wave wave = waves[currentWaveIndex];

            yield return StartCoroutine(SpawnWave(wave));

            currentWaveIndex++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        running = false;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        // Basic enemies
        for (int i = 0; i < wave.basicCount; i++)
        {
            SpawnEnemy(EnemyType.Basic);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        // Brutes
        for (int i = 0; i < wave.bruteCount; i++)
        {
            SpawnEnemy(EnemyType.Brute);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        // Snakes
        for (int i = 0; i < wave.snakeCount; i++)
        {
            SpawnEnemy(EnemyType.Snake);
            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    private void SpawnEnemy(EnemyType type)
    {
        if (!running) return;

        if (!prefabMap.TryGetValue(type, out GameObject prefab))
        {
            Debug.LogWarning($"No prefab registered for {type}");
            return;
        }

        float x = Random.Range(-spawnXRange, spawnXRange);
        Vector3 pos = new Vector3(x, spawnY, 0f);

        Instantiate(prefab, pos, Quaternion.identity);
    }

    // Optional helpers for LevelManager/debug
    public int GetCurrentWaveIndex() => currentWaveIndex;
    public int GetTotalWaves() => waves.Count;
}