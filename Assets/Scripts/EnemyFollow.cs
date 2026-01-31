using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopDistance = 0.9f;     // stop before touching player
    [SerializeField] private float slowDistance = 2.0f;     // start slowing down

    [Header("Separation (Anti-clump)")]
    [SerializeField] private float separationRadius = 0.7f;
    [SerializeField] private float separationStrength = 2.0f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Movement Smoothing")]
    [SerializeField] private float steering = 12f;          // higher = snappier
    [SerializeField] private float maxSpeed = 3.0f;

    private Rigidbody2D rb;
    private Collider2D myCol;

    private readonly Collider2D[] hits = new Collider2D[24];

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCol = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 pos = rb.position;
        Vector2 toPlayer = (Vector2)player.position - pos;
        float dist = toPlayer.magnitude;

        // --- Stop distance logic (don’t stack on player) ---
        float speedFactor = 1f;

        if (dist <= stopDistance)
        {
            // inside stop radius: don’t move toward player
            speedFactor = 0f;
        }
        else if (dist < slowDistance)
        {
            // smoothly slow down as you approach
            speedFactor = Mathf.InverseLerp(stopDistance, slowDistance, dist);
        }

        Vector2 chaseDir = (dist > 0.0001f) ? (toPlayer / dist) : Vector2.zero;

        // --- Separation (push away from nearby enemies) ---
        Vector2 sep = Vector2.zero;
        int count = Physics2D.OverlapCircleNonAlloc(pos, separationRadius, hits, enemyLayer);

        for (int i = 0; i < count; i++)
        {
            Collider2D c = hits[i];
            if (c == null || c == myCol) continue;

            Vector2 away = pos - (Vector2)c.transform.position;
            float d = away.magnitude;
            if (d < 0.0001f) continue;

            // stronger when closer (1/d^2)
            sep += away / (d * d);
        }

        if (sep != Vector2.zero) sep = sep.normalized;

        // --- Combine like “swarm” steering ---
        Vector2 desiredDir = chaseDir + sep * separationStrength;
        if (desiredDir != Vector2.zero) desiredDir = desiredDir.normalized;

        float targetSpeed = moveSpeed * speedFactor;
        Vector2 desiredVel = desiredDir * targetSpeed;

        // Smooth velocity change (prevents jitter)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVel, steering * Time.fixedDeltaTime);

        // Hard clamp (optional)
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

 

}
