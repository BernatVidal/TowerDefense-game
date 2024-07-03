using System.Collections;
using UnityEngine;

/// <summary>
/// Class used for any Projectile from the time that it's shot, from the time it hits the Enemy (or not) and despawns
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile_Controller : MonoBehaviour, IPoolable
{
    ParticleSystem[] ps;
    Transform target;
    Projectile_Data data;
    SphereCollider coll;

    enum ProjectileStatus
    {
        shooting,
        exploding
    }
    ProjectileStatus status;

    private void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>(true);
        coll = GetComponentInChildren<SphereCollider>();
    }

    public void Shoot(Vector3 originalPos, Transform target, Projectile_Data data)
    {
        transform.position = originalPos;
        this.target = target;
        ps[0].gameObject.SetActive(true);
        ps[1].gameObject.SetActive(false);
        this.data = data;

        coll.enabled = false;
        coll.isTrigger = true;
        coll.radius = data.attackRadius;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        status = ProjectileStatus.shooting;
    }


    public void DoStuff()
    {
        if (target.gameObject.activeSelf)
            Move();
        else Despawn();
    }

    void Move()
    {
        switch(status)
        {
            case ProjectileStatus.shooting:
                if (MoveToTarget_Solver.MoveToTarget(transform, target.position, data.projectileVelocity))
                {
                    GameEvents.Instance.OnProjectileImpactsEnemy(target.GetComponent<Enemy_Controller>(), data.damage, data.attackDuration);
                    StartCoroutine(Explode());
                }
                break;
            case ProjectileStatus.exploding:
                if (data.doFollowPlayerOnImpact)
                    MoveToTarget_Solver.MoveToTarget(transform, target.position, data.projectileVelocity, true);
                break;
        }
    }


    IEnumerator Explode()
    {
        status = ProjectileStatus.exploding;
        transform.position = target.position;
        Audio_Manager.Instance.PlaySound(data.explosionSoundFX);
        if (data.attackRadius > 0)
            coll.enabled = true;
        ps[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(ps[1].main.duration);
        ps[0].gameObject.SetActive(false);
        Despawn();
    }

    void Despawn()
    {
        GameEvents.Instance.OnProjectileExploded(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy_Controller>(out Enemy_Controller enemyHitted))
        {
            if (!enemyHitted.transform.Equals(target))
            {
                GameEvents.Instance.OnProjectileImpactsEnemy(enemyHitted, data.damage, data.attackDuration);
            }
        }
    }
}