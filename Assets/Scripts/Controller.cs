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

    public ExplosionSFX explosionSFX;

    [Header("Projectile Shoot (Mouse Button 1)")]
    public float shootThreshold = 66f;
    public Transform shootPoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float shootCooldown = 0.2f;
    private float nextShootTime = 0f;

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

        // Hold left click for area attack
        if (Input.GetMouseButton(0))
        {
            holdTimer += Time.deltaTime;
        }
        else
        {
            holdTimer = 0f;
        }

        if (Input.GetMouseButton(0) && holdTimer >= timeToStartAreaAttack)
        {
            if (insanityManager != null && insanityManager.insanity >= areaAttackThreshold)
            {
                if (Time.time >= nextAreaAttackTime)
                {
                    PerformAreaAttack();
                    nextAreaAttackTime = Time.time + areaCooldown;
                }
            }
        }

        // Tap left click for punch
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

        // Right click shooting (Mouse Button 1)
        if (Input.GetMouseButtonDown(1))
        {
            TryShootProjectile();
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

    void TryShootProjectile()
    {
        if (insanityManager == null) return;
        if (insanityManager.insanity < shootThreshold) return;
        if (projectilePrefab == null || shootPoint == null) return;
        if (Time.time < nextShootTime) return;

        nextShootTime = Time.time + shootCooldown;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D prb = proj.GetComponent<Rigidbody2D>();
        if (prb != null)
        {
            // In your rotation system, "up" is the forward direction (because you used -90).
            Vector2 dir = shootPoint.up;
            prb.linearVelocity = dir * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Projectile prefab has no Rigidbody2D!");
        }
    }

    void PerformAreaAttack()
    {
        Debug.Log("BOOM! Area Attack Triggered.");

        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSFX != null)
        {
            explosionSFX.PlayExplosion();
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaRadius);

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
