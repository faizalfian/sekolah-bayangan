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
        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f) * 100f;
            loadingText.text = $"Loading... {Mathf.RoundToInt(progress)}%";

            if (operation.progress >= 0.9f)
            {
                loadingText.text = "Press any key to continue";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("_LoadingScreenScene");
    }
}
