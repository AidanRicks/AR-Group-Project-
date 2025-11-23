using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public enum MovementAxis { Horizontal, Vertical }

    [Header("Platform Settings")]
    [SerializeField] private MovementAxis axis = MovementAxis.Horizontal; // Switch between horizontal and vertical
    [SerializeField] private float movementDistance = 3f;
    [SerializeField] private float speed = 2f;

    private bool movingForward = true;
    private float startPos; // starting position along chosen axis
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = (axis == MovementAxis.Horizontal) ? transform.position.x : transform.position.y;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position;

        if (axis == MovementAxis.Horizontal)
        {
            if (movingForward)
                newPos.x += speed * Time.fixedDeltaTime;
            else
                newPos.x -= speed * Time.fixedDeltaTime;

            if (newPos.x >= startPos + movementDistance)
            {
                newPos.x = startPos + movementDistance;
                movingForward = false;
            }
            else if (newPos.x <= startPos - movementDistance)
            {
                newPos.x = startPos - movementDistance;
                movingForward = true;
            }
        }
        else // Vertical movement
        {
            if (movingForward)
                newPos.y += speed * Time.fixedDeltaTime;
            else
                newPos.y -= speed * Time.fixedDeltaTime;

            if (newPos.y >= startPos + movementDistance)
            {
                newPos.y = startPos + movementDistance;
                movingForward = false;
            }
            else if (newPos.y <= startPos - movementDistance)
            {
                newPos.y = startPos - movementDistance;
                movingForward = true;
            }
        }

        rb.MovePosition(newPos);
    }
}

