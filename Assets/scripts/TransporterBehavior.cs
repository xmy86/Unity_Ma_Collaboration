// TransporterBehavior.cs To Transpoter
using UnityEngine;
using UnityEngine.AI;

public class TransporterBehavior : MovableAgent
{
    [SerializeField] new protected float _moveSpeed = 2f;
    [SerializeField] new protected float _turnSpeed = 100f;
    [SerializeField] new protected bool _isHeuristicMode = false;
    [SerializeField] private GameObject pickMark;
    public Vector3 realtimeWaypoint;
    public bool pickableTag = false;

    protected override void Start()
    {
        base.Start();

        if (pickMark != null)
        {
            pickMark.SetActive(false);
        }
    }

    protected override void SetMovementAttributes()
    {
        moveSpeed = _moveSpeed;
        turnSpeed = _turnSpeed;
        isHeuristicMode = _isHeuristicMode;
    }

    private void Update()
    {
        Move(realtimeWaypoint);
        HeuristicPickup();
    }

    private void HeuristicPickup()
    {
        if (isHeuristicMode && pickableTag && Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    public void Initialize()
    {
        // Reset the position and rotation of the GameObject.
        transform.localPosition = new Vector3(0f, 0f, -3.5f);
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        // Reset the velocity and angular velocity of the Rigidbody component.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset the position of the NavMeshAgent component.
        if (navMeshAgent != null)
            navMeshAgent.Warp(transform.position);

        // Hide the pick mark.
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "the_rescured")
        {
            pickableTag = false;
            if (pickMark != null)
            {
                pickMark.SetActive(false);
            }
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
