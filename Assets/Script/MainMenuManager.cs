using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Play clicked");
        SceneLoader.nextSceneName = "2. PrologScene";
        GameManagers.Instance.LoadScene("_LoadingScreenScene");
    }



    public void Exit()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}