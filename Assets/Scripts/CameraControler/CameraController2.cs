using UnityEngine;

public class CameraController2 : MonoBehaviour
{
    // Tags for the players. Ensure your prefabs are tagged correctly!
    private const string Player1Tag = "Player1";
    private const string Player2Tag = "Player2";

    [Header("Camera Movement")]
    [Tooltip("Smoothing factor for camera position.")]
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom Settings")]
    [Tooltip("The closest the camera will zoom in (smallest orthographic size).")]
    [SerializeField] private float minZoom = 5f;
    [Tooltip("The farthest the camera will zoom out (largest orthographic size).")]
    [SerializeField] private float maxZoom = 8f;
    [Tooltip("The player distance at which maxZoom is reached. E.g., at 10 units distance, zoom is maxZoom.")]
    [SerializeField] private float zoomLimiter = 10f;
    [Tooltip("Smoothing factor for camera zoom.")]
    [SerializeField] private float zoomSpeed = 5f;

    // Dynamic targets found in the scene
    private Transform player1Target;
    private Transform player2Target;

    private Camera cam;

    private void Start()
    {
        // Use Camera.main or GetComponent<Camera>() if the script is on the Main Camera
        cam = Camera.main;
        if (cam == null) Debug.LogError("Main Camera not found!");

        // Find the players immediately after they are spawned by GameInitializer
        FindPlayers();
    }

    private void FindPlayers()
    {
        // Find by tag. The GameInitializer must run before this for the players to exist.
        GameObject p1 = GameObject.FindGameObjectWithTag(Player1Tag);
        GameObject p2 = GameObject.FindGameObjectWithTag(Player2Tag);

        if (p1 != null)
        {
            player1Target = p1.transform;
        }
        else
        {
            Debug.LogError($"Could not find P1 target with tag '{Player1Tag}'.");
        }

        if (p2 != null)
        {
            player2Target = p2.transform;
        }
        else
        {
            Debug.LogWarning($"Could not find P2 target with tag '{Player2Tag}'. Camera will follow only P1.");
        }
    }

    private void LateUpdate()
    {
        // If both targets are null, do nothing
        if (player1Target == null && player2Target == null)
            return;

        UpdatePosition();
        UpdateZoom();
    }

    // Since targets are now class fields, we remove the arguments
    private void UpdatePosition()
    {
        Vector3 midpoint;

        if (player1Target != null && player2Target != null)
            midpoint = (player1Target.position + player2Target.position) / 2f;
        else if (player1Target != null)
            midpoint = player1Target.position;
        else if (player2Target != null)
            midpoint = player2Target.position;
        else
            return; // Should be caught by LateUpdate check, but good practice

        // Preserve the camera's original Z-depth for 2D perspective
        Vector3 targetPos = new Vector3(midpoint.x, midpoint.y, transform.position.z);

        // Use followSpeed * Time.deltaTime for frame-rate independent smoothing
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    // Since targets are now class fields, we remove the arguments
    private void UpdateZoom()
    {
        float targetZoom = minZoom;

        if (player1Target != null && player2Target != null)
        {
            float distance = Vector2.Distance(player1Target.position, player2Target.position);

            // Lerp from minZoom to maxZoom based on the distance relative to the zoomLimiter
            // Mathf.Clamp01 ensures the interpolation factor is between 0 and 1
            float lerpFactor = Mathf.Clamp01(distance / zoomLimiter);
            targetZoom = Mathf.Lerp(minZoom, maxZoom, lerpFactor);
        }
        else if (player1Target != null || player2Target != null)
        {
            targetZoom = minZoom; // Zoom in if only one player is active
        }

        // Use zoomSpeed * Time.deltaTime for frame-rate independent smoothing
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}