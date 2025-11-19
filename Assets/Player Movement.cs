using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{



    [Header("Movement Parameters")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 16f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter;


    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;


    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float horizontalInput;
    private Vector2 moveInput;

    // Double jump variables
    private const int maxJumps = 1;
    private int jumpCounter;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Flip sprite
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);



        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();


        else
        {
            // Normal movement
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime;
                jumpCounter = maxJumps;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }

    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0)
            return;

        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        jumpCounter--;
        coyoteCounter = 0;
    }

    private bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return hit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer
        );
        return hit.collider != null;
    }

   
}
