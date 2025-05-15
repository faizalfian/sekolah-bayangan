using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogBranchRouter : MonoBehaviour
{
    public DialogManager dialogManager;

    [Header("Penghubung consequenceTag ke scene")]
    public string tagUnlockSkill = "UnlockSkill";
    public string sceneForUnlockSkill = "Scene_UnlockSkill";

    public string tagToEpilog = "Epilog";
    public string sceneForEpilog = "Scene_Epilog";

    public string tagFight = "Fight";
    public string sceneForFight = "Scene_Fight";

    public string tagPeace = "Peace";
    public string sceneForPeace = "Scene_Peace";

    void Start()
    {
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnd = CekPilihanDanLanjutScene;
        }
        else
        {
            Debug.LogError("DialogManager belum disambungkan.");
        }
    }

    void CekPilihanDanLanjutScene()
    {
        if (dialogManager.playerDecisions.Count == 0)
        {
            Debug.LogWarning("Belum ada keputusan diambil.");
            return;
        }

        string tag = dialogManager.playerDecisions[dialogManager.playerDecisions.Count - 1];
        Debug.Log("Pilihan terakhir: " + tag);

        if (tag == tagUnlockSkill)
        {
            SceneManager.LoadScene(sceneForUnlockSkill);
        }
        else if (tag == tagToEpilog)
        {
            SceneManager.LoadScene(sceneForEpilog);
        }
        else if (tag == tagFight)
        {
            SceneManager.LoadScene(sceneForFight);
        }
        else if (tag == tagPeace)
        {
            SceneManager.LoadScene(sceneForPeace);
        }
        else
        {
            Debug.LogWarning("Tag tidak dikenali: " + tag);
        }
    }
}
