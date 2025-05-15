using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerWithDelay : MonoBehaviour
{
    [Tooltip("Nama scene tujuan (case-sensitive)")]
    public string targetSceneName;

    [Tooltip("Delay sebelum pindah scene (detik)")]
    public float delay = 2f;

    void Start()
    {
        // Mulai coroutine untuk pindah scene setelah delay
        StartCoroutine(ChangeSceneAfterDelay());
    }

    System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        // Tunggu selama delay
        yield return new WaitForSeconds(delay);

        // Pindah scene
        SceneLoader.nextSceneName = targetSceneName;
        SceneManager.LoadScene("_LoadingScreenScene");
    }
}