using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 mousePos;

    [SerializeField] private float speed = 5.0f;

    [Header("Punch")]
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject punchPrefab;
    public float punchLifetime = 0.2f;

    private bool canHit = true;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private bool isKnocked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isKnocked)
            Movement();

        if (Input.GetMouseButtonDown(0) && canHit)
        {
            Punch();
            canHit = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            canHit = true;
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
            mousePos.y - transform.position.y,
            mousePos.x - transform.position.x
        ) * Mathf.Rad2Deg - 90f;

        float a = Mathf.DeltaAngle(0f, angle);
        float snapped = Mathf.Round(a / 45f) * 45f;
        if (snapped == 180f) snapped = -180f;

        transform.localRotation = Quaternion.Euler(0f, 0f, snapped);
    }

    void Punch()
    {
        GameObject punch = Instantiate(
            punchPrefab,
            firingPoint.position,
            firingPoint.rotation
        );

        Destroy(punch, punchLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Best direction for collisions (more reliable than positions):
            Vector2 dir = collision.GetContact(0).normal;

            Debug.Log("Knockback Dir: " + dir);

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
