using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 mousePos;

    [SerializeField] private float speed = 5.0f;

    [Header("Referance")]
    public InsanityManager insanityManager;

    [Header("Punch")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject punchPrefab;
    public float punchLifetime = 0.2f;

    private bool canHit = true;

    [Header("AreaAttack")]
    public float areaAttackThreshold = 33f; // Min insanity needed
    public float areaRadius = 3.5f;         // Range of attack
    public int areaDamage = 50;             // Damage amount
    public float areaCooldown = 1.0f;       // Time between explosions
    public GameObject areaEffectPrefab;     // Optional: Drag a particle effect here
    private float nextAreaAttackTime = 0f;

    public float timeToStartAreaAttack = 0.5f; 
    private float holdTimer = 0f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private bool isKnocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isKnocked)
            Movement();

        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;
        }
        else
        {
            holdTimer = 0f; // Reset if we let go
        }
        if (Input.GetMouseButton(0) && holdTimer >= timeToStartAreaAttack)
        {
            
            if (Input.GetMouseButton(0) && insanityManager != null && insanityManager.insanity >= areaAttackThreshold)
        {
            if (Time.time >= nextAreaAttackTime)
            {
                PerformAreaAttack();
                nextAreaAttackTime = Time.time + areaCooldown;
            }
        }

        }
        
        if (Input.GetMouseButtonDown(0) && canHit)
        {
            Punch();
            canHit = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            canHit = true;
            holdTimer = 0f;
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
            mousePos.y - transform.position.y,
            mousePos.x - transform.position.x
        ) * Mathf.Rad2Deg - 90f;

        float a = Mathf.DeltaAngle(0f, angle);
        float snapped = Mathf.Round(a / 45f) * 45f;
        if (snapped == 180f) snapped = -180f;

        transform.localRotation = Quaternion.Euler(0f, 0f, snapped);
    }
    void PerformAreaAttack()
    {
        Debug.Log("BOOM! Area Attack Triggered.");

        // 1. Visual Effect (Optional)
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Detect all colliders within radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaRadius);

        // 3. Loop through them and deal damage
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyHealth healthScript = enemy.GetComponent<EnemyHealth>();
                if (healthScript != null)
                {
                    healthScript.TakeDamage(areaDamage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
    
    void Punch()
    {
        GameObject punch = Instantiate(
            punchPrefab,
            firingPoint.position,
            firingPoint.rotation
        );

        Destroy(punch, punchLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Best direction for collisions (more reliable than positions):
            Vector2 dir = collision.GetContact(0).normal;

            Debug.Log("Knockback Dir: " + dir);

            StopAllCoroutines();
            StartCoroutine(KnockbackRoutine(dir));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        isKnocked = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }
}
