using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int highScore;
    public int score;
    
    // Main Highscore Display
    public TextMeshProUGUI highScoreValueText;
    
    // Audio Display
    public GameObject audioIcon;
    public Sprite[] audioIcons;
    [SerializeField] private bool audioMuted;
    
    // Audio Source
    private AudioSource _audioSource;
    private float _audioPitch;
    [SerializeField] private float audioPitchAdjust;
    public AudioClip deathClip;
    private Coroutine _audioLoopCoroutine;
    private Coroutine _deathSoundCoroutine;
    
    public TextMeshProUGUI scoreText;
    public GameObject gameOverCanvas;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestScoreText;
    
    public MovementManager movementController;
    
    [SerializeField] private bool gameActive;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        
        // Authentic GameCenter On Load
        GameCenterManager.Authenticate();
        
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreValueText.text = highScore.ToString();
        
        _audioSource = GetComponent<AudioSource>();
        _audioPitch = _audioSource.pitch;
        audioPitchAdjust = _audioPitch;
        
        audioMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
        UpdateAudioIcon();
    }

    public void IncrementPitch(float progressionRate)
    {
        audioPitchAdjust = _audioPitch + _audioPitch * progressionRate;
        _audioSource.pitch = audioPitchAdjust;
    }
    private void UpdateAudioIcon()
    {
        audioIcon.GetComponent<SpriteRenderer>().sprite = audioMuted ? audioIcons[1] : audioIcons[0];
    }
    public void ChangeAudio()
    {
        audioMuted = !audioMuted;
        var val = audioMuted ? 1 : 0;
        PlayerPrefs.SetInt("AudioMuted", val);
        PlayerPrefs.Save();

        UpdateAudioIcon();
    }

    private IEnumerator PlayDeathSoundCoroutine()
    {
        // Stop and play clip
        _audioSource.Stop();

        _audioSource.pitch = 0.7f;
        _audioSource.PlayOneShot(deathClip);

        // Wait for the clip
        yield return new WaitForSeconds(deathClip.length);

        if (!gameActive)
        {
            // Play clip with pitch down
            _audioSource.time = 25.7f;
            _audioSource.Play();
        }
    }

    private void PlayGameAudio()
    {
        _audioSource.Stop();
        _audioSource.pitch = _audioPitch;
        _audioSource.Play();
    }

    private void CheckStopFadeCoroutine()
    {
        if (_audioLoopCoroutine == null) return;
        
        StopCoroutine(_audioLoopCoroutine);
        _audioLoopCoroutine = null;
    }
    
    private IEnumerator FadeAudioLoop()
    {
        
        float fadeDuration = 2f;
        float originalVolume = _audioSource.volume;
 
        while (true)
        {
            if (!gameActive)
            {
                yield return null;
                continue;
            }
            
            // Fade out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(originalVolume, 0f, t / fadeDuration);
                yield return null;
            }
            _audioSource.volume = 0f;
 
            // Restart audio
            _audioSource.Stop();
            _audioSource.time = 0f;
            _audioSource.Play();
 
            // Fade in
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0f, originalVolume, t / fadeDuration);
                yield return null;
            }
            _audioSource.volume = originalVolume;
 
            // Wait for clip duration minus fade time
            yield return new WaitForSeconds(_audioSource.clip.length - 2 * fadeDuration);
        }
    }
 
    private void StartLoopFade()
    {
        _audioSource.pitch = _audioPitch;
        _audioSource.loop = false;

        if (_audioLoopCoroutine != null)
        {
            StopCoroutine(_audioLoopCoroutine);
        }

        _audioLoopCoroutine = StartCoroutine(FadeAudioLoop());
    }
    
    public void PlayLoopingGameAudio()
    {
        StartLoopFade();
    }
    
    public int GetScore()
    {
        return score;
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

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateHighScore()
    {
        // Update Leaderboard
        GameCenterManager.SubmitScore(score);
        
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
        // Audio
        CheckStopFadeCoroutine();
        if (!audioMuted) _audioSource.Stop();
        
        // Actions
        UpdateHighScore();
        FishManager.Instance.ReturnAllFish();
        movementController.Reset();
    }
    
    public void GameOver()
    {
        // Score
        UpdateHighScore();

        // Audio
        CheckStopFadeCoroutine();
        if (!audioMuted) StartCoroutine(PlayDeathSoundCoroutine());

        
        // Actions
        FishManager.Instance.DeactivateFish();
        
        gameOverCanvas.SetActive(true);
        gameOverScoreText.text = score.ToString();
        gameOverBestScoreText.text = highScore.ToString();
        
        movementController.Deactivate();
        
        gameActive = false;
    }

    public void Restart()
    {
        // Audio
        CheckStopFadeCoroutine();
        if (!audioMuted) PlayGameAudio();
        
        // Actions
        FishManager.Instance.ReturnAllFish();
        FishManager.Instance.ActivateFish();
        ResetScore();

        gameOverCanvas.SetActive(false);
        movementController.Activate();
        
        gameActive = true;
    }

    public void Pause()
    {
        // Audio
        if (!audioMuted) _audioSource.Pause();
        
        // Actions
        FishManager.Instance.DeactivateFish();
        movementController.Deactivate();

        gameActive = false;
    }

    public void Resume()
    {
        // Audio
        if (!audioMuted) _audioSource.UnPause();
        
        // Actions
        FishManager.Instance.ActivateFish();
        movementController.Activate();

        gameActive = true;
    }
}
