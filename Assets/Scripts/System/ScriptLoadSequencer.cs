using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public static class ScriptLoadSequencer
{
    static string id = "S.L.S :";
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
            Debug.Log(id +" LOADING - " + obj);
            try
            {
                obj?.Initialize();
            }
            catch
            {
                Debug.LogError(id + $" Trouble initializing script {obj}");
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
    SYSTEM = -2,
    PLAYER = -1,
    LEVEL = 0,
    /*
    * ROOMS = 1
    * DOORS = 2
    */
    AUTOMATONS = 50,
}
