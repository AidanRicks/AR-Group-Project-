using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Players (Parent Objects)")]
    [SerializeField] private Transform player1Parent;
    [SerializeField] private Transform player2Parent;

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
        // Get the active child of each player parent
        Transform player1 = GetActiveChild(player1Parent);
        Transform player2 = GetActiveChild(player2Parent);

        // If both are null, do nothing
        if (player1 == null && player2 == null)
            return;

        UpdatePosition(player1, player2);
        UpdateZoom(player1, player2);
    }

    private Transform GetActiveChild(Transform parent)
    {
        if (parent == null) return null;

        foreach (Transform child in parent)
        {
            if (child.gameObject.activeInHierarchy)
                return child; // Return the first active child
        }

        return null; // No active children
    }

    private void UpdatePosition(Transform player1, Transform player2)
    {
        Vector3 midpoint;

        if (player1 != null && player2 != null)
            midpoint = (player1.position + player2.position) / 2f;
        else if (player1 != null)
            midpoint = player1.position;
        else
            midpoint = player2.position;

        Vector3 targetPos = new Vector3(midpoint.x, midpoint.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    private void UpdateZoom(Transform player1, Transform player2)
    {
        float targetZoom = minZoom;

        if (player1 != null && player2 != null)
        {
            float distance = Vector2.Distance(player1.position, player2.position);
            targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        }
        else if (player1 != null || player2 != null)
        {
            targetZoom = minZoom; // Zoom in if only one active child
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
