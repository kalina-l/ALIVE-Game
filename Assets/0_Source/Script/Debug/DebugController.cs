using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour {

    public bool debugAI;

    public static DebugController Instance;

    public DebugType logType1;
    public DebugType logType2;
    public DebugType logType3;

    public enum DebugType
    {
        GameFlow,
        Activity,
        Feedback,
        UI,
        Animation,
        System,
        Multiplayer,
        Emotion,
        None
    }

    void Awake()
    {
        Instance = this;
    }

	public void Log(string msg, DebugType type)
    {
        if(type == logType1 || type == logType2 || type == logType3)
        {
            Debug.Log(msg);
        }
    }
}
