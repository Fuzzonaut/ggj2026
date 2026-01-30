using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 3;
    public int maxHealth = 5;

    // This method is called by the pill
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Ensure we don't go over max health
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Health is now: " + currentHealth);
    }
}