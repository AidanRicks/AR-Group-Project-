using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 16f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    // Input values from PlayerInput
    private Vector2 moveInput;

    // Double jump
    private int jumpCounter;
    private int maxJumps = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleSpriteFlip();
        HandleCoyoteTime();
    }

    // -------------------------------
    // MOVEMENT
    // -------------------------------
    private void HandleMovement()
    {
        body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
    }

    private void HandleSpriteFlip()
    {
        if (moveInput.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (moveInput.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // -------------------------------
    // JUMPING
    // -------------------------------
    private void HandleCoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpCounter = maxJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );
    }

    private void PerformJump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0)
            return;

        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);

        jumpCounter--;
        coyoteCounter = 0;
    }

    // -------------------------------
    // INPUT SYSTEM EVENTS
    // -------------------------------
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            PerformJump();
    }
}
