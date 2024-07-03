using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads the game injecting dependencies on the main components, and keeps track of main game events
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields

    GameSettings gameSettings;
    Camera_Controller cameraController;
    UI_Manager uiManager;
    [SerializeField] Map_Generator mapGrid;
    [SerializeField] Enemies_Manager enemiesManager;
    [SerializeField] Projectiles_Manager projectilesManager;

    int currentHitsReceived;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        gameSettings = GetComponent<GameSettings>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera_Controller>();
        uiManager = GameObject.FindGameObjectWithTag("UI_Manager").GetComponent<UI_Manager>();
    }

    void Start()
    {
        currentHitsReceived = 0;

        uiManager.UpdateGameStatusText(UI_Manager.GameStatus.loadingLevel);
        uiManager.UpdateLivesAmountText(currentHitsReceived, gameSettings.playerLives);

        /**** Map Generation ****/
        // Decode Map file
        Level level = new Level_Decoder().DecodeMapFromFile(gameSettings.levelData);
        if (level.mapGrid == null || level.enemyWaves == null)
            LogHandler.LogError("This Level cannot be played as its file contains errors.", true);

        // Generat Map and get its path        
        Stack<Vector3> mapPath = mapGrid.GenerateMapGrid(level.mapGrid, gameSettings.mapLayout);
        if (mapPath == null)
            LogHandler.LogError("This Map cannot be played as its file contains errors ont he Map Grid.", true);

        // Position Camera according to map size
        cameraController.PositionCameraToMap(mapGrid.MapSize);

        /************************/


        /**** Set EnemiesManager and Projectiles Manager****/

        enemiesManager.SetEnemyManager(gameSettings.enemies, mapPath, level.enemyWaves);
        projectilesManager.SetProjectilesManager(new Tower_Data[] { gameSettings.mapLayout.prefab_bombTower.GetComponent<Tower>().towerData,
                                                                    gameSettings.mapLayout.prefab_freezeTower.GetComponent<Tower>().towerData});
        /***************************************************/

        Subscribe();

    }

    #endregion


    #region Game Events

    void OnSpawningNewWave(int currentWave, int totalWaves)
    {
        if (currentWave == 1)
            Audio_Manager.Instance.PlaySound(gameSettings.mapLayout.music);
        LogHandler.LogInfo($"Spawning wave: {currentWave}/{totalWaves}");
        uiManager.UpdateGameStatusText(UI_Manager.GameStatus.spawningWave, $"{currentWave} / {totalWaves}");
    }

    void OnWaveIsOver()
    {
        LogHandler.LogInfo("Wave completed!");
        uiManager.UpdateGameStatusText(UI_Manager.GameStatus.waveCompleted);
    }

    void OnLastWaveEnded()
    {
        if (currentHitsReceived < gameSettings.playerLives)
        {
            LogHandler.LogInfo("GAME COMPLETED!");
            uiManager.UpdateGameStatusText(UI_Manager.GameStatus.levelCompleted);
            Audio_Manager.Instance.PlaySound(Sounds.SoundID.LevelCompleted);
        }
    }
    
    void OnPlayerLooseALive(Enemy_Controller _)
    {
        ++currentHitsReceived;
        uiManager.UpdateLivesAmountText(currentHitsReceived, gameSettings.playerLives);
        Audio_Manager.Instance.PlaySound(Sounds.SoundID.PlayerHurt);
        if (currentHitsReceived >= gameSettings.playerLives)
            GameOver();
    }

    void GameOver()
    {
        enemiesManager.enabled = false; // Stop enemies
        projectilesManager.enabled = false; // Stop projectiles
        uiManager.UpdateGameStatusText(UI_Manager.GameStatus.gameOver);
        Audio_Manager.Instance.PlaySound(Sounds.SoundID.GameOver);
    }


    private void Subscribe()
    {
        GameEvents.Instance.onSpawningNewWave += OnSpawningNewWave;
        GameEvents.Instance.onWaveIsOver += OnWaveIsOver;
        GameEvents.Instance.onLastWaveIsOver += OnLastWaveEnded;
        GameEvents.Instance.onEnemyAttack += OnPlayerLooseALive;
    }

    void OnDisable()
    {
        GameEvents.Instance.onSpawningNewWave -= OnSpawningNewWave;
        GameEvents.Instance.onWaveIsOver -= OnWaveIsOver;
        GameEvents.Instance.onLastWaveIsOver -= OnLastWaveEnded;
        GameEvents.Instance.onEnemyAttack -= OnPlayerLooseALive;
    }

    #endregion

}
