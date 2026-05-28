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
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float screenPadding = 1f;
    [SerializeField] private float spawnYOffset = 2f;


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

    private void SpawnEnemy(EnemyType type, FormationType formation = FormationType.Single, int count = 1)
    {
        if (!running) return;

        if (!prefabMap.TryGetValue(type, out GameObject prefab))
        {
            Debug.LogWarning($"No prefab registered for {type}");
            return;
        }

        //  Guard for using only Snakes in the Flank formation
        if (formation == FormationType.Flank && type != EnemyType.Snake)
        {
            Debug.LogWarning("Flank formation is only allowed for Snake. Falling back to Single.");
            formation = FormationType.Single;
        }

        //  Guard for using only Flank formation with Snakes
        if (type == EnemyType.Snake && formation != FormationType.Flank)
        {
            Debug.LogWarning("Snake is temporarily restricted to Flank formation. Overriding.");

            formation = FormationType.Flank;
        }


        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        Vector3 camPos = mainCamera.transform.position;

        float leftBound = camPos.x - camWidth + screenPadding;
        float rightBound = camPos.x + camWidth - screenPadding;

        float spawnY = camPos.y + camHeight + spawnYOffset;

        float centerX = Random.Range(leftBound, rightBound);

        
        // -----------------------------
        // SINGLE
        // -----------------------------
        if (formation == FormationType.Single)
        {
            Vector3 pos = new Vector3(centerX, spawnY, 0f);
            Instantiate(prefab, pos, Quaternion.identity);
            return;
        }

        // -----------------------------
        // LINE
        // -----------------------------
        if (formation == FormationType.Line)
        {
            float spacing = 1.2f;

            float totalWidth = (count - 1) * spacing;
            float startX = centerX - totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                float x = startX + i * spacing;
                Vector3 pos = new Vector3(x, spawnY, 0f);

                Instantiate(prefab, pos, Quaternion.identity);
            }

            return;
        }

        // -----------------------------
        // V SHAPE
        // -----------------------------
        if (formation == FormationType.VShape)
        {
            float spacing = 1.0f;

            for (int i = 0; i < count; i++)
            {
                int row = i / 2;          // depth
                int side = (i % 2 == 0) ? -1 : 1;

                float xOffset = side * row * spacing;
                float yOffset = -row * 0.6f;

                Vector3 pos = new Vector3(centerX + xOffset, spawnY + yOffset, 0f);

                Instantiate(prefab, pos, Quaternion.identity);
            }

            return;
        }

        // -----------------------------
        // FLANK - exclusive to Snakes
        // -----------------------------
        if (formation == FormationType.Flank)
        {
            

            float spawnYLocal = camPos.y + camHeight + spawnYOffset + 1f;

            bool spawnLeft = Random.value > 0.5f;

            float x = spawnLeft
                ? camPos.x - camWidth - screenPadding
                : camPos.x + camWidth + screenPadding;

            Vector3 basePos = new Vector3(x, spawnYLocal, 0f);

            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(0f, -i * 0.6f, 0f);
                Instantiate(prefab, basePos + offset, Quaternion.identity);
            }

            return;
        }

        // -----------------------------
        // A SHAPE
        // -----------------------------
        if (formation == FormationType.AShape)
        {
            float spacing = 1.0f;

            for (int i = 0; i < count; i++)
            {
                float offset = i * spacing;

                float xOffset = (i % 2 == 0) ? offset : -offset;
                float yOffset = -i * 0.3f;

                Vector3 pos = new Vector3(centerX + xOffset, spawnY + yOffset, 0f);

                Instantiate(prefab, pos, Quaternion.identity);
            }

            return;
        }



        // -----------------------------
        // DEFAULT FALLBACK
        // -----------------------------
        Vector3 fallback = new Vector3(centerX, spawnY, 0f);
        Instantiate(prefab, fallback, Quaternion.identity);
    }

    // Optional helpers for LevelManager/debug
    public int GetCurrentWaveIndex() => currentWaveIndex;
    public int GetTotalWaves() => waves.Count;

    public enum FormationType
    {
        Single,
        Line,
        VShape,
        Flank,
        AShape
    }


}