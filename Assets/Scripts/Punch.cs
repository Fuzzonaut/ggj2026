using UnityEngine;

public class Punch : MonoBehaviour
{
    public int punchDamage = 25;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(punchDamage);
            }
        }
    }
}
