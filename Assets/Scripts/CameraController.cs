using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Map Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (player == null) return;

        float targetX = player.position.x;
        float targetY = player.position.y;

        float clampedX = Mathf.Clamp(targetX, minX + camHalfWidth, maxX - camHalfWidth);
        float clampedY = Mathf.Clamp(targetY, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}