using UnityEngine;

public class PlayersHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Collision Damage")]
    [SerializeField] private int collisionDamage = 10;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private float nextDamageTime = 0f;
    private Rigidbody2D rb;
    private bool isKnockedBack;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;

        if (Time.time >= nextDamageTime)
        {
            // Calculate direction: from enemy to player
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;

            TakeDamage(collisionDamage, knockbackDirection);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(int amount, Vector2 direction)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player HP: " + currentHealth);

        // Apply Knockback
        ApplyKnockback(direction);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Vector2 direction)
    {
        if (rb == null) return;

        // Reset velocity to ensure the knockback is consistent
        rb.linearVelocity = Vector2.zero;

        // Add an impulse force
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        // Optional: Start a timer to disable player movement script during knockback
        // StartCoroutine(KnockbackTimer()); 
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Disable this script or handle game over logic
    }
}