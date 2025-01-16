using UnityEngine;
using UnityEngine.AI;

public class EvaderBehavior : MonoBehaviour
{
    [SerializeField] private Transform target; // 目标
    [SerializeField] private ChaserAgent pursuer; // 追击者
    [SerializeField] private GameObject playManager; // 游戏管理器
    [SerializeField] private float moveSpeed = 4f; // 移动速度
    [SerializeField] private float turnSpeed = 200f; // 转向速度
    private bool hasCollided = false; // 碰撞状态

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        // 初始化 NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
            return;
        }

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;

        InitializePosition(); // 初始化位置
    }

    private void Update()
    {
        if (navMeshAgent != null && target != null)
        {
            // 设置导航目标位置为 target 的位置
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void InitializePosition()
    {
        // 初始化 Evader 的位置和方向
        transform.localPosition = new Vector3(0f, 0f, -3.5f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 初始化目标位置
        if (target != null && target.GetComponent<targetBehavior>() != null)
        {
            target.GetComponent<targetBehavior>().InitializePosition();
        }

        // 确保 NavMeshAgent 的位置与对象一致
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position);
        }

        hasCollided = false; // 重置碰撞状态
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "target")
        {
            HandleCollision(1f, "target");
        }
        else if (other.gameObject.tag == "wall")
        {
            HandleCollision(-0.1f, "wall");
        }
        else if (other.gameObject.tag == "pursuer")
        {
            HandleCollision(-0.3f, "pursuer");
        }
    }

    private void HandleCollision(float reward, string tag)
    {
        if (!hasCollided)
        {
            Debug.Log($"Collision with {tag}. Reward: {reward}");
            hasCollided = true;
        }

        // 重置状态
        pursuer.Initialize();
        if (playManager != null)
        {
            playManager.GetComponent<PlayManager>().episodeCount++;
        }
        InitializePosition(); // 重置位置
    }
}
