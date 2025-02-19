using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody), typeof(CheckpointSystem))]
public class CarAgent : Agent
{
    [Header("Vehicle Physics")]
    public float accelerationForce = 500f;
    public float steeringForce = 30f;
    public float maxSpeed = 20f;
    public float brakeDrag = 5f;
    private float normalDrag = 0.1f;

    [Header("Training Parameters")]
    public float checkpointReward = 1.0f;
    public float wallPenalty = -1.0f;
    public float timePenalty = -0.001f;
    public float completeBonus = 5.0f;

    private Rigidbody rb;
    private CheckpointSystem checkpointSystem;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private int totalCheckpoints;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        checkpointSystem = GetComponent<CheckpointSystem>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        totalCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Velocity (3 values)
        sensor.AddObservation(rb.linearVelocity.normalized);
        
        // Next checkpoint direction (3 values)
        sensor.AddObservation(checkpointSystem.GetNextCheckpointDirection().normalized);
        
        // Progress (1 value)
        sensor.AddObservation((float)checkpointSystem.CurrentCheckpoint / totalCheckpoints);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        
        // Steering [-1, 1]
        float steering = Mathf.Clamp(continuousActions[0], -1f, 1f);
        transform.Rotate(Vector3.up * steering * steeringForce * Time.deltaTime);
        
        // Acceleration [0, 1]
        float acceleration = Mathf.Clamp(continuousActions[1], 0f, 1f);
        if(rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * acceleration * accelerationForce, 
                       ForceMode.Acceleration);
        }
        
        // Braking [0, 1]
        float braking = Mathf.Clamp(continuousActions[2], 0f, 1f);
        rb.linearDamping = Mathf.Lerp(normalDrag, brakeDrag, braking);
        
        AddReward(timePenalty);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Checkpoint"))
        {
            if(checkpointSystem.IsCorrectCheckpoint(other.gameObject))
            {
                AddReward(checkpointReward);
                checkpointSystem.UpdateNextCheckpoint();
                
                if(checkpointSystem.CurrentCheckpoint >= totalCheckpoints)
                {
                    AddReward(completeBonus);
                    EndEpisode();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Walls"))
        {
            AddReward(wallPenalty);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        checkpointSystem.ResetCheckpoints();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // Steering
        continuousActions[1] = Input.GetKey(KeyCode.W) ? 1f : 0f; // Acceleration
        continuousActions[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f; // Braking
    }
}
