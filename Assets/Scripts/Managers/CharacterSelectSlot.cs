using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectSlot : MonoBehaviour
{
    [Header("Player Setup")]
    // 0 for P1, 1 for P2 (Crucial for GameManager access)
    public int playerIndex = 0;
    // Keys assigned in Inspector: P1 (1, 2, 3), P2 (8, 9, 0)
    public KeyCode[] selectionKeys = new KeyCode[3];

    [Header("Data References")]
    public List<CharacterData> availableCharacters;

    [Header("UI References")]
    public Image portraitDisplay;
    public Text nameText;
    public GameObject lockInOverlay; // Used only to ensure it remains hidden

    private int selectedIndex = 0;

    void Start()
    {
        if (availableCharacters.Count > 0)
        {
            gameObject.SetActive(true);
            lockInOverlay.SetActive(false);

            // Default selection to the first character
            SelectCharacter(0);
        }
    }

    void Update()
    {
        // --- Handle Direct Selection (1, 2, 3 or 8, 9, 0) ---
        for (int i = 0; i < selectionKeys.Length; i++)
        {
            if (Input.GetKeyDown(selectionKeys[i]))
            {
                if (i < availableCharacters.Count)
                {
                    SelectCharacter(i);
                }
                break;
            }
        }
    }

    private void SelectCharacter(int newIndex)
    {
        // Check for GameManager instance safety (prevents NRE if manager is missing)
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is missing! Cannot register selection.");
            return;
        }

        selectedIndex = newIndex;

        CharacterData selectedData = availableCharacters[selectedIndex];

        // 1. Update UI
        portraitDisplay.sprite = selectedData.characterPortrait;
        nameText.text = $"P{playerIndex + 1}: {selectedData.characterName}";

        // 2. Register selection in GameManager immediately
        GameManager.Instance.SetSelectedCharacter(playerIndex, selectedData);

        // 3. Check Start Button Status (The button will become active if both players have selected)
        FindObjectOfType<StartGameButtonController>()?.CheckReadyStatus();
    }
}