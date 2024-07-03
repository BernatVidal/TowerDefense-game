using UnityEngine;
using TMPro;

/// <summary>
/// A Passive UI Manager, shows basic game info, called from GameManager
/// </summary>
public class UI_Manager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameStatusText;
    [SerializeField] TextMeshProUGUI livesAmountText;
    [SerializeField] GameObject levelCompleted;
    [SerializeField] GameObject gameOver;

    public enum GameStatus
    {
        loadingLevel,
        spawningWave,
        waveCompleted,
        gameOver,
        levelCompleted
    }

    GameStatus status;

    // Start is called before the first frame update
    void Start()
    {
        levelCompleted.SetActive(false);
        gameOver.SetActive(false);
    }

    public void UpdateGameStatusText(GameStatus status, string extraInfo = "")
    {
        this.status = status;
        gameStatusText.text = $"{GetGameStatusText()} {extraInfo} </color>";
        // Show GameOver or LevelCompleted texts if required
        gameOver.SetActive(status == GameStatus.gameOver);
        levelCompleted.SetActive(status == GameStatus.levelCompleted);
    }

    public void UpdateLivesAmountText(int currentHits, int totalLives)
    {
        // Set text color according to remaining lives
        string currentColor = (float)currentHits / totalLives <= (float)1 / 3 ? "<color=green>" :
                              (float)currentHits / totalLives <= (float)2 / 3 ? "<color=orange>" : "<color=red>";

        livesAmountText.text = $"{currentColor} {currentHits} / {totalLives} </color>";        
    }

    string GetGameStatusText()
    {
        switch(status)
        {
            case GameStatus.loadingLevel:
                return "<color=red>An Enemy Horde is comming to your Tower..";
            case GameStatus.spawningWave:
                return "<color=orange>Spawning wave";
            case GameStatus.waveCompleted:
                return "<color=green>Wave completed!";
            case GameStatus.gameOver:
                return "<color=red>The Enemy Horde has conquered your Tower..";
            case GameStatus.levelCompleted:
                return "<color=green>Enemy Horde Defeated!";
            default:
                return "<color=white>";
        }
    }
}
