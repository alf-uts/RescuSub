using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float smoothTime = 0.2f;
    public Vector3 offset;

    [Header("Map Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private float camHalfHeight;
    private float camHalfWidth;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        Camera cam = Camera.main;

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float clampedX = Mathf.Clamp(desiredPosition.x, minX + camHalfWidth, maxX - camHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY + camHalfHeight, maxY - camHalfHeight);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, smoothTime);
    }
}