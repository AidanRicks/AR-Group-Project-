// Scripts/Characters/CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName = "Warrior";
    public Sprite characterPortrait; // For the UI display
    public GameObject characterPrefab; // The actual prefab to spawn in-game
    public string description = "A strong, balanced fighter.";

    // Character-specific stats
    public float baseHealth = 100f;
    public float baseMovementSpeed = 5f;
}