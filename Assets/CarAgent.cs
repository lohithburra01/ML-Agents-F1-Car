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
    public float completeBonus = 25.0f;
    public float timeoutPenalty = -5.0f;
    public float checkpointTimeout = 30.0f; // 30 seconds timeout
    public float wrongcheckpointPenalty = -25.0f;

    private Rigidbody rb;
    private CheckpointSystem checkpointSystem;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private int totalCheckpoints;
    private float checkpointTimer;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        checkpointSystem = GetComponent<CheckpointSystem>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        totalCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;
        checkpointTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
   {
       // Velocity (2 values - x and z only)
       Vector3 velocityXZ = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
       sensor.AddObservation(velocityXZ.normalized);
    
       // Next checkpoint direction (2 values - x and z only)
       Vector3 checkpointDirectionXZ = checkpointSystem.GetNextCheckpointDirection();
       checkpointDirectionXZ.y = 0f; // Ignore the y-axis
       sensor.AddObservation(checkpointDirectionXZ.normalized);
   }


    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousActions = actions.ContinuousActions;
        
        // Steering [-1, 1]
        float steering = Mathf.Clamp(continuousActions[0], -1f, 1f);
        transform.Rotate(Vector3.up * steering * steeringForce * Time.deltaTime);
        
        // Acceleration [0, 1]
        float acceleration = Mathf.Clamp(continuousActions[1], 0f, 1f);
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * acceleration * accelerationForce, 
                       ForceMode.Acceleration);
        }
        
        // Braking [0, 1]
        float braking = Mathf.Clamp(continuousActions[2], 0f, 1f);
        rb.drag = Mathf.Lerp(normalDrag, brakeDrag, braking);
        
        AddReward(timePenalty);
        AddReward(acceleration / 50);

        Debug.Log($"accelaration:{acceleration} breaking:{braking} steering{steering}");

        // Update checkpoint timer
        checkpointTimer += Time.deltaTime;
        if (checkpointTimer > checkpointTimeout)
        {
            AddReward(timeoutPenalty);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (checkpointSystem.IsCorrectCheckpoint(other.gameObject))
            {
                AddReward(checkpointReward);
                checkpointSystem.UpdateNextCheckpoint();
                checkpointTimer = 0f; // Reset timer when reaching a checkpoint
                
                if (checkpointSystem.CurrentCheckpoint >= totalCheckpoints)
                {
                    AddReward(completeBonus);
                    EndEpisode();
                }
            }
            else
            {
                AddReward(wrongcheckpointPenalty);
                EndEpisode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            AddReward(wallPenalty);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        checkpointSystem.ResetCheckpoints();
        checkpointTimer = 0f; // Reset the timer
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // Steering
        continuousActions[1] = Input.GetKey(KeyCode.W) ? 1f : 0f; // Acceleration
        continuousActions[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f; // Braking
    }
}
