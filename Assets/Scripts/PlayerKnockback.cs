using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public float knockbackForce = 8f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();


    }



    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 knockbackDir =
                (transform.position - collision.transform.position).normalized;

            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

            Debug.Log(knockbackDir);
        }
    }


    



}


