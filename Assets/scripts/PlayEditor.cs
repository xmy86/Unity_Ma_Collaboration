using UnityEditor;
class PlayEditor
{
    [MenuItem("Tools/Play")]
    static void Play()
    {
        EditorApplication.isPlaying = true;
    }
}