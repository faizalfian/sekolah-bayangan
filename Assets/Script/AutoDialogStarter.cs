using UnityEngine;

public class AutoDialogStarter : MonoBehaviour
{
    public DialogManager dialogManager;

    void Start()
    {
        dialogManager.StartDialog();
    }
}
