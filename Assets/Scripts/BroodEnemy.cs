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

    [Header("Limit (Optional)")]
    public int maxAliveMinions = 20;

    [Header("Movement")]
    public float speed = 4f;

    [Tooltip("Brood stops moving when player is within this distance.")]
    public float stopDistance = 6f;

    [Tooltip("Brood resumes moving when player is farther than this distance.")]
    public float resumeDistance = 8f;

    private float nextBatchTime;
    private Transform player;

    private Rigidbody2D rb;
    private Collider2D myCol;

    private bool isFollowing = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCol = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    void Start()
    {
        nextBatchTime = Time.time + firstBatchDelay;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Safety: ensure resumeDistance > stopDistance
        if (resumeDistance <= stopDistance)
            resumeDistance = stopDistance + 0.5f;
    }

    void Update()
    {
        if (player == null) return;

        // 1) Decide follow/stop based on distance (with hysteresis)
        float dist = Vector2.Distance(transform.position, player.position);

        if (isFollowing && dist <= stopDistance)
            isFollowing = false; // reached spawn area -> stop

        else if (!isFollowing && dist >= resumeDistance)
            isFollowing = true; // got far -> follow again

        // 2) Spawning batches (you can keep spawning while stopped)
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
            {
                SpawnOneMinion();
            }

            nextBatchTime = Time.time + timeBetweenBatches;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (isFollowing)
        {
            Vector2 nextPos = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);
        }
        // else: do nothing -> stays still
    }

    void SpawnOneMinion()
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector2 pos = (Vector2)transform.position + offset;

        GameObject minion = Instantiate(baseEnemyPrefab, pos, Quaternion.identity);

        // smaller
        minion.transform.localScale *= minionScaleMultiplier;

        // mark for counting
        MinionMarker marker = minion.GetComponent<MinionMarker>();
        if (marker == null) marker = minion.AddComponent<MinionMarker>();
        marker.owner = this;
    }

    int CountAliveMinions()
    {
        MinionMarker[] markers = FindObjectsOfType<MinionMarker>();
        int count = 0;

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] != null && markers[i].owner == this)
                count++;
        }

        return count;
    }

    public class MinionMarker : MonoBehaviour
    {
        public BroodEnemy owner;
    }
}
