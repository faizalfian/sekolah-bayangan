using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogBranchRouter : MonoBehaviour
{
    public DialogManager dialogManager;

    [Header("Nama Scene berdasarkan Tag Konsekuensi")]
    public string tagFight = "Fight";        // contoh consequenceTag
    public string sceneForFight = "Cutscene_Fight";

    public string tagPeace = "Peace";
    public string sceneForPeace = "Cutscene_Peace";

    void Start()
    {
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnd += CekPilihanDanPindahScene;
        }
        else
        {
            Debug.LogError("DialogManager belum disambungkan di DialogBranchRouter!");
        }
    }

    void CekPilihanDanPindahScene()
    {
        if (dialogManager.playerDecisions.Count == 0)
        {
            Debug.LogWarning("Belum ada pilihan yang diambil.");
            return;
        }

        string lastTag = dialogManager.playerDecisions[dialogManager.playerDecisions.Count - 1];

        if (lastTag == tagFight)
        {
            SceneManager.LoadScene(sceneForFight);
        }
        else if (lastTag == tagPeace)
        {
            SceneManager.LoadScene(sceneForPeace);
        }
        else
        {
            Debug.LogWarning("Tag tidak dikenali: " + lastTag);
        }
    }
}
