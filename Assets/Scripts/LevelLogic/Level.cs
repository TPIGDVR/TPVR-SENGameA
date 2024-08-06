using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField]
    Room[] rooms;
    [SerializeField]
    Level_Door door;
    [SerializeField]
    Room startingRoom;
    public void Initialize()
    {
        rooms = GetComponentsInChildren<Room>();
        door = GetComponentInChildren<Level_Door>();
        EventSystem.level.TriggerEvent(LevelEvents.ENTER_NEW_ROOM, startingRoom);
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this,(int)LevelLoadSequence.LEVELSYSTEM);
    }
}
