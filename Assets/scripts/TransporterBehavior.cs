// TransporterBehavior.cs To Transpoter
using UnityEngine;
using UnityEngine.AI;

public class TransporterBehavior : MovableAgent
{
    [SerializeField] new protected float moveSpeed = 4f;
    [SerializeField] new protected float turnSpeed = 200f;
    [SerializeField] new protected bool isHeuristicMode = false;
    [SerializeField] private InterceptorBehavior pursuer;
    [SerializeField] private GameObject playManager;
    [SerializeField] private GameObject pickMark;
    public Vector3 realtimeWaypoint;
    public bool pickableTag = false;

    protected override void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;

        if (pickMark != null)
        {
            pickMark.SetActive(false);
        }
    }

    private void Update()
    {
        Move(realtimeWaypoint);
        HeuristicPickup();
    }

    private void HeuristicPickup()
    {
        if (pickableTag && Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
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

        if (pickMark != null)
        {
            pickMark.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "the_rescured")
        {
            pickableTag = true;
        }
    }

    private void Pickup()
    {
        if (pickableTag)
        {
            GameObject rescuedObject = GameObject.FindGameObjectWithTag("the_rescured");
            if (rescuedObject != null)
            {
                rescuedObject.SetActive(false);
                pickableTag = false;
            }
            if (pickMark != null)
            {
                pickMark.SetActive(true);
            }
        }
    }
}
