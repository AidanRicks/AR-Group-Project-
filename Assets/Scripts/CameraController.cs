using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player Tags")]
    [SerializeField] private string player1Tag = "Player1";
    [SerializeField] private string player2Tag = "Player2";

    [Header("Camera Movement")]
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float zoomLimiter = 10f;
    [SerializeField] private float zoomSpeed = 5f;

    private Camera cam;

    private Transform player1;
    private Transform player2;

    private void Start()
    {
        cam = Camera.main;
        RefreshPlayers();
    }

    private void LateUpdate()
    {
        // Try to refetch players if missing (useful if they spawn later)
        if (player1 == null || player2 == null)
            RefreshPlayers();

        if (player1 == null && player2 == null)
            return;

        UpdatePosition();
        UpdateZoom();
    }

    private void RefreshPlayers()
    {
        GameObject p1 = GameObject.FindGameObjectWithTag(player1Tag);
        GameObject p2 = GameObject.FindGameObjectWithTag(player2Tag);

        player1 = p1 != null ? p1.transform : null;
        player2 = p2 != null ? p2.transform : null;
    }

    private void UpdatePosition()
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

    private void UpdateZoom()
    {
        float targetZoom = minZoom;

        if (player1 != null && player2 != null)
        {
            float distance = Vector2.Distance(player1.position, player2.position);
            targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
