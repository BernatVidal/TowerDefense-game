using System.Collections.Generic;

/// <summary>
/// Enemy Waves Controller
/// </summary>
public class EnemyWaves
{
    int waveCounter;
    int[,] waves;
    Enemy_Data[] enemies;

    public int WaveCounter => waveCounter;
    public int TotalWaves => waves.GetLength(0);

    public EnemyWaves(int[,] waves, Enemy_Data[] enemies)
    {
        waveCounter = 0;
        this.waves = waves;
        this.enemies = enemies;
    }

    public Stack<Enemy_Data> GetNextWave()
    {
        Stack<Enemy_Data> enemiesWave = new();
        for(int i = 0; i < waves.GetLength(1); i++)
        {
            for(int j = 0; j < waves[waveCounter, i]; j++)
            {
                enemiesWave.Push(enemies[i]);
            }
        }
        ++waveCounter;
        return new (enemiesWave);
    }
}
