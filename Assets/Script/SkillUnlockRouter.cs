using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillUnlockRouter : MonoBehaviour
{
    public DialogManager dialogManager;

    [Header("Tag Pilihan")]
    public string tagMembunuh = "Kill";
    public string tagMemaafkan = "Forgive";

    [Header("Scene Skill")]
    public string sceneKillSkill = "Scene_Skill_Kill";
    public string sceneForgiveSkill = "Scene_Skill_Forgive";

    void Start()
    {
        if (dialogManager != null)
        {
            dialogManager.OnDialogEnd = CekPilihanDanPindahScene;
        }
        else
        {
            Debug.LogError("DialogManager belum di-assign di SkillUnlockRouter!");
        }
    }

    void CekPilihanDanPindahScene()
    {
        if (dialogManager.playerDecisions.Count == 0)
        {
            Debug.LogWarning("Tidak ada pilihan untuk menentukan skill.");
            return;
        }

        string lastTag = dialogManager.playerDecisions[^1]; // Tag terakhir
        Debug.Log("Pilihan terakhir: " + lastTag);

        if (lastTag == tagMembunuh)
        {
            SceneManager.LoadScene(sceneKillSkill);
        }
        else if (lastTag == tagMemaafkan)
        {
            SceneManager.LoadScene(sceneForgiveSkill);
        }
        else
        {
            Debug.LogWarning("Tag tidak cocok dengan pilihan skill: " + lastTag);
        }
    }
}
