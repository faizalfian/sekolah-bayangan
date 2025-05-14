using UnityEngine;

public class CombatStarter : MonoBehaviour
{
    public DialogManager dialogManager;
    public GameObject combatSystem;

    void Start()
    {
        combatSystem.SetActive(false); // Matikan dulu combat
        dialogManager.OnDialogEnd = () => combatSystem.SetActive(true);
    }
}
