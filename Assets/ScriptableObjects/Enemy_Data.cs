using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "MyAssets/Enemy")]
public class Enemy_Data : ScriptableObject
{
    public string Name;
    public float velocity;
    public int hp;
    public GameObject enemyPrefab;

    public Animations[] animations;

    public Sounds.SoundID dieSoundFX;
}
