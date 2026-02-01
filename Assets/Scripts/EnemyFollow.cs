using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Separation (Anti-clump)")]
    [SerializeField] private float separationRadius = 0.7f;
    [SerializeField] private float separationStrength = 2.0f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Movement Smoothing")]
    [SerializeField] private float steering = 12f;
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

        Vector2 chaseDir = toPlayer.sqrMagnitude > 0.0001f
            ? toPlayer.normalized
            : Vector2.zero;

        // --- Separation ---
        Vector2 sep = Vector2.zero;
        int count = Physics2D.OverlapCircleNonAlloc(pos, separationRadius, hits, enemyLayer);

        for (int i = 0; i < count; i++)
        {
            Collider2D c = hits[i];
            if (c == null || c == myCol) continue;

            Vector2 away = pos - (Vector2)c.transform.position;
            float d = away.magnitude;
            if (d < 0.0001f) continue;

            sep += away / (d * d);
        }

        if (sep != Vector2.zero)
            sep = sep.normalized;

        Vector2 desiredDir = chaseDir + sep * separationStrength;
        if (desiredDir != Vector2.zero)
            desiredDir = desiredDir.normalized;

        Vector2 desiredVel = desiredDir * moveSpeed;

        rb.linearVelocity = Vector2.Lerp(
            rb.linearVelocity,
            desiredVel,
            steering * Time.fixedDeltaTime
        );

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
