using System;
using Newtonsoft.Json;
using UnityEngine;

public class ChaserAgent : MonoBehaviour
{
    [SerializeField] private Rigidbody pursuerRb;
    [SerializeField] private Transform evader;
    [SerializeField] private Transform distanceSensor;
    [SerializeField] private GameObject laser;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float dragFactor = 0.95f;
    [SerializeField] private bool isManualControl = false;
    [SerializeField] private float sensorRange = 10f;
    [SerializeField] private float laserDuration = 0.5f;
    [SerializeField] private int maxLaserShots = 2; // 每局最多发射激光次数

    [SerializeField] private TextAsset decisionTreeJson;
    [SerializeField] private GameObject playManager;

    private DecisionTreeNode rootNode;
    private float laserTimer = 0f;
    private int currentLaserShots = 0; // 当前已发射的激光次数

    public void OnEpisodeBegin()
    {
        Initialize();
        HideLaser();
    }

    void Start()
    {
        if (decisionTreeJson != null)
        {
            rootNode = DecisionTreeNode.FromJson(decisionTreeJson.text, this);
        }
    }

    void Update()
    {
        if (isManualControl)
        {
            HandleInput();
        }
        else if (rootNode != null)
        {
            rootNode.Execute();
        }

        float forwardDistance = MeasureSensorDistance();
        UpdateLaser(forwardDistance);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
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

    private float MeasureSensorDistance()
    {
        if (distanceSensor == null)
        {
            Debug.LogError("DistanceSensor transform is not assigned!");
            return sensorRange;
        }

        Vector3 sensorOrigin = distanceSensor.position;
        Vector3 sensorDirection = distanceSensor.forward;

        Ray ray = new Ray(sensorOrigin, sensorDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, sensorRange))
        {
            if (laserTimer > 0f && hit.distance < 1f && hit.collider.CompareTag("evader"))
            {
                Debug.Log("[Remote Attack] Evader hit by laser!");
                EndGame();
            }
            return hit.distance;
        }
        else
        {
            return sensorRange;
        }
    }

    private void FireLaser()
    {
        if (currentLaserShots >= maxLaserShots)
        {
            Debug.Log("Maximum laser shots reached. Cannot fire more lasers this round.");
            return;
        }

        ShowLaser();
        laserTimer = laserDuration;
        currentLaserShots++; // 增加激光发射计数
    }

    private void UpdateLaser(float forwardDistance)
    {
        if (laserTimer > 0f)
        {
            laserTimer -= Time.deltaTime;
            float clampedDistance = Mathf.Min(1f, forwardDistance);

            if (laser != null)
            {
                Vector3 currentScale = laser.transform.localScale;
                Vector3 currentPosition = laser.transform.localPosition;

                float lengthDifference = currentScale.z - clampedDistance;

                laser.transform.localScale = new Vector3(currentScale.x, currentScale.y, clampedDistance);
                laser.transform.localPosition = new Vector3(
                    currentPosition.x,
                    currentPosition.y,
                    currentPosition.z - lengthDifference / 2f
                );
            }

            if (laserTimer <= 0f)
            {
                HideLaser();
            }
        }
    }

    private void ShowLaser()
    {
        if (laser != null)
        {
            laser.SetActive(true);
        }
    }

    private void HideLaser()
    {
        if (laser != null)
        {
            laser.SetActive(false);
        }
    }

    private void Caught()
    {
        Debug.Log("Successfully caught up!");
        Initialize();
    }

