
using UnityEngine;

public class firingPointControl : MonoBehaviour
{

   
    [SerializeField] private Transform firingPoint;

    private float mx;
    private float my;
    private Vector2 mousePos;

    void Awake()
    {

    }


    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg - 90f;
        transform.localRotation = Quaternion.Euler(0, 0, angle);


    }


}
