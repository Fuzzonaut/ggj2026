using UnityEngine;

public class Pill : MonoBehaviour
{
    public int healthAmount = 1; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with is the Player
        if (other.CompareTag("Player"))
        {
            // 1. Find the health script on the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                
                // 3. Destroy this pill object so it disappears
                Destroy(gameObject);
            }
        }
    }
