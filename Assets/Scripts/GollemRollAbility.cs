using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRollAbility : MonoBehaviour
{
    [Header("Roll Settings")]
    [SerializeField] private KeyCode rollKey = KeyCode.X;
    [SerializeField] private float windupTime = 1f;
    [SerializeField] private float rollDuration = 3f;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private float ballSpeedMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Colliders")]
    [SerializeField] private CircleCollider2D ballCollider;
    [SerializeField] private BoxCollider2D normalCollider;

    [Header("Roll Damage")]
    [SerializeField] private float rollDamage = 25f;   // damage dealt on collision while rolling

    private WASD wasdMovement;
    private Arrows arrowMovement;
    private Animator anim;
    private Rigidbody2D rb;
    private Health health;

    private bool rollingWindup = false;
    private bool isRolling = false;
    private bool isOnCooldown = false;

    private float rollTimer = 0f;
    private float cooldownTimer = 0f;

    // Prevents hitting the same enemy multiple times per roll
    private HashSet<Health> damagedTargets = new HashSet<Health>();

    private void Awake()
    {
        wasdMovement = GetComponent<WASD>();
        arrowMovement = GetComponent<Arrows>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        if (wasdMovement != null)
            rb = wasdMovement.rb;
        else if (arrowMovement != null)
            rb = arrowMovement.rb;
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

            // Block movement & jumping
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

        damagedTargets.Clear(); // reset hit list

        if (health != null) health.invincible = true;

        if (normalCollider != null) normalCollider.enabled = false;
        if (ballCollider != null) ballCollider.enabled = true;

        if (wasdMovement != null) wasdMovement.speed *= ballSpeedMultiplier;
        else if (arrowMovement != null) arrowMovement.speed *= ballSpeedMultiplier;

        if (wasdMovement != null) { wasdMovement.blockMovement = false; wasdMovement.blockJumping = true; }
        else if (arrowMovement != null) { arrowMovement.blockMovement = false; arrowMovement.blockJumping = true; }
    }

    private void ExitBallForm()
    {
        Debug.Log("ROLL END!");

        isRolling = false;
        anim.SetBool("rolling", false);
        transform.rotation = Quaternion.identity;

        if (health != null) StartCoroutine(RemoveInvincibilityAfterDelay(0.2f));

        if (ballCollider != null) ballCollider.enabled = false;
        if (normalCollider != null) normalCollider.enabled = true;

        if (wasdMovement != null) wasdMovement.speed /= ballSpeedMultiplier;
        else if (arrowMovement != null) arrowMovement.speed /= ballSpeedMultiplier;

        if (wasdMovement != null) { wasdMovement.blockMovement = false; wasdMovement.blockJumping = false; }
        else if (arrowMovement != null) { arrowMovement.blockMovement = false; arrowMovement.blockJumping = false; }

        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

    private IEnumerator RemoveInvincibilityAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (health != null) health.invincible = false;
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

    // DAMAGE HANDLER — called whenever ballCollider hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRolling) return;

        Health targetHealth = collision.collider.GetComponent<Health>();

        if (targetHealth != null && !damagedTargets.Contains(targetHealth))
        {
            damagedTargets.Add(targetHealth); // prevent repeat hits
            targetHealth.TakeDamage(rollDamage);

            Debug.Log("ROLL HIT! Damaged: " + collision.collider.name);
        }
    }
}