    private void EndGame()
    {
        Debug.Log("[Game Over] ChaserAgent wins!");
        playManager.GetComponent<PlayManager>().episodeCount++;
        Initialize();
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        if (pursuerRb == null)
        {
            Debug.LogError("Rigidbody not assigned to pursuer!");
            return;
        }

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

    private void AvoidObstacle()
    {
        float randomTurn = UnityEngine.Random.Range(-1f, 1f);
        transform.Rotate(0, randomTurn * turnSpeed * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        Debug.Log("Avoiding obstacle");
    }

    public void Initialize()
    {
        transform.localPosition = new Vector3(3f, 0f, 3f); // Pursuer 的位置
        transform.localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f); // Pursuer 的随机方向
        evader.localPosition = new Vector3(-3f, 0f, -3f); // Evader 的位置
        evader.localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f); // Evader 的随机方向
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        if (evader.GetComponent<Rigidbody>() != null)
        {
            evader.GetComponent<Rigidbody>().velocity = Vector3.zero;
            evader.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        currentLaserShots = 0;
    }


    public abstract class DecisionTreeNode
    {
        public abstract void Execute();

        public static DecisionTreeNode FromJson(string json, ChaserAgent agent)
        {
            var nodeData = JsonConvert.DeserializeObject<DecisionTreeNodeData>(json);
            return CreateNode(nodeData, agent);
        }

        private static DecisionTreeNode CreateNode(DecisionTreeNodeData data, ChaserAgent agent)
        {
            if (data.type == "DecisionNode")
            {
                return new DecisionNode(
                    () => EvaluateCondition(data.condition, agent, data.args),
                    CreateNode(data.trueNode, agent),
                    CreateNode(data.falseNode, agent)
                );
            }
            else if (data.type == "ActionNode")
            {
                return new ActionNode(() => ExecuteAction(data.action, agent));
            }
            throw new Exception("Invalid node type");
        }

        private static bool EvaluateCondition(string condition, ChaserAgent agent, string[] args)
        {
            if (condition == "DistanceBetweenEntities")
            {
                if (args.Length < 4)
                {
                    throw new Exception("Invalid arguments for DistanceBetweenEntities condition");
                }

                string entityA = args[0];
                string entityB = args[1];
                float threshold = float.Parse(args[2]);
                string comparison = args[3];

                Transform transformA = GetEntityTransform(entityA, agent);
                Transform transformB = GetEntityTransform(entityB, agent);

                float distance = Vector3.Distance(transformA.position, transformB.position);

                if (comparison == "less")
                {
                    return distance < threshold;
                }
                else if (comparison == "greater")
                {
                    return distance > threshold;
                }
                else
                {
                    throw new Exception("Invalid comparison operator");
                }
            }
            else if (condition == "LaserDistanceToEntity") // 新增条件：激光测距
            {
                if (args.Length < 2)
                {
                    throw new Exception("Invalid arguments for LaserDistanceToEntity condition");
                }

                string targetEntity = args[0]; // 目标实体，例如 "Evader"
                float threshold = float.Parse(args[1]);

                Transform targetTransform = GetEntityTransform(targetEntity, agent);

                if (targetTransform == null)
                {
                    throw new Exception($"Invalid entity for LaserDistanceToEntity: {targetEntity}");
                }

                // 使用激光测距获取目标的距离
                Vector3 sensorOrigin = agent.distanceSensor.position;
                Vector3 sensorDirection = agent.distanceSensor.forward;

                if (Physics.Raycast(sensorOrigin, sensorDirection, out RaycastHit hit, agent.sensorRange))
                {
                    if (hit.collider.CompareTag(targetEntity)) // 检查是否命中指定目标
                    {
                        float laserDistance = hit.distance;

                        if (laserDistance < threshold)
                        {
                            return true;
                        }
                    }
                }

                // 如果没有击中目标或距离不符合，返回 false
                return false;
            }

            throw new Exception("Unknown condition");
        }


        private static Transform GetEntityTransform(string entityName, ChaserAgent agent)
        {
            return entityName switch
            {
                "self" => agent.transform,
                "evader" => agent.evader,
                "barrier" => agent.distanceSensor, // 假设障碍物由激光测距器检测
                _ => throw new Exception($"Unknown entity: {entityName}")
            };
        }

        private static void ExecuteAction(string action, ChaserAgent agent)
        {
            switch (action)
            {
                case "Caught":
                    agent.Caught();
                    break;
                case "FireLaser":
                    agent.FireLaser();
                    break;
                case "MoveTowardsTarget":
                    agent.MoveTowardsTarget(agent.evader.position);
                    break;
                case "AvoidObstacle":
                    agent.AvoidObstacle();
                    break;
                default:
                    throw new Exception($"Unknown action: {action}");
            }
        }
    }

    public class DecisionNode : DecisionTreeNode
    {
        private Func<bool> condition;
        private DecisionTreeNode trueNode;
        private DecisionTreeNode falseNode;

        public DecisionNode(Func<bool> condition, DecisionTreeNode trueNode, DecisionTreeNode falseNode)
        {
            this.condition = condition;
            this.trueNode = trueNode;
            this.falseNode = falseNode;
        }

        public override void Execute()
        {
            if (condition())
            {
                trueNode.Execute();
            }
            else
            {
                falseNode.Execute();
            }
        }
    }

    public class ActionNode : DecisionTreeNode
    {
        private Action action;

        public ActionNode(Action action)
        {
            this.action = action;
        }

        public override void Execute()
        {
            action();
        }
    }

    [Serializable]
    public class DecisionTreeNodeData
    {
        public string type;
        public string condition;
        public string action;
        public DecisionTreeNodeData trueNode;
        public DecisionTreeNodeData falseNode;
        public string[] args;
    }
}
