using UnityEngine;

public class Printer : MonoBehaviour
{
    public bool position = false;
    public bool rotation = false;
    public bool scale = false;
    public float callInterval = 0.5f;

    private void Start()
    {
        InvokeRepeating(nameof(print), 0f, callInterval);
    }

    private void print()
    {
        if (position)
        {
            Debug.Log($"localPos: {transform.localPosition}");
            Debug.Log($"globalPos: {transform.position}");
        }
        if (rotation)
        {
            Debug.Log($"localRot: {transform.localRotation}");
            Debug.Log($"globalRot: {transform.rotation}");
        }
        if (scale)
        {
            Debug.Log($"localScale: {transform.localScale}");
            Debug.Log($"globalScale: {transform.lossyScale}");
        }
    }
}
