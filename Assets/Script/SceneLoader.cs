using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string targetScene;
    [SerializeField] private TMP_Text loadingText;

    private void Start()
    {
        StartCoroutine(LoadPrologThenStage());
    }

    private IEnumerator LoadPrologThenStage()
    {
        // Pertama-tama load prolog
        AsyncOperation prologOperation = SceneManager.LoadSceneAsync("PrologScene");
        prologOperation.allowSceneActivation = false;

        while (!prologOperation.isDone)
        {
            float progress = Mathf.Clamp01(prologOperation.progress / 0.9f) * 100f;
            loadingText.text = $"Loading Prolog... {Mathf.RoundToInt(progress)}%";

            if (prologOperation.progress >= 0.9f)
            {
                loadingText.text = "Tekan tombol apa saja untuk melanjutkan ke stage 1";
                if (Input.anyKeyDown)
                {
                    prologOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // Setelah prolog selesai, load stage 1
        AsyncOperation stageOperation = SceneManager.LoadSceneAsync("Stage1");
        stageOperation.allowSceneActivation = false;

        while (!stageOperation.isDone)
        {
            float progress = Mathf.Clamp01(stageOperation.progress / 0.9f) * 100f;
            loadingText.text = $"Loading Stage 1... {Mathf.RoundToInt(progress)}%";

            if (stageOperation.progress >= 0.9f)
            {
                loadingText.text = "Tekan tombol apa saja untuk melanjutkan";
                if (Input.anyKeyDown)
                {
                    stageOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("LoadScreenScene");
    }
}
