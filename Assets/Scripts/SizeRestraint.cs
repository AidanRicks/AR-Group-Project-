using UnityEngine;

public class SizeRestraint : MonoBehaviour
{
    private Vector3 originalScale;
    private int flipSign = 1; // 1 = normal, -1 = flipped

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    /// Call this method to flip the character
    public void Flip(bool facingRight)
    {
        flipSign = facingRight ? 1 : -1;
    }

    private void LateUpdate()
    {
        // Apply flipping while keeping original size
        transform.localScale = new Vector3(originalScale.x * flipSign, originalScale.y, originalScale.z);
    }
}