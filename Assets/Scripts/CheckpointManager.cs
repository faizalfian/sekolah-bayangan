using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Checkpoint[] allCheckpoints;
    private Checkpoint currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FindAllCheckpoints();
    }

    private void FindAllCheckpoints()
    {
        allCheckpoints = FindObjectsOfType<Checkpoint>();
        Debug.Log($"Found {allCheckpoints.Length} checkpoints in scene");
    }

    public void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null && currentCheckpoint != checkpoint)
        {
            currentCheckpoint.DeactivateCheckpoint();
        }

        currentCheckpoint = checkpoint;
        Debug.Log($"Current checkpoint set to: {currentCheckpoint.gameObject.name}");
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.GetRespawnPosition();
        }

        // Default spawn point jika tidak ada checkpoint
        Debug.LogWarning("No checkpoint active, returning default spawn position");
        return Vector3.zero;
    }
}