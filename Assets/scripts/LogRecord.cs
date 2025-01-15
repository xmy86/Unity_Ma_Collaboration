using System.Collections;
using System.IO;
using UnityEngine;

public class EntityLogger : MonoBehaviour
{
    [SerializeField] private Transform evader;
    [SerializeField] private Rigidbody evaderRb;
    [SerializeField] private Transform pursuer;
    [SerializeField] private Rigidbody pursuerRb;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject laser;

    private string logFilePath;
    [SerializeField] private float logTimeInterval = 0.1f;

    void Start()
    {
        string logDirectory = Path.Combine(Application.dataPath, "../LLM_iteration/Log");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        logFilePath = GetMaxLogFilePath(logDirectory);
        StartCoroutine(LogEntityData());
    }

    private string GetMaxLogFilePath(string logDirectory)
    {
        string[] logFiles = Directory.GetFiles(logDirectory, "Log*.txt");
        int maxIndex = 0;

        foreach (string file in logFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName == "Log")
            {
                maxIndex = Mathf.Max(maxIndex, 1);
            }
            else if (fileName.StartsWith("Log"))
            {
                string indexPart = fileName.Substring(3);
                if (int.TryParse(indexPart, out int index))
                {
                    maxIndex = Mathf.Max(maxIndex, index);
                }
            }
        }
        string newFileName = maxIndex == 0 ? "Log.txt" : $"Log{maxIndex + 1}.txt";
        return Path.Combine(logDirectory, newFileName);
    }

    private IEnumerator LogEntityData()
    {
        while (true)
        {
            string evaderPosition = evader.position.ToString();
            string evaderVelocity = evaderRb.velocity.ToString();

            string pursuerPosition = pursuer.position.ToString();
            string pursuerVelocity = pursuerRb.velocity.ToString();

            string targetPosition = target.position.ToString();

            bool isLaserActive = laser.activeSelf;

            string logEntry = $"Timestamp: {Time.time:F2}\n" +
                              $"Evader Position: {evaderPosition}, Velocity: {evaderVelocity}\n" +
                              $"Pursuer Position: {pursuerPosition}, Velocity: {pursuerVelocity}\n" +
                              $"Target Position: {targetPosition}\n" +
                              $"Laser Active: {isLaserActive}\n\n";

            File.AppendAllText(logFilePath, logEntry);

            yield return new WaitForSeconds(logTimeInterval);
        }
    }
}
