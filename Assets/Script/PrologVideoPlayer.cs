using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

public class PrologVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName;

    void Start()
    {
        // 👉 Set path video ke dalam StreamingAssets
        string fullPath = Path.Combine(Application.streamingAssetsPath, "prolog.mov");
        videoPlayer.url = fullPath;

        // Mulai play setelah path diset
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
