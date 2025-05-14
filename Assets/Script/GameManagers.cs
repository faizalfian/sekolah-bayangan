using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagers : MonoBehaviour
{
    public static GameManagers Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup antar scene
        }
        else
        {
            Destroy(gameObject); // Hanya 1 GameManager di scene manapun
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Delegasi ke SceneLoader
    }
}
