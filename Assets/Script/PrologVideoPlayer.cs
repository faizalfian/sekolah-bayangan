using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

public class PrologVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName;
    [SerializeField]
    public string videoName;

    void Start()
    {
        // 👉 Set path video ke dalam StreamingAssets
        string fullPath = Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.url = fullPath;

        // Mulai play setelah path diset
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneLoader.nextSceneName = "grave";
        SceneManager.LoadScene("_LoadingScreenScene");
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     videoPlayer.Stop();
        //     SceneManager.LoadScene(nextSceneName);
        // }
    }
}
