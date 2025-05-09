using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] private UnityEvent onPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter.Invoke();
        }
    }
}