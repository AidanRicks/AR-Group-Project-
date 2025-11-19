using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float movementDistance = 3f;
    [SerializeField] private float speed = 2f;

    private bool movingLeft = true;
    private float leftEdge;
    private float rightEdge;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; 

        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position;

        if (movingLeft)
        {
            newPos.x -= speed * Time.fixedDeltaTime;
            if (newPos.x <= leftEdge)
            {
                newPos.x = leftEdge;
                movingLeft = false;
            }
        }
        else
        {
            newPos.x += speed * Time.fixedDeltaTime;
            if (newPos.x >= rightEdge)
            {
                newPos.x = rightEdge;
                movingLeft = true;
            }
        }

        rb.MovePosition(newPos);
    }
}
