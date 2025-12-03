using UnityEngine;

public class Health : MonoBehaviour
{
    [HideInInspector] public bool invincible = false;

    [Header("Health")]
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private float deathDelay = 1.2f;  // time before despawning

    public float currentHealth { get; private set; }

    private Animator anim;
    private bool dead = false;
    private BoxCollider2D boxCollider;

    // Store all scripts that should disable upon death
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        currentHealth = startingHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        // Cache every script on the player EXCEPT this one
        playerScripts = GetComponents<MonoBehaviour>();
    }

    public void TakeDamage(float damage)
    {
        if (dead || invincible) return;  // skip damage if invincible

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;

        // Trigger death animation
        anim.SetTrigger("die");

        // Disable the hitbox immediately
        if (boxCollider)
            boxCollider.enabled = false;

        // Disable all player behavior scripts (movement, attacking, etc.)
        foreach (var script in playerScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // Begin delayed removal
        StartCoroutine(DeathSequence());
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        // Optional: wait for animation length
        yield return new WaitForSeconds(deathDelay);

        // Finally deactivate the player object
        gameObject.SetActive(false);
    }
}
