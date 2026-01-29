using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb;
    [Range(1, 10)]
    [SerializeField] private float speed = 10f;


    [Range(1, 10)]
    [SerializeField] private float lifeTime = 3f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.up * speed;

    }
}
