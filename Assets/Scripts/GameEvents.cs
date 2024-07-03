using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Fields

	public static GameEvents Instance;

	public event Action<Enemy_Controller> onEnemyIsKilled;
	public event Action<Enemy_Controller> onEnemyAttack;
	public event Action<Enemy_Controller, int, float> onProjectileImpactsEnemy;

	public event Action<Projectile_Controller> onProjectileExploded;
	public event Action<Vector3, Transform, Projectile_Data> onShootProjectileRequest;

	public event Action<int,int> onSpawningNewWave;
	public event Action onWaveIsOver;
	public event Action onLastWaveIsOver;

	#endregion


	#region Unity Methods

	void Awake()
	{
		Instance = this;
	}

    #endregion



    #region Public Methods

    /// Triggered when enemy is killed by a tower
    public void OnEnemyisKilled(Enemy_Controller enemy)
    {
		onEnemyIsKilled?.Invoke(enemy);
    }

    /// Triggered when Enemy attacks the tower
    public void OnEnemyAttack(Enemy_Controller enemy)
    {
		onEnemyAttack?.Invoke(enemy);
    }
	
	/// Triggered when Projectile impacts with an enemy
    public void OnProjectileImpactsEnemy(Enemy_Controller enemy, int damage, float duration)
    {
		onProjectileImpactsEnemy?.Invoke(enemy, damage, duration);
    }

	/// Triggered when Projectile explosion is over
	public void OnProjectileExploded(Projectile_Controller projectile)
	{
		onProjectileExploded?.Invoke(projectile);
	}

	/// Triggered when Projectile is requested by a Tower to shoot
	public void OnShootProjectileRequest(Vector3 originalPos, Transform destinationPos, Projectile_Data projectileData)
	{
		onShootProjectileRequest?.Invoke(originalPos, destinationPos, projectileData);
	}



	/// Triggered when Spawning a new Wave
	public void OnSpawningNewWave(int currentWave, int totalWaves)
	{
		onSpawningNewWave?.Invoke(currentWave, totalWaves);
	}

	/// Triggered when the last enemy from a wave despawned
	public void OnWaveIsOver()
	{
		onWaveIsOver?.Invoke();
	}
	/// Triggered when the last enemy from the last wave despawned
	public void OnLastWaveIsOver()
	{
		onLastWaveIsOver?.Invoke();
	}


	#endregion
}
