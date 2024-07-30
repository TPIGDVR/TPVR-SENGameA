using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptLoadSequencer
{
    //the smaller number => higher priority
    static PriorityQueue<object> ScriptQueue = new();

    public static void Enqueue(object obj,int prio)
    {
        if ((IScriptLoadQueuer)obj == null) return;

        ScriptQueue.Enqueue(obj, prio);
    }

    public static void LoadScripts()
    {
        while (!ScriptQueue.IsEmpty)
        {
            var obj = (IScriptLoadQueuer)ScriptQueue.Dequeue().Item1;
            Debug.LogAssertion("LOADING : " + obj);
            try
            {
                obj?.Initialize();
            }
            catch
            {
                Debug.LogError($"TROUBLE INITIALIZING : {obj}");
            }
        }
    }
}

public interface IScriptLoadQueuer
{
    public void Initialize();
}

public enum LevelLoadSequence
{
    LEVEL = 0,
    /*
    * ROOMS = 1
    * DOORS = 2
    */
    AUTOMATONS = 50,
    PLAYER = 100,
}
