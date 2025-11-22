using UnityEngine;

public class OutOfRange : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the object exiting has a Health component in itself or its parents
        Health playerHealth = collision.GetComponentInParent<Health>();

        if (playerHealth != null)
        {
            // Instantly kill the player
            playerHealth.TakeDamage(playerHealth.currentHealth);
        }
    }
}