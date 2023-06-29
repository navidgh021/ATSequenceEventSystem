using UnityEngine;
using AT.Sequence;
using AT.Sequence.Runtime;

[ATSequence (path: "Action/Logger")]
public class SequenceAction_Logger : Action
{
    [AT.Sequence.Property]
    public string LogText = "Logger Is Called!";

    [Action]
    public void Log()
    {
        Debug.Log (LogText);
    }

    [Action]
    public void LogWarning()
    {
        Debug.Log (LogText);
    }

    [Action]
    public void LogError()
    {
        Debug.Log (LogText);
    }
}
