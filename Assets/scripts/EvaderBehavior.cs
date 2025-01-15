using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class EvaderBehavior : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private ChaserAgent pursuer;
    [SerializeField] private GameObject playManager;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 200f;
    private bool hasCollided = false;

    // move smoothing
    private float smoothTurn = 0f;
    private float smoothMoveForward = 0f;
    private float turnSmoothingFactor = 20f;
    private float moveSmoothingFactor = 20f;

    public override void OnEpisodeBegin()
    {
        hasCollided = false;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        target.localPosition = new Vector3(Random.Range(-3f, 3f), 0.3f, Random.Range(-3f, 3f));
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(pursuer.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float turn = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        // 平滑更新
        smoothTurn = Mathf.Lerp(smoothTurn, turn, Time.deltaTime * turnSmoothingFactor);
        smoothMoveForward = Mathf.Lerp(smoothMoveForward, moveForward, Time.deltaTime * moveSmoothingFactor);

        Move(smoothTurn, smoothMoveForward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        float inputTurn = Input.GetAxisRaw("Horizontal");
        float inputMoveForward = Input.GetAxisRaw("Vertical");

        smoothTurn = Mathf.Lerp(smoothTurn, inputTurn, Time.deltaTime * turnSmoothingFactor);
        smoothMoveForward = Mathf.Lerp(smoothMoveForward, inputMoveForward, Time.deltaTime * moveSmoothingFactor);

        continousActions[0] = smoothTurn;
        continousActions[1] = smoothMoveForward;
    }

    private void Move(float turn, float moveForward)
    {
        transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * moveForward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "target")
            HandleCollision(1f, other.gameObject.tag);
        else if (other.gameObject.tag == "wall")
            HandleCollision(-0.1f, other.gameObject.tag);
        else if (other.gameObject.tag == "pursuer")
            HandleCollision(-0.3f, other.gameObject.tag);
    }

    private void HandleCollision(float reward, string tag)
    {
        if (!hasCollided)
        {
            AddReward(reward);
            hasCollided = true;
        }

        pursuer.InitializePositions();
        playManager.GetComponent<PlayManager>().episodeCount++;
        EndEpisode();
    }
}
