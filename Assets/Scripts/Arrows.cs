using UnityEngine;

public class ArrowKeys : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private SizeRestraint sizeRestraint;

    [Header("Jump Mechanics")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter;
    [SerializeField] private int extraJumps = 1;
    private int jumpCounter;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ball Form")]
    [SerializeField] private KeyCode rollKey = KeyCode.X;
    [SerializeField] private float ballSpeedMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 720f; // Degrees per second while rolling
    [SerializeField] private CircleCollider2D ballCollider;
    [SerializeField] private BoxCollider2D normalCollider;

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isRolling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleJump();
        HandleCoyoteTime();
        HandleRolling();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
        RotateWhileRolling();
    }

    private void HandleInput()
    {
        // Arrow key movement
        horizontalInput = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

        // Flip sprite based on movement
        if (horizontalInput != 0)
        {
            bool facingRight = horizontalInput > 0;
            sizeRestraint.Flip(facingRight);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void HandleCoyoteTime()
    {
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

    private void HandleRolling()
    {
        if (Input.GetKey(rollKey))
        {
            if (!isRolling) EnterBallForm(); // Sets isRolling = true and anim.SetBool("rolling", true)
        }
        else
        {
            if (isRolling) ExitBallForm(); // Sets isRolling = false and anim.SetBool("rolling", false)
        }
    }

    private void Move()
    {
        float currentSpeed = isRolling ? speed * ballSpeedMultiplier : speed;
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private void RotateWhileRolling()
    {
        if (isRolling && horizontalInput != 0)
        {
            float rotationAmount = rotationSpeed * Time.fixedDeltaTime * Mathf.Sign(horizontalInput);
            transform.Rotate(0, 0, -rotationAmount);
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

        if (!IsGrounded() && coyoteCounter <= 0 && jumpCounter > 0)
            jumpCounter--;

        coyoteCounter = 0;

        if (anim) anim.SetTrigger("jump");
    }

    private void EnterBallForm()
    {
        isRolling = true;
        anim.SetBool("rolling", true);

        // Swap colliders if assigned
        if (ballCollider != null && normalCollider != null)
        {
            normalCollider.enabled = false;
            ballCollider.enabled = true;
        }
    }

    private void ExitBallForm()
    {
        isRolling = false;
        anim.SetBool("rolling", false);
        transform.rotation = Quaternion.identity;

        // Swap colliders back
        if (ballCollider != null && normalCollider != null)
        {
            normalCollider.enabled = true;
            ballCollider.enabled = false;
        }
    }

    private void UpdateAnimator()
    {
        if (anim)
        {
            anim.SetBool("walking", horizontalInput != 0);
            anim.SetBool("grounded", IsGrounded());
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}