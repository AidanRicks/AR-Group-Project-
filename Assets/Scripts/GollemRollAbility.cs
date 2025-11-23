using System.Collections;
using UnityEngine;

public class GolemRollAbility : MonoBehaviour
{
    [Header("Roll Settings")]
    [SerializeField] private KeyCode rollKey = KeyCode.X;
    [SerializeField] private float windupTime = 1f;
    [SerializeField] private float rollDuration = 3f;      // Max time in ball form
    [SerializeField] private float cooldownTime = 2f;      // Wait time before using again
    [SerializeField] private float ballSpeedMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Colliders")]
    [SerializeField] private CircleCollider2D ballCollider;
    [SerializeField] private BoxCollider2D normalCollider;

    private WASD wasdMovement;
    private ArrowMovement arrowMovement;
    private Animator anim;
    private Rigidbody2D rb;

    private bool rollingWindup = false;
    private bool isRolling = false;
    private bool isOnCooldown = false;

    private float rollTimer = 0f;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        // Detect which movement script exists
        wasdMovement = GetComponent<WASD>();
        arrowMovement = GetComponent<ArrowMovement>();
        anim = GetComponent<Animator>();

        // Use Rigidbody2D from whichever movement script exists
        if (wasdMovement != null) rb = wasdMovement.rb;
        else if (arrowMovement != null) rb = arrowMovement.rb;
    }

    private void Update()
    {
        HandleRollTimers();
        HandleRollInput();
        RotateWhileRolling();
    }

    private void HandleRollInput()
    {
        if (isOnCooldown || isRolling || rollingWindup)
            return;

        if (Input.GetKeyDown(rollKey))
        {
            rollingWindup = true;

            // Block player during windup
            if (wasdMovement != null)
            {
                wasdMovement.blockMovement = true;
                wasdMovement.blockJumping = true;
            }
            else if (arrowMovement != null)
            {
                arrowMovement.blockMovement = true;
                arrowMovement.blockJumping = true;
            }

            anim.SetBool("rolling", true);
            anim.SetTrigger("roll");

            StartCoroutine(StartRollAfterDelay());
        }
    }

    private IEnumerator StartRollAfterDelay()
    {
        yield return new WaitForSeconds(windupTime);

        rollingWindup = false;
        EnterBallForm();
    }

    private void EnterBallForm()
    {
        isRolling = true;
        rollTimer = rollDuration;
        Debug.Log("ROLL START!");

        // Swap colliders
        normalCollider.enabled = false;
        ballCollider.enabled = true;

        // Increase speed
        if (wasdMovement != null) wasdMovement.speed *= ballSpeedMultiplier;
        else if (arrowMovement != null) arrowMovement.speed *= ballSpeedMultiplier;

        // Allow movement but block jumping
        if (wasdMovement != null) { wasdMovement.blockMovement = false; wasdMovement.blockJumping = true; }
        else if (arrowMovement != null) { arrowMovement.blockMovement = false; arrowMovement.blockJumping = true; }
    }

    private void ExitBallForm()
    {
        Debug.Log("ROLL END!");

        isRolling = false;
        anim.SetBool("rolling", false);
        transform.rotation = Quaternion.identity;

        // Restore colliders
        ballCollider.enabled = false;
        normalCollider.enabled = true;

        // Reset speed
        if (wasdMovement != null) wasdMovement.speed /= ballSpeedMultiplier;
        else if (arrowMovement != null) arrowMovement.speed /= ballSpeedMultiplier;

        // Restore controls
        if (wasdMovement != null) { wasdMovement.blockMovement = false; wasdMovement.blockJumping = false; }
        else if (arrowMovement != null) { arrowMovement.blockMovement = false; arrowMovement.blockJumping = false; }

        // Start cooldown
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

    private void HandleRollTimers()
    {
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
                ExitBallForm();
        }

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
                isOnCooldown = false;
        }
    }

    private void RotateWhileRolling()
    {
        if (!isRolling) return;

        float h = 0f;
        if (wasdMovement != null) h = wasdMovement.horizontalInput;
        else if (arrowMovement != null) h = arrowMovement.horizontalInput;

        if (h != 0)
        {
            float rot = rotationSpeed * Time.deltaTime * Mathf.Sign(h);
            transform.Rotate(0, 0, -rot);
        }
    }
}
