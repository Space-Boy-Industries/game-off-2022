using UnityEngine;

[CreateAssetMenu(fileName = "New Game Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public string[] MinigameScenes;
    public int[] DifficultyUpAfter;
}
