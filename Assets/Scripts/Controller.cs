using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Camera mainCam; // Need camera to find mouse

    [SerializeField] private float speed = 5.0f;

    [Header("References")]
    public InsanityManager insanityManager;
    public ExplosionSFX explosionSFX;

    [Header("Punch")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject punchPrefab;
    public float punchLifetime = 0.2f;
    private bool canHit = true;

    [Header("AreaAttack")]
    public float areaAttackThreshold = 33f; 
    public float areaRadius = 3.5f;        
    public int areaDamage = 50;            
    public float areaCooldown = 1.0f;      
    public GameObject areaEffectPrefab;    
    private float nextAreaAttackTime = 0f;

    public float timeToStartAreaAttack = 0.5f;
    private float holdTimer = 0f;

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

    // NEW: We track where we are looking separately from moving
    private Vector2 lookDirection = Vector2.down; 
    private Vector2 mousePos;
    
    public float attackPointOffset = 1.0f; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        mainCam = Camera.main; // Cache the camera for better performance
    }

    void Update()
    {
        // 1. Calculate Mouse Position first (We need this for everything)
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 2. Calculate the direction from Player -> Mouse
        Vector2 directionToMouse = (mousePos - (Vector2)transform.position).normalized;
        lookDirection = directionToMouse;

        if (!isKnocked)
            Movement();

        // 3. Update Gun/Fist Rotation to face Mouse
        UpdateAttackPointRotation();

        // --- ATTACK LOGIC ---

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

        if (Input.GetMouseButtonDown(1))
        {
            TryShootProjectile();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); 
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 1. Move based on Keyboard (WASD)
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        rb.linearVelocity = movement * speed;

        // 2. Animate based on MOUSE (Look Direction)
        if (anim != null)
        {
            // Update the Blend Tree to face the mouse
            anim.SetFloat("InputX", lookDirection.x);
            anim.SetFloat("InputY", lookDirection.y);

            // 3. Handle Animation Speed (Legs moving)
            // If we are moving via keyboard, play animation. 
            // If standing still but looking around, freeze animation frame.
            if (movement.magnitude > 0.01f)
            {
                anim.speed = 1; // Play "Run" animation (Strafing)
            }
            else
            {
                anim.speed = 0; // Freeze frame (Standing still facing mouse)
            }
        }
    }

    void UpdateAttackPointRotation()
    {
        // Use lookDirection (Mouse) instead of movement direction
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

        Vector3 offsetPosition = (Vector3)lookDirection * attackPointOffset;

        if (shootPoint != null)
        {
            shootPoint.position = transform.position + offsetPosition;
            shootPoint.rotation = rotation;
        }

        if (firingPoint != null)
        {
            firingPoint.position = transform.position + offsetPosition;
            firingPoint.rotation = rotation;
        }
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
            // Shoot towards the mouse (shootPoint.up is now facing mouse)
            prb.linearVelocity = shootPoint.up * projectileSpeed;
        }
    }

    // ... (Keep Punch, AreaAttack, DrawGizmos, and Knockback logic exactly the same below) ...
    
    void Punch()
    {
        GameObject punch = Instantiate(punchPrefab, firingPoint.position, firingPoint.rotation);
        Destroy(punch, punchLifetime);
    }

    void PerformAreaAttack()
    {
        if (areaEffectPrefab != null) Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        if (explosionSFX != null) explosionSFX.PlayExplosion();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyHealth healthScript = enemy.GetComponent<EnemyHealth>();
                if (healthScript != null) healthScript.TakeDamage(areaDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 dir = collision.GetContact(0).normal;
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