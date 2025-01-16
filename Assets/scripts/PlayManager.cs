using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public int episodeCount = 0;
    [SerializeField] private int maxEpisodes = 10;
    [SerializeField] private bool debug = false;
    [SerializeField] private ChaserBehavior pursuerAgent;
    [SerializeField] private EvaderBehavior evaderAgent;
    [SerializeField] private targetBehavior target;

    void Update()
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

    public void Initialize()
    {
        episodeCount++;
        Debug.Log("Episode " + episodeCount);
        pursuerAgent.Initialize();
        evaderAgent.Initialize();
        target.Initialize();
    }
}
