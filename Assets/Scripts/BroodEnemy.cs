using UnityEngine;

public class BroodEnemy : MonoBehaviour
{
    [Header("Minion Prefab (Base Enemy)")]
    public GameObject baseEnemyPrefab;

    [Header("Batch Spawn Settings")]
    public int herdSize = 5;                 // spawn 5 at a time
    public float timeBetweenBatches = 3.0f;  // wait time then spawn again
    public float firstBatchDelay = 0.5f;

    [Header("Spawn Position")]
    public float spawnRadius = 2.0f;

    [Header("Minion Tuning")]
    [Range(0.1f, 1f)] public float minionScaleMultiplier = 0.65f;
    [Range(0f, 1f)] public float minionDamageMultiplier = 0.5f;

    [Header("Limit (Optional)")]
    public int maxAliveMinions = 20;

    private float nextBatchTime;

    void Start()
    {
        nextBatchTime = Time.time + firstBatchDelay;
    }

    void Update()
    {
        if (baseEnemyPrefab == null) return;

        if (Time.time >= nextBatchTime)
        {
            int canSpawn = herdSize;

            if (maxAliveMinions > 0)
            {
                int alive = CountAliveMinions();
                int spaceLeft = Mathf.Max(0, maxAliveMinions - alive);
                canSpawn = Mathf.Min(herdSize, spaceLeft);
            }

            for (int i = 0; i < canSpawn; i++)
              

            nextBatchTime = Time.time + timeBetweenBatches;
        }
    }


    void SpawnOneMinion()
    {
        // scatter them a bit around the brood
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector2 pos = (Vector2)transform.position + offset;

        GameObject minion = Instantiate(baseEnemyPrefab, pos, Quaternion.identity);

        // smaller
        minion.transform.localScale *= minionScaleMultiplier;

        

        // tag for counting
        MinionMarker marker = minion.GetComponent<MinionMarker>();
        if (marker == null) marker = minion.AddComponent<MinionMarker>();
        marker.owner = this;
    }


    int CountAliveMinions()
    {
        MinionMarker[] markers = FindObjectsOfType<MinionMarker>();
        int count = 0;
        for (int i = 0; i < markers.Length; i++)
            if (markers[i] != null && markers[i].owner == this) count++;
        return count;
    }
}

public class MinionMarker : MonoBehaviour
{
    public BroodEnemy owner;
}
