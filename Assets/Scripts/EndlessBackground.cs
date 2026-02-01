using UnityEngine;

public class SmoothEndlessTile : MonoBehaviour
{
    public Transform cameraTransform;
    public float tileSize;
    public float followSpeed = 1f;

    void Update()
    {
        Vector3 cam = cameraTransform.position;
        Vector3 pos = transform.position;

        // Smooth follow (very slight, prevents snapping feel)
        pos = Vector3.Lerp(pos, pos, followSpeed * Time.deltaTime);

        // X axis wrap
        float dx = cam.x - pos.x;
        if (dx > tileSize)
            pos.x += tileSize * 2f;
        else if (dx < -tileSize)
            pos.x -= tileSize * 2f;

        // Y axis wrap
        float dy = cam.y - pos.y;
        if (dy > tileSize)
            pos.y += tileSize * 2f;
        else if (dy < -tileSize)
            pos.y -= tileSize * 2f;

        transform.position = pos;
    }
}
