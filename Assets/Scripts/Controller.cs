using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 mousePos;

    [SerializeField] private float speed = 5.0f;

    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject punchPrefab;

    public float punchLifetime = 0.2f;
    public float punchCooldown = 0.4f;   // ⬅ time between punches

    private float nextPunchTime = 0f;     // ⬅ internal timer
    private bool canHit = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
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
}
