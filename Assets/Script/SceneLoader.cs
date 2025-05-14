using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Video;
using System.Collections;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    public static string targetScene;

    [Header("Loading UI")]
    [SerializeField] private TMP_Text loadingText;

    [Header("Optional Video Prolog")]
    public bool playPrologVideo = false;
    public VideoPlayer videoPlayer;
    public string videoFileName = "prolog.mov";
    public string stageSceneName = "Stage1";

    private bool hasEnded = false;

    private void Start()
    {
        if (playPrologVideo && videoPlayer != null)
        {
            StartCoroutine(PlayPrologThenLoadStage());
        }
        else
        {
            StartCoroutine(LoadTargetSceneRoutine());
        }
    }

    // 🔹 Panggil dari scene manapun
    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene("LoadScreenScene"); // harus ada di Build Settings
    }

    private IEnumerator LoadTargetSceneRoutine()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(targetScene);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f) * 100f;
            loadingText.text = $"Loading {targetScene}... {Mathf.RoundToInt(progress)}%";

            if (async.progress >= 0.9f)
            {
                loadingText.text = "Tekan tombol apa saja untuk melanjutkan";
                if (Input.anyKeyDown)
                {
                    async.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    private IEnumerator PlayPrologThenLoadStage()
    {
        // 🔸 Atur path ke StreamingAssets
        string fullPath = Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = fullPath;

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();

        loadingText.text = "Memutar prolog... Tekan SPACE untuk skip";

        while (!hasEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                videoPlayer.Stop();
                OnVideoEnd(videoPlayer);
            }
            yield return null;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (hasEnded) return;
        hasEnded = true;

        StartCoroutine(LoadStageAfterProlog());
    }

    private IEnumerator LoadStageAfterProlog()
    {
        AsyncOperation stageOp = SceneManager.LoadSceneAsync(stageSceneName);
        stageOp.allowSceneActivation = false;

        while (!stageOp.isDone)
        {
            float progress = Mathf.Clamp01(stageOp.progress / 0.9f) * 100f;
            loadingText.text = $"Loading {stageSceneName}... {Mathf.RoundToInt(progress)}%";

            if (stageOp.progress >= 0.9f)
            {
                loadingText.text = "Tekan tombol apa saja untuk lanjut ke gameplay";
                if (Input.anyKeyDown)
                {
                    stageOp.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
