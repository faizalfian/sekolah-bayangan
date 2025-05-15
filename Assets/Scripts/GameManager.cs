using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    List<int> enemies = new List<int>();
    [SerializeField]
    BimaMvController playerMovement;
    [SerializeField]
    DialogManager dialogBegin;
    [SerializeField]
    DialogManager dialogEnd;
    [SerializeField] GameObject boss;
    [SerializeField]
    AudioSource backgroundMusic;

    public bool isPlaying = false;
    public bool isOver = false;
    public bool isDialog = true;

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

    void Start()
    {
        startDialog();
        dialogBegin.StartDialog();
        dialogBegin.OnDialogEnd += () => {
            endDialog();
        };
    }

    void startDialog()
    {
        isDialog = true;
        isPlaying = false;

        // Beri delay sebelum mengunci movement
        StartCoroutine(DelayedMovementLock());
    }

    void endDialog()
    {
        playerMovement.enableMovement(true);
        playerMovement.GetComponent<BimaCombat>().enabled = true;
        isDialog = false;
        isPlaying = true;
        backgroundMusic.volume = 0.4f;
    }

    IEnumerator DelayedMovementLock()
    {
        // Biarkan bergerak selama 1 detik sebelum dikunci
        yield return new WaitForSeconds(1f);
        playerMovement.enableMovement(false);
        playerMovement.GetComponent<BimaCombat>().enabled = false;
    }

    public void addEnemy()
    {
        Debug.Log("Enemy Added");
        enemies.Add(1);
    }

    public void removeEnemy()
    {
        Debug.Log(enemies);
        enemies.RemoveAt(0);
        //Debug.Log(enemies);
        Debug.Log("Enemy Removed");
        if (enemies.Count == 0)
        {
            startDialog();
            dialogEnd.StartDialog();
            dialogEnd.OnDialogEnd = () =>
            {
                endDialog();
                Destroy(boss);
                SceneLoader.nextSceneName = "0. InterludeScene";
                SceneManager.LoadScene("_LoadingScreenScene");
            };
            Debug.Log("Game Over");
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
        Debug.Log("Loading stage: " + stageIndex);
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


    public void goToDialogScene()
    {
        Debug.Log("Oke finish stage");
        SceneManager.LoadScene("4. DialogScene");
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