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
            .OrderBy(go => ExtractNumber(go.name)) // Sort by extracted number
            .ToList();
        ResetCheckpoints();
        Debug.Log("Checkpoints initialized. Total checkpoints: " + checkpoints.Count);
        Debug.Log("Checkpoint List: " + string.Join(", ", checkpoints.Select(cp => cp.name)));
    }

    private void Update()
    {
        Debug.Log("Current Checkpoint: " + CurrentCheckpoint);
        Debug.Log("Checkpoint List: " + string.Join(", ", checkpoints.Select(cp => cp.name)));
    }

    public Vector3 GetNextCheckpointDirection()
    {
        Vector3 direction = checkpoints[CurrentCheckpoint].transform.position - transform.position;
        Debug.Log("Next checkpoint direction: " + direction);
        return direction;
    }

    public bool IsCorrectCheckpoint(GameObject checkpoint)
    {
        bool isCorrect = checkpoint == checkpoints[CurrentCheckpoint];
        Debug.Log("Is correct checkpoint: " + isCorrect);
        return isCorrect;
    }

    public void UpdateNextCheckpoint()
    {
        CurrentCheckpoint = CurrentCheckpoint + 1;
        Debug.Log("Updated to checkpoint: " + CurrentCheckpoint);
        Debug.Log("Checkpoint List: " + string.Join(", ", checkpoints.Select(cp => cp.name)));
    }

    public void ResetCheckpoints()
    {
        CurrentCheckpoint = 0;
        Debug.Log("Checkpoints reset. Current checkpoint: " + CurrentCheckpoint);
        Debug.Log("Checkpoint List: " + string.Join(", ", checkpoints.Select(cp => cp.name)));
    }
}

