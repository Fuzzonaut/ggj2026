using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    
    // Delilik verisini okumak için Manager
    public InsanityManager insanityManager; 
    
    // Konumunu kontrol etmek için Player
    public Transform playerTransform; 
    
    // Düşmanların doğacağı alan
    public Collider2D spawnArea; 

    [Header("Spawn Settings")]
    public float spawnRateAtZeroInsanity = 5f;
    public float spawnRateAtMaxInsanity = 0.5f;

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            CalculateNextSpawnTime();
        }
    }

    void SpawnEnemy()
    {
        Bounds bounds = spawnArea.bounds;
        Vector2 spawnPos;
        
        int attempts = 0;
        do 
        {
            // Rastgele bir konum seç
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            spawnPos = new Vector2(randomX, randomY);
            attempts++;
        } 
        
        while (Vector2.Distance(spawnPos, playerTransform.position) < 3f && attempts < 10);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    void CalculateNextSpawnTime()
    {
        float insanityPercent = insanityManager.insanity / insanityManager.maxInsanity;
        float delay = Mathf.Lerp(spawnRateAtZeroInsanity, spawnRateAtMaxInsanity, insanityPercent);
        nextSpawnTime = Time.time + delay;
    }
}