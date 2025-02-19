using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The car to follow
    public Vector3 offset = new Vector3(0, 0, -100); // Camera position relative to the car
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate the desired position for the camera
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera look at the car
        transform.LookAt(target);
    }
}