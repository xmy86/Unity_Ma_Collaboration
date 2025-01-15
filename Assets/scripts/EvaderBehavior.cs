using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class EvaderBehavior : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private ChaserAgent pursuer;
    [SerializeField] private GameObject playManager;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 200f;
    private bool hasCollided = false;

    public NavMeshAgent navMeshAgent;

    public override void Initialize()
    {
        base.Initialize();
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = turnSpeed;
        }
    }

    public override void OnEpisodeBegin()
    {
        hasCollided = false;
        InitializePosition();
    }

    private void Update()
    {
        if (navMeshAgent != null && target != null)
        {
            // 设置目标位置为 target 的位置
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void InitializePosition()
    {
        // 重置位置和方向
        transform.localPosition = new Vector3(-3f, 0f, -3f);
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 初始化目标位置
        target.GetComponent<targetBehavior>().InitializePosition();

        // 设置 NavMeshAgent 的起始位置
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position); // 确保 NavMeshAgent 的位置与对象一致
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(pursuer.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 自动导航模式下，不需要处理手动移动逻辑
        // 如果需要，可以加逻辑覆盖手动模式
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 在启用 NavMesh 时，不需要手动控制
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

    public void HandleCollision(float reward, string tag)
    {
        if (!hasCollided)
        {
            AddReward(reward);
            hasCollided = true;
        }

        pursuer.Initialize();
        playManager.GetComponent<PlayManager>().episodeCount++;
        EndEpisode();
    }
}
