using System;
using Newtonsoft.Json;
using UnityEngine;

public class ChaserAgent : MonoBehaviour
{
    [SerializeField] private Rigidbody pursuerRb;
    [SerializeField] private Transform evader;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float dragFactor = 0.95f;
    [SerializeField] private bool isManualControl = false;
    [SerializeField] private TextAsset decisionTreeJson;
    [SerializeField] private GameObject playManager;

    private DecisionTreeNode rootNode;

    public void OnEpisodeBegin()
    {
        Initialize();
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

    private void Caught()
    {
        Debug.Log("Successfully caught up!");
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
        transform.localPosition = new Vector3(0f, 0f, 3.5f);
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        evader.localPosition = new Vector3(0f, 0f, -3.5f);
        evader.localRotation = Quaternion.Euler(0f, 0f, 0f);
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

            throw new Exception("Unknown condition");
        }

        private static Transform GetEntityTransform(string entityName, ChaserAgent agent)
        {
            return entityName switch
            {
                "self" => agent.transform,
                "evader" => agent.evader,
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
