using UnityEngine;

public class MultiplayerInput : MonoBehaviour
{
    [Header("Player References")]
    public PlayerMovement playerWASD;    // Assign Player 1
    public PlayerMovement playerArrows;  // Assign Player 2

    private void Update()
    {
        // ---------- WASD Player ----------
        float horizontalW = Input.GetAxisRaw("Horizontal"); // WASD axis
        bool jumpW = Input.GetKeyDown(KeyCode.W);

        playerWASD.SetMovement(horizontalW, jumpW);

        // ---------- Arrow Keys Player ----------
        float horizontalA = Input.GetAxisRaw("Horizontal_Arrows"); // Arrow axis
        bool jumpA = Input.GetKeyDown(KeyCode.UpArrow);

        playerArrows.SetMovement(horizontalA, jumpA);
    }
}