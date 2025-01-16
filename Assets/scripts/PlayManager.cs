using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public int episodeCount = 0;
    [SerializeField] private int maxEpisodes = 10;
    [SerializeField] private bool debug = false;
    [SerializeField] private ChaserBehavior pursuerAgent;
    [SerializeField] private EvaderBehavior evaderAgent;
    [SerializeField] private TargetBehavior target;
    [SerializeField] private float maxTime = 10f;

    private float timer;

    void Update()
    {
        // Handle time-based episode logic
        timer += Time.deltaTime;
        if (timer >= maxTime)
        {
            Debug.Log("Time's up! Ending episode.");
            Initialize();
        }

        // Check for max episodes
        if (episodeCount >= maxEpisodes)
        {
            Debug.Log("Max episodes reached.");
#if UNITY_EDITOR
            if (debug)
            {
                Debug.Log("Stopping play mode in the editor.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Debug.Log("Exiting Unity Editor.");
                UnityEditor.EditorApplication.Exit(0);
            }
#else
            Application.Quit(0);
#endif
        }
    }

    public void Initialize()
    {
        if(timer > 0.1f)
        {
            episodeCount++;
            Debug.Log("Starting Episode " + episodeCount);
            pursuerAgent.Initialize();
            evaderAgent.Initialize();
            target.Initialize();
        }
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = 0f;
    }
}
