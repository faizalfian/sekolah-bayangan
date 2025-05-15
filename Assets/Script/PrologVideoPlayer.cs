using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class PrologVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoName;
    public ScreenFader screenFader;  // reference ke fader

    void Start()
    {
	videoPlayer.errorReceived += OnVideoError;
        StartCoroutine(PlayWithFade());
    }

    IEnumerator PlayWithFade()
    {
        // Fade in dulu (layar dari hitam jadi terlihat)
        yield return StartCoroutine(screenFader.FadeIn());

        // Set path video
        string fullPath = Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.url = fullPath;
	    videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = false;

        // Prepare video dulu sebelum play supaya lancar
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Play video dan daftarkan event end video
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Mulai coroutine fade out, baru pindah scene
        StartCoroutine(FadeOutAndLoadNext());
    }

    IEnumerator FadeOutAndLoadNext()
    {
        yield return StartCoroutine(screenFader.FadeOut());
        SceneLoader.nextSceneName = "grave";  // sesuaikan nama scene berikutnya
        SceneManager.LoadScene("_LoadingScreenScene");
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError("VideoPlayer error: " + message);
    }
}
