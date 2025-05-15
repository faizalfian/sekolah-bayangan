using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class EpilogVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoName;
    public ScreenFader screenFader;

    void Start()
    {
        StartCoroutine(PlayWithFade());
    }

    IEnumerator PlayWithFade()
    {
        yield return StartCoroutine(screenFader.FadeIn());

        string fullPath = Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.url = fullPath;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeIn());
        }
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        yield return StartCoroutine(screenFader.FadeOut());
        SceneLoader.nextSceneName = "1. MainMenu";
        SceneManager.LoadScene("_LoadingScreenScene");
    }
}
