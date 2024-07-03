using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsable of all Enemies and their pooling system
/// </summary>
public class Enemies_Manager : Poolables_Manager
{
    #region Fields
    const int TIME_BETWEEN_SPAWNS = 1;
    const int TIME_BETWEEN_WAVES = 3;

    Enemy_Data[] spawnableEnemies;
    Stack<Vector3> path;

    EnemyWaves wavesController;

    #endregion


    #region Public Methods

    public void SetEnemyManager(Enemy_Data[] spawnableEnemies, Stack<Vector3> path, int[,] enemyWaves_data)
    {
        this.spawnableEnemies = spawnableEnemies;
        this.path = path;

        wavesController = new EnemyWaves(enemyWaves_data, spawnableEnemies);
        
        Subscribe();
        FillPool();
        StartCoroutine(SpawnNextEnemyWave());
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Spawns enemies, if doesnt have Enemy class yet on it, inject it
    /// </summary>
    void SpawnEnemy(Enemy_Data data, Stack<Vector3> path)
    {
        GameObject enemy = Pools_Manager.Instance.UnPoolObject(data.Name);
        if (!enemy.TryGetComponent<Enemy_Controller>(out _))
            enemy.AddComponent<Enemy_Controller>();
        enemy.GetComponent<Enemy_Controller>().SetEnemy(data, path);
        unpooledObjects.Add(enemy.GetComponent<Enemy_Controller>());
    }

    IEnumerator SpawnNextEnemyWave()
    {
        yield return new WaitForSeconds(TIME_BETWEEN_WAVES);

        Stack<Enemy_Data> wave = wavesController.GetNextWave();
        GameEvents.Instance.OnSpawningNewWave(wavesController.WaveCounter, wavesController.TotalWaves);
        // Empty wave case
        if (wave.Count == 0)
            LastEnemyFromWaveDespawned();
        // Spawn Enemies
        while (wave.Count > 0)
        {
            yield return new WaitForSeconds(TIME_BETWEEN_SPAWNS + Random.Range(-0.2f,0.2f));
            SpawnEnemy(wave.Pop(), path);
        }
    }
    
    /// <summary>
    /// Decides wether to send next wave, or to end the game
    /// </summary>
    void LastEnemyFromWaveDespawned()
    {
        // Check if its the las wave
        if (wavesController.WaveCounter >= wavesController.TotalWaves)
            GameEvents.Instance.OnLastWaveIsOver();
        else
        {
            StartCoroutine(SpawnNextEnemyWave());
            GameEvents.Instance.OnWaveIsOver();
        }
    }

    #endregion


    #region Poolables_Manager Methods

    protected override void FillPool()
    {
        unpooledObjects = new HashSet<IPoolable>();

        // Generate PoolInfo and start a Pool
        foreach (Enemy_Data enemy in spawnableEnemies)
        {
            PoolInfo enemiesPool = new PoolInfo(20, enemy.enemyPrefab, this.transform);
            Pools_Manager.Instance.FillPool(enemiesPool);
        }
    }

    /// <summary>
    /// Despawn enemies at the end of frame
    /// </summary>
    protected override IEnumerator PoolObject(IPoolable poolableEnemy)
    {
        yield return base.PoolObject(poolableEnemy);

        Enemy_Controller enemy = poolableEnemy as Enemy_Controller;
        Pools_Manager.Instance.PoolObject(enemy.gameObject);
        unpooledObjects.Remove(enemy.GetComponent<Enemy_Controller>());
        if (unpooledObjects.Count == 0)
            LastEnemyFromWaveDespawned();
    }
    #endregion


    #region Event Methods

    void OnEnemyIsKilled(Enemy_Controller enemy)
    {
        StartCoroutine(PoolObject(enemy));
    }

    void OnEnemyReachesEnd(Enemy_Controller enemy)
    {
        StartCoroutine(PoolObject(enemy));
    }

    void OnEnemyHitted(Enemy_Controller enemy, int damage, float duration)
    {
        if (unpooledObjects.Contains(enemy))
            StartCoroutine(enemy.HitByProjectile(damage, duration));
    }

    private void Subscribe()
    {
        GameEvents.Instance.onEnemyIsKilled += OnEnemyReachesEnd;
        GameEvents.Instance.onEnemyAttack += OnEnemyIsKilled;
        GameEvents.Instance.onProjectileImpactsEnemy += OnEnemyHitted;
    }

    void OnDisable()
    {
        GameEvents.Instance.onEnemyIsKilled -= OnEnemyReachesEnd;
        GameEvents.Instance.onEnemyAttack -= OnEnemyIsKilled;
        GameEvents.Instance.onProjectileImpactsEnemy -= OnEnemyHitted;
    }
    #endregion
}
