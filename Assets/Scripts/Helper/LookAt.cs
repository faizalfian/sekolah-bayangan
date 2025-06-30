using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(target);
    }
}
