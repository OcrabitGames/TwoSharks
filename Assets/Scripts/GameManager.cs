using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public int highScore;
    public int score = 0;

    // Main Highscore Display
    public TextMeshProUGUI highScoreValueText;
    
    public TextMeshProUGUI scoreText;
    public GameObject gameOverCanvas;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestScoreText;
    
    public MovementManager movementController;
    
    [SerializeField] private bool gameActive = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreValueText.text = highScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    public void UpdateHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            highScore = score;
            highScoreValueText.text = highScore.ToString();
        }
    }

    public void GoHome()
    {
        UpdateHighScore();
        FishManager.Instance.ReturnAllFish();
        movementController.Reset();
    }
    
    public void GameOver()
    {
        UpdateHighScore();
        
        FishManager.Instance.DeactivateFish();
        
        gameOverCanvas.SetActive(true);
        gameOverScoreText.text = score.ToString();
        gameOverBestScoreText.text = highScore.ToString();
        
        movementController.Deactivate();
        
        gameActive = false;
    }

    public void Restart()
    {
        FishManager.Instance.ReturnAllFish();
        FishManager.Instance.ActivateFish();
        ResetScore();

        gameOverCanvas.SetActive(false);
        movementController.Activate();
        
        gameActive = true;
    }

    public void Pause()
    {
        FishManager.Instance.DeactivateFish();
        movementController.Deactivate();

        gameActive = false;
    }

    public void Resume()
    {
        FishManager.Instance.ActivateFish();
        movementController.Activate();

        gameActive = true;
    }
}
