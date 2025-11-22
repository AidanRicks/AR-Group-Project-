using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed = 5f;
    public float jumpPower = 12f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f; // Time player can jump after leaving ground
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    public int extraJumps = 1;
    private int jumpCounter;

    [Header("Layers")]
    public LayerMask groundLayer;

    [HideInInspector] public float horizontalInput; // Controlled externally
    [HideInInspector] public bool jumpPressed;      // Controlled externally

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
        // Flip sprite based on movement
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Update animator
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());

        // Jump logic
        if (jumpPressed)
            Jump();

        // Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);

        // Horizontal movement
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        // Reset jumpPressed each frame
        jumpPressed = false;

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

    // Called externally by master input
    public void SetMovement(float horizontal, bool jump)
    {
        horizontalInput = horizontal;
        jumpPressed = jump;
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
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return raycastHit.collider != null;
    }

    public bool CanAttack()
    {
        return horizontalInput == 0 && IsGrounded();
    }
}