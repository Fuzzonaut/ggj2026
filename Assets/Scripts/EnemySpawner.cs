using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject standardEnemyPrefab;
    public GameObject explodingEnemyPrefab;
    public InsanityManager insanityManager;
    public Transform playerTransform;

    // REMOVED: public Collider2D spawnArea; 
    
    [Header("Spawn Distance (The Donut)")]
    public float minSpawnDistance = 10f; // Minimum distance from player
    public float maxSpawnDistance = 15f; // Maximum distance from player

    [Header("Spawn Settings")]
    public float spawnRateAtZeroInsanity = 5f;
    public float spawnRateAtMaxInsanity = 0.5f;

    private float nextSpawnTime;

    void Update()
    {
        if (playerTransform == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            CalculateNextSpawnTime();
        }
    }

    void SpawnEnemy()
    {
        // 1. Calculate a random position around the player
        // Get a random direction (like a compass heading)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        // Pick a random distance between Min and Max
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        
        // Final position = Player Position + (Direction * Distance)
        Vector2 spawnPos = (Vector2)playerTransform.position + (randomDirection * randomDistance);

        // 2. Decide WHICH enemy to spawn (Logic from previous step)
        GameObject enemyToSpawn = standardEnemyPrefab;
        if (insanityManager.insanity >= 66)
        {
            if (Random.value > 0.5f) enemyToSpawn = explodingEnemyPrefab;
        }

        // 3. Spawn it
        Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
    }

    void CalculateNextSpawnTime()
    {
        float insanityPercent = insanityManager.insanity / insanityManager.maxInsanity;
        float delay = Mathf.Lerp(spawnRateAtZeroInsanity, spawnRateAtMaxInsanity, insanityPercent);
        nextSpawnTime = Time.time + delay;
    }

    // Visualize the spawn ring in the Editor
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnDistance);
        }
    }
}