using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("Level data")]
    public TextAsset levelData;

    [Header ("Map Layout")]
    public MapLayout mapLayout;

    [Header("Enemy types")]
    public Enemy_Data[] enemies;
    [Header("Tower types")]
    int nothing2;

    [Header("Difficulty Settings")]
    public int playerLives = 20;
}
