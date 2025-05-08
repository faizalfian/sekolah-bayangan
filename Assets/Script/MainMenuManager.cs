using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Play clicked");
        SceneManager.LoadScene("SekolahBayangan-GamePlay");
    }

    public void Exit()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}