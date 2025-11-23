using System.Collections;
using UnityEngine;

public class WASD : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speed = 5f;
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

    public Rigidbody2D rb;
    private Animator anim;

    [HideInInspector] public float horizontalInput;

    // These are controlled by the roll ability
    [HideInInspector] public bool blockMovement = false;
    [HideInInspector] public bool blockJumping = false;

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
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void HandleInput()
    {
        if (blockMovement)
        {
            horizontalInput = 0;
            return;
        }

        horizontalInput = 0;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        if (horizontalInput != 0)
            sizeRestraint.Flip(horizontalInput > 0);
    }

    private void HandleJump()
    {
        if (blockJumping) return;

        if (Input.GetKeyDown(KeyCode.W))
            Jump();
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

    private void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
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

    private void UpdateAnimator()
    {
        if (anim)
        {
            anim.SetBool("walking", horizontalInput != 0);
            anim.SetBool("grounded", IsGrounded());
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
