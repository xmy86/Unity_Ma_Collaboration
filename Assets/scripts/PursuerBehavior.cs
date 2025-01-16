using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class ChaserBehavior : MonoBehaviour
{
    [SerializeField] private Rigidbody pursuerRb;
    [SerializeField] private Transform evader;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float dragFactor = 0.95f;
    [SerializeField] private bool isManualControl = false;
    [SerializeField] private GameObject playManager;
    [SerializeField] private TextAsset pathJson;

    private List<Vector3> waypoints;
    private int currentWaypointIndex = 0;
    private bool isFollowingEvader = false;
    private float captureDistance;

    void Start()
    {
        LoadPathFromJson();

        if (waypoints.Count > 0)
        {
            currentWaypointIndex = 0;
        }
    }

    void Update()
    {
        if (isManualControl)
        {
            HandleInput();
        }
        else
        {
            NavigatePath();
        }

        CheckCapture();
    }

    private void LoadPathFromJson()
    {
        if (pathJson == null)
        {
            Debug.LogError("Path JSON not provided!");
            return;
        }

        var pathData = JsonConvert.DeserializeObject<PathData>(pathJson.text);

        waypoints = new List<Vector3>();
        foreach (var waypoint in pathData.waypoints)
        {
            waypoints.Add(new Vector3(waypoint.x, waypoint.y, waypoint.z));
        }

        captureDistance = pathData.captureDistance;
    }

    private void NavigatePath()
    {
        if (isFollowingEvader)
        {
            MoveTowardsTarget(evader.position);
            return;
        }

        if (currentWaypointIndex < waypoints.Count)
        {
            Vector3 currentTarget = waypoints[currentWaypointIndex];
            if (Vector3.Distance(transform.position, currentTarget) <= captureDistance)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    isFollowingEvader = true;
                    Debug.Log("All waypoints reached. Now following evader.");
                }
            }
            else
            {
                MoveTowardsTarget(currentTarget);
            }
        }
    }

    private void CheckCapture()
    {
        if (isFollowingEvader && Vector3.Distance(transform.position, evader.position) <= captureDistance)
        {
            Debug.Log("Evader captured!");
            playManager.GetComponent<PlayManager>().Initialize();
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        float turn = Mathf.Clamp(angleToTarget, -1f, 1f);
        float forwardForce = Mathf.Clamp(1f - Mathf.Abs(angleToTarget) / 90f, 0f, 1f);

        float torque = turn * turnSpeed;
        pursuerRb.AddTorque(Vector3.up * torque, ForceMode.Force);

        Vector3 force = transform.forward * forwardForce * moveSpeed;
        pursuerRb.AddForce(force, ForceMode.Force);

        if (pursuerRb.velocity.magnitude > moveSpeed)
        {
            pursuerRb.velocity = pursuerRb.velocity.normalized * moveSpeed;
        }

        pursuerRb.velocity *= dragFactor;
    }

    private void HandleInput()
    {
        float turn = 0f;
        float moveForward = 0f;

        if (Input.GetKey(KeyCode.I))
        {
            moveForward = 1f;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            moveForward = -1f;
        }

        if (Input.GetKey(KeyCode.J))
        {
            turn = -1f;
        }
        else if (Input.GetKey(KeyCode.L))
        {
            turn = 1f;
        }

        if (Mathf.Abs(turn) > 0.1f)
        {
            float torque = turn * turnSpeed;
            pursuerRb.AddTorque(Vector3.up * torque, ForceMode.Force);
        }

        if (Mathf.Abs(moveForward) > 0.1f)
        {
            Move(moveForward);
        }
    }

    private void Move(float forwardForce)
    {
        Vector3 force = transform.forward * forwardForce * moveSpeed;
        pursuerRb.AddForce(force, ForceMode.Force);

        if (pursuerRb.velocity.magnitude > moveSpeed)
        {
            pursuerRb.velocity = pursuerRb.velocity.normalized * moveSpeed;
        }

        pursuerRb.velocity *= dragFactor;
    }

    public void Initialize()
    {
        transform.localPosition = new Vector3(0f, 0f, 3.5f);
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        isFollowingEvader = false;
        currentWaypointIndex = 0;

        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    [Serializable]
    private class PathData
    {
        public List<Waypoint> waypoints;
        public string finalAction;
        public float captureDistance;
    }

    [Serializable]
    private class Waypoint
    {
        public float x;
        public float y;
        public float z;
    }
}
