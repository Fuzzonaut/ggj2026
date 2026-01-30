using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField] private float speed = 5.0f;
    private float mx;
    private float my;
    private Vector2 mousePos;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

    }


    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
            
        rb.linearVelocity = new Vector2 (horizontalInput * speed, verticalInput * speed);



        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        float a = Mathf.DeltaAngle(0f, angle); // normalize to [-180, 180]

        // snap to nearest 45°
        float snapped = Mathf.Round(a / 45f) * 45f;

        // optional: unify 180 and -180
        if (snapped == 180f) snapped = -180f;

        transform.localRotation = Quaternion.Euler(0f, 0f, snapped);









    }


}
