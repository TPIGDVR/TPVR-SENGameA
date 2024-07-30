using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Room[] rooms;
    [SerializeField]
    Level_Door door;

    public void Initialize()
    {
        rooms = GetComponentsInChildren<Room>();
        door = GetComponentInChildren<Level_Door>();
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.LEVEL);
    }
}
