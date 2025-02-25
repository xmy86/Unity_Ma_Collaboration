using UnityEngine;

public class EpisodeManager : MonoBehaviour
{
    [SerializeField] private float maxTime = 10f;
    [SerializeField] private int maxEpisodes = 9;
    [SerializeField] private InterceptorBehavior pursuerAgent;
    [SerializeField] private TransporterBehavior transporter;
    [SerializeField] private TargetBehavior target;
    [SerializeField] private bool debug = false;

    private float timer;
    private int episodeCount = 0;

    void Update()
    {
        EpisodeTimeLimit();
        EpisodeCount();
    }

    private void EpisodeTimeLimit()
    {
        timer += Time.deltaTime;
        if (timer >= maxTime)
        {
            Debug.Log("Time's up! Episode restart.");
            EpisodeRestart();
        }
    }

    private void EpisodeCount()
    {
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

    public void EpisodeRestart()
    {
        if(timer > 0.1f) // Avoid rapid repeated calls.
        {
            episodeCount++;
            Debug.Log("Starting Episode " + episodeCount);
            pursuerAgent.Initialize();
            transporter.Initialize();
            target.Initialize();
        }
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = 0f;
    }
}
