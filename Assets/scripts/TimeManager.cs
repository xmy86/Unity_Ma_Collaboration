using UnityEngine;
using Unity.MLAgents;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Agent evaderAgent;
    [SerializeField] private ChaserBehavior pursuerAgent;
    [SerializeField] private float maxTime = 10f;
    [SerializeField] private GameObject playManager;
    private float timer;
    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= maxTime)
        {
            Debug.Log("Time's up! Ending episode.");
            if (evaderAgent != null)
            {
                evaderAgent.AddReward(-0.1f);
                playManager.GetComponent<PlayManager>().Initialize();
            }
            ResetTimer();
        }
    }
    public void ResetTimer()
    {
        timer = 0f;
    }
}
