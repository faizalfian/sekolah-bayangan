using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float activationRadius = 3f;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private ParticleSystem activationEffect;

    [Header("Debug")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool hasActive = false;

    private SphereCollider checkpointCollider;
    private static Action<Checkpoint> OnCheckpointActivated;
    public Action onEnter;

    private void Awake()
    {
        checkpointCollider = GetComponent<SphereCollider>();
        checkpointCollider.radius = activationRadius;
    }

    private void OnEnable()
    {
        OnCheckpointActivated += HandleOtherCheckpointActivation;
    }

    private void OnDisable()
    {
        OnCheckpointActivated -= HandleOtherCheckpointActivation;
    }

    private void OnTriggerEnter(Collider other)
    {
        onEnter?.Invoke();
        if (other.CompareTag("Player") && !isActive && !hasActive)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        isActive = true;
        hasActive = true;

        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        OnCheckpointActivated?.Invoke(this);
        CheckpointManager.Instance.SetCurrentCheckpoint(this);
        Debug.Log($"Checkpoint activated: {gameObject.name}");
    }

    private void HandleOtherCheckpointActivation(Checkpoint activatedCheckpoint)
    {
        if (activatedCheckpoint != this && isActive)
        {
            DeactivateCheckpoint();
        }
    }

    public void DeactivateCheckpoint()
    {
        isActive = false;
    }


    public bool IsActive()
    {
        return isActive;
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position + Vector3.up * 1.5f; // Sedikit di atas checkpoint
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? activeColor : inactiveColor;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}