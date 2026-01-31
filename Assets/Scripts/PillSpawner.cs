using UnityEngine;

public class PillSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject pillPrefab;       // SanityPill prefabını buraya sürükle
    public Transform playerTransform;   // Player'ı buraya sürükle

    [Header("Settings")]
    public float spawnInterval = 10f;   // Kaç saniyede bir hap çıksın?
    public float minSpawnDistance = 5f; // Oyuncunun dibinde bitmesin
    public float maxSpawnDistance = 12f; // Çok uzakta bitmesin

    private float timer;

    void Update()
    {
        if (playerTransform == null) return;

        timer += Time.deltaTime;

        // Süre dolduysa hap oluştur
        if (timer >= spawnInterval)
        {
            SpawnPill();
            timer = 0f; // Sayacı sıfırla
        }
    }

    void SpawnPill()
    {
        // 1. Oyuncunun etrafında rastgele bir yön ve mesafe seç
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        
        // 2. Pozisyonu hesapla (Player Konumu + Rastgele Yön * Mesafe)
        Vector2 spawnPos = (Vector2)playerTransform.position + (randomDirection * randomDistance);

        // 3. Hapı oluştur
        Instantiate(pillPrefab, spawnPos, Quaternion.identity);
    }
}