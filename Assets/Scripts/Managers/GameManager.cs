using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Slots to store the selected ScriptableObject data for P1 (Index 0) and P2 (Index 1)
    private CharacterData[] selectedCharacters = new CharacterData[2];
    // Removed: private bool[] playersReady = new bool[2];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Sets the selected character data based on Player Index (0 or 1)
    public void SetSelectedCharacter(int playerIndex, CharacterData data)
    {
        if (playerIndex >= 0 && playerIndex < selectedCharacters.Length)
        {
            selectedCharacters[playerIndex] = data;
            Debug.Log($"Player {playerIndex + 1} selected: {data.characterName}");
        }
    }

    public CharacterData GetSelectedCharacter(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < selectedCharacters.Length)
        {
            return selectedCharacters[playerIndex];
        }
        return null;
    }

    // The simplified check: Game is ready if BOTH players have selected character data.
    public bool AreBothPlayersReady()
    {
        return selectedCharacters[0] != null && selectedCharacters[1] != null;
    }
}