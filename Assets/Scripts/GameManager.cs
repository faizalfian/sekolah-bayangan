using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Settings")]
    [Tooltip("Daftar urutan stage/scene yang akan dimainkan")]
    public List<string> stageSequence = new List<string>();

    [Header("Debug")]
    [SerializeField] private int _currentStageIndex = 0;
    [SerializeField] private int _currentScore = 0;
    [SerializeField] private int _highScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadSavedData()
    {
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        _currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);

        // Pastikan index tidak melebihi jumlah stage
        if (_currentStageIndex >= stageSequence.Count)
        {
            _currentStageIndex = 0;
        }
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }

    public int GetHighScore()
    {
        return _highScore;
    }

    public void ResetScore()
    {
        _currentScore = 0;
    }

    // Memuat stage berdasarkan index
    public void LoadStage(int stageIndex)
    {
        Debug.Log("Loading stage: "+stageIndex);
        if (stageIndex >= 0 && stageIndex < stageSequence.Count)
        {
            _currentStageIndex = stageIndex;
            PlayerPrefs.SetInt("CurrentStageIndex", _currentStageIndex);
            SceneManager.LoadScene(stageSequence[stageIndex]);
        }
        else
        {
            Debug.LogError("Stage index out of range!");
        }
    }

    // Memuat stage berikutnya dalam urutan
    public void LoadNextStage()
    {
        int nextStage = _currentStageIndex + 1;

        if (nextStage < stageSequence.Count)
        {
            LoadStage(nextStage);
        }
        else
        {
            Debug.Log("All stages completed! Returning to first stage.");
            LoadStage(0); // Kembali ke stage pertama
        }
    }

    // Memuat stage sebelumnya
    public void LoadPreviousStage()
    {
        int prevStage = _currentStageIndex - 1;
        LoadStage(prevStage >= 0 ? prevStage : 0);
    }

    // Memuat ulang stage saat ini
    public void ReloadCurrentStage()
    {
        LoadStage(_currentStageIndex);
    }

    // Untuk tombol UI atau debug
    public void CompleteCurrentStage()
    {
        AddScore(100); // Bonus saat menyelesaikan stage
        LoadNextStage();
    }

    [ContextMenu("Reset Progress")]
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        _currentScore = 0;
        _highScore = 0;
        _currentStageIndex = 0;
        Debug.Log("All progress reset!");
    }
}