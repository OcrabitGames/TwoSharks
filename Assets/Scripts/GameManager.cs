using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int highScore = 0;
    public int score = 0;
    
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

    public void GameOver()
    {
        if (score > highScore)
        {
            highScore = score;
        }
        
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
}
