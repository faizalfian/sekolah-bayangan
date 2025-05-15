using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

public class EpilogVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    [SerializeField] public string videoName;

    void Start()
{
    string fullPath = Path.Combine(Application.streamingAssetsPath, videoName);
    Debug.Log("Full path: " + fullPath);

    videoPlayer.url = fullPath;
    videoPlayer.loopPointReached += OnVideoEnd;
    videoPlayer.Play();
}


    void OnVideoEnd(VideoPlayer vp)
    {
        SceneLoader.nextSceneName = "1. MainMenu"; // Ganti dengan nama scene Main Menu kamu yang sebenarnya
        SceneManager.LoadScene("_LoadingScreenScene");
    }
}
