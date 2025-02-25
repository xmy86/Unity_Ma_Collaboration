// InterceptorBehavior.cs To Interceptor
using UnityEngine;
using UnityEngine.AI;

public class InterceptorBehavior : MovableAgent
{
    [SerializeField] new protected float _moveSpeed = 3f;
    [SerializeField] new protected float _turnSpeed = 150f;
    [SerializeField] new protected bool _isHeuristicMode = false;
    public Vector3 realtimeWaypoint;

    protected override void Start()
    {
        base.Start();
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
    }
}
