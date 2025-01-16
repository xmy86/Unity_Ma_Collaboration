using UnityEngine;
using UnityEngine.AI;

public class EvaderBehavior : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ChaserBehavior pursuer;
    [SerializeField] private GameObject playManager;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 200f;
    private bool hasCollided = false;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
            return;
        }

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;

        playManager.GetComponent<PlayManager>().Initialize();
    }

    private void Update()
    {
        if (navMeshAgent != null && target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    public void Initialize()
    {
        transform.localPosition = new Vector3(0f, 0f, -3.5f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position);
        }

        hasCollided = false;
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
        playManager.GetComponent<PlayManager>().Initialize();
    }
}
