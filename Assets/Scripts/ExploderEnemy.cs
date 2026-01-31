using UnityEngine;

public class ExploderEnemy : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 4f;
    public float stopDistance = 1.0f; // How close before exploding
    public int damage = 20;
    public GameObject explosionEffect; // Drag your particle effect here

    private Transform player;
    private bool hasExploded = false;


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
        // Find the player automatically by Tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        // 1. Move towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // 2. Check distance
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stopDistance)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // 1. Look for the script with the CORRECT name (PlayerHealth2D)
        PlayerHealth2D ph = player.GetComponent<PlayerHealth2D>();
        
        if (ph != null)
        {
            ph.TakeDamage(damage); 
        }
        

        Destroy(gameObject);
    }
}