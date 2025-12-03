using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Character Prefabs")]
    [Tooltip("Order: P1-A, P1-B, P1-C (WASD), then P2-A, P2-B, P2-C (Arrows)")]
    public GameObject[] characterPrefabs = new GameObject[6];

    [Header("Player 1 Spawn Points")]
    [Tooltip("Order: Level 1 P1 Spawn, Level 2 P1 Spawn, Level 3 P1 Spawn")]
    // This will hold the spawn points for Player 1 (the WASD player)
    public Transform[] p1SpawnPoints = new Transform[3];

    [Header("Player 2 Spawn Points")]
    [Tooltip("Order: Level 1 P2 Spawn, Level 2 P2 Spawn, Level 3 P2 Spawn")]
    // This will hold the spawn points for Player 2 (the Arrow Key player)
    public Transform[] p2SpawnPoints = new Transform[3];

    private void Awake()
    {
        SpawnCharacters();
    }

    private void SpawnCharacters()
    {
        // 1. Get Selections
        int p1Index = PlayerPrefs.GetInt("P1_Char_Index", 0);
        int p2Index = PlayerPrefs.GetInt("P2_Char_Index", 0);
        int levelNumber = PlayerPrefs.GetInt("Selected_Level", 1);

        // Array index is level number minus 1 (1 -> 0, 2 -> 1, 3 -> 2)
        int spawnIndex = levelNumber - 1;

        // Safety check for spawn points (checks both arrays size)
        if (spawnIndex < 0 || spawnIndex >= p1SpawnPoints.Length || p1SpawnPoints.Length != 3 || p2SpawnPoints.Length != 3)
        {
            Debug.LogError($"Invalid Level selection ({levelNumber}) or Spawn Point setup. Check array sizes.");
            // Default to Level 1 spawn (index 0) if there's an issue
            spawnIndex = 0;
        }

        // 2. Determine Spawn Positions
        // P1 Position is taken from the P1 array at the calculated index
        Vector3 p1SpawnPos = p1SpawnPoints[spawnIndex].position;

        // P2 Position is taken from the P2 array at the calculated index
        Vector3 p2SpawnPos = p2SpawnPoints[spawnIndex].position;

        // 3. Determine Prefabs
        int p1PrefabIndex = p1Index;
        int p2PrefabIndex = p2Index + 3; // P2 prefabs are indices 3, 4, 5

        if (characterPrefabs.Length < 6)
        {
            Debug.LogError("Not enough character prefabs assigned! Need 6.");
            return;
        }

        GameObject p1Prefab = characterPrefabs[p1PrefabIndex];
        GameObject p2Prefab = characterPrefabs[p2PrefabIndex];

        // 4. Instantiate Players at their DISTINCT locations

        if (p1Prefab != null)
        {
            // Instantiate P1 at the specific P1 spawn point for this level
            Instantiate(p1Prefab, p1SpawnPos, Quaternion.identity);
            Debug.Log($"Spawned P1 Character (Index: {p1Index}) at Level {levelNumber} P1 spawn.");
        }
        if (p2Prefab != null)
        {
            // Instantiate P2 at the specific P2 spawn point for this level
            Instantiate(p2Prefab, p2SpawnPos, Quaternion.identity);
            Debug.Log($"Spawned P2 Character (Index: {p2Index}) at Level {levelNumber} P2 spawn.");
        }
    }
}