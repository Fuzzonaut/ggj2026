using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject standardEnemyPrefab;
    public GameObject explodingEnemyPrefab;

    [Tooltip("NEW: Big enemy that spawns mini standard enemies.")]
    public GameObject broodEnemyPrefab;

    public InsanityManager insanityManager;
    public Transform playerTransform;

    [Header("Spawn Distance (The Donut)")]
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 15f;

    [Header("Spawn Settings")]
    public float spawnRateAtZeroInsanity = 5f;
    public float spawnRateAtMaxInsanity = 0.5f;

    [Header("Tier Chances")]
    [Range(0f, 1f)] public float chanceToSpawnBroodAt66Plus = 0.25f;
    [Range(0f, 1f)] public float chanceToSpawnExploderAt33Plus = 0.5f;

    private float nextSpawnTime;

    void Update()
    {
        if (playerTransform == null || insanityManager == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            CalculateNextSpawnTime();
        }
    }

    void SpawnEnemy()
    {
        // 1) Position around player
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector2 spawnPos = (Vector2)playerTransform.position + (randomDirection * randomDistance);

        // 2) Decide which enemy to spawn
        GameObject enemyToSpawn = standardEnemyPrefab;

        float insanity = insanityManager.insanity;

        // NEW TIER: 66+
        if (insanity >= 66f && broodEnemyPrefab != null)
        {
            if (Random.value < chanceToSpawnBroodAt66Plus)
            {
                enemyToSpawn = broodEnemyPrefab;
            }
            else if (insanity >= 33f && explodingEnemyPrefab != null)
            {
                // fallback to existing 33+ logic
                if (Random.value < chanceToSpawnExploderAt33Plus)
                    enemyToSpawn = explodingEnemyPrefab;
            }
        }
        else if (insanity >= 33f && explodingEnemyPrefab != null)
        {
            if (Random.value < chanceToSpawnExploderAt33Plus)
                enemyToSpawn = explodingEnemyPrefab;
        }

        // 3) Spawn it
        Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
    }

    void CalculateNextSpawnTime()
    {
        float insanityPercent = insanityManager.insanity / insanityManager.maxInsanity;
        float delay = Mathf.Lerp(spawnRateAtZeroInsanity, spawnRateAtMaxInsanity, insanityPercent);
        nextSpawnTime = Time.time + delay;
    }
}
