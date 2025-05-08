using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // Target yang diikuti (player)
    public Vector3 offset = new Vector3(3f, 2f, 0f); // Offset posisi kamera
    public float smoothSpeed = 5f; // Kehalusan pergerakan kamera

    [Header("Boundary Settings")]
    public bool useBoundaries = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = 2f;
    public float maxY = 5f;
    public float minZ = -10f;
    public float maxZ = 10f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not assigned!");
            return;
        }

        // Hitung posisi yang diinginkan
        Vector3 desiredPosition = target.position + offset;

        // Jika menggunakan boundary
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
        }

        // Interpolasi posisi kamera secara halus
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    // Method untuk mengubah target secara runtime
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Method untuk mengubah offset secara runtime
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Visualisasi boundary di editor
    void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (minX + maxX) / 2,
                (minY + maxY) / 2,
                (minZ + maxZ) / 2
            );
            Vector3 size = new Vector3(
                maxX - minX,
                maxY - minY,
                maxZ - minZ
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}