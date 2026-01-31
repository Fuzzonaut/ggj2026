using UnityEngine;
using TMPro; // ✅ ADDED

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Collision Damage (Touching Enemies)")]
    [SerializeField] private int collisionDamage = 10;
    [SerializeField] private float damageCooldown = 0.5f;
    private float nextDamageTime = 0f;

    [Header("UI (Real Time)")]               // ✅ ADDED
    public TMP_Text healthText;              // ✅ ADDED

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()                            // ✅ ADDED
    {
        UpdateHealthUI();                    // ✅ ADDED
    }

    // ✅ ADDED
    private void UpdateHealthUI()
    {
        if (healthText == null) return;

        if (currentHealth <= 0)
            healthText.text = "HP: 0 (DEAD)";
        else
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    // This is called by Exploding Enemies AND standard collision
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Prevent health from going below 0
        if (currentHealth < 0) currentHealth = 0;

        // Since we have no UI, we log to console
        Debug.Log("Ouch! Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Handles damage when touching a normal enemy continuously
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= nextDamageTime)
            {
                TakeDamage(collisionDamage);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER DIED!");
        // Add logic here later (Restart Level / Show Game Over Screen)

        // Optional: Disable player movement so they can't walk while dead
        GetComponent<Controller>().enabled = false;

        // Optional: Change sprite to dead version or play animation
    }
}
