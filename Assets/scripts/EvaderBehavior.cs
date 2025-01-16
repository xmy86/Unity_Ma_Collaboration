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

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;
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
            navMeshAgent.Warp(transform.position);

        hasCollided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "target")
            playManager.GetComponent<PlayManager>().Initialize();
    }
}
