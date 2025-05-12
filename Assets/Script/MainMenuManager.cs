using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Play clicked");
        GameManagers.Instance.LoadScene("2. PrologScene");
    }



    public void Exit()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}