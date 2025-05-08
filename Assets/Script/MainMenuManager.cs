using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
   public void Play()
   {
      Debug.Log("Clicked");
    SceneManager.LoadScene("SekolahBayangan-GamePlay");
   }

   public void Exit()
   {
    Application.Quit();
   }
}
