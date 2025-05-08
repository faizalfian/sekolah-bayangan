using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public DialogManager dialogManager;

    void Update()
    {
    if (Input.GetKeyDown(KeyCode.T))
    {
        Debug.Log("Dialog");
        dialogManager.StartDialog();
    }
    } 
}
