using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    
    // SmoothTime is approximately the time it takes to reach the target.
    // 0.1f = Fast/Snappy, 0.3f = Heavy/Cinematic
    public float smoothTime = 0.2f; 
    
    // This variable is used by Unity for internal calculations (don't touch it)
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // SmoothDamp is superior to Lerp for cameras
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );
    }
}