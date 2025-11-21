using UnityEngine;

public class OutofRange : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if what exited is the player
        Health playerHealth = collision.GetComponent<Health>();

        if (playerHealth != null)
        {
            // Instantly kill the player
            playerHealth.TakeDamage(playerHealth.currentHealth);
        }
    }
}
