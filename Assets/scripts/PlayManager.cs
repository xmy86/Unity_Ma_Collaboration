using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public int episodeCount = 0;
    [SerializeField] private int maxEpisodes = 10;

    void Update()
    {
        if (episodeCount >= maxEpisodes)
        {
            Debug.Log("Max episodes reached. Exiting Unity...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0); // 编辑器模式下退出
#else
            Application.Quit(0); // 运行时模式下退出
#endif
        }
    }   
}