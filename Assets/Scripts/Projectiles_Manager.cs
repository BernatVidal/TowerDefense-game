using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsable of all Projectiles and their pooling system
/// </summary>
public class Projectiles_Manager : Poolables_Manager
{
    Tower_Data[] towersData;


    #region Public Methods
    public void SetProjectilesManager(Tower_Data[] towerTypes)
    {
        towersData = towerTypes;
        Subscribe();
        FillPool();
    }

    #endregion


    #region PoolablesManager Methods

    protected override void FillPool()
    {
        unpooledObjects = new HashSet<IPoolable>();
        foreach (Tower_Data tower in towersData)
        {
            PoolInfo projectilesPool = new PoolInfo(10, tower.projectileData.projectilePrefab, this.transform);
            Pools_Manager.Instance.FillPool(projectilesPool);
        }
    }

    protected override IEnumerator PoolObject(IPoolable poolableProjectile)
    {
        yield return base.PoolObject(poolableProjectile);

        Projectile_Controller projectile = poolableProjectile as Projectile_Controller;
        Pools_Manager.Instance.PoolObject(projectile.gameObject);
        unpooledObjects.Remove(projectile.GetComponent<Projectile_Controller>());
    }

    #endregion


    #region Event Methods

    void OnProjectileShootRequest(Vector3 originalPos, Transform destinationPos, Projectile_Data projectileData)
    {
        GameObject projectile = Pools_Manager.Instance.UnPoolObject(projectileData.projectilePrefab.name);
        if (!projectile.TryGetComponent<Projectile_Controller>(out _))
            projectile.AddComponent<Projectile_Controller>();
        projectile.GetComponent<Projectile_Controller>().Shoot(originalPos, destinationPos, projectileData);
        unpooledObjects.Add(projectile.GetComponent<Projectile_Controller>());
    }

    void OnProjectileCanDespawn(Projectile_Controller proj)
    {
        StartCoroutine(PoolObject(proj));
    }
    void OnDisable()
    {
        UnSubscribe();
    }


    void Subscribe()
    {
        GameEvents.Instance.onProjectileExploded += OnProjectileCanDespawn;
        GameEvents.Instance.onShootProjectileRequest += OnProjectileShootRequest;
    }

    void UnSubscribe()
    {
        GameEvents.Instance.onProjectileExploded -= OnProjectileCanDespawn;
        GameEvents.Instance.onShootProjectileRequest -= OnProjectileShootRequest;
    }
    #endregion
}
