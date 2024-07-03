using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "MyAssets/Projectile")]
public class Projectile_Data : ScriptableObject
{
    public string Name;
    public int damage;
    public float attackRadius;
    public float attackDuration;
    public float projectileVelocity;
    public bool doFollowPlayerOnImpact;
    public GameObject projectilePrefab;
    public Sounds.SoundID explosionSoundFX;
}
