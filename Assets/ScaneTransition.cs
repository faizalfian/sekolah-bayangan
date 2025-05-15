using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaneTransition : MonoBehaviour
{
    [SerializeField] private float delay = 2f;

    void Start()
    {
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(SceneLoader.nextSceneName))
        {
            SceneManager.LoadScene(SceneLoader.nextSceneName);
        }
        else
        {
            Debug.LogError("NextSceneName is not set!");
        }
    }
}
