using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 25;

    [Header("Lifetime")]
    public float lifetime = 2f;

    [Header("Knockback (Optional)")]
    public float knockbackForce = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Damage
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Knockback
            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }

        // Optional: destroy if it hits walls
        //if (other.CompareTag("Wall"))
        //{
            //Destroy(gameObject);
        //}
    }
}
