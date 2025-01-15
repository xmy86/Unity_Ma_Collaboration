using UnityEngine;
using Unity.MLAgents;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Agent evaderAgent;
    [SerializeField] private ChaserAgent pursuerAgent;
    [SerializeField] private float maxTime = 10f;
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
                evaderAgent.EndEpisode();
                pursuerAgent.Initialize();
            }
            ResetTimer();
        }
    }
    public void ResetTimer()
    {
        timer = 0f;
    }
}
