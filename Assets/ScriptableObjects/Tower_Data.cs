using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "MyAssets/Tower")]
public class Tower_Data : ScriptableObject
{
    public string Name;
    public float dettectionRadius;
    public float attackRecovery;
    public GameObject towerPrefab;
    public Projectile_Data projectileData;

    public Animations[] animations;
    public Sounds.SoundID shootSFX;
}
