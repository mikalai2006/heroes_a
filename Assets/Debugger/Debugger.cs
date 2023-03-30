using System.Collections;

using UnityEngine;

public class Debugger : MonoBehaviour
{
    uint qsize = 15;
    Queue myLogQueue = new Queue();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        // if (type == LogType.Exception)
        //     myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        GUIStyle logStyle = new GUIStyle();
        logStyle.fontSize = 20;
        logStyle.normal.textColor = Color.white;
        GUILayout.BeginArea(new Rect(Screen.width - 800, 0, 800, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), logStyle);
        GUILayout.EndArea();
    }
}
