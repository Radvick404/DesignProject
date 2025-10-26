using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public Transform target;                         // Player to follow
    public Vector3 offset = new Vector3(-10, 10, -10); // Isometric-style offset
    public float followSpeed = 5f;                   // Follow smoothing

    private Quaternion fixedRotation = Quaternion.Euler(35, 45, 0); // Isometric angle

    void Start()
    {
        // Set the camera to the fixed isometric angle
        transform.rotation = fixedRotation;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Follow player position with offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Keep rotation locked to isometric view
        transform.rotation = fixedRotation;
    }
}
