using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Target yang akan diikuti
    public bool followOnStart = true; // Mulai follow saat Start()

    [Header("Follow Options")]
    public bool followPosition = true;
    public bool followRotation = true;

    [Header("Position Settings")]
    public Vector3 positionOffset = Vector3.zero;
    public float positionSmoothness = 5f; // Kehalusan pergerakan (0 = instant)

    [Header("Rotation Settings")]
    public Vector3 rotationOffset = Vector3.zero;
    public float rotationSmoothness = 5f; // Kehalusan rotasi (0 = instant)

    private bool isFollowing = false;

    void Start()
    {
        if (followOnStart && target != null)
        {
            StartFollowing();
        }
    }

    void LateUpdate()
    {
        if (isFollowing && target != null)
        {
            if (followPosition)
            {
                // Smooth follow position
                transform.position = positionSmoothness > 0
                    ? Vector3.Lerp(transform.position, target.position + positionOffset, positionSmoothness * Time.deltaTime)
                    : target.position + positionOffset;
            }

            if (followRotation)
            {
                // Smooth follow rotation
                Quaternion targetRotation = target.rotation * Quaternion.Euler(rotationOffset);
                transform.rotation = rotationSmoothness > 0
                    ? Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime)
                    : targetRotation;
            }
        }
    }

    // API untuk kontrol dari script lain
    public void StartFollowing()
    {
        isFollowing = true;

        // Snap ke posisi awal jika smoothness = 0
        if (positionSmoothness <= 0 && followPosition)
        {
            transform.position = target.position + positionOffset;
        }

        if (rotationSmoothness <= 0 && followRotation)
        {
            transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }

    public void SetTarget(Transform newTarget, bool startFollowing = true)
    {
        target = newTarget;
        if (startFollowing) StartFollowing();
    }

    // Untuk debug di Editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(target.position + positionOffset, 0.1f);
        }
    }
}