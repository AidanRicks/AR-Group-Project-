using System.Collections;
using UnityEngine;

public class WASD : MonoBehaviour
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
    private bool rollingInputHeld = false;
    private bool rollingWindup = false; // true during the 1-second delay

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
        // Prevent movement during rolling windup
        if (rollingWindup)
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
        if (!isRolling && !rollingWindup && Input.GetKeyDown(KeyCode.W))
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

    private void HandleRolling()
    {
        if (Input.GetKeyDown(rollKey) && !isRolling && !rollingWindup)
        {
            rollingInputHeld = true;
            rollingWindup = true;

            // Set rolling animation immediately
            anim.SetBool("rolling", true);
            anim.SetTrigger("roll");


            // Start the wind-up coroutine
            StartCoroutine(StartRollingAfterDelay(1f));
        }

        if (Input.GetKeyUp(rollKey))
        {
            rollingInputHeld = false;

            // Cancel rolling if windup not finished
            if (rollingWindup)
            {
                rollingWindup = false;
                anim.SetBool("rolling", false);
            }

            if (isRolling)
                ExitBallForm();
        }
    }

    private IEnumerator StartRollingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rollingInputHeld)
        {
            rollingWindup = false;
            EnterBallForm(); // now physics, collider swap, movement multiplier starts
        }
        else
        {
            // Player released key during windup
            anim.SetBool("rolling", false);
            rollingWindup = false;
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
        Debug.Log("Rolling started!");

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
