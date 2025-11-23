using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Camera Movement")]
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float zoomLimiter = 10f;
    [SerializeField] private float zoomSpeed = 5f;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        UpdatePosition();
        UpdateZoom();
    }

    private void UpdatePosition()
    {
        // Always focus exactly on the midpoint — the CENTER of the action
        Vector3 midpoint = (player1.position + player2.position) / 2f;

        Vector3 targetPos = new Vector3(
            midpoint.x,
            midpoint.y,
            transform.position.z // Keep the camera's Z distance
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    private void UpdateZoom()
    {
        float distance = Vector2.Distance(player1.position, player2.position);

        // Convert distance -> zoom value  
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
