using System.Collections;
using UnityEngine;


/// <summary>
/// Main class for Towers, responsable of their main behaviour
/// </summary>
[RequireComponent(typeof(Animations_Controller))]
public abstract class Tower : MonoBehaviour
{
    #region Fields

    [SerializeField] public Tower_Data towerData;
    [SerializeField] protected Transform shootingPos;

    protected SphereCollider dettectionCollider;
    protected Enemy_Controller target;
    protected Animations_Controller anim;

    protected enum TowerStatus
    {
        aggressive,
        attacking,
        waiting
    }
    protected TowerStatus status;

    #endregion

    #region Unity Methods
    private void Start()
    {
        SetTower();
    }

    protected virtual void FixedUpdate()
    {
       if(status == TowerStatus.aggressive)
            if (target != null)
                StartCoroutine(SetToAttack());
    }
    #endregion

    #region Public Methods
    public virtual void SetTower()
    {
        status = TowerStatus.aggressive;
        anim = GetComponent<Animations_Controller>();
        anim.SetAnimationController(towerData.animations);

        // Set dettection collider (with RB)
        dettectionCollider = gameObject.AddComponent<SphereCollider>();
        dettectionCollider.radius = towerData.dettectionRadius;
        dettectionCollider.isTrigger = true;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    #endregion


    #region Status Methods
    protected virtual void Attack()
    {
        GameEvents.Instance.OnShootProjectileRequest(shootingPos.position, target.transform, towerData.projectileData);
        Audio_Manager.Instance.PlaySound(towerData.shootSFX);
    }

    protected virtual IEnumerator SetToAttack()
    {
        status = TowerStatus.attacking;
        Attack();
        yield return new WaitForSeconds(towerData.projectileData.attackDuration);
        status = TowerStatus.waiting;
        StartCoroutine(SetToWait());
    }
    
    protected IEnumerator SetToWait()
    {
        status = TowerStatus.waiting;
        yield return new WaitForSeconds(towerData.attackRecovery);
        status = TowerStatus.aggressive;
    }

    #endregion

    void SetTarget(Enemy_Controller enemy)
    {
        target = enemy;
        GameEvents.Instance.onEnemyIsKilled += DiscardTarget;
        GameEvents.Instance.onEnemyAttack += DiscardTarget;
    }

    void DiscardTarget(Enemy_Controller _)
    {
        target = null;
        GameEvents.Instance.onEnemyIsKilled -= DiscardTarget;
        GameEvents.Instance.onEnemyAttack -= DiscardTarget;
    }


    #region Colliders
    private void OnTriggerStay(Collider collision)
    {
        if (target == null && collision.TryGetComponent<Enemy_Controller>(out Enemy_Controller enemy))
            SetTarget(enemy);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<Enemy_Controller>(out Enemy_Controller enemy))
            if (enemy.Equals(target))
                DiscardTarget(enemy);
    }
    #endregion

}
