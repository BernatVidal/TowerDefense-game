using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsable of Enemies movement and general behaviour
/// </summary>
[RequireComponent(typeof(Animations_Controller))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy_Controller : MonoBehaviour , IPoolable
{
    #region Fields

    float velocity;
    int hp;
    Sounds.SoundID dieSound;
    Stack<Vector3> path;
    Animations_Controller anim;

    enum EnemyStatus
    {
        idle,
        walking,
        freezed,
        attacking,
        dying
    }
    EnemyStatus status;

    #endregion

    #region Public Methods

    public void SetEnemy(Enemy_Data data, Stack<Vector3> path)
    {
        this.gameObject.name = data.Name;
        this.velocity = data.velocity;
        this.hp = data.hp;
        this.dieSound = data.dieSoundFX;

        this.path = new Stack<Vector3>(path);
        transform.position = this.path.Peek();

        if(anim == null)
        { 
            anim = GetComponent<Animations_Controller>();
            anim.SetAnimationController(data.animations);
        }

        status = EnemyStatus.idle;
    }

    public void DoStuff()
    {
        Move();
    }

    #endregion


    #region Movement Methods
    void Move()
    {
        if (status != EnemyStatus.dying)
        {
            if (path.Count > 1)
            {
                if (status == EnemyStatus.idle)
                    SetToWalk();
                if(MoveToTarget_Solver.MoveToTarget(transform, path.Peek(), GetCurrentVelocity(), true))
                    path.Pop();
            }
            else
                Attack();
        }
    }


    /// <summary>
    /// Returns current velocity according to status
    /// </summary>
    float GetCurrentVelocity()
    {
        switch (status)
        {
            case EnemyStatus.walking:
                return velocity;
            case EnemyStatus.freezed:
                return velocity/2;
            case EnemyStatus.attacking or EnemyStatus.dying:
                return 0;
            default:
                return velocity;
        }
    }

    #endregion


    #region Status Methods

    void SetToWalk()
    {
        status = EnemyStatus.walking;
        anim.PlayAnimation(Animations.AnimationType.walk);
    }

    void Attack()
    {
        if (status != EnemyStatus.attacking)
            StartCoroutine(SetToAttack());
        //Aim to the tower direction
        if (!transform.position.Equals(path.Peek()))
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path.Peek() - transform.position), Time.deltaTime * GetCurrentVelocity() * 5);
    }

    IEnumerator SetToAttack()
    {
        status = EnemyStatus.attacking;
        anim.PlayAnimation(Animations.AnimationType.attack);
        yield return new WaitForSeconds(anim.GetAnimDuration());
        GameEvents.Instance.OnEnemyAttack(this);
    }

    public IEnumerator HitByProjectile(int damage, float duration)
    {
        // Loose live
        hp -= damage;
        // Die
        if (hp <= 0)
        {
            if (status != EnemyStatus.dying)
            {
                yield return (StartCoroutine(SetToDying()));
                GameEvents.Instance.OnEnemyisKilled(this);
            }
        }
        else
        {
            // Freeze / Contention
            if (duration > 0.3)
                StartCoroutine(SetToFreezed(duration));
            // Set to hitted
            else if(damage > 0)
                StartCoroutine(SetToHitted());
        }
    }

    IEnumerator SetToHitted()
    {
        anim.PlayAnimation(Animations.AnimationType.hit);
        yield return new WaitForSeconds(anim.GetAnimDuration());
    }

    IEnumerator SetToFreezed(float timeFreezed)
    {                
        float timeCount = 0;

        while (timeCount < timeFreezed)
        {
            if (status != EnemyStatus.dying)
                status = EnemyStatus.freezed;
            timeCount += Time.deltaTime;
            yield return null;
        }

        if (status == EnemyStatus.freezed)
            status = EnemyStatus.idle;
    }

    IEnumerator SetToDying()
    {
        status = EnemyStatus.dying;
        anim.PlayAnimation(Animations.AnimationType.die);
        Audio_Manager.Instance.PlaySound(dieSound);
        yield return new WaitForSeconds(anim.GetAnimDuration());
    }

    #endregion

}
