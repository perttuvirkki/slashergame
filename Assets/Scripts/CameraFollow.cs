using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform; // Drag your player's Transform component here in the inspector
    public Vector3 offset; // The offset distance between the player and the camera
    public float smoothSpeed = 0.007f; // This controls the rate of smoothing

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 desiredPosition = playerTransform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
