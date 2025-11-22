using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Arrows : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed = 5f;
    public float jumpPower = 12f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    public int extraJumps = 1;
    private int jumpCounter;

    [Header("Layers")]
    public LayerMask groundLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Get Arrow keys input
        float horizontalInput = Input.GetAxisRaw("Horizontal_Arrows"); // Left/Right arrows
        bool jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);

        // Flip sprite
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Update animator
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());

        // Jump
        if (jumpPressed)
            Jump();

        // Adjustable jump height
        if (Input.GetKeyUp(KeyCode.UpArrow) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);

        // Horizontal movement
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        // Update coyote time and extra jumps
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpCounter = extraJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0) return;

        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);

        if (!IsGrounded())
            jumpCounter--;

        coyoteCounter = 0;
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
}