using UnityEngine;

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Collision Damage")]
    [SerializeField] private int collisionDamage = 10;
    [SerializeField] private float damageCooldown = 0.5f; // prevents instant-drain

    private float nextDamageTime = 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // If you use triggers instead, use OnTriggerStay2D
        if (!collision.gameObject.CompareTag("Enemy")) return;

        if (Time.time >= nextDamageTime)
        {
            TakeDamage(collisionDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // TODO: game over, respawn, disable movement, etc.
        // Destroy(gameObject);
    }
}
