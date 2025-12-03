using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; // Needed for .ToArray() if using List in Inspector

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Scene Setup")]
    public string gameSceneName = "GameLevelScene"; // Set this to the name of your main level scene

    [Header("Player 1 Sprites (Keys 1, 2, 3)")]
    // Sprites for P1 selection (e.g., Character A sprite, Character B sprite, Character C sprite)
    public SpriteRenderer[] p1SelectionSprites = new SpriteRenderer[3];
    private int p1SelectedIndex = 0; // Starts at 0 (Character A)

    [Header("Player 2 Sprites (Keys 8, 9, 0)")]
    // Sprites for P2 selection
    public SpriteRenderer[] p2SelectionSprites = new SpriteRenderer[3];
    private int p2SelectedIndex = 0; // Starts at 0 (Character A)

    [Header("Level Sprites (Keys 4, 5, 6)")]
    // Sprites for Level selection
    public SpriteRenderer[] levelSelectionSprites = new SpriteRenderer[3];
    private int levelSelectedIndex = 0; // 0 for Level 1, 1 for Level 2, 2 for Level 3

    private void Start()
    {
        // Set initial PlayerPrefs and update sprites
        SetInitialSelections();
        UpdateAllSprites();
    }

    private void SetInitialSelections()
    {
        // Initialize PlayerPrefs if they don't exist
        p1SelectedIndex = PlayerPrefs.GetInt("P1_Char_Index", 0);
        p2SelectedIndex = PlayerPrefs.GetInt("P2_Char_Index", 0);

        // Note: The level key will be 1, 2, or 3. 0 means no level has been explicitly selected.
        // We convert the saved level (1, 2, 3) to an array index (0, 1, 2) for the sprite logic.
        int savedLevel = PlayerPrefs.GetInt("Selected_Level", 0);
        levelSelectedIndex = (savedLevel > 0) ? savedLevel - 1 : -1; // -1 means no level sprite is active
    }

    private void Update()
    {
        HandlePlayer1Input();
        HandlePlayer2Input();
        HandleLevelInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private void HandlePlayer1Input()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { p1SelectedIndex = 0; SaveAndRefreshP1(); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { p1SelectedIndex = 1; SaveAndRefreshP1(); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { p1SelectedIndex = 2; SaveAndRefreshP1(); }
    }

    private void SaveAndRefreshP1()
    {
        PlayerPrefs.SetInt("P1_Char_Index", p1SelectedIndex);
        UpdateSpriteVisibility(p1SelectionSprites, p1SelectedIndex);
    }

    private void HandlePlayer2Input()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8)) { p2SelectedIndex = 0; SaveAndRefreshP2(); }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { p2SelectedIndex = 1; SaveAndRefreshP2(); }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) { p2SelectedIndex = 2; SaveAndRefreshP2(); }
    }

    private void SaveAndRefreshP2()
    {
        PlayerPrefs.SetInt("P2_Char_Index", p2SelectedIndex);
        UpdateSpriteVisibility(p2SelectionSprites, p2SelectedIndex);
    }

    private void HandleLevelInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) { levelSelectedIndex = 0; SaveAndRefreshLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { levelSelectedIndex = 1; SaveAndRefreshLevel(); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { levelSelectedIndex = 2; SaveAndRefreshLevel(); }
    }

    private void SaveAndRefreshLevel()
    {
        // Level value is 1, 2, or 3 (index + 1)
        PlayerPrefs.SetInt("Selected_Level", levelSelectedIndex + 1);
        UpdateSpriteVisibility(levelSelectionSprites, levelSelectedIndex);
    }

    private void UpdateAllSprites()
    {
        UpdateSpriteVisibility(p1SelectionSprites, p1SelectedIndex);
        UpdateSpriteVisibility(p2SelectionSprites, p2SelectedIndex);

        // Handle initial level sprite state (index -1 means none active)
        if (levelSelectedIndex >= 0)
        {
            UpdateSpriteVisibility(levelSelectionSprites, levelSelectedIndex);
        }
        else
        {
            // Turn off all level sprites if no explicit selection was made
            foreach (var sprite in levelSelectionSprites)
            {
                if (sprite != null) sprite.enabled = false;
            }
        }
    }

    // Helper function to toggle sprites: only enable the one at the selected index
    private void UpdateSpriteVisibility(SpriteRenderer[] sprites, int activeIndex)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                sprites[i].enabled = (i == activeIndex);
            }
        }
    }

    private void StartGame()
    {
        int finalLevel = PlayerPrefs.GetInt("Selected_Level", 0);

        // Handle the "no level selected" random choice
        if (finalLevel == 0)
        {
            // Randomly pick a level between 1 and 3 (inclusive)
            finalLevel = Random.Range(1, 4);
            Debug.Log($"No level was selected. Randomly picking Level {finalLevel}.");
            PlayerPrefs.SetInt("Selected_Level", finalLevel);
        }

        Debug.Log($"Starting Game with P1: {PlayerPrefs.GetInt("P1_Char_Index")}, P2: {PlayerPrefs.GetInt("P2_Char_Index")}, Level: {finalLevel}");
        SceneManager.LoadScene(gameSceneName);
    }
}