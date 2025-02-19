using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CheckpointSystem : MonoBehaviour
{
    private List<GameObject> checkpoints;
    public int CurrentCheckpoint { get; private set; }

    private void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint")
            .OrderBy(go => go.name)
            .ToList();
        ResetCheckpoints();
    }

    public Vector3 GetNextCheckpointDirection()
    {
        return checkpoints[CurrentCheckpoint].transform.position - transform.position;
    }

    public bool IsCorrectCheckpoint(GameObject checkpoint)
    {
        return checkpoint == checkpoints[CurrentCheckpoint];
    }

    public void UpdateNextCheckpoint()
    {
        CurrentCheckpoint = (CurrentCheckpoint + 1) % checkpoints.Count;
    }

    public void ResetCheckpoints()
    {
        CurrentCheckpoint = 0;
    }
}
